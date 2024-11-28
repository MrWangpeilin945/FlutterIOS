import { useEffect, useState } from "react";
import {
  Button,
  Group,
  Text,
  Flex,
  Paper,
  Center,
  Box,
  Stack,
} from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamItemDetailOption,
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";

type SelectProps = {
  examItems: InputExamItem;
  onRegisterPressed: boolean;
  onClick: (updatedExamItem: InputExamItem | undefined) => void;
};

export default function ExamSelectLR({
  examItems,
  onRegisterPressed,
  onClick,
}: SelectProps) {
  const examItemDetails = examItems.examItemDetails;
  if (!examItemDetails) {
    return null;
  }

  const [errMessages, setErrMessages] = useState<
    ExamRegistResult[] | undefined
  >(examItems.examRegistResults);

  // 選択状態の管理
  const [selectedValues, setSelectedValues] = useState<string[]>(
    examItemDetails.map((detail) => detail.value ?? ""),
  );

  const handleErrorMessage = () => {
    const initialMessages = examItems.examRegistResults || [];
    let updatedMessages = [...initialMessages];

    // 必須チェック用エラーメッセージ
    const errorMessage: ExamRegistResult = {
      description: getErrorMessage(
        errorMessages.required,
        `${examItems.name}は`,
      ),
      errorLevel: InputErrorLevel.異常,
    };

    // 空の選択肢があるかどうかチェック
    const hasEmptyValue = selectedValues.some((value) => value === "");

    if (onRegisterPressed === 1 && hasEmptyValue) {
      // 空の値がある場合、エラーメッセージを追加
      if (
        !updatedMessages.some(
          (msg) => msg.description === errorMessage.description,
        )
      ) {
        updatedMessages.push(errorMessage);
      }
    } else {
      // 空の値がない場合、エラーメッセージを削除
      updatedMessages = updatedMessages.filter(
        (msg) => msg.description !== errorMessage.description,
      );
    }

    // エラーメッセージをソート
    updatedMessages.sort((a, b) => (b.errorLevel ?? 0) - (a.errorLevel ?? 0));
    setErrMessages(updatedMessages);
  };

  useEffect(handleErrorMessage, [
    examItems.examRegistResults,
    examItems.name,
    onRegisterPressed,
    selectedValues,
  ]);

  const onSelect = (selector: ExamItemDetailOption, index: number) => {
    setSelectedValues((prevSelected) => {
      const updatedSelected = [...prevSelected];
      updatedSelected[index] =
        updatedSelected[index] === selector.code ? "" : (selector.code ?? "");
      return updatedSelected;
    });
  };

  // selectedValuesが更新されるたびにexamItemsの値も更新
  useEffect(() => {
    const updatedExamItems: InputExamItem = {
      ...examItems,
      examItemDetails: examItemDetails.map((detail, i) => ({
        ...detail,
        value: selectedValues[i], // updatedSelectedValuesを使用
      })),
    };

    // onClickで更新されたexamItemsを渡す
    onClick(updatedExamItems);
  }, [selectedValues, examItems, examItemDetails, onClick]);

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
          <Center>
            <Text size="lg" fw={700}>
              {examItems.name}
            </Text>
          </Center>
        </Paper>
      </Box>

      <Flex gap={40}>
        {examItemDetails.map((detail, index) => (
          <Stack key={index} gap={0}>
            <Paper w={524} bg="gray02" c="white" radius="itemName">
              <Center>
                <Text size="lg" fw={700}>
                  {detail.name}
                </Text>
              </Center>
            </Paper>
            <Text ml="auto" fw={700} style={{ marginTop: 0, marginBottom: 0 }}>
              (前回：{detail.prevValue})
            </Text>
            <Stack key={index}>
              {detail.examItemDetailOptions?.map(
                (selector: ExamItemDetailOption) => {
                  const isSelected = selectedValues[index] === selector.code;
                  return (
                    <Button
                      key={selector.orderNumber}
                      w={524}
                      h={78}
                      size="xl"
                      fw={700}
                      onClick={() => onSelect(selector, index)}
                      variant="outline"
                      bg={isSelected ? "green03" : "white"}
                      color={isSelected ? "primary" : "gray02"}
                    >
                      {selector.name}
                    </Button>
                  );
                },
              )}
            </Stack>
          </Stack>
        ))}
      </Flex>

      {(errMessages || []).map((error, index) => (
        <Group key={index} c={error.errorLevel === 2 ? "warning" : "error"}>
          <IconExclamationCircleFilled size={"32px"} />
          <Text size="sm" fw={700}>
            {error.description}
          </Text>
        </Group>
      ))}
    </Flex>
  );
}
