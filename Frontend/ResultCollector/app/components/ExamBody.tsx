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
import type { ExamItemDetail, InputExamItem } from "~/domain/wellship.schemas";
import NumericKeyboard from "./NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
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

  // キーボードの表示状態を管理する
  const [showKeyboards, setShowKeyboards] = useState(
    examItems.map(() => false), // 初期状態はすべて false
  );
  const closeKeyBoard = useClickOutside(() =>
    setShowKeyboards(Array(examItems.length).fill(false)),
  );
  const handleConfirm = () => {
    setShowKeyboards(Array(examItems.length).fill(false));
  };
  const toggleKeyboard = (index: number) => {
    setShowKeyboards((prev) => {
      const updated = [...prev];
      updated[index] = !updated[index];
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
          index === self.findIndex((r) => r.description === result.description),
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
            `${item.name}は`,
          ),
        }),
    });

    // バリデーション対象データを取得
    const valueToValidate = item.examItemDetails?.[0]?.value || "";
    const result = schema.safeParse({ value: valueToValidate });

    // エラーメッセージを更新
    let updatedErrors = item.examRegistResults || [];

    // バリデーションが失敗した場合
    if (!result.success) {
      const error = result.error.errors[0]; // 最初のエラーだけ取得
      updatedErrors.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    } else {
      // 必須エラーを削除するために確認
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

  // 表示時の小数点追加処理
  const formatDecimalValue = (
    value: string,
    integerLength: number,
    decimalLength: number,
  ): string => {
    if (!value || integerLength <= 0 || decimalLength < 0) return value;
    const totalLength = integerLength + decimalLength;
    const paddedValue = value.padStart(totalLength, "0");

    let integerPart = paddedValue.slice(0, integerLength);
    const decimalPart = paddedValue.slice(integerLength, totalLength);

    // 整数部が不足する場合、0で補填
    if (integerPart.length < integerLength) {
      integerPart =
        "0".repeat(integerLength - integerPart.length) + integerPart;
    }
    let formattedValue = `${integerPart}.${decimalPart}`;

    // 整数部が1未満の場合、整数部の先頭ゼロは除去しない
    if (Number.parseInt(formattedValue) < 1) {
      formattedValue = `0.${decimalPart}`;
    } else {
      // 整数部の先頭にゼロがついている場合、それを除去
      formattedValue = formattedValue.replace(/^0+/, "");
    }

    return formattedValue;
  };

  //変更イベント
  const handleChange = (positionNumber: number | undefined, value: string) => {
    const updatedExamItems = [...examItemsData];

    // 該当するアイテムを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // 該当するexamItemDetailsの最初のvalueを更新
        item.examItemDetails = item.examItemDetails?.map((detail, idx) => {
          const decimalLength = detail.decimalLength ?? 0;
          const integerLength = detail.integerLength ?? 0;

          const cleanedValue = formatDecimalValue(
            value,
            integerLength,
            decimalLength,
          );

          if (idx === 0) {
            return { ...detail, value: cleanedValue };
          }
          return detail;
        });
      }

      //BMIの計算処理
      const height =
        Number.parseFloat(
          updatedExamItems[0].examItemDetails?.[0].value ?? "0",
        ) / 100;
      const weight = Number.parseFloat(
        updatedExamItems[1].examItemDetails?.[0].value ?? "0",
      );
      const bmi = weight && height ? String(weight / height ** 2) : "0";
      // BMIの値を設定
      if (item.positionNumber === 4) {
        item.examItemDetails = item.examItemDetails?.map((detail, idx) => {
          const decimalLength = detail.decimalLength ?? 0;
          const integerLength = detail.integerLength ?? 0;
          const maxDigits = decimalLength + integerLength + 1;
          if (idx === 0) {
            if (detail.integerLength) {
              return { ...detail, value: bmi.slice(0, maxDigits) };
            }
            return { ...detail, value: bmi };
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
            getErrorMessage(errorMessages.numericString, `${item.name}は`),
      ),
    );
    if (!isValidatedError) {
      onChange(updatedExamItems);
    }
  };

  // 小数点と先頭の0を除去して数値部分だけを取得する処理
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
                  onFocus={() => toggleKeyboard(index)}
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
            {showKeyboards[index] && (
              <Box ref={closeKeyBoard} mx="auto">
                <NumericKeyboard
                  value={removeDecimalAndLeadingZero(detail?.value ?? "")}
                  onChange={(newValue) =>
                    handleChange(positionNumber, newValue)
                  }
                  onConfirm={handleConfirm}
                />
              </Box>
            )}
          </Flex>
        );
      })}
    </>
  );
}
