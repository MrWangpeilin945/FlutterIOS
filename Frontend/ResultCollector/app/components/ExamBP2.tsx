import React, { useEffect, useState } from "react";
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
  InputExamItem,
  ExamItemDetail,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import type { ValidationHandle } from "~/routes/consult-input.$consultnumber";
import NumericKeyboard from "./NumericKeyboard";
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import styles from "~/styles/common.module.css";

type ExamBP2Props = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  isInitialDisplay: boolean;
  hasConnectionEquipment: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
  validationRef: React.RefObject<ValidationHandle | null>;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

const ExamBP2 = ({
  examItems,
  onRegisterPressed,
  isInitialDisplay,
  hasConnectionEquipment,
  onChange,
  validationRef,
}: ExamBP2Props) => {
  //登録ボタンプレスフラグを制御するための変数
  let isRegisterPressed = onRegisterPressed;

  // 定数で定義
  const 血圧1回目 = 1;
  const 血圧2回目 = 2;
  const 平均値 = 3;
  const 上 = 1;
  const 下 = 2;

  // 引数のチェック
  if (!examItems || examItems.length === 0) {
    return null;
  }
  // positionNumberが1のexamItemが存在し、かつexamItemDetailsに必要な値が存在するかのチェック
  const isValidBPfirst = examItems.some(
    (item) =>
      item.positionNumber === 血圧1回目 &&
      item.examItemDetails?.some(
        (detail) =>
          detail.positionNumber === 上 &&
          item.examItemDetails?.some((detail) => detail.positionNumber === 下),
      ),
  );

  // positionNumberが2のexamItemが存在し、かつexamItemDetailsに必要な値が存在するかのチェック
  const isValidBPsecond = examItems.some(
    (item) =>
      item.positionNumber === 血圧2回目 &&
      item.examItemDetails?.some((detail) => detail.positionNumber === 上) &&
      item.examItemDetails?.some((detail) => detail.positionNumber === 下),
  );

  // 両方を満たさない時、nullを返す
  if (!isValidBPfirst && !isValidBPsecond) {
    return null;
  }

  // positionNumberが3のexamItemが存在し、かつexamItemDetailsに必要な値が存在するかのチェック
  const isValidAVE = examItems.some(
    (item) =>
      item.positionNumber === 平均値 &&
      item.examItemDetails?.some((detail) => detail.positionNumber === 上) &&
      item.examItemDetails?.some((detail) => detail.positionNumber === 下),
  );

  // examItemの管理
  const [examItemsData, setExamItemsData] = useState(examItems);
  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);
  // キーボードの表示状態を管理する
  const [showKeyboards, setShowKeyboards] = useState<{
    first: {
      high: boolean;
      low: boolean;
    };
    second: {
      high: boolean;
      low: boolean;
    };
  }>({
    first: {
      high: false,
      low: false,
    },
    second: {
      high: false,
      low: false,
    },
  });

  // キーボードを非表示にする
  const resetKeyboards = () => {
    setShowKeyboards({
      first: {
        high: false,
        low: false,
      },
      second: {
        high: false,
        low: false,
      },
    });
  };

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
    //初期表示以外は平均値を計算
    if (!isInitialDisplay) {
      updatedItems = calculateAverage(updatedItems);
    }
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  // キーボード外部をクリックした際に非表示にする
  const closeKeyBoard = useClickOutside(resetKeyboards);

  // キーボードの表示/非表示をトグルする関数
  const toggleKeyboard = (
    positionNumber: number,
    detailPositionNumber: number,
  ) => {
    setShowKeyboards((prev) => {
      const updated = { ...prev };
      if (positionNumber === 血圧1回目) {
        if (detailPositionNumber === 上) {
          updated.first = {
            ...updated.first,
            high: !updated.first.high,
          };
        } else if (detailPositionNumber === 下) {
          updated.first = {
            ...updated.first,
            low: !updated.first.low,
          };
        }
      } else if (positionNumber === 血圧2回目) {
        if (detailPositionNumber === 上) {
          updated.second = {
            ...updated.second,
            high: !updated.second.high,
          };
        } else if (detailPositionNumber === 下) {
          updated.second = {
            ...updated.second,
            low: !updated.second.low,
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

  // APIのエラーメッセージで更新する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = backendValidation.find(
      (error) => error.itemPositionNumber === item.positionNumber,
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };
  // 血圧の上下の値が適正かのチェック
  const validateBPValues = (item: InputExamItem) => {
    const bpHValue =
      item.examItemDetails?.find((detail) => detail.positionNumber === 上)
        ?.value ?? null;
    const bpLValue =
      item.examItemDetails?.find((detail) => detail.positionNumber === 下)
        ?.value ?? null;
    if (
      bpHValue &&
      bpLValue &&
      Number.parseFloat(bpHValue) < Number.parseFloat(bpLValue)
    ) {
      const BPErrorMessage: ExamRegistResult = {
        description: "血圧の値が逆転しています。",
        errorLevel: InputErrorLevel.警告,
      };
      return BPErrorMessage;
    }
  };

  // バリデーションチェック
  const validationCheck = (item: InputExamItem) => {
    // 血圧は上下どちらかがグレーアウトの場合、両方バリデーションチェックを行わない
    const highDetail = item.examItemDetails?.find(
      (detail: ExamItemDetail) => detail.positionNumber === 上,
    );
    const lowDetail = item.examItemDetails?.find(
      (detail: ExamItemDetail) => detail.positionNumber === 下,
    );
    const hasDisableItem =
      !highDetail?.hasOrder ||
      !!highDetail.cancelReasonId ||
      !lowDetail?.hasOrder ||
      !!lowDetail.cancelReasonId;
    if (hasDisableItem) {
      return { validateResult: item, hasCallback: true };
    }
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    let componentErrorMessage: ExamRegistResult[] = [];
    // detailsのpositionNumberが1と2のものについてバリデーションチェックを行う
    for (const { positionNumber, value } of item.examItemDetails ?? []) {
      if (positionNumber !== 上 && positionNumber !== 下) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }
      // [登録する]が押されたときは必須・半角数字チェック、その他は半角数字チェックのみ行う。
      const schema = isRegisterPressed
        ? z
            .string()
            .min(1, getErrorMessage(errorMessages.required, "血圧は")) // 必須チェック
            .refine((value) => /^\d+(\.\d+)?$/.test(value), {
              message: getErrorMessage(errorMessages.numericString, "血圧は"),
            })
        : z.string().refine((value) => /^(\d+(\.\d+)?|)$/.test(value), {
            message: getErrorMessage(errorMessages.numericString, "血圧は"),
          });

      // バリデーション対象データを取得
      const valueToValidate = value ?? "";
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

    // 血圧の上下の値が逆転していないかのチェック
    const BPError = validateBPValues(item);
    if (BPError) {
      componentErrorMessage.push(BPError);
    }
    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...setRangesErrorMessage(item));

    // 血圧の上下で同一のエラーメッセージが発生する可能性があるため、重複を削除する
    componentErrorMessage = componentErrorMessage.filter(
      (error, index) =>
        componentErrorMessage.findIndex(
          (e) => e.description === error.description,
        ) === index,
    );
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
      validateResult: sortErrorMessage(resultItem),
      hasCallback: isCallback,
    };
    return ValidationResult;
  };

  // 平均値の計算,保存処理
  const calculateAverage = (updatedExamItems: InputExamItem[]) => {
    // 引数に平均値のexamItemが無いときは計算しない
    if (!isValidAVE) {
      return updatedExamItems;
    }
    const bpHValues: number[] = [];
    const bpLValues: number[] = [];
    for (const item of updatedExamItems) {
      if (
        item.positionNumber !== 血圧1回目 &&
        item.positionNumber !== 血圧2回目
      ) {
        continue;
      }
      for (const detail of item.examItemDetails ?? []) {
        if (detail.value === undefined) {
          continue;
        }
        const parsedValue = Number.parseFloat(detail.value);
        if (Number.isNaN(parsedValue)) {
          continue;
        }
        if (detail.positionNumber === 上) {
          bpHValues.push(parsedValue);
        } else if (detail.positionNumber === 下) {
          bpLValues.push(parsedValue);
        }
      }
    }
    // 平均値の計算
    const sum = (values: number[]) => values.reduce((a, b) => a + b, 0);

    const averageBpHigh =
      bpHValues.length > 0
        ? bpHValues.length > 2
          ? Math.round(sum(bpHValues) / 2)
          : Math.round(sum(bpHValues) / bpHValues.length)
        : 0;

    const averageBpLow =
      bpLValues.length > 0
        ? bpLValues.length > 2
          ? Math.round(sum(bpLValues) / 2)
          : Math.round(sum(bpLValues) / bpLValues.length)
        : 0;

    // 平均値の保存
    return updatedExamItems.map((item) => {
      if (item.positionNumber === 平均値) {
        item.examItemDetails = item.examItemDetails?.map((detail) => {
          const integerLength = detail.integerLength ?? 0;
          const maxDigits = integerLength; // 平均値では小数点を考慮しない

          if (detail.positionNumber === 上) {
            return {
              ...detail,
              value: averageBpHigh.toString().slice(0, maxDigits),
            };
          }
          if (detail.positionNumber === 下) {
            return {
              ...detail,
              value: averageBpLow.toString().slice(0, maxDigits),
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
    detailPositionNumber?: number,
  ) => {
    // 対象のpositionNumberか確認
    if (positionNumber !== 血圧1回目 && positionNumber !== 血圧2回目) {
      return null;
    }
    const updatedExamItems = [...examItemsData];

    // 該当するitemを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // クリアボタン用の処理
        if (detailPositionNumber === undefined) {
          item.examItemDetails = item.examItemDetails?.map((detail) => ({
            ...detail,
            // disableのexamItemDetailのvalueを消去しない
            value:
              detail.hasOrder && !detail.cancelReasonId ? value : detail.value,
          }));
        } else {
          // 該当するdetailの値を更新
          item.examItemDetails = item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailPositionNumber
              ? { ...detail, value: value }
              : detail,
          );
        }
      }
    }

    // 平均値の処理
    const addAVEExamItems = calculateAverage(updatedExamItems);
    // 更新されたデータをステートに設定
    setExamItemsData(addAVEExamItems);

    // バリデーションチェックを実施
    for (const item of updatedExamItems) {
      const { validateResult } = validationCheck(item);

      // バリデーション結果を反映
      Object.assign(item, validateResult);
    }

    onChange(addAVEExamItems);
  };

  // 血圧1回目のexamItem
  let BPfirstItem: InputExamItem = {
    positionNumber: 血圧1回目,
    name: "血圧1",
    examItemDetails: [{ positionNumber: 上 }, { positionNumber: 下 }],
  };
  if (isValidBPfirst) {
    const foundItem = examItemsData.find(
      (item) => item.positionNumber === 血圧1回目,
    );
    if (foundItem) {
      BPfirstItem = { ...foundItem, name: "血圧1" };
    }
  }
  // 血圧2回目のexamItem
  let BPsecondItem: InputExamItem = {
    positionNumber: 血圧2回目,
    name: "血圧2",
    examItemDetails: [{ positionNumber: 上 }, { positionNumber: 下 }],
  };
  if (isValidBPsecond) {
    const foundItem = examItemsData.find(
      (item) => item.positionNumber === 血圧2回目,
    );
    if (foundItem) {
      BPsecondItem = { ...foundItem, name: "血圧2" };
    }
  }
  // 平均値のexamItem
  let AVEItem: InputExamItem = {
    positionNumber: 平均値,
    name: "平均",
    examItemDetails: [{ positionNumber: 上 }, { positionNumber: 下 }],
  };

  if (isValidAVE) {
    const foundItem = examItemsData.find(
      (item) => item.positionNumber === 平均値,
    );
    if (foundItem) {
      AVEItem = { ...foundItem, name: "平均" };
    }
  }
  const targetExamItems = [BPfirstItem, BPsecondItem, AVEItem];

  return (
    <>
      {targetExamItems.map((item) => {
        const {
          examItemDetails,
          positionNumber,
          name,
          examRegistResults = [],
        } = item;

        const highDetail = examItemDetails?.find((detail: ExamItemDetail) =>
          detail.positionNumber === 1 ? detail : null,
        );
        const lowDetail = examItemDetails?.find((detail: ExamItemDetail) =>
          detail.positionNumber === 2 ? detail : null,
        );
        const isAVE = item?.positionNumber === 平均値;
        const isDisableItem =
          !highDetail?.hasOrder ||
          !!highDetail.cancelReasonId ||
          !lowDetail?.hasOrder ||
          !!lowDetail.cancelReasonId;

        return (
          <Flex
            key={positionNumber}
            justify="flex-start"
            align="flex-start"
            direction="column"
            mb={16}
            w={1101}
          >
            <Flex align="center" gap="md">
              <Paper
                w={274}
                h={80}
                bg={isAVE ? "white" : "gray02"}
                c={isAVE ? "gray02" : "white"}
                radius="itemName"
                py={16}
              >
                <Text size="lg" fw={700} ta="center">
                  {name?.slice(0, 8)}
                </Text>
              </Paper>
              {examItemDetails?.map((detail) => {
                // グレーアウト表示判定

                return (
                  <Flex key={detail?.positionNumber}>
                    {isAVE ? (
                      <Text
                        w={170}
                        h={80}
                        size="inputComponent"
                        c={
                          examRegistResults?.some(
                            (x) => x.errorLevel === InputErrorLevel.異常,
                          )
                            ? "error"
                            : examRegistResults?.some(
                                  (x) => x.errorLevel === InputErrorLevel.警告,
                                )
                              ? "warning"
                              : "black"
                        }
                        px={32}
                        mt={-8}
                        ta="right"
                      >
                        {isDisableItem ? "" : detail?.value}
                      </Text>
                    ) : (
                      <TextInput
                        classNames={{
                          input: `${styles["input-textbox"]} ${
                            isDisableItem
                              ? `${styles["input-disabled"]}`
                              : examRegistResults?.some(
                                    (x) =>
                                      x.errorLevel === InputErrorLevel.異常,
                                  )
                                ? `${styles["input-error"]}`
                                : examRegistResults?.some(
                                      (x) =>
                                        x.errorLevel === InputErrorLevel.警告,
                                    )
                                  ? `${styles["input-warning"]}`
                                  : ""
                          }`,
                        }}
                        w={172}
                        radius="md"
                        size="inputComponent"
                        bg={isDisableItem ? "gray03" : ""}
                        c={isDisableItem ? "gray02" : ""}
                        value={isDisableItem ? "" : detail?.value}
                        onChange={(e) =>
                          handleChange(
                            e.currentTarget.value,
                            positionNumber,
                            detail?.positionNumber,
                          )
                        }
                        onClick={() =>
                          toggleKeyboard(
                            positionNumber ?? 0,
                            detail?.positionNumber ?? 0,
                          )
                        }
                        disabled={isDisableItem}
                      />
                    )}
                    {detail?.positionNumber === 上 && (
                      <Text w={30} h={72} ml={16} c="gray02" size="80px">
                        /
                      </Text>
                    )}
                  </Flex>
                );
              })}
              <Stack w={216} h={80} gap={4} justify="space-between">
                <Box>
                  {highDetail?.prevValue && lowDetail?.prevValue && (
                    <Text fw={700} mt={0}>
                      (前回：{highDetail.prevValue}/{lowDetail.prevValue})
                    </Text>
                  )}
                </Box>
                <Box>
                  <Text size="xs" mb={0}>
                    {highDetail?.unit}
                  </Text>
                </Box>
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
                  disabled={isDisableItem}
                >
                  クリア
                </Button>
              )}
            </Flex>

            {/* エラーメッセージの表示 */}
            {(examRegistResults || []).map((error, index) => {
              const isWarning = error.errorLevel === InputErrorLevel.警告;
              return (
                <Group key={index} c={isWarning ? "warning" : "error"}>
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
            {!hasConnectionEquipment &&
              examItemDetails?.map((detail) => (
                <React.Fragment key={detail?.positionNumber}>
                  {/* キーボード表示 */}
                  {positionNumber !== 平均値 &&
                    showKeyboards[
                      positionNumber === 血圧1回目 ? "first" : "second"
                    ][detail?.positionNumber === 上 ? "high" : "low"] && (
                      <Box ref={closeKeyBoard} ml={257}>
                        {detail?.keyboard?.keyboardType ===
                        KeyboardType.テンキー ? (
                          <NumericKeyboard
                            value={detail?.value ?? ""}
                            integerLength={detail?.integerLength}
                            decimalLength={detail?.decimalLength}
                            onChange={(newValue) =>
                              handleChange(
                                newValue,
                                positionNumber,
                                detail?.positionNumber,
                              )
                            }
                            onConfirm={resetKeyboards}
                          />
                        ) : (
                          <CollectionKeyboard
                            value={detail?.value ?? ""}
                            keyboardValues={detail.keyboard?.values ?? []}
                            onChange={(newValue) =>
                              handleChange(
                                newValue,
                                positionNumber,
                                detail.positionNumber,
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
};

export default ExamBP2;
