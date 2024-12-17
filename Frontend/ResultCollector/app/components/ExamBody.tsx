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

  // キーボードの表示インデックスを状態として管理する
  const [activeKeyboard, setActiveKeyboard] = useState<number | null>(null);

  // キーボードの表示/非表示をトグルする関数
  const toggleKeyboard = (positionNumber: number) => {
    setActiveKeyboard((prevNumber) =>
      prevNumber === positionNumber ? null : positionNumber,
    );
  };
  const handleConfirm = () => {
    setActiveKeyboard(null);
  };
  const closeKeyBoard = useClickOutside(() => setActiveKeyboard(null));

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
          index === self.findIndex((r) => r.description === result.description),
      );
    }

    return item;
  };

  const validationCheck = (item: InputExamItem) => {
    // BMIはバリデーションチェックを実施しない
    if (item.positionNumber === 4) return item;
    // 必須チェックと半角数字チェックを一度に行うスキーマ
    const schema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, `${item.name}は`)) // 必須チェック
      .refine((value) => /^\d+(\.\d+)?$/.test(value), {
        message: getErrorMessage(errorMessages.numericString, `${item.name}は`),
      });

    // バリデーション対象データを取得
    const valueToValidate = item.examItemDetails?.[0]?.value || "";
    const result = schema.safeParse(valueToValidate);

    // エラーメッセージを更新
    let updatedErrors = item.examRegistResults || [];

    // 必須エラーを削除
    const errorMessageRequired = getErrorMessage(
      errorMessages.required,
      `${item.name}は`,
    );
    updatedErrors = updatedErrors.filter(
      (error) => error.description !== errorMessageRequired,
    );

    // 半角数字エラーを削除
    const errorMessageNumeric = getErrorMessage(
      errorMessages.numericString,
      `${item.name}は`,
    );
    updatedErrors = updatedErrors.filter(
      (error) => error.description !== errorMessageNumeric,
    );

    // バリデーションが失敗した場合
    if (!result.success) {
      const error = result.error.errors[0]; // 最初のエラーだけ取得
      updatedErrors.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    }

    const prevItem = {
      ...item,
      examRegistResults: updatedErrors,
    };
    const rangesValidatedItem = setRangesErrorMessage(prevItem);
    return handleErrorMessage(rangesValidatedItem);
  };

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      let validatedData: InputExamItem = item;
      // onRegisterPressedがtrueの場合のみvalidationCheckを実行
      if (onRegisterPressed) {
        validatedData = validationCheck(item);
      }
      return validatedData;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  // BMI計算処理
  const setBMIValue = (updatedExamItems: InputExamItem[]): InputExamItem[] => {
    const heightValue = updatedExamItems[0].examItemDetails?.[0].value ?? "0";
    const weightValue = updatedExamItems[1].examItemDetails?.[0].value ?? "0";

    // BMIの計算
    const height = Number.parseFloat(heightValue) / 100;
    const weight = Number.parseFloat(weightValue);
    const bmi = Number.isNaN(weight / height ** 2) ? 0 : weight / height ** 2;
    const bmiString = String(bmi);

    return updatedExamItems.map((item) => {
      if (item.positionNumber === 4) {
        item.examItemDetails = item.examItemDetails?.map((detail, idx) => {
          const decimalLength = detail.decimalLength ?? 0;
          const integerLength = detail.integerLength ?? 0;
          const maxDigits = decimalLength + integerLength + 1;

          if (idx === 0) {
            if (detail.integerLength) {
              return { ...detail, value: bmiString.slice(0, maxDigits) };
            }
            return { ...detail, value: bmiString };
          }
          return detail;
        });
      }
      return item;
    });
  };

  //変更イベント
  const handleChange = (positionNumber: number | undefined, value: string) => {
    const updatedExamItems = [...examItemsData];

    // 該当するアイテムを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // 該当するexamItemDetailsの最初のvalueを更新
        item.examItemDetails = item.examItemDetails?.map((detail, idx) => {
          if (idx === 0) {
            return { ...detail, value: value };
          }
          return detail;
        });
      }

      // バリデーションチェックを実施
      const validatedItem = validationCheck(item);
      // バリデーション結果を反映
      Object.assign(item, validatedItem);
    }

    // BMIの計算と設定処理
    const bmiUpdatedExamItems = setBMIValue(updatedExamItems);
    // 更新されたデータをステートに設定
    setExamItemsData(bmiUpdatedExamItems);
    // 3つのエラーが存在するか確認
    const isValidatedError = bmiUpdatedExamItems.some((item) =>
      item.examRegistResults?.some(
        (error) =>
          error.description ===
            getErrorMessage(errorMessages.required, `${item.name}は`) ||
          error.description ===
            getErrorMessage(errorMessages.numericString, `${item.name}は`) ||
          //TODO:基準値エラーメッセージは未確定（部分一致予定）
          error.description?.includes("正常値ではありません"),
      ),
    );

    if (!isValidatedError) {
      onChange(bmiUpdatedExamItems);
    }
  };

  return (
    <>
      {examItemsData.map((item) => {
        const { positionNumber, name, examRegistResults = [] } = item;
        const detail = item.examItemDetails?.[0];
        // グレーアウト表示判定
        const isDisabled = !detail?.hasOrder || !!detail?.cancelReasonId;
        const isBMI = item.positionNumber === 4;

        return (
          <Flex
            key={positionNumber}
            justify="flex-start"
            align="flex-start"
            direction="column"
            w={1038}
            mb={16}
          >
            <Flex align="center" gap="md">
              <Paper
                w={274}
                h={80}
                bg={isBMI ? "white" : "gray02"}
                c={isBMI ? "gray02" : "white"}
                radius="itemName"
                px={32}
                py={16}
              >
                <Text size="lg" fw={700} ta="center">
                  {name}
                </Text>
              </Paper>

              {isBMI ? (
                <Text
                  w={340}
                  h={80}
                  size="inputComponent"
                  ta="right"
                  c={isDisabled ? "gray02" : "black"}
                  px={32}
                >
                  {detail?.value}
                </Text>
              ) : (
                <TextInput
                  classNames={{
                    input: `${styles["input-textbox"]} ${
                      examRegistResults?.some(
                        (x) => x.errorLevel === InputErrorLevel.異常,
                      )
                        ? `${styles["input-error"]}`
                        : examRegistResults?.some(
                              (x) => x.errorLevel === InputErrorLevel.警告,
                            )
                          ? `${styles["input-warning"]}`
                          : ""
                    }`,
                  }}
                  w={340}
                  radius="md"
                  size="inputComponent"
                  bg={isDisabled ? "gray03" : ""}
                  c={isDisabled ? "gray02" : ""}
                  value={detail?.value}
                  onChange={(e) =>
                    handleChange(positionNumber, e.currentTarget.value)
                  }
                  onClick={() => toggleKeyboard(item.positionNumber ?? 0)}
                  disabled={isDisabled}
                />
              )}
              <Stack w={173} gap={4} mt="auto">
                {detail?.prevValue && (
                  <Text fw={700}>(前回：{detail.prevValue})</Text>
                )}
                <Text size="xs">{detail?.unit}</Text>
              </Stack>
              {!isBMI && (
                <Button
                  w={154}
                  h={64}
                  size="lg"
                  bg={"white"}
                  variant="outline"
                  onClick={() => handleChange(positionNumber, "")}
                  ml={49}
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
            {/* キーボード表示 */}
            {activeKeyboard === item.positionNumber && (
              <Box ref={closeKeyBoard} mx="auto">
                {detail?.keyboard?.keyboardType === 1 ? (
                  <NumericKeyboard
                    value={detail?.value ?? ""}
                    integerLength={detail?.integerLength}
                    decimalLength={detail?.decimalLength}
                    onChange={(newValue) =>
                      handleChange(positionNumber, newValue)
                    }
                    onConfirm={handleConfirm}
                  />
                ) : (
                  <CollectionKeyboard
                    keyboardValues={detail?.keyboard?.values ?? []}
                    onChange={(newValue) =>
                      handleChange(positionNumber, newValue)
                    }
                  />
                )}
              </Box>
            )}
          </Flex>
        );
      })}
    </>
  );
}
