import type React from "react";
import { z } from "zod";
import { useEffect, useState } from "react";
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
import { useClickOutside } from "@mantine/hooks";
import Keyboard from "~/components/NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericLRProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newValue: InputExamItem) => void;
};

export default function ExamNumericLR({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericLRProps) {
  // 必要な引数のチェック
  if (
    !examItems[0]?.examItemDetails ||
    examItems[0].examItemDetails.length < 2
  ) {
    return null;
  }
  const firstExamItemDetails = examItems[0].examItemDetails;

  // 状態の宣言
  const [showKeyboard, setShowKeyboard] = useState<boolean[]>([false, false]);
  const [examValue, setExamValue] = useState<string[]>(
    firstExamItemDetails.map((item) => item.value || "")
  );
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();
  const closeKeyBoard = useClickOutside(() => setShowKeyboard([false, false]));
  const handleConfirm: () => void = () => {
    setShowKeyboard([false, false]);
  };

  // APIのエラーメッセージの取得
  const getAPIErrorMessages = () => {
    // TODO:共通関数の追加 ExamNormalValueRangeを参照したエラーメッセージを追加
    const APIerror = examItems[0].examRegistResults || [];
    return APIerror || [];
  };

  // 半角数字のチェック
  const validationNumeric = () => {
    if (!examValue) {
      return [];
    }
    const validationSchema = z
      .string()
      .regex(
        /^[0-9]+$/,
        getErrorMessage(errorMessages.numericString, `${examItems[0].name}は`)
      );
    const result = validationSchema.safeParse(examValue);
    if (!result.success) {
      const numericMessage: ExamRegistResult = {
        description: result.error.errors[0].message,
        errorLevel: InputErrorLevel.異常,
      };
      return [numericMessage];
    }
    return [];
  };
  // 必須バリデーションチェック
  const validationRequire = () => {
    const requireSchema = z
      .string()
      .min(
        1,
        getErrorMessage(errorMessages.required, `${examItems[0].name}は`)
      );
    const result = requireSchema.safeParse(examValue);
    if (!result.success && onRegisterPressed) {
      const numericMessage: ExamRegistResult = {
        description: result.error.errors[0].message,
        errorLevel: InputErrorLevel.異常,
      };
      return [numericMessage];
    }
    return [];
  };

  // 重複を削除して、エラーレベルに応じた並び替えを行う
  const sortErrorMessages = (result: ExamRegistResult[]) => {
    const uniqueErrorMessages = Array.from(
      new Map(result.map((msg) => [msg.description, msg])).values()
    );
    const sortedMessages = uniqueErrorMessages.sort(
      (a, b) => (b.errorLevel ?? 0) - (a.errorLevel ?? 0)
    );
    return sortedMessages;
  };
  // バリデーションチェックの走査
  useEffect(() => {
    const result = sortErrorMessages(
      getAPIErrorMessages()
      // .concat(validationNumeric())
      // .concat(validationRequire())
    );
    setErrMessages(result);
  }, [examValue, onRegisterPressed]);

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Stack>
        <Paper
          w={274}
          h={80}
          className={styles["basic-grey"]}
          radius="itemName"
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Title size="lg" fw={700}>
            {examItems[0].name}
          </Title>
        </Paper>
        <Group>
          {firstExamItemDetails?.map((detail, index) => (
            <Stack key={index}>
              <Paper
                w={524}
                h={51}
                className={styles["basic-grey"]}
                radius="itemName"
                style={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <Title size="lg" fw={700}>
                  {detail.name}
                </Title>
              </Paper>
              <Group>
                <TextInput
                  classNames={{
                    input: `${styles["input-textbox"]} ${
                      errMessages?.some(
                        (x) => x.errorLevel === InputErrorLevel.異常
                      )
                        ? `${styles["input-error"]}`
                        : errMessages?.some(
                            (x) => x.errorLevel === InputErrorLevel.警告
                          )
                        ? `${styles["input-warning"]}`
                        : ""
                    }`,
                  }}
                  w={"340"}
                  radius={"md"}
                  size="inputComponent"
                  value={detail.value}
                  onClick={() => setShowKeyboard(true)}
                  onChange={(e) => {}}
                />
                {/* TODO:前回値のマックス横幅設定 */}
                <Stack gap="0">
                  <Text size="md" fw="700" maw={"172"}>
                    {detail.prevValue ? `(前回: ${detail.prevValue})` : ""}
                  </Text>
                  <Text size="xs" fw="400">
                    {detail.unit}
                  </Text>
                </Stack>
              </Group>
            </Stack>
          ))}
        </Group>
      </Stack>
      {/* エラーメッセージの表示 */}
      {(errMessages || []).map((error, index) => (
        <Group
          key={index}
          c={error.errorLevel === InputErrorLevel.異常 ? "error" : "warning"}
        >
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error.description}</Text>
        </Group>
      ))}
    </Flex>
  );
}
