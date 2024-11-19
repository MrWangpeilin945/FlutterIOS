import type React from "react";
import { useEffect, useState } from "react";
import { z } from "zod";
import {
  Group,
  Stack,
  Title,
  Box,
  Center,
  Paper,
  Text,
  TextInput,
  Button,
  Flex,
} from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import Keyboard from "~/components/SoftwareKeyboard";
import AuthWrapper from "~/components/AuthWrapper";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

export type ExamNumericProps = {
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
  onChange: (newValue: string) => void;
};

export default function ExamNumeric({ examItems, onChange }: ExamNumericProps) {
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setexamvalue] = useState("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [errorLevel, setErrorLevel] = useState<number | null>(null);

  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));

  const handleConfirm: () => void = () => {
    setShowKeyboard(false);
  };

  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    // rangesからエラーレベルの取得
    const getErrorLevelFromRanges = () => {
      const numericValue = Number.parseFloat(examValue);
      const ranges = examItems.examItemDetails[0].ranges;
      let errorLevel = null;

      for (const range of ranges) {
        if (
          numericValue >= range.numericMin &&
          numericValue <= range.numericMax
        ) {
          errorLevel = range.errorLevel;
          break; // エラーレベルが決まったらループを抜ける
        }
      }
      if (errorLevel !== null) {
        setErrorMessage(getErrorMessage(errorMessages.invalid));
        return false;
      }
      return true;
    };

    // バリデーションチェック
    const validationCheck = () => {
      const validationSchema = z
        .string()
        .min(1, getErrorMessage(errorMessages.required, "検査値は"))
        .regex(
          /^[0-9]+$/,
          getErrorMessage(errorMessages.numericString, "検査値は")
        );

      const result = validationSchema.safeParse(examValue);
      if (!result.success) {
        setErrorMessage(result.error.errors[0].message);

        return false;
      }
      return true;
    };
    const isValid = validationCheck();
    const hasError = getErrorLevelFromRanges();

    if (isValid && !hasError) {
      setErrorMessage(null);
    }
  }, [examValue]);

  // 入力値の変化時にバリデーションチェックを実行
  const handleInputChange = (value: string): void => {
    setexamvalue(value); // 状態を更新するだけ
  };

  return (
    <>
      <AuthWrapper>
        <Flex align={"flex-start"} direction={"column"} mb={"xs"}>
          <Group mb={"xs"}>
            <Paper className={styles["basic-grey"]} radius="lg" px="md" py="10">
              <Title order={1} fw={500}>
                {examItems.examItemName}
              </Title>
            </Paper>
            <TextInput
              size="xl"
              value={examValue}
              onClick={() => setShowKeyboard(true)}
              w={"300"}
              onChange={(e) => {
                setexamvalue(e.currentTarget.value);
                handleInputChange;
              }}
            />
            <Stack>
              <Text size="sm">
                前回値（{examItems.examItemDetails[0].prevValue}）
              </Text>
              <Text size="sm">{examItems.examItemDetails[0].unit}</Text>
            </Stack>
            <Button
              bg={"white"}
              variant="outline"
              onClick={() => setexamvalue("")}
            >
              クリア
            </Button>
          </Group>
          {errorMessage && (
            <Group c={"worning"}>
              <IconExclamationCircleFilled color="red" size={"1.7rem"} />
              <Text c={"primary"}>{errorMessage}</Text>
            </Group>
          )}
          {showKeyboard && (
            <div ref={closeKeyBoard}>
              <Keyboard
                size={150}
                value={examValue}
                onChange={(e: string) => handleInputChange(e)}
                onConfirm={handleConfirm}
              />
            </div>
          )}
        </Flex>
      </AuthWrapper>
    </>
  );
}
