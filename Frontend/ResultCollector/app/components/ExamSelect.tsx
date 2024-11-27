import { useEffect, useState } from "react";
import { Button, Group, Text, Flex, Paper, Center } from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamItemDetailOption,
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";

type SelectProps = {
  examItems: InputExamItem;
  onRegisterPressed: number;
  onClick: (updatedExamItem: InputExamItem | undefined) => void;
};

export default function ExamSelect({
  examItems,
  onRegisterPressed,
  onClick,
}: SelectProps) {
  if (!examItems.examItemDetails?.length) {
    return null; // examItemDetailsが空の場合は何も表示しない
  }

  const examItemDetail = examItems.examItemDetails[0];
  if (!examItemDetail) {
    return null; // examItemDetailがundefinedの場合は何も表示しない
  }

  const [errMessages, setErrMessages] = useState<
    ExamRegistResult[] | undefined
  >(examItems.examRegistResults);
  const [selected, setSelected] = useState(
    examItemDetail.value || examItemDetail.prevValue || undefined,
  );

  const handleErrorMessage = () => {
    const initialMessages = examItems.examRegistResults || [];
    let updatedMessages = [...initialMessages];

    //必須チェック
    const errorMessage: ExamRegistResult = {
      description: getErrorMessage(
        errorMessages.required,
        `${examItems.name}は`,
      ),
      errorLevel: 2,
    };

    if (onRegisterPressed === 1 && selected === "") {
      //メッセージを追加
      updatedMessages.push(errorMessage);
    } else {
      //重複メッセージを削除
      updatedMessages = updatedMessages.filter(
        (message) => message.description !== errorMessage.description,
      );
    }

    //メッセージをソート
    updatedMessages.sort((a, b) => (b.errorLevel ?? 0) - (a.errorLevel ?? 0));
    setErrMessages(updatedMessages);
  };

  useEffect(handleErrorMessage, [
    examItems.examRegistResults,
    examItems.name,
    onRegisterPressed,
    selected,
  ]);

  const onSelect = (selector: ExamItemDetailOption) => {
    const newSelected = selected === selector.code ? "" : selector.code;

    setSelected(newSelected);

    //examItemsのvalueを更新
    const updatedExamItems: InputExamItem = {
      ...examItems,
      examItemDetails: (examItems.examItemDetails || []).map((detail) =>
        detail.examItemDetailId === examItemDetail.examItemDetailId
          ? { ...detail, value: newSelected }
          : detail,
      ),
    };

    onClick(updatedExamItems);
  };

  const selectors = examItemDetail.examItemDetailOptions || [];

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Flex mb={16}>
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
        <Text>（前回：{examItemDetail.prevValue}）</Text>
      </Flex>

      <Group>
        {selectors.map((selector) => {
          const isCancelled = examItemDetail.cancelReasonId !== undefined;
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
              bg={isCancelled ? "gray03" : isSelected ? "green03" : "white"}
              color={isCancelled ? "gray02" : isSelected ? "primary" : "gray02"}
              disabled={!!isCancelled}
            >
              {selector.name}
            </Button>
          );
        })}
      </Group>

      {(errMessages || []).map((error, index) => (
        <Group key={index} c={error.errorLevel === 2 ? "warning" : "error"} >
          <IconExclamationCircleFilled size={"32px"} />
          <Text size="sm" fw={700}>
            {error.description}
          </Text>
        </Group>
      ))}
    </Flex>
  );
}
