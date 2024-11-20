import { useEffect, useState } from "react";
import { Button, Group, Text, Flex, Paper, Title } from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";

type selectProps = {
  //Orvalで生成したschemaを参照予定
  examItems: {
    positionNumber: number;
    examItemId: number;
    examItemName: string;
    errorMessages: { value: string }[];
    examItemDetails: {
      positionNumber: number;
      examItemDetailId: number;
      examItemDetailName: string;
      value: string;
      prevValue: string;
      unit: string;
      examItemDetailType: string;
      isCancelled: boolean;
      kikiDetail: string;
      afterDecimalPointDigit: number;
      keyboard: {
        Type: number;
        keys: string[];
      };
      selectors: {
        selectorId: string;
        selectorName: string;
      }[];
      ranges: {
        errorLevel: number;
        numericMin: number;
        numericMax: number;
      }[];
    }[];
  };
  onRegisterPressed: number;
  onClick: (option: string) => void;
};

export default function ExamSelect({
  examItems,
  onRegisterPressed,
  onClick,
}: selectProps) {
  const [selected, setSelected] = useState(() => {
    const examItemDetail = examItems.examItemDetails[0];
    if (examItemDetail.value && examItemDetail.value !== "") {
      return examItemDetail.value;
    }
    if (examItemDetail.prevValue && examItemDetail.prevValue !== "") {
      return examItemDetail.prevValue;
    }
    return "";
  });
  //TODO：エラーメッセージの形式が未確定のため決定次第、修正予定
  // errorMessagesをexamItemsに基づいて設定
  const [errorMessages, setErrorMessages] = useState<string[]>(() =>{
    if (examItems.errorMessages) {
      const extractedValues = examItems.errorMessages.map(
        (error: { value: string }) => error.value,
      );
      return extractedValues;
    }
    return [""];
  });


  // selectedが空の時にエラーメッセージを追加
  useEffect(() => {
    const errorMessage = `${examItems.examItemName}は必須項目です`;

    if (onRegisterPressed === 1 && selected === "") {
      setErrorMessages((items) => [...items, errorMessage]);
    } else {
      // selectedが空でなくなった場合、エラーメッセージを削除
      setErrorMessages((prevErrors) =>
        prevErrors.filter((message) => message !== errorMessage),
      );
    }
  }, [examItems.examItemName, onRegisterPressed, selected]);

  //選択/未選択の切替
  const onSelect = (selector: { selectorId: string; selectorName: string }) => {
    if (selected === selector.selectorId) {
      // すでに選択されている場合、選択を解除する
      setSelected("");
      onClick("");
    } else {
      // 新しく選択する
      setSelected(selector.selectorId);
      onClick(selector.selectorId);
    }
  };

  const selectors = examItems.examItemDetails[0].selectors;

  return (
    <>
      <Flex mb="xs">
        <Paper bg="gray02" c="white" radius="lg" px="md" py="10">
          <Title order={1} fw={500}>
            {examItems.examItemName}
          </Title>
        </Paper>
        <Text mt="xs">（前回：{examItems.examItemDetails[0].prevValue}）</Text>
      </Flex>

      <Group mb="xs">
        {selectors.map((selector) => {
          const isCancelled = examItems.examItemDetails[0].isCancelled;
          const isSelected = selected === selector.selectorId;

          return (
            <Button
              size="xl"
              key={selector.selectorId}
              onClick={() => onSelect(selector)}
              variant="outline"
              //グレーアウト、選択済/未選択によって色を変更
              bg={isCancelled ? "#e0e0e0" : isSelected ? "green03" : "white"} 
              color={isCancelled ? "#9e9e9e" : isSelected ? "primary" : "gray02"} 
              disabled={isCancelled}  // isCancelledがtrueの場合、ボタンを無効化
            >
              {selector.selectorName}
            </Button>
          );
        })}
      </Group>
      
      {/* エラーメッセージを表示 */}
      {errorMessages.map((error,index) => (
        <Group key={index} c="warning">
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error}</Text>
        </Group>
      ))}
    </>
  );
}
