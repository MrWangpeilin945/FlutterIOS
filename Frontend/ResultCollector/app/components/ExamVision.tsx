import React from "react";
import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Flex,
  Grid,
  GridCol,
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
  ExamItemDetail,
  ExamItemDetailOption,
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import NumericKeyboard from "./NumericKeyboard";
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import styles from "~/styles/common.module.css";

type VisionProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (updatedExamItem: InputExamItem[] | undefined) => void;
};

export default function ExamVision({
  examItems,
  onRegisterPressed,
  onChange,
}: VisionProps) {
  if (!examItems || examItems.length === 0) {
    return null;
  }
  // 定数で定義
  const 裸眼 = 1;
  const 矯正 = 2;
  const 特記 = 3;
  const 左眼 = 1;
  const 右眼 = 2;
  const 両眼 = 3;
  const 矯正区分_左 = 1;
  const 矯正区分_右 = 2;
  const 矯正入力値_左 = 3;
  const 矯正入力値_右 = 4;
  const 矯正入力値_両眼 = 5;

  // 必要な検査項目が1つも存在しない場合は表示しない
  const visionItemPositionNumbers = [裸眼, 矯正, 特記];
  const allMissing = visionItemPositionNumbers.every(
    (position) => !examItems.some((item) => item.positionNumber === position),
  );

  if (allMissing) {
    return null; // 存在しない場合、表示しない
  }

  const [examItemsData, setExamItemsData] = useState(examItems);
  // 必須チェック用フラグ
  const [showRequiredError, setShowRequiredError] = useState(false);
  // 半角数字、矯正区分チェック用フラグ
  let hasValidationError = false;

  // キーボードの表示状態をオブジェクトで管理
  const [activeKeyboard, setActiveKeyboard] = useState<{
    itemNumber: number;
    detailNumber: number;
  } | null>(null);

  useEffect(() => {
    // 登録ボタンフラグがtrueの場合、必須チェック
    if (onRegisterPressed) {
      requiredCheck(examItems);
    }
    setExamItemsData(examItems);
  }, [onRegisterPressed, examItems]);

  // キーボード以外の部分を押下時に非表示
  const closeKeyboard = useClickOutside(() => {
    setActiveKeyboard(null);
  });
  const handleConfirm = () => {
    setActiveKeyboard(null);
  };
  const toggleKeyboard = (itemNumber: number, detailNumber: number) => {
    setActiveKeyboard((prev) =>
      prev &&
      prev.itemNumber === itemNumber &&
      prev.detailNumber === detailNumber
        ? null
        : { itemNumber, detailNumber },
    );
  };
  const isKeyboardVisible = (index: number, detailIndex: number) =>
    activeKeyboard?.itemNumber === index &&
    activeKeyboard?.detailNumber === detailIndex;

  // エラーメッセージ管理
  const handleErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      // エラーレベル順に並び替え
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });
    }
    return item;
  };

  // 1項目でも入力されているかをチェック
  const requiredCheck = (examItems: InputExamItem[]) => {
    const anyFilled = examItems.some((item) =>
      (item.examItemDetails ?? []).some((detail) => detail.value),
    );
    setShowRequiredError(!anyFilled);
  };

  // 半角数字チェック
  const validateNumeric = (detail: ExamItemDetail) => {
    const schema = z
      .string()
      .refine(
        (value) => value === "" || /^\d+(\.\d+)?$/.test(value),
        getErrorMessage(errorMessages.numericString, `${detail.name}は`),
      );

    const result = schema.safeParse(detail.value);

    if (!result.success) {
      // メッセージを追加
      const error = result.error.errors[0];
      return {
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      };
    }
    return undefined;
  };

  // 矯正区分が選択されているかチェック
  const collectionCheck = (
    value: string,
    targetDetail: ExamItemDetail,
  ): ExamRegistResult | undefined => {
    const collectionSchema = z
      .string()
      .min(
        1,
        getErrorMessage(errorMessages.required, `${targetDetail?.name}は`),
      );

    // 矯正の入力値が存在する場合、矯正区分を必須チェック
    if (value) {
      const result = collectionSchema.safeParse(targetDetail?.value);
      if (!result.success) {
        const error = result.error.errors[0];
        return {
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        };
      }
    }
    return undefined;
  };

  // APIからのエラーメッセージを保存
  const APIErrors = examItems.map((item) => ({
    positionNumber: item.positionNumber,
    examRegistResults: item.examRegistResults || [],
  }));
  // 引数のexamItemのpositionNumberを参照し、エラーメッセージを初期化する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = APIErrors.find(
      (error) => error.positionNumber === item.positionNumber,
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };

  // バリデーションチェック
  const validationCheck = (item: InputExamItem) => {
    resetErrorMessages(item);

    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // detailsをエラーチェック
    for (const detail of item.examItemDetails || []) {
      // 選択系の場合はバリデーションチェックをスキップ
      const isSelector =
        Array.isArray(detail.examItemDetailOptions) &&
        detail.examItemDetailOptions.length > 0;
      if (isSelector) continue;

      // オーダーが存在するかつ中止理由が存在しない場合
      if (detail.hasOrder && !detail.cancelReasonId) {
        // 半角数字チェック
        const result = validateNumeric(detail);
        if (result !== undefined) {
          componentErrorMessage.push(result);
        }
      }

      // 矯正の値が入力された場合に、矯正区分が選択されているかチェック
      if (
        item.positionNumber === 矯正 &&
        (detail.positionNumber === 矯正入力値_左 ||
          detail.positionNumber === 矯正入力値_右)
      ) {
        const targetDetail =
          detail.positionNumber === 矯正入力値_左
            ? item.examItemDetails?.find(
                (d) => d.positionNumber === 矯正区分_左,
              )
            : item.examItemDetails?.find(
                (d) => d.positionNumber === 矯正区分_右,
              );

        // 対象の矯正区分のオーダーが存在するかつ中止理由が存在しない場合
        if (targetDetail?.hasOrder && !targetDetail.cancelReasonId) {
          // 矯正区分チェック
          const result = collectionCheck(
            detail.value ?? "",
            targetDetail ?? {},
          );
          if (result !== undefined) {
            componentErrorMessage.push(result);
          }
        }
      }
    }

    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...(setRangesErrorMessage(item) ?? []));
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック
    const hasError = componentErrorMessage.some(
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
      hasError: hasError,
    };
    return ValidationResult;
  };

  // 値取得用
  const getValueByPositionNumbers = (
    examItems: InputExamItem[],
    itemNumber: number,
    detailNumber: number,
  ): string | undefined => {
    return examItems
      .find((item) => item.positionNumber === itemNumber) // 指定された item を探す
      ?.examItemDetails?.find(
        (detail) => detail.positionNumber === detailNumber,
      )?.value; // 指定された detail を探して、value を取得
  };

  // 値設定用
  const setValueByPositionNumbers = (
    examItems: InputExamItem[],
    itemNumber: number,
    detailNumber: number,
    newValue: string,
  ): typeof examItems =>
    examItems.map((item) => {
      if (item.positionNumber === itemNumber) {
        // validationCheckの結果を取得
        const { validateResult, hasError } = validationCheck({
          ...item,
          examItemDetails: item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailNumber
              ? { ...detail, value: newValue } // value を更新
              : detail,
          ),
        });

        // エラーがあった場合セット
        if (hasError) {
          hasValidationError = hasError;
        }

        // validateResultを返す
        return validateResult;
      }
      return item;
    });

  // 各項目の値変更時
  const handleChange = (
    value: string,
    itemNumber: number,
    detailNumber: number,
    isSelector?: boolean,
  ) => {
    hasValidationError = false;

    // 選択/未選択切替処理
    const checkedValue =
      isSelector &&
      value ===
        getValueByPositionNumbers(examItemsData, itemNumber, detailNumber)
        ? ""
        : value;

    // 値を更新
    const updatedExamItems = setValueByPositionNumbers(
      examItemsData,
      itemNumber,
      detailNumber,
      checkedValue,
    );

    setExamItemsData(updatedExamItems);
    // 必須チェック
    requiredCheck(updatedExamItems);

    // エラーが存在しない場合のみ onChange を実行
    if (!hasValidationError && !showRequiredError) {
      onChange(updatedExamItems);
    }
  };

  // 必要なpositionNumberのリスト
  const requiredPositions = [裸眼, 矯正, 特記];

  // データ処理
  const targetExamItems: InputExamItem[] = requiredPositions.map(
    (positionNumber) => {
      const detailRequiredPositions =
        positionNumber === 裸眼
          ? [左眼, 右眼, 両眼]
          : positionNumber === 矯正
            ? [
                矯正区分_左,
                矯正区分_右,
                矯正入力値_左,
                矯正入力値_右,
                矯正入力値_両眼,
              ]
            : [左眼, 右眼]; //特記
      // 該当するexamItemを検索
      let examItem = examItemsData.find(
        (item) => item.positionNumber === positionNumber,
      );

      // 該当するexamItemがなければデフォルトを設定
      if (!examItem) {
        examItem = {
          positionNumber,
          name:
            positionNumber === 裸眼
              ? "裸眼"
              : positionNumber === 矯正
                ? "矯正"
                : "特記",
          examItemDetails: [],
          examRegistResults: [],
        };
      }

      // examItemDetailsを処理
      const examItemDetails = examItem.examItemDetails ?? [];
      const { name, examRegistResults } = examItem;

      // 各examItemDetailsの中で必要なpositionNumberを補完
      const details = detailRequiredPositions.map((detailPosition) => {
        const detail = examItemDetails.find(
          (detail) => detail.positionNumber === detailPosition,
        );

        //選択系補完用の選択肢データ
        let detailOptions: ExamItemDetailOption[] = [];
        if (
          positionNumber === 矯正 &&
          (detailPosition === 矯正区分_左 || detailPosition === 矯正区分_右)
        ) {
          detailOptions = [
            { orderNumber: 1, name: "メガネ" },
            { orderNumber: 2, name: "コンタクト" },
          ];
        } else if (positionNumber === 特記) {
          detailOptions = [
            { orderNumber: 1, name: "メガネ不要" },
            { orderNumber: 2, name: "コンタクト不要" },
          ];
        }

        // 該当するdetailがなければデフォルトを設定
        return (
          detail || {
            positionNumber: detailPosition,
            examItemDetailOptions: detailOptions,
          }
        );
      });

      return {
        positionNumber: positionNumber,
        name: name,
        examItemDetails: details,
        examRegistResults: examRegistResults,
      };
    },
  );

  const detailNames = ["視力　左", "視力　右", "両眼"];
  return (
    <>
      <Flex
        justify="flex-start"
        align="flex-start"
        direction="column"
        w={1700}
        gap={0}
      >
        {/* 検査項目明細名 */}
        <Flex pl={108} gap={16}>
          {detailNames.map((name, index) => (
            <Paper
              key={index}
              w={index === 両眼 - 1 ? 373 : 592}
              h={51}
              bg="gray02"
              c="white"
              radius="itemName"
            >
              <Text size="lg" fw={700} ta="center">
                {name}
              </Text>
            </Paper>
          ))}
        </Flex>
        {/* 検査項目単位 */}
        {targetExamItems.map((item) => {
          const isCorrection = item.positionNumber === 矯正;
          return (
            <>
              <Group key={item.positionNumber} gap={16} w={1716} mt={16}>
                <Paper
                  w={92}
                  h={isCorrection ? 170 : 80}
                  bg="gray02"
                  c="white"
                  radius="itemName"
                >
                  <Text
                    size="lg"
                    fw={700}
                    ta="center"
                    px={16}
                    py={isCorrection ? 60 : 16}
                  >
                    {item.name}
                  </Text>
                </Paper>
                <Grid
                  key={item.positionNumber}
                  w={1608}
                  h={isCorrection ? 170 : 80}
                  gutter={16}
                >
                  {/* 検査項目明細単位 */}
                  {item.examItemDetails?.map((detail) => {
                    const isDisabled =
                      !detail.hasOrder || !!detail.cancelReasonId;

                    let isBothEyes = false;
                    if (isCorrection) {
                      isBothEyes = detail.positionNumber === 矯正入力値_両眼;
                    } else {
                      isBothEyes = detail.positionNumber === 両眼;
                    }
                    // 選択ボタン用
                    const isSelector =
                      Array.isArray(detail.examItemDetailOptions) &&
                      detail.examItemDetailOptions.length > 0;

                    return (
                      <GridCol
                        span={isBothEyes ? 3 : 4.5}
                        key={detail.positionNumber}
                      >
                        {isSelector ? (
                          // 選択ボタン
                          <Group key={detail.positionNumber} h={78}>
                            {detail.examItemDetailOptions?.map((option) => {
                              const isSelected =
                                getValueByPositionNumbers(
                                  examItemsData,
                                  item.positionNumber ?? 0,
                                  detail.positionNumber ?? 0,
                                ) === option.code;
                              return (
                                <Button
                                  key={option.orderNumber}
                                  w={288}
                                  h={78}
                                  variant="outline"
                                  bd={`2px solid ${isDisabled ? "" : isSelected ? "primary" : "gray03"}`}
                                  bg={
                                    isDisabled
                                      ? "gray03"
                                      : isSelected
                                        ? "green03"
                                        : "white"
                                  }
                                  c={
                                    isDisabled
                                      ? "gray02"
                                      : isSelected
                                        ? "primary"
                                        : "black"
                                  }
                                  size="xl"
                                  fw={700}
                                  value={option.code}
                                  disabled={isDisabled}
                                  onClick={(e) =>
                                    handleChange(
                                      e.currentTarget.value,
                                      item.positionNumber ?? 0,
                                      detail.positionNumber ?? 0,
                                      true,
                                    )
                                  }
                                >
                                  {option.name}
                                </Button>
                              );
                            })}
                          </Group>
                        ) : (
                          // テキストボックス
                          <Group>
                            <TextInput
                              classNames={{
                                input: `${styles["input-textbox"]} ${
                                  isDisabled
                                    ? ""
                                    : item.examRegistResults?.some(
                                          (x) =>
                                            x.errorLevel ===
                                            InputErrorLevel.異常,
                                        ) || showRequiredError
                                      ? `${styles["input-error"]}`
                                      : item.examRegistResults?.some(
                                            (x) =>
                                              x.errorLevel ===
                                              InputErrorLevel.警告,
                                          )
                                        ? `${styles["input-warning"]}`
                                        : ""
                                }`,
                              }}
                              bg={isDisabled ? "gray03" : "white"}
                              w={isBothEyes ? 192 : 398}
                              h={78}
                              size="inputComponent"
                              value={detail.value}
                              ml={isBothEyes ? 0 : 8}
                              onClick={() =>
                                toggleKeyboard(
                                  item.positionNumber ?? 0,
                                  detail.positionNumber ?? 0,
                                )
                              }
                              onChange={(e) =>
                                handleChange(
                                  e.currentTarget.value,
                                  item.positionNumber ?? 0,
                                  detail.positionNumber ?? 0,
                                )
                              }
                              disabled={isDisabled}
                            />
                            {detail.prevValue && (
                              <Text w={170} size="lg" fw={700}>
                                (前回：{detail.prevValue})
                              </Text>
                            )}
                          </Group>
                        )}
                      </GridCol>
                    );
                  })}
                </Grid>
              </Group>
              {/* エラーメッセージ */}
              <Stack key={item.positionNumber} gap={0}>
                {(item.positionNumber === 裸眼 ||
                  item.positionNumber === 矯正) &&
                  showRequiredError && (
                    <Group c="error">
                      <IconSquareRoundedXFilled size={32} />
                      <Text size="sm" fw={700}>
                        {getErrorMessage(errorMessages.required, "視力は")}
                      </Text>
                    </Group>
                  )}
                {item.examRegistResults?.map((error, index) => {
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
              </Stack>
              {/* キーボード */}
              {item?.examItemDetails?.map((detail) => {
                const typeMap: Record<number, string> = isCorrection
                  ? { 3: "left", 4: "center", 5: "right" }
                  : { 1: "left", 2: "center", 3: "right" };

                const isDetailType =
                  detail.positionNumber != null &&
                  typeMap[detail.positionNumber]
                    ? typeMap[detail.positionNumber]
                    : null;

                return (
                  <React.Fragment key={detail.positionNumber}>
                    {/* キーボード表示 */}
                    {isKeyboardVisible(
                      item.positionNumber ?? 0,
                      detail.positionNumber ?? 0,
                    ) && (
                      <Box
                        ref={closeKeyboard}
                        ml={
                          isDetailType === "left"
                            ? 116
                            : isDetailType === "center"
                              ? 725
                              : "auto"
                        }
                      >
                        {detail?.keyboard?.keyboardType ===
                        KeyboardType.テンキー ? (
                          <NumericKeyboard
                            value={detail?.value ?? ""}
                            integerLength={detail.integerLength}
                            decimalLength={detail.decimalLength}
                            onChange={(newValue) =>
                              handleChange(
                                newValue,
                                item.positionNumber ?? 0,
                                detail.positionNumber ?? 0,
                              )
                            }
                            onConfirm={handleConfirm}
                          />
                        ) : (
                          <CollectionKeyboard
                            value={detail.value ?? ""}
                            keyboardValues={detail.keyboard?.values ?? []}
                            onChange={(newValue) =>
                              handleChange(
                                newValue,
                                item.positionNumber ?? 0,
                                detail.positionNumber ?? 0,
                              )
                            }
                          />
                        )}
                      </Box>
                    )}
                  </React.Fragment>
                );
              })}
            </>
          );
        })}
      </Flex>
    </>
  );
}
