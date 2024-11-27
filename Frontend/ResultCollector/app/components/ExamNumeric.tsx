import type React from "react";
import { useEffect, useState } from "react";
import { z } from "zod";
import {
  Group,
  Stack,
  Title,
  Paper,
  Text,
  TextInput,
  Button,
  Flex,
  Box,
} from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import Keyboard from "~/components/NumericKeyboard";
import AuthWrapper from "~/components/AuthWrapper";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";
import { replace } from "@remix-run/react";

type ExamNumericProps = {
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
  onChange: (newValue: string) => void;
};

export default function ExamNumeric({ examItems, onChange }: ExamNumericProps) {
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setExamValue] = useState("");
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));
  const handleConfirm: () => void = () => {
    setShowKeyboard(false);
  };

  // エラーメッセージをAPIのレスポンスによって初期化する
  function getApiErrorMessages(
    errorMessages: { value: string }[] | undefined
  ): string[] {
    if (errorMessages) {
      return errorMessages.map((error: { value: string }) => error.value);
    }
    return [];
  }
  // TODO:エラーメッセージに関するAPI仕様確定後に修正
  const [errorMessage, setErrorMessage] = useState<string[]>(() =>
    getApiErrorMessages(examItems.errorMessages)
  );

  // examItemsのrangesから、エラーレベルを取得して
  // エラーメッセージを追加する。
  const getErrorLevelFromRanges = () => {
    const numericValue = Number.parseFloat(formatDecimalValue(examValue));
    const ranges = examItems.examItemDetails[0].ranges;

    const errorLevel = ranges.find((range) => {
      return (
        numericValue >= range.numericMin && numericValue <= range.numericMax
      );
    })?.errorLevel;
    if (errorLevel === 4) {
      setErrorMessage((prevErrorMessages) => [
        ...prevErrorMessages,
        getErrorMessage(errorMessages[4]),
      ]);
    }
    if (errorLevel === 3) {
      setErrorMessage((prevErrorMessages) => [
        ...prevErrorMessages,
        getErrorMessage(errorMessages[3]),
      ]);
    }
  };
  // 半角数字のチェック
  const numericRegex = /^[0-9]+$/;
  const numericCheck = () => {
    if (!examValue) {
      return; // 入力値が存在しない場合はバリデーションを行わない
    }
    const value = examValue.replace(".", "");
    if (!numericRegex.test(value)) {
      const errorMessage = getErrorMessage(
        errorMessages.numericString,
        "検査値は"
      );
      setErrorMessage((prevErrorMessages) => [
        ...prevErrorMessages,
        errorMessage,
      ]);
    }
  };

  // 数値が変化するごとに行うバリデーションチェック
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    setErrorMessage(getApiErrorMessages(examItems.errorMessages));
    numericCheck();
    getErrorLevelFromRanges();
  }, [examValue]); // 入力値の変化時にバリデーションチェックを実行

  // 小数点処理
  const formatDecimalValue = (value: string) => {
    let result = value.replace(".", "");
    if (result.length >= 2) {
      result = `${value.slice(0, -1)}.${value.slice(-1)}`; // 一番後ろから1つめと2つ目の間に小数点を挿入
    }
    return result;
  };
  // テキストボックス入力時の処理
  const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.currentTarget.value;
    setExamValue(value);
  };
  // キーボード入力時の処理
  const handleKeyChange = (e: string) => {
    setExamValue(e);
  };
  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Group mb={"xs"}>
        <Paper className={styles["basic-grey"]} radius="lg" px="md" py="10">
          <Title size="lg" fw={700}>
            {examItems.examItemName}
          </Title>
        </Paper>
        <TextInput
          size="xl"
          value={formatDecimalValue(examValue)}
          maxLength={5}
          onClick={() => setShowKeyboard(true)}
          w={"340"}
          onChange={(e) => {
            handleTextChange;
          }}
        />
        <Stack gap="0">
          <Text size="md" fw="700">
            前回値（{examItems.examItemDetails[0].prevValue}）
          </Text>
          <Text size="xs" fw="400">
            {examItems.examItemDetails[0].unit}
          </Text>
        </Stack>
        <Button bg={"white"} variant="outline" onClick={() => setExamValue("")}>
          クリア
        </Button>
      </Group>
      {/* エラーメッセージを表示する。 */}
      {errorMessage.map((error, index) => (
        <Group key={index} c="warning">
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error}</Text>
        </Group>
      ))}
      <Box ml={80}>
        {showKeyboard && (
          <div ref={closeKeyBoard}>
            <Keyboard
              value={examValue}
              onChange={(e: string) => {
                handleKeyChange(e);
              }}
              onConfirm={handleConfirm}
            />
          </div>
        )}
      </Box>
    </Flex>
  );
}
