import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Flex,
  Group,
  Paper,
  Stack,
  Text,
  TextInput,
} from "@mantine/core";
import { useClickOutside } from "@mantine/hooks";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import { z } from "zod";
import { InputErrorLevel } from "~/domain/enums";
import type { InputExamItem } from "~/domain/wellship.schemas";
import NumericKeyboard from "./NumericKeyboard";
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import styles from "~/styles/common.module.css";
import React from "react";

type BodyProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
};

export default function ExamBody({
  examItems,
  onRegisterPressed,
  onChange,
}: BodyProps) {
  if (!examItems || examItems.length === 0) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);

  // キーボードの表示状態を管理する
  const [showKeyboards, setShowKeyboards] = useState<boolean[][]>(
    examItems.map((examItem) => {
      const { examItemDetails } = examItem;
      return examItemDetails ? examItemDetails.map(() => false) : [];
    })
  );
  const closeKeyBoard = useClickOutside(() => {
    setShowKeyboards((prev) => prev.map((row) => row.map(() => false)));
  });
  const handleConfirm = () => {
    setShowKeyboards((prev) => prev.map((row) => row.map(() => false)));
  };
  const toggleKeyboard2 = (index: number, detailIndex: number) => {
    setShowKeyboards((prev) => {
      const updated = [...prev];
      updated[index][detailIndex] = true;
      return updated;
    });
  };
  const handleErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      // エラーレベルが高い順にソート
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });

      // 重複を除外
      item.examRegistResults = item.examRegistResults.filter(
        (result, index, self) =>
          index === self.findIndex((r) => r.description === result.description)
      );
    }

    return item;
  };

  const validationCheck = (item: InputExamItem) => {
    // 必須チェックと半角数字チェックを一度に行うスキーマ
    const schema = z.object({
      value: z
        .string()
        .min(1, {
          message: getErrorMessage(errorMessages.required, `${item.name}は`),
        }) // 必須チェック
        .refine((value) => /^\d+(\.\d+)?$/.test(value), {
          // 半角数字チェック
          message: getErrorMessage(
            errorMessages.numericString,
            `${item.name}は`
          ),
        }),
    });
    // 基準値によるチェック
    setRangesErrorMessage(item);
    // エラーメッセージを更新
    let updatedErrors = item.examRegistResults || [];
    // 既に存在するコンポーネントのエラーメッセージを削除
    const errorMessageRequired = getErrorMessage(
      errorMessages.required,
      `${item.name}は`
    );
    updatedErrors = updatedErrors.filter(
      (error) => error.description !== errorMessageRequired
    );

    // 半角数字エラーを削除
    const errorMessageNumeric = getErrorMessage(
      errorMessages.numericString,
      `${item.name}は`
    );
    updatedErrors = updatedErrors.filter(
      (error) => error.description !== errorMessageNumeric
    );

    // バリデーションチェック
    for (const detail of item.examItemDetails || []) {
      const valueToValidate = detail.value || "";
      const result = schema.safeParse({ value: valueToValidate });

      // エラーメッセージを追加する
      if (!result.success) {
        const errorMessage = result.error.errors[0].message;
        updatedErrors.push({
          description: errorMessage,
          errorLevel: InputErrorLevel.異常,
        });
        break; // この検査項目についてのチェックを終える
      }
    }

    const prevItem = {
      ...item,
      examRegistResults: updatedErrors,
    };

    return handleErrorMessage(prevItem);
  };

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      let validatedData: InputExamItem = item;
      // onRegisterPressedがtrueの場合のみvalidationCheckを実行
      if (onRegisterPressed) {
        validatedData = validationCheck(item); // 必須バリデーションを実行
      }
      return validatedData;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  //変更イベント
  const handleChange = (
    positionNumber: number | undefined,
    value: string,
    detailPositionNumber?: number
  ) => {
    const updatedExamItems = [...examItemsData];

    // 該当するitemを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // 該当するitemの全てのdetailの値をvalueに変更
        if (detailPositionNumber === undefined) {
          item.examItemDetails = item.examItemDetails?.map((detail) => ({
            ...detail,
            value: value,
          }));
        } else {
          // 該当するdetailの値を更新
          item.examItemDetails = item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailPositionNumber
              ? { ...detail, value: value.slice(0, detail.integerLength ?? 3) }
              : detail
          );
        }
      }
      // 平均値の計算処理
      const AVE1 = Math.round(
        (Number.parseFloat(
          updatedExamItems[0].examItemDetails?.[0].value ?? ""
        ) +
          Number.parseFloat(
            updatedExamItems[1].examItemDetails?.[0].value ?? ""
          )) /
          2
      );
      const AVE2 = Math.round(
        (Number.parseFloat(
          updatedExamItems[0].examItemDetails?.[1].value ?? ""
        ) +
          Number.parseFloat(
            updatedExamItems[1].examItemDetails?.[1].value ?? ""
          )) /
          2
      );
      // 平均値の保存処理
      if (item.positionNumber === 3) {
        item.examItemDetails = item.examItemDetails?.map((detail, idx) => {
          const integerLength = detail.integerLength ?? 0; // 血圧なので、小数部分は考慮しない
          const maxDigits = integerLength + 1;
          if (idx === 0) {
            if (AVE1) {
              if (detail.integerLength) {
                return {
                  ...detail,
                  value: AVE1.toString().slice(0, maxDigits),
                };
              }
              return { ...detail, value: AVE1.toString() };
            }
            return { ...detail, value: "" }; // 計算する値が無い場合、空白にする
          }
          if (idx === 1) {
            if (AVE2) {
              if (detail.integerLength) {
                return {
                  ...detail,
                  value: AVE2.toString().slice(0, maxDigits),
                };
              }
              return { ...detail, value: AVE2.toString() };
            }
            return { ...detail, value: "" };
          }
          return detail;
        });
      }
      // 各アイテムに対してバリデーションを実行
      const validatedItem = validationCheck(item);
      // バリデーション結果を反映
      Object.assign(item, validatedItem);
    }
    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);
    // 2つのエラーが存在するか確認
    const isValidatedError = updatedExamItems.some((item) =>
      item.examRegistResults?.some(
        (error) =>
          error.description ===
            getErrorMessage(errorMessages.required, `${item.name}は`) ||
          error.description ===
            getErrorMessage(errorMessages.numericString, `${item.name}は`)
      )
    );
    if (!isValidatedError) {
      onChange(updatedExamItems);
    }
  };

  // 小数点と先頭の0を除去して数値部分だけを取得する処理
  // TODO：キーボードのコンポーネントの仕様変更に伴い削除予定
  const removeDecimalAndLeadingZero = (value: string): string => {
    // 小数点を取り除く
    const withoutDecimal = value.replace(".", "");
    // 先頭の0を除去
    const withoutLeadingZero = withoutDecimal.replace(/^0+/, "");
    return withoutLeadingZero || "0"; // 空になった場合は "0" を返す
  };

  return (
    <>
      {examItemsData.map((item, index) => {
        const { positionNumber, name, examRegistResults = [] } = item;
        const details = item.examItemDetails;
        const isAVE = item.positionNumber === 3;

        return (
          <Flex
            key={positionNumber}
            justify="flex-start"
            align="flex-start"
            direction="column"
            mb={16}
          >
            <Flex align="center" gap="md">
              <Paper
                key={positionNumber}
                w={274}
                h={80}
                bg={isAVE ? "white" : "gray02"}
                c={isAVE ? "gray02" : "white"}
                radius="itemName"
                px={32}
                py={16}
              >
                <Text size="lg" fw={700} ta="center">
                  {name}
                </Text>
              </Paper>
              {details?.map((detail, detailIndex) => {
                // グレーアウト表示判定
                const isDisabled =
                  !!details?.[detailIndex]?.cancelReasonId ||
                  !details?.[detailIndex]?.hasOrder;

                return (
                  <Flex key={detail.positionNumber}>
                    {isAVE ? (
                      <Text
                        w={170}
                        h={80}
                        size="inputComponent"
                        c={
                          isDisabled
                            ? "gray"
                            : examRegistResults?.some(
                                (x) => x.errorLevel === InputErrorLevel.異常
                              )
                            ? "error"
                            : examRegistResults?.some(
                                (x) => x.errorLevel === InputErrorLevel.警告
                              )
                            ? "warning"
                            : "black"
                        }
                        px={32}
                        mt={-8}
                        ta={"right"}
                      >
                        {detail?.value}
                      </Text>
                    ) : (
                      <TextInput
                        classNames={{
                          input: `${styles["input-textbox"]} ${
                            isDisabled
                              ? `${styles["input-textbox"]}`
                              : examRegistResults?.some(
                                  (x) => x.errorLevel === InputErrorLevel.異常
                                )
                              ? `${styles["input-error"]}`
                              : examRegistResults?.some(
                                  (x) => x.errorLevel === InputErrorLevel.警告
                                )
                              ? `${styles["input-warning"]}`
                              : ""
                          }`,
                        }}
                        w={170}
                        radius="md"
                        size="inputComponent"
                        bg={isDisabled ? "gray03" : ""}
                        c={isDisabled ? "gray02" : ""}
                        value={detail?.value}
                        maxLength={detail?.integerLength ?? 3}
                        onChange={(e) =>
                          handleChange(
                            positionNumber,
                            e.currentTarget.value,
                            detail.positionNumber
                          )
                        }
                        onClick={() => toggleKeyboard2(index, detailIndex)}
                        disabled={isDisabled}
                      />
                    )}
                    {detailIndex !== details.length - 1 && (
                      <Text w={30} h={72} ml={16} c={"gray02"} size={"80px"}>
                        /
                      </Text>
                    )}
                  </Flex>
                );
              })}
              <Stack w={216} gap={4} mt="auto">
                {details?.[0].prevValue && details?.[1].prevValue && (
                  <Text fw={700}>
                    (前回：{details[0].prevValue}/{details[1].prevValue})
                  </Text>
                )}
                <Text size="xs">{details?.[0].unit}</Text>
              </Stack>
              {!isAVE && (
                <Button
                  w={154}
                  h={64}
                  ml={16}
                  size="lg"
                  bg={"white"}
                  variant="outline"
                  onClick={() => handleChange(positionNumber, "")}
                  tabIndex={-1}
                >
                  クリア
                </Button>
              )}
            </Flex>

            {/* エラーメッセージの表示 */}
            {examRegistResults.map((error, idx) => {
              const isWarning = error.errorLevel === InputErrorLevel.警告;
              return (
                <Group key={idx} c={isWarning ? "warning" : "error"}>
                  <IconExclamationCircleFilled size={"32px"} />
                  <Text size="sm" fw={700}>
                    {error.description}
                  </Text>
                </Group>
              );
            })}
            {details?.map((detail, detailIndex) => (
              <React.Fragment key={detail.positionNumber}>
                {/* キーボード表示 */}
                {showKeyboards[index][detailIndex] && (
                  <Box ref={closeKeyBoard} mx="auto">
                    {detail.keyboard?.keyboardType === 1 ||
                    !detail.keyboard?.keyboardType ? (
                      <NumericKeyboard
                        value={removeDecimalAndLeadingZero(detail?.value ?? "")}
                        onChange={(newValue) =>
                          handleChange(
                            positionNumber,
                            newValue,
                            detail.positionNumber
                          )
                        }
                        onConfirm={handleConfirm}
                      />
                    ) : (
                      <CollectionKeyboard
                        keyboardValues={detail.keyboard?.values ?? []}
                        onChange={handleConfirm}
                      />
                    )}
                  </Box>
                )}
              </React.Fragment>
            ))}
          </Flex>
        );
      })}
    </>
  );
}
