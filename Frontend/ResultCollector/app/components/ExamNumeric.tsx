import type React from "react";
import { useState } from "react";
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
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

// Todo 検査入力系の引数設定　今は仮で入れてます
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

//仮用

export default function ExamNumeric({ examItems, onChange }: ExamNumericProps) {
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setexamvalue] = useState("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));

  // バリデーションチェック
  const validationCheck = () => {
    const validationSchema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, "検査値は"))
      .max(20, getErrorMessage(errorMessages.maxLength, "検査値は", 20))
      .regex(
        /^[a-zA-Z0-9]+$/,
        getErrorMessage(errorMessages.alphaNumericString, "検査値は")
      );
    const result = validationSchema.safeParse(examValue);
    if (!result.success) {
      setErrorMessage(result.error.errors[0].message);

      return false;
    }
    return true;
  };
  // 確定処理
  const handleConfirm = async () => {
    validationCheck;
    if (validationCheck()) {
      setErrorMessage(null);
    }
  };

  return (
    <>
      <Flex align={"flex-start"} direction={"column"} mb={"xs"}>
        <Group mb={"xs"}>
          <Paper className={styles["basic-grey"]} radius="lg" px="xl" py="md">
            <Title order={1} fw={500}>
              {examItems.examItemName}
            </Title>
          </Paper>
          <TextInput
            size="xl"
            value={examValue}
            onClick={() => setShowKeyboard(true)}
            w={"300"}
            // onChange={(e) => handleInputChange(e)}
          />
          <Stack>
            <Text size="ms">
              前回値（{examItems.examItemDetails[0].prevValue}）
            </Text>
            <Text size="ms">{examItems.examItemDetails[0].unit}</Text>
          </Stack>
          <Button variant="outline" onClick={() => setexamvalue("")}>
            クリア
          </Button>
        </Group>
        {errorMessage && (
          <Group className={styles["alert-orange"]}>
            <IconExclamationCircleFilled color="red" size={"1.7rem"} />
            <Text c={"red"}>{errorMessage}</Text>
          </Group>
        )}
        {showKeyboard && (
          <div ref={closeKeyBoard}>
            <Keyboard
              size={150}
              value={examValue}
              onChange={(e: string) => setexamvalue(e)}
              onConfirm={handleConfirm}
            />
          </div>
        )}
      </Flex>
    </>
  );
}
