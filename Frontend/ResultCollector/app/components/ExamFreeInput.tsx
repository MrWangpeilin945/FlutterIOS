import type React from "react";
import { z } from "zod";
import { useEffect, useState } from "react";
import {
  Group,
  Title,
  Paper,
  Text,
  Textarea,
  Button,
  Flex,
} from "@mantine/core";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import type {
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamFreeInputProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newValue: InputExamItem) => void;
};

export default function ExamFreeInput({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamFreeInputProps) {
  const firstExamItem = examItems?.[0];
  const firstExamItemDetail = firstExamItem.examItemDetails?.[0];
  if (!firstExamItemDetail) {
    return null;
  }
  const [examValue, setExamValue] = useState(firstExamItemDetail?.value || "");
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();

  // APIのエラーメッセージの取得
  const getAPIErrorMessages = () => {
    const APIerror = firstExamItem.examRegistResults || [];
    return APIerror || [];
  };

  // 必須バリデーションチェック
  const validationRequire = () => {
    const requireSchema = z
      .string()
      .min(
        1,
        getErrorMessage(errorMessages.required, `${firstExamItem.name}は`)
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
  // エラーレベルに応じた並び替え
  const sortErrorMessages = (result: ExamRegistResult[]) => {
    const sortedMessages = result.sort(
      (a, b) => (b.errorLevel ?? 0) - (a.errorLevel ?? 0)
    );
    setErrMessages(sortedMessages);
  };
  // バリデーションチェックの走査
  useEffect(() => {
    const messages = getAPIErrorMessages();
    const requireError = validationRequire();
    const result = messages.concat(requireError);
    sortErrorMessages(result);
  }, [examValue, onRegisterPressed]);

  // examItemsを更新して渡す処理
  // TODO:エラーレベルでコールバックを制御するか確認
  const updatedExamItems = () => {
    if (errMessages?.some((x) => x.errorLevel === InputErrorLevel.異常)) {
      return;
    }
    const newExamItems: InputExamItem = {
      ...examItems,
      examItemDetails: [
        ...(firstExamItemDetail
          ? [
              {
                ...firstExamItemDetail,
                value: examValue,
              },
            ]
          : []),
        ...(firstExamItem.examItemDetails?.slice(1) ?? []),
      ],
    };
    onChange(newExamItems);
  };

  // テキストボックス入力時の処理
  const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.currentTarget.value;
    setExamValue(value);
    updatedExamItems();
  };

  return (
    <Flex mt={16} justify="flex-start" align="flex-start" direction="column">
      <Flex gap={16} justify="flex-start" align="flex-start">
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
            {firstExamItem.name}
          </Title>
        </Paper>
        <Textarea
          classNames={{
            input: `${styles["input-textare"]} ${
              errMessages?.some((x) => x.errorLevel === InputErrorLevel.異常)
                ? `${styles["input-error"]}`
                : errMessages?.some(
                    (x) => x.errorLevel === InputErrorLevel.警告
                  )
                ? `${styles["input-warning"]}`
                : ""
            }`,
          }}
          w={"524"}
          radius={"md"}
          size="sm"
          value={examValue}
          onChange={(e) => {
            handleTextChange(e);
          }}
          autosize
          minRows={1}
          maxRows={4}
        />
        {/* TODO:前回値のマックス横幅設定 */}

        <Button
          w={154}
          h={64}
          ml={48}
          size="lg"
          bg={"white"}
          variant="outline"
          onClick={() => setExamValue("")}
        >
          クリア
        </Button>
      </Flex>
      <Text
        mt={16}
        ml={287}
        size="md"
        fw="700"
        className={styles["text-multiline"]}
      >
        {firstExamItemDetail?.prevValue
          ? `(前回値)\n${firstExamItemDetail?.prevValue}`
          : ""}
      </Text>
      {/* エラーメッセージを表示する。 */}
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
