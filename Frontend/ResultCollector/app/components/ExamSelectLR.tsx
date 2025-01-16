import { useEffect, useState } from "react";
import { Button, Group, Text, Flex, Paper, Box, Stack } from "@mantine/core";
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamItemDetailOption,
  InputExamItem,
  ExamRegistResult,
  ExamItemDetail,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";

type SelectProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onClick: (updatedExamItem: InputExamItem[] | undefined) => void;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

export default function ExamSelectLR({
  examItems,
  onRegisterPressed,
  onClick,
}: SelectProps) {
  if (!examItems || examItems.length === 0) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);
  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);
  const examItem = examItemsData.find((item) => item.positionNumber === 1);
  if (!examItem?.examItemDetails?.length) {
    return null; // examItemDetailsが空の場合は何も表示しない
  }
  const examItemDetails = examItem.examItemDetails;
  if (!examItemDetails) {
    return null;
  }

  const sortErrorMessage = (item: InputExamItem): InputExamItem => {
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

  const validationCheck = (item: InputExamItem) => {
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // detailsのpositionNumberが1と2のものについてバリデーションチェックを行う
    for (const {
      positionNumber,
      name,
      value,
      hasOrder,
      cancelReasonId,
    } of item.examItemDetails ?? []) {
      if (positionNumber !== 1 && positionNumber !== 2) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }

      const requiredMessage = getErrorMessage(
        errorMessages.required,
        `${name ?? (positionNumber === 1 ? "左" : "右")}は`,
      );

      // バリデーションが失敗した場合
      if (hasOrder && !cancelReasonId && !value) {
        componentErrorMessage.push({
          description: requiredMessage,
          errorLevel: InputErrorLevel.異常,
        });
      }
    }
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

  // 初回読み込み時にAPIのエラーメッセージを保存する
  useEffect(() => {
    const backendErrorMessages: BackendValidation[] = examItems.map((item) => ({
      itemPositionNumber: item.positionNumber ?? 0,
      examRegistResults: item.examRegistResults ?? [],
    }));
    setBackendValidation(backendErrorMessages);
  }, []);
  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      let validatedData = item;
      // onRegisterPressedがtrueの場合のみvalidationCheckを実行
      if (onRegisterPressed) {
        validatedData = validationCheck(validatedData)?.validateResult;
      }

      return validatedData;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  // 選択ボタン押下時
  const onSelect = (
    selector: ExamItemDetailOption,
    targetDetail: ExamItemDetail,
  ) => {
    const newSelected =
      targetDetail.value === selector.code ? "" : selector.code;

    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = false;

    // examItemsのvalueを更新
    const updatedExamItems: InputExamItem[] = examItemsData.map((item) => {
      if (item.positionNumber === 1) {
        // 値を更新
        const updatedExamItem = {
          ...item,
          examItemDetails: item.examItemDetails?.map((detail) =>
            detail.positionNumber === targetDetail.positionNumber
              ? {
                  ...detail,
                  value: newSelected,
                }
              : detail,
          ),
        };

        // バリデーションチェックを実施
        const { validateResult, hasCallback } =
          validationCheck(updatedExamItem);
        if (!hasCallback) {
          // falseのexamItemがあればコールバックを行わない
          hasValidationError = true;
        }

        // バリデーション結果を反映
        return {
          ...updatedExamItem,
          ...validateResult,
        };
      }

      return item;
    });

    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);

    if (!hasValidationError) {
      onClick(updatedExamItems);
    }
  };

  // 対象のexamItemの中の、positionNumberが1のexamItemDetail
  let leftItemDetail = examItemDetails?.find(
    (detail) => detail.positionNumber === 1,
  );
  if (!leftItemDetail) {
    leftItemDetail = {
      positionNumber: 1,
      name: "左",
    };
  }
  //  対象のexamItemの中の、positionNumberが2のexamItemDetail
  let rightItemDetail = examItemDetails?.find(
    (detail) => detail.positionNumber === 2,
  );
  if (!rightItemDetail) {
    rightItemDetail = {
      positionNumber: 2,
      name: "右",
    };
  }
  const targetDetails = [leftItemDetail, rightItemDetail];

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Box mb={16}>
        <Paper w={274} h={80} bg="gray02" c="white" radius="itemName" py={16}>
          <Text size="lg" fw={700} ta="center">
            {examItem.name?.slice(0, 8)}
          </Text>
        </Paper>
      </Box>

      <Flex gap={40}>
        {targetDetails.map((detail) => {
          // グレーアウト表示判定
          const isDisabled = !detail.hasOrder || !!detail.cancelReasonId;
          return (
            <Stack key={detail.positionNumber} gap={0}>
              <Paper w={524} bg="gray02" c="white" radius="itemName">
                <Text size="lg" fw={700} ta="center">
                  {detail.name
                    ? detail.name.slice(0, 14)
                    : detail.positionNumber === 1
                      ? "左"
                      : "右"}
                </Text>
              </Paper>
              <Box ml="auto" h={43.4}>
                {detail.prevValue &&
                  (() => {
                    const prevName =
                      detail.examItemDetailOptions?.find(
                        (option) => option.code === detail.prevValue,
                      )?.name || detail.prevValue;
                    return (
                      <Text ml="auto" fw={700} maw={271}>
                        (前回：{prevName})
                      </Text>
                    );
                  })()}
              </Box>
              <Stack>
                {detail.examItemDetailOptions?.map(
                  (selector: ExamItemDetailOption) => {
                    const isSelected = detail.value === selector.code;
                    return (
                      <Button
                        key={selector.orderNumber}
                        w={524}
                        h={78}
                        variant="outline"
                        bd={`2px solid ${
                          isDisabled ? "" : isSelected ? "primary" : "gray03"
                        }`}
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
                              : "gray02"
                        }
                        size="xl"
                        fw={700}
                        disabled={isDisabled}
                        onClick={() => onSelect(selector, detail)}
                      >
                        {selector.name?.slice(0,8)}
                      </Button>
                    );
                  },
                )}
              </Stack>
            </Stack>
          );
        })}
      </Flex>

      {examItem.examRegistResults?.map((error, index) => {
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
    </Flex>
  );
}
