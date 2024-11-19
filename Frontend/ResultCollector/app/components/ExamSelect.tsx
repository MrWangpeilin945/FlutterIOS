import { useState } from "react";
import { Button, Group, Text, Flex, Paper, Title } from "@mantine/core";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type selectProps = {
  examItems: {
    positionNumber: number;
    examItemId: number;
    examItemName: string;
    examItemDetails: {
      positionNumber: number;
      examItemDetailId: number;
      examItemDetailName: string;
      value: string;
      prevValue: string;
      unit: string;
      examItemDetailType: string;
      isCanceld: boolean;
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

  onClick: (option: string) => void;
};

export default function ExamSelect({ examItems, onClick }: selectProps) {
  const [selected, setSelected] = useState(examItems.examItemDetails[0].value);
  const errorMessage = `${examItems.examItemName}は必須項目です`;

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
        {selectors.map((selector) => (
          <Button
            size="xl"
            key={selector.selectorId}
            onClick={() => onSelect(selector)}
            variant="outline"
            color="black"
            className={
              selected === selector.selectorId ? styles["selected-button"] : ""
            }
          >
            {selector.selectorName}
          </Button>
        ))}
      </Group>
      {/* selectedが空文字列の場合のみエラーメッセージを表示 */}
      {selected === "" && (
        <Group c="warning">
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{errorMessage}</Text>
        </Group>
      )}
    </>
  );
}
