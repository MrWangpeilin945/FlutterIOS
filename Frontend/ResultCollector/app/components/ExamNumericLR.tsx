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
} from "@mantine/core";
import { useClickOutside } from "@mantine/hooks";
import Keyboard from "~/components/NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericLRProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newValue: InputExamItem) => void;
};

export default function ExamNumericLR({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericLRProps) {
  // 必要な引数のチェック
  if (!examItems || examItems.length === 0) {
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
  const toggleKeyboard = (positionNumber: number) => {
    setActiveKeyboard((prevNumber) =>
      prevNumber === positionNumber ? null : positionNumber
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
          index === self.findIndex((r) => r.description === result.description)
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
          error.description?.includes("正常値ではありません")
      )
    );

    if (!isValidatedError) {
      onChange(bmiUpdatedExamItems);
    }
  };

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
            {examItems[0].name}
          </Title>
        </Paper>
        <Group gap={16}>
          {firstExamItemDetails?.map((detail, index) => (
            <Stack key={index}>
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
                  {detail.name}
                </Title>
              </Paper>
              <Group>
                <TextInput
                  classNames={{
                    input: `${styles["input-textbox"]} ${
                      errMessages?.some(
                        (x) => x.errorLevel === InputErrorLevel.異常
                      )
                        ? `${styles["input-error"]}`
                        : errMessages?.some(
                            (x) => x.errorLevel === InputErrorLevel.警告
                          )
                        ? `${styles["input-warning"]}`
                        : ""
                    }`,
                  }}
                  w={"340"}
                  radius={"md"}
                  size="inputComponent"
                  value={formatDecimalValue(examValues[index], index)}
                  onClick={() => handleTextboxClick(index)}
                  onChange={(e) => {
                    handleTextChange(e, index);
                  }}
                />
                {/* TODO:前回値のマックス横幅設定 */}
                <Stack gap="0">
                  <Text size="md" fw="700" maw={"172"}>
                    {detail.prevValue ? `(前回: ${detail.prevValue})` : ""}
                  </Text>
                  <Text size="xs" fw="400">
                    {detail.unit}
                  </Text>
                </Stack>
              </Group>
            </Stack>
          ))}
        </Group>
        {/* エラーメッセージの表示 */}
        {(errMessages || []).map((error, index) => (
          <Group
            key={index}
            c={error.errorLevel === InputErrorLevel.異常 ? "error" : "warning"}
          >
            <IconExclamationCircleFilled size={"1.7rem"} />
            <Text>{error.description}</Text>
          </Group>
        ))}
        <Group>
          {firstExamItemDetails?.map((detail, index) => (
            <Stack key={index}>
              <Box w={540}>
                {showKeyboards[index] && (
                  <div ref={closeKeyBoard}>
                    <Keyboard
                      value={examValues[index]}
                      onChange={(e: string) => {
                        handleKeyChange(e, index);
                      }}
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
