import {
  useEffect,
  useState,
} from "react";
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
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import styles from "~/styles/common.module.css";

type ExamBodyProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  isInitialDisplay: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
  validationRef: React.RefObject<ValidationHandle | null>;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

const ExamBody = ({
  examItems,
  onRegisterPressed,
  isInitialDisplay,
  onChange,
  validationRef,
}: ExamBodyProps) => {
  if (!examItems || examItems.length === 0) {
    return null;
  }
  //登録ボタンプレスフラグを制御するための変数
  let isRegisterPressed = onRegisterPressed;

  // 定数で定義
  const 身長 = 1;
  const 体重 = 2;
  const 体脂肪率 = 3;
  const BMI = 4;

  // 必要な検査項目が1つも存在しない場合は表示しない
  const bodyItemPositionNumbers = [身長, 体重, 体脂肪率];
  const hasValidDetail = examItems.some(
    (item) =>
      bodyItemPositionNumbers.includes(item.positionNumber ?? 0) &&
      item.examItemDetails?.some((detail) => detail.positionNumber === 1),
  );

  if (!hasValidDetail) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);

  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);

  // BMI計算処理に必要な要素が存在するかを確認
  const bmiItemPositionNumbers = [身長, 体重, BMI];
  const existBMI = bmiItemPositionNumbers.every((position) =>
    examItems.some(
      (item) =>
        item.positionNumber === position &&
        (item.examItemDetails ?? []).some(
          (detail) =>
            detail.positionNumber === 1 &&
            detail.hasOrder === true &&
            !detail.cancelReasonId,
        ),
    ),
  );

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
            const { hasCallback } = validationCheck(item);
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

  // 初回読み込み時にAPIのエラーメッセージを保存する
  useEffect(() => {
    const backendErrorMessages: BackendValidation[] = examItems.map((item) => ({
      itemPositionNumber: item.positionNumber ?? 0,
      examRegistResults: item.examRegistResults ?? [],
    }));
    setBackendValidation(backendErrorMessages);
  }, []);

  useEffect(() => {
    let updatedItems = examItems.map((item) => {
      const validatedData = validationCheck(item).validateResult;
      return validatedData;
    });
    //初期表示以外BMIを計算
    if (!isInitialDisplay) {
      updatedItems = setBmiValue(updatedItems);
    }
    setExamItemsData(updatedItems);
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

  const handleErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      // エラーレベルが高い順にソート
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });
    }
    return item;
  };

  // APIのエラーメッセージに更新する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = backendValidation.find(
      (error) => error.itemPositionNumber === item.positionNumber,
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };

  const validationCheck = (item: InputExamItem) => {
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // BMIはバリデーションチェックを実施しない
    if (item.positionNumber === BMI)
      return { validateResult: item, hasCallback: true };
    const message =
      item.positionNumber === 身長
        ? "身長は"
        : item.positionNumber === 体重
          ? "体重は"
          : item.positionNumber === 体脂肪率
            ? "体脂肪率は"
            : "";
    // [登録する]が押されたときは必須・半角数字チェック、その他は半角数字チェックのみ行う。
    const schema = isRegisterPressed
      ? z
          .string()
          .min(
            1,
            getErrorMessage(
              errorMessages.required,
              item.name ? `${item.name}は` : message,
            ), // 必須チェック
          )
          .refine((value) => /^\d+(\.\d+)?$/.test(value), {
            message: getErrorMessage(
              errorMessages.numericString,
              item.name ? `${item.name}は` : message,
            ),
          })
      : z.string().refine((value) => /^(\d+(\.\d+)?|)$/.test(value), {
          message: getErrorMessage(
            errorMessages.numericString,
            item.name ? `${item.name}は` : "",
          ),
        });

    // バリデーション対象データを取得
    const targetDetail = item.examItemDetails?.find(
      (item) => item.positionNumber === 1,
    );
    const result = schema.safeParse(targetDetail?.value);

    // バリデーションが失敗した場合
    if (
      !result.success &&
      targetDetail?.hasOrder &&
      !targetDetail.cancelReasonId
    ) {
      const error = result.error.errors[0]; // 最初のエラーだけ取得
      componentErrorMessage.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    }

    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...(setRangesErrorMessage(item) ?? []));
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック
    const isCallback = !componentErrorMessage.some(
      (error) => error.errorLevel === InputErrorLevel.異常,
    );
    // エラーメッセージをexamItemに保存
    const resultItem: InputExamItem = {
      ...item,
      examRegistResults: item.examRegistResults
        ? item.examRegistResults.concat(componentErrorMessage)
        : componentErrorMessage,
    };
    // バリデーションチェックを行ったexamItemと、
    // コールバックを判断するフラグを返す
    const ValidationResult = {
      validateResult: handleErrorMessage(resultItem),
      hasCallback: isCallback,
    };
    return ValidationResult;
  };

  // BMI計算処理
  const setBmiValue = (updatedExamItems: InputExamItem[]): InputExamItem[] => {
    // 身長、体重、BMIの明細が存在しない場合はそのまま返す
    if (!existBMI) return updatedExamItems;

    const heightValue =
      updatedExamItems
        .find((item) => item.positionNumber === 身長)
        ?.examItemDetails?.find((detail) => detail.positionNumber === 1)
        ?.value ?? "0";
    const weightValue =
      updatedExamItems
        .find((item) => item.positionNumber === 体重)
        ?.examItemDetails?.find((detail) => detail.positionNumber === 1)
        ?.value ?? "0";

    // BMIの計算
    const height = Number.parseFloat(heightValue) / 100;
    const weight = Number.parseFloat(weightValue);
    const bmi = weight / height ** 2;
    // NaN または Infinity の場合に 0 を代入
    const validBmi = Number.isFinite(bmi) ? bmi : 0;

    return updatedExamItems.map((item) => {
      if (item.positionNumber === BMI) {
        item.examItemDetails = item.examItemDetails?.map((detail) => {
          const decimalLength = detail.decimalLength ?? 0;
          const integerLength = detail.integerLength ?? 0;

          if (detail.positionNumber === 1 && bmi !== null) {
            // 四捨五入して指定された桁数までの値を作成
            const roundedBMI =
              validBmi === 0 ? "" : validBmi.toFixed(decimalLength); // 小数点以下で四捨五入

            // 最大表示桁数を制限
            const maxDigits =
              integerLength + decimalLength + (decimalLength > 0 ? 1 : 0); // 小数点を含む桁数
            const truncatedValue = roundedBMI.slice(0, maxDigits);

            return { ...detail, value: truncatedValue };
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
        item.examItemDetails = item.examItemDetails?.map((detail) => {
          if (detail.positionNumber === 1) {
            return { ...detail, value: value };
          }
          return detail;
        });
      }

      // バリデーションチェックを実施
      const { validateResult } = validationCheck(item);

      // バリデーション結果を反映
      Object.assign(item, validateResult);
    }

    // BMIの計算と設定処理
    const bmiUpdatedExamItems = setBmiValue(updatedExamItems);
    // 更新されたデータをステートに設定
    setExamItemsData(bmiUpdatedExamItems);

    onChange(bmiUpdatedExamItems);
  };

  // 必要なpositionNumberのリスト
  const requiredPositions = [身長, 体重, 体脂肪率, BMI];

  // データ処理
  const targetExamItems: InputExamItem[] = requiredPositions.map(
    (positionNumber) => {
      // 該当するexamItemを検索
      let examItem = examItemsData.find(
        (item) => item.positionNumber === positionNumber,
      );

      // 該当するexamItemがなければデフォルトを設定
      if (!examItem) {
        examItem = {
          positionNumber,
          name:
            positionNumber === 身長
              ? "身長"
              : positionNumber === 体重
                ? "体重"
                : positionNumber === 体脂肪率
                  ? "体脂肪率"
                  : "BMI",
          examItemDetails: [{ positionNumber: 1 }],
          examRegistResults: [],
        };
      }

      return examItem;
    },
  );

  return (
    <>
      {targetExamItems.map((item) => {
        const { positionNumber, name, examRegistResults = [] } = item;
        const detail =
          item.examItemDetails?.find((detail) => detail.positionNumber === 1) ??
          {};
        // グレーアウト表示判定
        const isDisabled = !detail?.hasOrder || !!detail?.cancelReasonId;
        const isBMI = item.positionNumber === BMI;
        const hasError = examRegistResults?.some(
          (x) => x.errorLevel === InputErrorLevel.異常,
        );
        const hasWarning = examRegistResults?.some(
          (x) => x.errorLevel === InputErrorLevel.警告,
        );

        return (
          <Flex
            key={positionNumber}
            justify="flex-start"
            align="flex-start"
            direction="column"
            w={1038}
          >
            <Flex align="center" gap="md">
              <Paper
                w={274}
                h={80}
                bg={isBMI ? "white" : "gray02"}
                c={isBMI ? "gray02" : "white"}
                radius="itemName"
                py={16}
              >
                <Text size="lg" fw={700} ta="center">
                  {name
                    ? name.slice(0, 8)
                    : positionNumber === 身長
                      ? "身長"
                      : positionNumber === 体重
                        ? "体重"
                        : positionNumber === 体脂肪率
                          ? "体脂肪率"
                          : positionNumber === BMI
                            ? "BMI"
                            : ""}
                </Text>
              </Paper>

              {isBMI ? (
                <Text
                  w={340}
                  h={80}
                  size="inputComponent"
                  ta="right"
                  c={
                    isDisabled
                      ? "gray02"
                      : hasError
                        ? "error"
                        : hasWarning
                          ? "warning"
                          : "black"
                  }
                  px={32}
                >
                  {detail?.value}
                </Text>
              ) : (
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
              <Stack w={173} h={80} gap={4} justify="space-between">
                <Box>
                  {detail?.prevValue && (
                    <Text fw={700} mt={0}>
                      (前回：{detail.prevValue})
                    </Text>
                  )}
                </Box>
                <Box>
                  {!isBMI && (
                    <Text size="xs" mb={0}>
                      {detail?.unit}
                    </Text>
                  )}
                </Box>
              </Stack>

              {!isBMI && (
                <Button
                  w={154}
                  h={64}
                  size="lg"
                  bg={"white"}
                  variant="outline"
                  bd={"2px,solid"}
                  onClick={() => {
                    if (!isDisabled) {
                      handleChange(positionNumber, "");
                    }
                  }}
                  disabled={isDisabled}
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
            {activeKeyboard === item.positionNumber && (
              <Box ref={closeKeyBoard} mx="auto">
                {detail?.keyboard?.keyboardType === KeyboardType.テンキー ? (
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
                    value={detail.value ?? ""}
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
};

export default ExamBody;
