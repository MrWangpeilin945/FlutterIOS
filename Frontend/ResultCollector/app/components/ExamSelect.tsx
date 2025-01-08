import { useEffect, useState } from "react";
import { Button, Group, Text, Flex, Paper } from "@mantine/core";
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamItemDetailOption,
  ExamRegistResult,
  InputExamItem,
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

export default function ExamSelect({
  examItems,
  onRegisterPressed,
  onClick,
}: SelectProps) {
  if (!examItems || examItems.length === 0) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);
  const examItem = examItemsData.find((item) => item.positionNumber === 1);
  if (!examItem?.examItemDetails?.length) {
    return null; // examItemDetailsが空の場合は何も表示しない
  }

  const examItemDetail = examItem.examItemDetails.find(
    (detail) => detail.positionNumber === 1
  );
  if (!examItemDetail) {
    return null; // examItemDetailがundefinedの場合は何も表示しない
  }

  const [selected, setSelected] = useState(
    examItemDetail.value || examItemDetail.prevValue || undefined
  );
  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);

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
      (error) => error.itemPositionNumber === item.positionNumber
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
    // detailsのpositionNumberが1のものについてバリデーションチェックを行う
    for (const {
      positionNumber,
      value,
      hasOrder,
      cancelReasonId,
    } of item.examItemDetails ?? []) {
      if (positionNumber !== 1) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }

      const requiredMessage = getErrorMessage(
        errorMessages.required,
        `${item.name}は`
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
  const onSelect = (selector: ExamItemDetailOption) => {
    const newSelected = selected === selector.code ? "" : selector.code;

    setSelected(newSelected);

    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = false;

    // examItemsのvalueを更新
    const updatedExamItems: InputExamItem[] = examItems.map((item) => {
      if (item.positionNumber === 1) {
        // 値を更新
        const updatedExamItem = {
          ...item,
          examItemDetails: item.examItemDetails?.map((detail) =>
            detail.positionNumber === 1
              ? {
                  ...detail,
                  value: newSelected,
                }
              : detail
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

  const selectors = examItemDetail.examItemDetailOptions || [];

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Flex mb={16} gap={16}>
        <Paper
          w={274}
          h={80}
          bg="gray02"
          c="white"
          radius="itemName"
          px={32}
          py={16}
        >
          <Text size="lg" fw={700} ta="center">
            {examItem.name}
          </Text>
        </Paper>
        {examItemDetail.prevValue &&
          (() => {
            const prevName =
              examItemDetail.examItemDetailOptions?.find(
                (option) => option.code === examItemDetail.prevValue
              )?.name || examItemDetail.prevValue;
            return (
              <Text ml="auto" fw={700} maw={271}>
                (前回：{prevName})
              </Text>
            );
          })()}
      </Flex>

      <Group>
        {selectors.map((selector) => {
          // グレーアウト表示判定
          const isDisabled =
            !examItemDetail.hasOrder || !!examItemDetail.cancelReasonId;
          const isSelected = selected === selector.code;

          return (
            <Button
              w={320}
              h={78}
              size="xl"
              fw={700}
              key={selector.orderNumber}
              onClick={() => onSelect(selector)}
              variant="outline"
              bd={`2px solid ${
                isDisabled ? "" : isSelected ? "primary" : "gray03"
              }`}
              bg={isDisabled ? "gray03" : isSelected ? "green03" : "white"}
              c={isDisabled ? "gray02" : isSelected ? "primary" : "gray02"}
              disabled={isDisabled}
            >
              {selector.name}
            </Button>
          );
        })}
      </Group>

      {(examItem.examRegistResults || []).map((error, index) => {
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
