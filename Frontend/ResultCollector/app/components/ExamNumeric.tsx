import type React from "react";
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

type ExamNumericProps = {
  examItems: InputExamItem;
  onRegisterPressed: boolean;
  onChange: (newValue: InputExamItem) => void;
};

export default function ExamNumeric({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericProps) {
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setExamValue] = useState(
    examItems.examItemDetails?.at(0)?.value || ""
  );
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));
  const handleConfirm: () => void = () => {
    setShowKeyboard(false);
  };
  // APIのレスポンスがあればエラーメッセージの初期に設定
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();

  // エラーメッセージの初期化
  const getAPIErrorMessages = () => {
    const APIerror = examItems.examRegistResults || [];
    return APIerror || [];
  };
  // examItemsのrangesから、エラーレベルを取得して
  // エラーメッセージを追加する。
  const addErrorMessagesFromRanges = () => {
    const numericValue = Number.parseFloat(formatDecimalValue(examValue));
    const ranges = examItems.examItemDetails?.at(0)?.examNormalValueRanges;
    const errorLevel = ranges?.find((range) => {
      const minValue = range.minValue;
      const maxValue = range.maxValue;
      if (typeof minValue === "number" && typeof maxValue === "number")
        return numericValue >= minValue && numericValue <= maxValue;
    })?.errorLevel;
    const warningMessage: ExamRegistResult = {
      // TODO:具体的なメッセージが決定したら差し替え
      description: "異常エラーです。",
      errorLevel: InputErrorLevel.異常,
    };
    const alertMessage: ExamRegistResult = {
      // TODO:具体的なメッセージが決定したら差し替え
      description: "警告エラーです。",
      errorLevel: InputErrorLevel.警告,
    };
    if (errorLevel === 3) {
      return [warningMessage];
    }
    if (errorLevel === 2) {
      return [alertMessage];
    }
    return [];
  };
  // 半角数字のチェック
  const validationNumeric = () => {
    const numericRegex = /^[0-9]+$/;
    if (examValue && !numericRegex.test(examValue)) {
      const numericMessage: ExamRegistResult = {
        description: getErrorMessage(
          errorMessages.alphaNumericString,
          `${examItems.name}は`
        ),
        errorLevel: InputErrorLevel.警告,
      };
      return numericMessage || [];
    }
    return [];
  };
  // 必須バリデーションチェック
  const validationRequire = () => {
    const requireMessage: ExamRegistResult = {
      description: getErrorMessage(
        errorMessages.required,
        `${examItems.name}は`
      ),
      errorLevel: InputErrorLevel.異常,
    };
    if (onRegisterPressed && examValue === "") {
      return requireMessage || [];
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
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    const messages = getAPIErrorMessages();
    const rangeError = addErrorMessagesFromRanges();
    const numericError = validationNumeric();
    const requireError = validationRequire();
    const result = messages
      .concat(rangeError)
      .concat(numericError)
      .concat(requireError);
    sortErrorMessages(result);
  }, [examValue, onRegisterPressed]);

  // 表示時の小数点処理
  const formatDecimalValue = (value: string) => {
    const floatValue = Number.parseFloat(value);
    if (Number.isNaN(floatValue)) {
      return value;
    }
    const afterDecimalDigit = examItems?.examItemDetails?.at(0)?.decimalLength;
    if (afterDecimalDigit) {
      const result = (floatValue / 10 ** afterDecimalDigit)
        .toFixed(afterDecimalDigit)
        .toString();
      return result;
    }
    return value;
  };
  // examItemsを更新して渡す処理
  const updatedExamItems = () => {
    if (errMessages?.at(0)?.errorLevel === 3) {
      return;
    }
    const newExamItems: InputExamItem = {
      ...examItems,
      examItemDetails: [
        ...(examItems.examItemDetails?.[0]
          ? [
              {
                ...examItems.examItemDetails[0],
                value: examValue,
              },
            ]
          : []),
        ...(examItems.examItemDetails?.slice(1) ?? []),
      ],
    };
    onChange(newExamItems);
  };
  // テキストボックス入力時の処理
  const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.currentTarget.value;
    value.replace(".", "");
    setExamValue(value);
    updatedExamItems();
  };
  // キーボード入力時の処理
  const handleKeyChange = (e: string) => {
    setExamValue(e);
  };

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Group w="1036" gap="md">
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
            {examItems.name}
          </Title>
        </Paper>
        <TextInput
          className="input-textbox"
          classNames={{
            input:
              errMessages?.at(0)?.errorLevel === 3
                ? styles.inputerror
                : errMessages?.at(0)?.errorLevel === 2
                ? styles.inputwarning
                : styles.inputTextbox,
          }}
          w={"340"}
          radius={"md"}
          size="inputComponent"
          value={formatDecimalValue(examValue)}
          disabled={!!examItems.examItemDetails?.at(0)?.cancelReasonId}
          onClick={() => setShowKeyboard(true)}
          onChange={(e) => {
            handleTextChange(e);
          }}
        />
        {/* TODO:前回値のマックス横幅設定 */}
        <Stack gap="0">
          <Text size="md" fw="700" maw={""}>
            (前回 : {examItems?.examItemDetails?.at(0)?.prevValue}）
          </Text>
          <Text size="xs" fw="400">
            {examItems?.examItemDetails?.at(0)?.unit}
          </Text>
        </Stack>
        <Button
          w={154}
          h={64}
          size="lg"
          bg={"white"}
          variant="outline"
          ml={48}
          onClick={() => setExamValue("")}
        >
          クリア
        </Button>
      </Group>
      {/* エラーメッセージを表示する。 */}
      {(errMessages || []).map((error, index) => (
        <Group key={index} c={error.errorLevel === 3 ? "error" : "warning"}>
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error.description}</Text>
        </Group>
      ))}
      <Box ml={220} mt={50}>
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
