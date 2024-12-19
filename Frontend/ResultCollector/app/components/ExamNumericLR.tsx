import type React from "react";
import { z } from "zod";
import { useEffect, useState } from "react";
import {
  Group,
  Stack,
  Title,
  Paper,
  Text,
  TextInput,
  Flex,
  Box,
  Button,
} from "@mantine/core";
import { useClickOutside } from "@mantine/hooks";
import NumericKeyboard from "~/components/NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import type {
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericLRProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

export default function ExamNumericLR({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericLRProps) {
  // 引数のチェック
  if (!examItems || examItems.length === 0) {
    return null;
  }
  const firstPosition = examItems.find((item) => item.positionNumber === 1);

  if (
    !firstPosition?.examItemDetails?.some(
      (detail) => detail.positionNumber === 1
    ) ||
    !firstPosition?.examItemDetails?.some(
      (detail) => detail.positionNumber === 2
    )
  ) {
    return null;
  }
  const [examItemsData, setExamItemsData] = useState(examItems);

  // キーボードの表示インデックスを状態として管理する
  const [showKeyboards, setShowKeyboards] = useState<{
    left: boolean;
    right: boolean;
  }>({
    left: false,
    right: false,
  });

  // キーボードの表示/非表示をトグルする関数
  const toggleKeyboard = (detailPositionNumber: number) => {
    setShowKeyboards((prevKeyboards) => {
      // positionNumberによって切り替えるフラグを制御する
      if (detailPositionNumber === 1 || detailPositionNumber === 2) {
        return {
          ...prevKeyboards,
          left:
            detailPositionNumber === 1
              ? !prevKeyboards.left
              : prevKeyboards.left,
          right:
            detailPositionNumber === 2
              ? !prevKeyboards.right
              : prevKeyboards.right,
        };
      }
      return prevKeyboards;
    });
  };
  // キーボードのACボタン押下時にキーボードを非表示にする
  const handleConfirm = () => {
    setShowKeyboards({ left: false, right: false });
  };
  const closeKeyBoard = useClickOutside(() =>
    setShowKeyboards({ left: false, right: false })
  );

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
    // positionNumberが異なる場合、処理を行わない
    if (item.positionNumber !== 1) {
      return item;
    }
    // エラーメッセージを更新
    let updatedErrors = item.examRegistResults || [];

    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];

    // detailsのpositionNumberが1と2のものについてバリデーションチェックを行う
    for (const { name, positionNumber, value } of item.examItemDetails ?? []) {
      if (positionNumber !== 1 && positionNumber !== 2) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }
      // 必須チェックと半角数字チェックを一度に行うスキーマ
      const schema = z
        .string()
        .min(
          1,
          getErrorMessage(errorMessages.required, `${item.name}:${name}は`)
        ) // 必須チェック
        .refine((value) => /^\d+(\.\d+)?$/.test(value), {
          message: getErrorMessage(
            errorMessages.numericString,
            `${item.name}:${name}は`
          ),
        });

      // バリデーション対象データを取得
      const valueToValidate = value;
      const result = schema.safeParse(valueToValidate);

      // 必須エラーを削除
      const errorMessageRequired = getErrorMessage(
        errorMessages.required,
        `${item.name}:${name}は`
      );
      updatedErrors = updatedErrors.filter(
        (error) => error.description !== errorMessageRequired
      );

      // 半角数字エラーを削除
      const errorMessageNumeric = getErrorMessage(
        errorMessages.numericString,
        `${item.name}:${name}は`
      );
      updatedErrors = updatedErrors.filter(
        (error) => error.description !== errorMessageNumeric
      );

      // バリデーションが失敗した場合
      if (!result.success) {
        const error = result.error.errors[0]; // 最初のエラーだけ取得
        componentErrorMessage.push({
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        });
      }
    }
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック

    const prevItem = {
      ...item,
      examRegistResults: updatedErrors.concat(componentErrorMessage),
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

  //変更イベント
  const handleChange = (
    value: string,
    positionNumber: number | undefined,
    detailsPositionNumber?: number
  ) => {
    // 対象のpositionNumberか確認
    if (positionNumber !== 1) {
      return null;
    }
    const updatedExamItems = [...examItemsData];

    // 該当するアイテムを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // クリアボタン用の処理
        if (detailsPositionNumber === undefined) {
          item.examItemDetails = item.examItemDetails?.map((detail) => ({
            ...detail,
            value: value,
          }));
        } else {
          // 該当するdetailの値を更新
          item.examItemDetails = item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailsPositionNumber
              ? { ...detail, value: value }
              : detail
          );
        }
      }
      // バリデーションチェックを実施
      const validatedItem = validationCheck(item);
      // バリデーション結果を反映
      Object.assign(item, validatedItem);
    }

    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);
    // API以外の異常エラーが存在するか確認
    const isValidatedError = updatedExamItems.some((item) =>
      item.examRegistResults?.some(
        (error) =>
          error.description ===
            getErrorMessage(errorMessages.required, `${item.name}は`) ||
          error.description ===
            getErrorMessage(errorMessages.numericString, `${item.name}は`) ||
          error.description === "入力に誤りがあります。"
      )
    );

    if (!isValidatedError) {
      onChange(updatedExamItems);
    }
  };

  const firstPositionItem = examItemsData.find(
    (item) => item.positionNumber === 1
  );
  const { positionNumber, examItemDetails, examRegistResults, name } =
    firstPositionItem ?? {};
  const targetDetails = examItemDetails?.filter(
    (detail) => detail.positionNumber === 1 || detail.positionNumber === 2
  );
  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Stack>
        <Paper
          w={274}
          h={80}
          className={styles["basic-grey"]}
          radius="itemName"
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Title size="lg" fw={700}>
            {name}
          </Title>
        </Paper>
        <Group gap={16}>
          {targetDetails?.map((detail) => {
            const {
              hasOrder,
              positionNumber: detailPositionNumber,
              name: detailName,
              value,
              prevValue,
              unit,
              cancelReasonId,
            } = detail;
            const isDisabled = !hasOrder || !!cancelReasonId;
            return (
              <Stack key={detailPositionNumber}>
                <Paper
                  w={524}
                  h={51}
                  className={styles["basic-grey"]}
                  radius="itemName"
                  style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                  }}
                >
                  <Title size="lg" fw={700}>
                    {detailName}
                  </Title>
                </Paper>
                <Group>
                  <TextInput
                    classNames={{
                      input: `${styles["input-textbox"]} ${
                        isDisabled
                          ? ""
                          : firstPositionItem?.examRegistResults?.some(
                              (x) => x.errorLevel === InputErrorLevel.異常
                            )
                          ? `${styles["input-error"]}`
                          : firstPositionItem?.examRegistResults?.some(
                              (x) => x.errorLevel === InputErrorLevel.警告
                            )
                          ? `${styles["input-warning"]}`
                          : ""
                      }`,
                    }}
                    w={340}
                    radius="md"
                    size="inputComponent"
                    value={value}
                    onClick={() => toggleKeyboard(detailPositionNumber ?? 0)}
                    onChange={(e) =>
                      handleChange(
                        e.currentTarget.value,
                        positionNumber ?? 0,
                        positionNumber ?? 0
                      )
                    }
                    disabled={isDisabled}
                  />
                  {/* TODO:前回値のマックス横幅設定 */}
                  <Stack gap="0">
                    <Text size="md" fw="700" maw={172}>
                      {prevValue ? `(前回: ${prevValue})` : ""}
                    </Text>
                    <Text size="xs" fw="400">
                      {unit}
                    </Text>
                  </Stack>
                </Group>
              </Stack>
            );
          })}
        </Group>
        <Button
          w={154}
          h={64}
          mt={40}
          ml={914}
          size="lg"
          bg={"white"}
          variant="outline"
          onClick={() => handleChange("", positionNumber)}
          tabIndex={-1}
        >
          クリア
        </Button>
        {/* エラーメッセージの表示 */}
        {(examRegistResults || []).map((error, index) => (
          <Group
            key={index}
            c={error.errorLevel === InputErrorLevel.異常 ? "error" : "warning"}
          >
            <IconExclamationCircleFilled size={32} />
            <Text>{error.description}</Text>
          </Group>
        ))}
        <Group>
          {targetDetails?.map((detail) => (
            <Stack key={detail.positionNumber}>
              <Box w={540}>
                {showKeyboards[
                  detail.positionNumber === 1 ? "left" : "right"
                ] && (
                  <div ref={closeKeyBoard}>
                    <NumericKeyboard
                      value={detail?.value ?? ""}
                      integerLength={detail.integerLength}
                      decimalLength={detail.decimalLength}
                      onChange={(newValue) =>
                        handleChange(
                          newValue,
                          positionNumber ?? 0,
                          detail.positionNumber ?? 0
                        )
                      }
                      onConfirm={handleConfirm}
                    />
                  </div>
                )}
              </Box>
            </Stack>
          ))}
        </Group>
      </Stack>
    </Flex>
  );
}
