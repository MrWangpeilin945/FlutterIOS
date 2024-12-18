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
import { InputErrorLevel } from "~/domain/enums";
import type {
  ExamItemDetail,
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
  const [examItemsData, setExamItemsData] = useState(examItems);

  // キーボードの表示状態をオブジェクトで管理
  const [activeKeyboard, setActiveKeyboard] = useState<{
    itemNumber: number;
    detailNumber: number;
  } | null>(null);
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
      // 重複メッセージを削除
      item.examRegistResults = item.examRegistResults.filter(
        (result, index, self) =>
          index === self.findIndex((r) => r.description === result.description),
      );
    }
    return item;
  };

  // 全項目が存在しないかをチェック
  const requiredCheck = (examItems: InputExamItem[]) => {
    const allEmpty = examItems.every((item) =>
      (item.examItemDetails ?? []).every((detail) => !detail.value),
    );

    return examItems.map((item) => {
      const updatedResults = item.examRegistResults || [];

      if (allEmpty) {
        // メッセージを追加
        if (item.positionNumber === 1 || item.positionNumber === 2) {
          updatedResults.push({
            description: getErrorMessage(errorMessages.required, "視力は"),
            errorLevel: InputErrorLevel.異常,
          });
        }
      } else {
        // allEmpty が false の場合、メッセージを削除
        const filteredResults = updatedResults.filter(
          (result) =>
            result.description !==
            getErrorMessage(errorMessages.required, "視力は"),
        );
        return handleErrorMessage({
          ...item,
          examRegistResults: filteredResults,
        });
      }

      return handleErrorMessage({
        ...item,
        examRegistResults: updatedResults,
      });
    });
  };

  useEffect(() => {
    let newItems = [...examItems];
    // 登録ボタンフラグがtrueの場合、必須チェック
    if (onRegisterPressed) {
      newItems = requiredCheck(newItems);
    }
    setExamItemsData(newItems);
  }, [onRegisterPressed, examItems]);

  // 半角数字チェック
  const validateNumeric = (
    value: string,
    name: string,
    errors: ExamRegistResult[],
  ) => {
    const schema = z
      .string()
      .refine(
        (value) => value === "" || /^\d+(\.\d+)?$/.test(value),
        getErrorMessage(errorMessages.numericString, `${name}は`),
      );

    const result = schema.safeParse(value);

    if (!result.success) {
      // メッセージを追加
      const error = result.error.errors[0];
      errors.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    } else {
      // メッセージを削除
      const errorMessageNumeric = getErrorMessage(
        errorMessages.numericString,
        `${name}は`,
      );
      return errors.filter(
        (error) => error.description !== errorMessageNumeric,
      );
    }
    return errors;
  };

  // 矯正区分が選択されているかチェック
  const collectionCheck = (
    value: string,
    targetDetail: ExamItemDetail,
    errors: ExamRegistResult[],
  ) => {
    const collectionSchema = z
      .string()
      .min(
        1,
        getErrorMessage(errorMessages.required, `${targetDetail?.name}は`),
      );

    const errorMessage = getErrorMessage(
      errorMessages.required,
      `${targetDetail?.name}は`,
    );

    const filteredErrors = errors.filter(
      (error) => error.description !== errorMessage,
    );

    // 矯正の入力値が存在する場合、矯正区分を必須チェック
    if (value) {
      const result = collectionSchema.safeParse(targetDetail?.value);
      if (!result.success) {
        const error = result.error.errors[0];
        filteredErrors.push({
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        });
      }
    }
    return filteredErrors;
  };

  // バリデーションチェック
  const validationCheck = (item: InputExamItem) => {
    let updatedErrors = item.examRegistResults || [];
    // detailsをエラーチェック
    for (const detail of item.examItemDetails || []) {
      // 選択系の場合はバリデーションチェックをスキップ
      const isSelector =
        Array.isArray(detail.examItemDetailOptions) &&
        detail.examItemDetailOptions.length > 0;
      if (isSelector) continue;

      // 半角数字チェック
      const valueToValidate = detail.value || "";
      updatedErrors = validateNumeric(
        valueToValidate,
        detail.name ?? "",
        updatedErrors,
      );

      // 矯正の値が入力された場合に、矯正区分が選択されているかチェック
      if (
        item.positionNumber === 2 &&
        (detail.positionNumber === 3 || detail.positionNumber === 4)
      ) {
        const targetDetail =
          detail.positionNumber === 3
            ? item.examItemDetails?.find((d) => d.positionNumber === 1)
            : item.examItemDetails?.find((d) => d.positionNumber === 2);

        updatedErrors = collectionCheck(
          detail.value ?? "",
          targetDetail ?? {},
          updatedErrors,
        );
      }
    }

    const prevItem = {
      ...item,
      examRegistResults: updatedErrors,
    };
    // 基準値チェック
    const rangesValidatedItem = setRangesErrorMessage(prevItem);
    return handleErrorMessage(rangesValidatedItem);
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
      )?.value; // 指定された detail を探す // value を取得
  };

  // 値設定用
  const setValueByPositionNumbers = (
    examItems: InputExamItem[],
    itemNumber: number,
    detailNumber: number,
    newValue: string,
  ): typeof examItems =>
    examItems.map((item) =>
      item.positionNumber === itemNumber
        ? validationCheck({
            ...item,
            examItemDetails: item.examItemDetails?.map((detail) =>
              detail.positionNumber === detailNumber
                ? { ...detail, value: newValue } // value を更新
                : detail,
            ),
          })
        : item,
    );

  // 各項目の値変更時
  const handleChange = (
    value: string,
    itemNumber: number,
    detailNumber: number,
    isSelector?: boolean,
  ) => {
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

    // 矯正の入力値の場合、裸眼の入力値に空文字列を設定
    if (itemNumber === 2 && (detailNumber === 3 || detailNumber === 4)) {
      const nakedItemNumber = 1;
      const targetDetailNumber = detailNumber === 3 ? 1 : 2;
      const nakedValue = getValueByPositionNumbers(
        updatedExamItems,
        nakedItemNumber,
        targetDetailNumber,
      );
      if (!nakedValue) {
        setValueByPositionNumbers(
          updatedExamItems,
          itemNumber,
          targetDetailNumber,
          "",
        );
      }
    }
    setExamItemsData(updatedExamItems);

    // 3つのエラーが存在するか確認
    const isValidatedError = updatedExamItems.some((item) => {
      // item.examRegistResults が null でない場合のみ処理を行う
      return item.examRegistResults?.some((error) => {
        // 必須エラーの判定
        const isRequired =
          error.description ===
          getErrorMessage(errorMessages.required, "視力は");

        // 半角数字エラーの判定
        const isNumericError =
          error.description ===
          getErrorMessage(errorMessages.numericString, `${item.name}は`);

        // TODO:範囲エラーの判定
        const isRangeError = error.description?.includes(
          "正常値ではありません",
        );

        // 対象のエラーがあれば true を返す
        return isRequired || isNumericError || isRangeError;
      });
    });

    // エラーが存在しない場合のみ onChange を実行
    if (!isValidatedError) {
      onChange(updatedExamItems);
    }
  };

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
              w={index === 2 ? 373 : 592}
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
        {examItemsData.map((item) => {
          const isCorrection = item.positionNumber === 2;
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
                      isBothEyes = detail.positionNumber === 5;
                    } else {
                      isBothEyes = detail.positionNumber === 3;
                    }
                    //選択ボタン用
                    const isSelector =
                      Array.isArray(detail.examItemDetailOptions) &&
                      detail.examItemDetailOptions.length > 0;

                    return (
                      <>
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
                                    w={278}
                                    h={78}
                                    variant="outline"
                                    bg={
                                      isDisabled
                                        ? "gray03"
                                        : isSelected
                                          ? "green03"
                                          : "white"
                                    }
                                    color={
                                      isDisabled
                                        ? "gray02"
                                        : isSelected
                                          ? "primary"
                                          : "gray02"
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
                                    item.examRegistResults?.some(
                                      (x) =>
                                        x.errorLevel === InputErrorLevel.異常,
                                    )
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
                              <Text w={170} size="lg" fw={700}>
                                (前回：{detail.prevValue})
                              </Text>
                            </Group>
                          )}
                        </GridCol>
                      </>
                    );
                  })}
                </Grid>
              </Group>
              {/* エラーメッセージ */}
              <Stack key={item.positionNumber} gap={0}>
                {item.examRegistResults?.map((error, index) => {
                  const isWarning = error.errorLevel === InputErrorLevel.警告;
                  return (
                    <Group key={index} c={isWarning ? "warning" : "error"}>
                      {isWarning ? (
                        <IconExclamationCircleFilled size="32px" />
                      ) : (
                        <IconSquareRoundedXFilled size="32px" />
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
                        {detail?.keyboard?.keyboardType === 1 ? (
                          <NumericKeyboard
                            value={detail?.value ?? ""}
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
