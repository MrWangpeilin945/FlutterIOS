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
import { InputErrorLevel, KeyboardType } from "~/domain/enums";
import type {
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import NumericKeyboard from "./NumericKeyboard";
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import styles from "~/styles/common.module.css";
import React from "react";

type BP2Props = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
};

export default function ExamBP2({
  examItems,
  onRegisterPressed,
  onChange,
}: BP2Props) {
  // 定数で定義
  const 血圧1回目 = 1;
  const 血圧2回目 = 2;
  const 平均値 = 3;
  const 収縮期 = 1;
  const 拡張期 = 2;

  // 引数のチェック
  if (!examItems || examItems.length === 0) {
    return null;
  }
  // positionNumberが1のexamItemが存在し、かつexamItemDetailsに必要な値が存在するかのチェック
  const hasPosition1 = examItems.find((item) => item.positionNumber === 1);
  const hasContractionDetail1 = hasPosition1?.examItemDetails?.some(
    (detail) => detail.positionNumber === 収縮期
  );
  const hasExpansionDetail1 = hasPosition1?.examItemDetails?.some(
    (detail) => detail.positionNumber === 拡張期
  );
  const isValidPosition1 =
    hasPosition1 && hasContractionDetail1 && hasExpansionDetail1;

  // positionNumberが2のexamItemが存在し、かつexamItemDetailsに必要な値が存在するかのチェック
  const hasPosition2 = examItems.find(
    (item) => item.positionNumber === 血圧2回目
  );
  const hasContractionDetail2 = hasPosition2?.examItemDetails?.some(
    (detail) => detail.positionNumber === 収縮期
  );
  const hasExpansionDetail2 = hasPosition2?.examItemDetails?.some(
    (detail) => detail.positionNumber === 拡張期
  );
  const isValidPosition2 =
    hasPosition2 && hasContractionDetail2 && hasExpansionDetail2;

  // 両方を満たさない時、nullを返す
  if (!isValidPosition1 && !isValidPosition2) {
    return null;
  }
  const [examItemsData, setExamItemsData] = useState(examItems);
  // キーボードの表示状態を管理する
  const [showKeyboards, setShowKeyboards] = useState<{
    first: {
      contraction: boolean;
      expansion: boolean;
    };
    second: {
      contraction: boolean;
      expansion: boolean;
    };
  }>({
    first: {
      contraction: false,
      expansion: false,
    },
    second: {
      contraction: false,
      expansion: false,
    },
  });

  // キーボード外部をクリックした際に非表示にする
  const closeKeyBoard = useClickOutside(() => {
    setShowKeyboards({
      first: {
        contraction: false,
        expansion: false,
      },
      second: {
        contraction: false,
        expansion: false,
      },
    });
  });

  // キーボードの確定ボタン押下時に非表示にする
  const handleConfirm = () => {
    setShowKeyboards({
      first: {
        contraction: false,
        expansion: false,
      },
      second: {
        contraction: false,
        expansion: false,
      },
    });
  };

  // キーボードの表示/非表示をトグルする関数
  const toggleKeyboard = (
    positionNumber: number,
    detailPositionNumber: number
  ) => {
    setShowKeyboards((prev) => {
      const updated = { ...prev };
      if (positionNumber === 血圧1回目) {
        if (detailPositionNumber === 収縮期) {
          updated.first = {
            ...updated.first,
            contraction: !updated.first.contraction,
          };
        } else if (detailPositionNumber === 拡張期) {
          updated.first = {
            ...updated.first,
            expansion: !updated.first.expansion,
          };
        }
      } else if (positionNumber === 血圧2回目) {
        if (detailPositionNumber === 収縮期) {
          updated.second = {
            ...updated.second,
            contraction: !updated.second.contraction,
          };
        } else if (detailPositionNumber === 拡張期) {
          updated.second = {
            ...updated.second,
            expansion: !updated.second.expansion,
          };
        }
      }
      return updated;
    });
  };

  // エラーメッセージをエラーレベルで並び替え
  const sortErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });
    }
    return item;
  };

  // APIからのエラーメッセージを保存
  const APIErrors = examItems.map((item) => ({
    positionNumber: item.positionNumber,
    examRegistResults: item.examRegistResults || [],
  }));

  // 引数のexamItemのpositionNumberを参照し、エラーメッセージを初期化する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = APIErrors.find(
      (error) => error.positionNumber === item.positionNumber
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };

  // バリデーションチェック
  const validationCheck = (item: InputExamItem) => {
    // positionNumberが対象でなければ処理を終える
    if (
      item.positionNumber !== 血圧1回目 &&
      item.positionNumber !== 血圧2回目
    ) {
      return { validateResult: item, hasCallback: true };
    }
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // detailsのpositionNumberが1と2のものについてバリデーションチェックを行う
    for (const { name, positionNumber, value } of item.examItemDetails ?? []) {
      if (positionNumber !== 収縮期 && positionNumber !== 拡張期) {
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

      // バリデーションが失敗した場合
      if (!result.success) {
        const error = result.error.errors[0]; // 最初のエラーだけ取得
        componentErrorMessage.push({
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        });
      }
    }

    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...setRangesErrorMessage(item));
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック
    const isCallback = !componentErrorMessage.some(
      (error) => error.errorLevel === InputErrorLevel.異常
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
      validateResult: sortErrorMessage(resultItem),
      hasCallback: isCallback,
    };
    return ValidationResult;
  };

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      let validatedData = item;
      // onRegisterPressedがtrueの場合のみvalidationCheckを実行
      if (onRegisterPressed) {
        validatedData = validationCheck(validatedData).validateResult;
      }
      return validatedData;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  // 平均値の計算,保存処理
  const calculateAverage = (updatedExamItems: InputExamItem[]) => {
    const bpH_values: number[] = [];
    const bpL_values: number[] = [];

    for (const item of updatedExamItems) {
      for (const detail of item.examItemDetails ?? []) {
        if (item.positionNumber === 血圧1回目) {
          if (detail.positionNumber === 収縮期) {
            if (detail.value !== undefined) {
              const parsedValue = Number.parseFloat(detail.value);
              if (!Number.isNaN(parsedValue)) {
                bpH_values.push(parsedValue);
              }
            }
          } else if (detail.positionNumber === 拡張期) {
            if (detail.value !== undefined) {
              const parsedValue = Number.parseFloat(detail.value);
              if (!Number.isNaN(parsedValue)) {
                bpL_values.push(parsedValue);
              }
            }
          }
        } else if (item.positionNumber === 血圧2回目) {
          if (detail.positionNumber === 収縮期) {
            if (detail.value !== undefined) {
              const parsedValue = Number.parseFloat(detail.value);
              if (!Number.isNaN(parsedValue)) {
                bpH_values.push(parsedValue);
              }
            }
          } else if (detail.positionNumber === 拡張期) {
            if (detail.value !== undefined) {
              const parsedValue = Number.parseFloat(detail.value);
              if (!Number.isNaN(parsedValue)) {
                bpL_values.push(parsedValue);
              }
            }
          }
        }
      }
    }

    // 平均値の計算
    const bpH_AVE =
      Math.round(bpH_values.reduce((a, b) => a + b, 0) / bpH_values.length) ||
      0;
    const bpL_AVE =
      Math.round(bpL_values.reduce((a, b) => a + b, 0) / bpL_values.length) ||
      0;

    // 平均値の保存
    return updatedExamItems.map((item) => {
      if (item.positionNumber === 平均値) {
        item.examItemDetails = item.examItemDetails?.map((detail) => {
          const integerLength = detail.integerLength ?? 0;
          const maxDigits = integerLength; // 平均値では小数点を考慮しない

          if (detail.positionNumber === 収縮期) {
            return {
              ...detail,
              value: bpH_AVE.toString().slice(0, maxDigits),
            };
          }
          if (detail.positionNumber === 拡張期) {
            return {
              ...detail,
              value: bpL_AVE.toString().slice(0, maxDigits),
            };
          }
          return detail;
        });
      }
      return item;
    });
  };

  //変更イベント
  const handleChange = (
    value: string,
    positionNumber: number | undefined,
    detailPositionNumber?: number
  ) => {
    // 対象のpositionNumberか確認
    if (positionNumber !== 血圧1回目 && positionNumber !== 血圧2回目) {
      return null;
    }
    const updatedExamItems = [...examItemsData];
    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = true;

    // 該当するitemを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // クリアボタン用の処理
        if (detailPositionNumber === undefined) {
          item.examItemDetails = item.examItemDetails?.map((detail) => ({
            ...detail,
            value: value,
          }));
        } else {
          // 該当するdetailの値を更新
          item.examItemDetails = item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailPositionNumber
              ? { ...detail, value: value }
              : detail
          );
        }
      }
      // バリデーションチェックを実施
      const { validateResult, hasCallback } = validationCheck(item);
      if (!hasCallback) {
        // falseのexamItemがあればコールバックを行わない
        hasValidationError = false;
      }
      // バリデーション結果を反映
      Object.assign(item, validateResult);
    }

    // 平均値の処理
    const addAVEExamItems = calculateAverage(updatedExamItems);
    // 更新されたデータをステートに設定
    setExamItemsData(addAVEExamItems);

    // 全てのitemでバリデーションチェックが通った場合、コールバックする
    if (hasValidationError) {
      onChange(addAVEExamItems);
    }
  };

  // 対象のexamItemをpositionNumberから検索
  const targetExamItems = examItemsData.filter(
    (item) =>
      item.positionNumber === 血圧1回目 ||
      item.positionNumber === 血圧2回目 ||
      item.positionNumber === 平均値
  );

  return (
    <>
      {targetExamItems.map((item) => {
        const {
          examItemDetails,
          positionNumber,
          name,
          examRegistResults = [],
        } = item;
        const contractionDetail = examItemDetails?.find(
          (detail) => detail.positionNumber === 収縮期
        );
        const expansionDetail = examItemDetails?.find(
          (detail) => detail.positionNumber === 拡張期
        );
        const isAVE = item.positionNumber === 平均値;

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
              {examItemDetails?.map((detail) => {
                // グレーアウト表示判定
                const isDisabled =
                  !!detail?.cancelReasonId || !detail?.hasOrder;

                return (
                  <Flex key={detail?.positionNumber}>
                    {isAVE ? (
                      <Text
                        w={170}
                        h={80}
                        size="inputComponent"
                        c={
                          examRegistResults?.some(
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
                        ta="right"
                      >
                        {detail?.value}
                      </Text>
                    ) : (
                      <TextInput
                        classNames={{
                          input: `${styles["input-textbox"]} ${
                            isDisabled
                              ? ""
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
                        w={172}
                        radius="md"
                        size="inputComponent"
                        bg={isDisabled ? "gray03" : ""}
                        c={isDisabled ? "gray02" : ""}
                        value={detail?.value}
                        onChange={(e) =>
                          handleChange(
                            e.currentTarget.value,
                            positionNumber,
                            detail?.positionNumber
                          )
                        }
                        onClick={() =>
                          toggleKeyboard(
                            positionNumber ?? 0,
                            detail?.positionNumber ?? 0
                          )
                        }
                        disabled={isDisabled}
                      />
                    )}
                    {detail?.positionNumber === 収縮期 && (
                      <Text w={30} h={72} ml={16} c="gray02" size="80px">
                        /
                      </Text>
                    )}
                  </Flex>
                );
              })}
              <Stack w={216} gap={4} mt="auto">
                {contractionDetail?.prevValue && expansionDetail?.prevValue && (
                  <Text fw={700}>
                    (前回：{contractionDetail.prevValue}/
                    {expansionDetail.prevValue})
                  </Text>
                )}
                <Text size="xs">{contractionDetail?.unit}</Text>
              </Stack>
              {!isAVE && (
                <Button
                  w={154}
                  h={64}
                  ml={16}
                  size="lg"
                  bg="white01"
                  variant="outline"
                  bd={"2px,solid"}
                  onClick={() => handleChange("", positionNumber)}
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
                  <IconExclamationCircleFilled size={32} />
                  <Text size="sm" fw={700}>
                    {error.description}
                  </Text>
                </Group>
              );
            })}
            {examItemDetails?.map((detail) => (
              <React.Fragment key={detail?.positionNumber}>
                {/* キーボード表示 */}
                {positionNumber !== 平均値 &&
                  showKeyboards[
                    positionNumber === 血圧1回目 ? "first" : "second"
                  ][
                    detail?.positionNumber === 収縮期
                      ? "contraction"
                      : "expansion"
                  ] && (
                    <Box ref={closeKeyBoard} ml={257}>
                      {detail?.keyboard?.keyboardType ===
                      KeyboardType.テンキー ? (
                        <NumericKeyboard
                          value={detail?.value ?? ""}
                          onChange={(newValue) =>
                            handleChange(
                              newValue,
                              positionNumber,
                              detail?.positionNumber
                            )
                          }
                          onConfirm={handleConfirm}
                        />
                      ) : (
                        <CollectionKeyboard
                          keyboardValues={detail.keyboard?.values ?? []}
                          onChange={(newValue) =>
                            handleChange(
                              newValue,
                              positionNumber,
                              detail.positionNumber
                            )
                          }
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
