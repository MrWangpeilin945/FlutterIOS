import { useEffect, useRef, useState } from "react";
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
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import { z } from "zod";
import { InputErrorLevel, KeyboardType } from "~/domain/enums";
import type {
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import type { ValidationHandle } from "~/routes/consult-input.$consultnumber";
import NumericKeyboard from "./NumericKeyboard";
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import styles from "~/styles/common.module.css";

type ExamNumericRepeatWithSameValueProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
  validationRef: React.RefObject<ValidationHandle | null>;
};

type ErrorMap = Record<number, ExamRegistResult[]>;

const ExamNumericRepeatWithSameValue = ({
  examItems,
  onRegisterPressed,
  onChange,
  validationRef,
}: ExamNumericRepeatWithSameValueProps) => {
  const [examItemsData, setExamItemsData] = useState(examItems);
  if (!examItemsData || examItemsData.length === 0) {
    return null;
  }
  const firstItem = examItemsData.find((item) => item.positionNumber === 1);
  if (!((firstItem?.examItemDetails?.length ?? 0) > 0)) {
    return null;
  }

  //登録ボタンプレスフラグを制御するための変数
  let isRegisterPressed = onRegisterPressed;
  //エラー状態管理
  const [errorMap, setErrorMap] = useState<ErrorMap>({});
  //同一値チェックフラグ
  const [isSameValue, setIsSameValue] = useState(true);
  //テキストボックスフォーカス制御
  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

  // キーボードの表示インデックスを状態として管理する
  const [activeKeyboard, setActiveKeyboard] = useState<number | null>(null);

  // 画面からバリデーションチェックを行う
  useEffect(() => {
    if (validationRef) {
      validationRef.current = {
        triggerValidation: () => {
          isRegisterPressed = true;
          let hasError = false;
          for (const item of examItemsData) {
            const hasCallback = validationCheck(item);
            if (!hasCallback) {
              hasError = true;
              break;
            }
          }
          return { hasError };
        },
      };
    }
  }, [validationRef]);

  useEffect(() => {
    validationCheck(firstItem ?? {});
    setExamItemsData(examItems);
  }, [onRegisterPressed, examItems]);

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

  const handleErrorMessage = (errors: ExamRegistResult[]) => {
    if (errors) {
      // エラーレベルが高い順にソート
      errors.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });
    }
    return errors;
  };

  //エラーメッセージの更新
  const updateErrors = (positionNumber: number, errors: ExamRegistResult[]) => {
    setErrorMap((prev) => ({
      ...prev,
      [positionNumber]: errors,
    }));
  };

  // 同一値チェック
  const checkSameValue = (item: InputExamItem): boolean => {
    const values = (item.examItemDetails ?? [])
      .filter(
        (detail) =>
          detail.hasOrder &&
          !detail.cancelReasonId &&
          detail.value !== "" &&
          detail.value !== undefined,
      )
      .map((detail) => detail.value);

    // 完全一致で比較
    const hasSameValue =
      values.length <= 1 ? true : values.every((v) => v === values[0]);

    setIsSameValue(hasSameValue);
    return hasSameValue;
  };

  const validationCheck = (item: InputExamItem) => {
    let hasComponentError = false;

    for (const {
      positionNumber,
      name,
      value,
      hasOrder,
      cancelReasonId,
      examNormalValueRanges,
    } of item.examItemDetails ?? []) {
      if (!hasOrder || !!cancelReasonId) {
        continue;
      }

      const componentErrorMessage: ExamRegistResult[] = [];
      const label = name ? `${name}は` : "";

      // [登録する]が押されたときは必須・半角数字チェック、その他は半角数字チェックのみ行う。
      const schema = isRegisterPressed
        ? z
            .string()
            .min(1, getErrorMessage(errorMessages.required, label) ?? "")
            .refine((value) => /^\d+(\.\d+)?$/.test(value), {
              message:
                getErrorMessage(errorMessages.numericString, label) ?? "",
            })
        : z.string().refine((value) => /^(\d+(\.\d+)?)?$/.test(value), {
            message: getErrorMessage(errorMessages.numericString, label) ?? "",
          });

      const result = schema.safeParse(value ?? "");

      if (!result.success) {
        hasComponentError = true;
        const error = result.error.errors[0];
        componentErrorMessage.push({
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        });
      }

      if (value) {
        const numericValue = Number.parseFloat(value);
        if (!Number.isNaN(numericValue)) {
          for (const range of examNormalValueRanges ?? []) {
            if (
              typeof range.minValue !== "number" ||
              typeof range.maxValue !== "number"
            ) {
              continue;
            }
            if (
              numericValue >= range.minValue &&
              numericValue <= range.maxValue
            ) {
              hasComponentError = true;
              if (range.errorLevel === InputErrorLevel.異常) {
                componentErrorMessage.push({
                  description: "入力に誤りがあります。",
                  errorLevel: range.errorLevel,
                });
              } else if (range.errorLevel === InputErrorLevel.警告) {
                componentErrorMessage.push({
                  description: "入力値を確認してください。",
                  errorLevel: range.errorLevel,
                });
              }
            }
          }
        }
      }

      const sortedErrorMessage = handleErrorMessage(componentErrorMessage);
      updateErrors(positionNumber ?? 0, sortedErrorMessage);
    }

    const isSameValue = checkSameValue(item);

    return isSameValue && !hasComponentError;
  };

  //変更イベント
  const handleChange = (positionNumber: number | undefined, value: string) => {
    const updatedExamItems = [...examItemsData];

    // 該当する明細を更新
    for (const item of updatedExamItems) {
      // item.positionNumber が 1 の item の中から該当 detail を更新
      if (item.positionNumber === 1) {
        item.examItemDetails = item.examItemDetails?.map((detail) =>
          detail.positionNumber === positionNumber
            ? { ...detail, value }
            : detail,
        );

        // バリデーションチェックを実施
        validationCheck(item);

        break;
      }
    }

    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);

    onChange(updatedExamItems);
  };

  return (
    <Flex
      justify="flex-start"
      align="flex-start"
      direction="column"
      w={1038}
      gap={16}
    >
      {firstItem?.examItemDetails?.map((detail, index) => {
        // グレーアウト表示判定
        const isDisabled = !detail?.hasOrder || !!detail?.cancelReasonId;
        const detailErrors = errorMap[detail.positionNumber ?? 0] ?? [];

        const hasError =
          firstItem.examRegistResults?.some(
            (x) => x.errorLevel === InputErrorLevel.異常,
          ) ||
          detailErrors.some((x) => x.errorLevel === InputErrorLevel.異常) ||
          !isSameValue;
        const hasWarning = firstItem.examRegistResults?.some(
          (x) => x.errorLevel === InputErrorLevel.警告,
        );

        return (
          <Flex
            key={detail.positionNumber}
            justify="flex-start"
            align="flex-start"
            direction="column"
            w={1038}
          >
            <Flex align="center" gap="md">
              <Paper
                w={274}
                h={80}
                bg="gray02"
                c="white"
                radius="itemName"
                py={16}
              >
                <Text size="lg" fw={700} ta="center">
                  {detail.name?.slice(0, 8)}
                </Text>
              </Paper>

              <TextInput
                classNames={{
                  input: `${styles["input-textbox"]} ${
                    hasError
                      ? `${styles["input-error"]}`
                      : hasWarning
                        ? `${styles["input-warning"]}`
                        : ""
                  }`,
                }}
                w={528}
                radius="md"
                size="inputComponent"
                bg={isDisabled ? "gray03" : ""}
                c={isDisabled ? "gray02" : ""}
                value={detail?.value}
                onChange={(e) =>
                  handleChange(detail.positionNumber, e.currentTarget.value)
                }
                onClick={() => toggleKeyboard(detail.positionNumber ?? 0)}
                disabled={isDisabled}
                ref={(el) => {
                  inputRefs.current[index] = el;
                }}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    const next = inputRefs.current[index + 1];
                    next?.focus(); // 次の入力にフォーカス
                    e.preventDefault();
                    setActiveKeyboard(null);
                  }
                }}
              />

              <Button
                w={154}
                h={64}
                size="lg"
                bg={"white"}
                variant="outline"
                bd={"2px,solid"}
                onClick={() => {
                  if (!isDisabled) {
                    handleChange(detail.positionNumber, "");
                  }
                }}
                disabled={isDisabled}
                ml={48}
                tabIndex={-1}
              >
                クリア
              </Button>
            </Flex>
            {/* エラーメッセージ（明細） */}
            {errorMap[detail.positionNumber ?? 0]?.map((error, idx) => {
              const isWarning = error.errorLevel === InputErrorLevel.警告;
              return (
                <Group key={idx} c={isWarning ? "warning" : "error"}>
                  {isWarning ? (
                    <IconExclamationCircleFilled size={32} />
                  ) : (
                    <IconSquareRoundedXFilled size={32} />
                  )}
                  <Text size="sm" fw={700}>
                    {error.description}
                  </Text>
                </Group>
              );
            })}
            {/* キーボード表示 */}
            {activeKeyboard === detail.positionNumber && (
              <Box ref={closeKeyBoard} mx="auto">
                {detail?.keyboard?.keyboardType === KeyboardType.テンキー ||
                !detail.keyboard?.keyboardType ? (
                  <NumericKeyboard
                    value={detail?.value ?? ""}
                    integerLength={detail?.integerLength}
                    decimalLength={detail?.decimalLength}
                    onChange={(newValue) =>
                      handleChange(detail.positionNumber, newValue)
                    }
                    onConfirm={handleConfirm}
                  />
                ) : (
                  <CollectionKeyboard
                    value={detail.value ?? ""}
                    keyboardValues={detail?.keyboard?.values ?? []}
                    onChange={(newValue) =>
                      handleChange(detail.positionNumber, newValue)
                    }
                  />
                )}
              </Box>
            )}
          </Flex>
        );
      })}
      <Stack gap={0}>
        {/* エラーメッセージ（同一値） */}
        {!isSameValue && (
          <Group c="error">
            <IconSquareRoundedXFilled size={32} />
            <Text size="sm" fw={700}>
              入力された値が一致していません。
            </Text>
          </Group>
        )}
        {/* エラーメッセージ（項目） */}
        {firstItem?.examRegistResults?.map((error, idx) => {
          const isWarning = error.errorLevel === InputErrorLevel.警告;
          return (
            <Group key={idx} c={isWarning ? "warning" : "error"}>
              {isWarning ? (
                <IconExclamationCircleFilled size={32} />
              ) : (
                <IconSquareRoundedXFilled size={32} />
              )}
              <Text size="sm" fw={700}>
                {error.description}
              </Text>
            </Group>
          );
        })}
      </Stack>
    </Flex>
  );
};

export default ExamNumericRepeatWithSameValue;
