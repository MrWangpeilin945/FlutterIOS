import { useEffect, useState } from "react";
import { Button, Group, Text, Flex, Paper, Title } from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamItemDetailOption,
  ExamRegstResult,
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
  if (!examItems.examItemDetails || examItems.examItemDetails.length === 0) {
    return null; // examItemDetailsが空の場合は何も表示しない
  }

  const examItemDetail = examItems.examItemDetails[0];
  if (!examItemDetail) {
    return null; // examItemDetailがundefinedの場合は何も表示しない
  }

  const [selected, setSelected] = useState(() => {
    return examItemDetail.value || examItemDetail.prevValue || undefined;
  });

  const [errMessages, setErrMessages] = useState<ExamRegstResult[] | undefined>(
    examItems.examRegstResults,
  );

  useEffect(() => {
    // 初期化
    const initialMessages = examItems.examRegstResults || [];
    let updatedMessages = [...initialMessages]; 

    // 検証エラーメッセージの処理
    const errorMessage: ExamRegstResult = {
      description: getErrorMessage(
        errorMessages.required,
        `${examItems.name}は`,
      ),
      errorLevel: 2,
    };

    if (onRegisterPressed === 1 && selected === "") {
      // 新しいエラーメッセージを追加
      updatedMessages.push(errorMessage);
    } else {
      // エラーメッセージを削除
      updatedMessages = updatedMessages.filter(
        (message) => message.description !== errorMessage.description,
      );
    }

    // 最後にソートしてstateを更新
    updatedMessages.sort((a, b) => {
      const levelA = a.errorLevel ?? 0; // `undefined` の場合は 0 とみなす
      const levelB = b.errorLevel ?? 0; // `undefined` の場合は 0 とみなす
      return levelB - levelA; // 降順にソート
    });
    setErrMessages(updatedMessages);
  }, [
    examItems.examRegstResults, // この依存で変更を監視
    examItems.name,
    onRegisterPressed,
    selected,
  ]);

  const onSelect = (selector: ExamItemDetailOption) => {
    const newSelected = selected === selector.code ? "" : selector.code;

    setSelected(newSelected);

    // `examItems`の`examItemDetails`を更新
    const updatedExamItems = {
      ...examItems,
      examItemDetails: (examItems.examItemDetails || []).map((detail) =>
        detail.examItemDetailId === examItemDetail.examItemDetailId
          ? { ...detail, value: newSelected }
          : detail,
      ),
    };

    // 親に更新を通知
    onClick(updatedExamItems);
  };

  const selectors = examItems.examItemDetails[0].examItemDetailOptions || [];

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Flex mb="xs">
        <Paper bg="gray02" c="white" radius="lg" px="md" py="10">
          <Title order={1} fw={500}>
            {examItems.name}
          </Title>
        </Paper>
        <Text mt="xs">（前回：{examItems.examItemDetails[0].prevValue}）</Text>
      </Flex>

      <Group mb="xs">
        {selectors.map((selector) => {
          const isCancelled = examItemDetail.cancelReasonId !== undefined;
          const isSelected = selected === selector.code;

          return (
            <Button
              size="xl"
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
        <Group
          key={index}
          c={
            error.errorLevel === 2
              ? "warning" // レベル2: 警告
              : "error" // レベル3: エラー
          }
        >
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error.description}</Text>
        </Group>
      ))}
    </Flex>
  );
}
