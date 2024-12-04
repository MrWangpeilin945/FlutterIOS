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
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";

type SelectProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onClick: (updatedExamItem: InputExamItem[] | undefined) => void;
};

export default function ExamSelectLR({
  examItems,
  onRegisterPressed,
  onClick,
}: SelectProps) {
  const examItem = examItems[0];
  const examItemDetails = examItem.examItemDetails;
  if (!examItemDetails) {
    return undefined;
  }
  const [errMessages, setErrMessages] = useState<
    ExamRegistResult[] | undefined
  >(examItem.examRegistResults);

  // 選択状態の管理
  const [selectedValues, setSelectedValues] = useState<string[]>(
    examItemDetails.map((detail) => detail.value ?? ""),
  );

  //エラーメッセージ処理
  const handleErrorMessage = () => {
    const initialMessages = examItem.examRegistResults || [];
    let updatedMessages: ExamRegistResult[] = [...initialMessages];

    examItemDetails.forEach((detail, index) => {
      // 必須用エラーメッセージ
      const errorMessage: ExamRegistResult = {
        description: getErrorMessage(
          errorMessages.required,
          `${detail.name}は`, // 各 ExamItemDetail.name を使用
        ),
        errorLevel: InputErrorLevel.異常,
      };

      // 必須チェック: value が空ならエラーを追加
      if (onRegisterPressed && !selectedValues[index]) {
        // 同じエラーメッセージがない場合のみ追加
        if (
          !updatedMessages.some(
            (msg) => msg.description === errorMessage.description,
          )
        ) {
          updatedMessages.push(errorMessage);
        }
      }
    });

    // 既存メッセージから不要なものを削除
    updatedMessages = updatedMessages.filter(
      (msg) =>
        !examItemDetails.some(
          (detail, index) =>
            msg.description ===
              getErrorMessage(errorMessages.required, `${detail.name}は`) &&
            selectedValues[index] !== "",
        ),
    );

    // エラーメッセージをソート
    updatedMessages.sort((a, b) => (b.errorLevel ?? 0) - (a.errorLevel ?? 0));
    setErrMessages(updatedMessages);
  };

  useEffect(() => {
    handleErrorMessage();
  }, [examItem, onRegisterPressed, selectedValues]);

  // 選択ボタン押下時
  const onSelect = async (selector: ExamItemDetailOption, index: number) => {
    // 更新後の選択値を計算してnewSelectedに格納
    const newSelected = [...selectedValues];
    newSelected[index] =
      newSelected[index] === selector.code ? "" : (selector.code ?? "");

    // 状態を更新
    setSelectedValues(newSelected);

    const updatedExamItems: InputExamItem[] = examItems.map((item, index) =>
      index === 0
        ? {
            ...item,
            examItemDetails: item.examItemDetails?.map((detail, i) => ({
              ...detail,
              value: newSelected[i],
            })),
          }
        : item,
    );

    // onClickで更新されたexamItemsを渡す
    onClick(updatedExamItems);
  };

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Box mb={16}>
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
      </Box>

      <Flex gap={40}>
        {examItemDetails.map((detail, index) => {
          // グレーアウト表示判定
          const isDisabled = !detail.hasOrder || !!detail.cancelReasonId;
          return (
            <Stack key={index} gap={0}>
              <Paper w={524} bg="gray02" c="white" radius="itemName">
                <Text size="lg" fw={700} ta="center">
                  {detail.name}
                </Text>
              </Paper>
              {detail.prevValue && (
                <Text ml="auto" fw={700} maw={271}>
                  (前回：{detail.prevValue})
                </Text>
              )}
              <Stack key={index}>
                {detail.examItemDetailOptions?.map(
                  (selector: ExamItemDetailOption) => {
                    const isSelected = selectedValues[index] === selector.code;
                    return (
                      <Button
                        key={selector.orderNumber}
                        w={524}
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
                        size="xl"
                        fw={700}
                        disabled={isDisabled}
                        onClick={() => onSelect(selector, index)}
                      >
                        {selector.name}
                      </Button>
                    );
                  },
                )}
              </Stack>
            </Stack>
          );
        })}
      </Flex>

      {(errMessages || []).map((error, index) => (
        <Group key={index} c={error.errorLevel === 2 ? "warning" : "error"}>
          {error.errorLevel === 2 ? (
            <IconExclamationCircleFilled size="32px" />
          ) : (
            <IconSquareRoundedXFilled size="32px" />
          )}
          <Text size="sm" fw={700}>
            {error.description}
          </Text>
        </Group>
      ))}
    </Flex>
  );
}
