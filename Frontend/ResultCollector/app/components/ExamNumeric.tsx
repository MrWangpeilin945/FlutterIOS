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

type ExamNumericProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newValue: InputExamItem) => void;
};

export default function ExamNumeric({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericProps) {
  if (!examItems[0].examItemDetails?.length) {
    return null; // examItemDetailsが空の場合は何も表示しない
  }
  const firstExamItemDetail =
    examItems && examItems.length > 0 ? examItems[0].examItemDetails[0] : null;
  if (!firstExamItemDetail) {
    return null;
  }
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setExamValue] = useState(firstExamItemDetail?.value || "");
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));
  const handleConfirm: () => void = () => {
    setShowKeyboard(false);
  };

  // APIのエラーメッセージの取得
  const getAPIErrorMessages = () => {
    const APIerror = examItems[0].examRegistResults || [];
    return APIerror || [];
  };
  // examItemsのrangesから、エラーレベルを取得して
  // エラーメッセージを取得。
  const addErrorMessagesFromRanges = () => {
    const numericValue = Number.parseFloat(formatDecimalValue(examValue));
    const ranges = firstExamItemDetail?.examNormalValueRanges;
    const errorLevel = ranges?.find((range) => {
      const minValue = range.minValue;
      const maxValue = range.maxValue;
      if (typeof minValue === "number" && typeof maxValue === "number")
        return numericValue >= minValue && numericValue <= maxValue;
    })?.errorLevel;
    if (errorLevel === InputErrorLevel.異常) {
      const warningMessage: ExamRegistResult = {
        // TODO: 具体的なメッセージが決まったら差し替える
        description: "異常エラーです。",
        errorLevel: InputErrorLevel.異常,
      };
      return [warningMessage];
    }
    if (errorLevel === InputErrorLevel.警告) {
      const alertMessage: ExamRegistResult = {
        // TODO: 具体的なメッセージが決まったら差し替える
        description: "警告エラーです。",
        errorLevel: InputErrorLevel.警告,
      };
      return [alertMessage];
    }
    return [];
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
    const rangeError = addErrorMessagesFromRanges();
    const numericError = validationNumeric();
    const requireError = validationRequire();
    const result = messages
      .concat(rangeError)
      .concat(numericError)
      .concat(requireError);
    sortErrorMessages(result);
  }, [examValue, onRegisterPressed]);

  // 表示時の小数点追加処理
  const formatDecimalValue = (value: string) => {
    const floatValue = Number.parseFloat(value);
    const afterDecimalDigit = firstExamItemDetail?.decimalLength;
    if (afterDecimalDigit && !Number.isNaN(floatValue)) {
      const result = (floatValue / 10 ** afterDecimalDigit)
        .toFixed(afterDecimalDigit)
        .toString();
      return result;
    }
    return value;
  };

  // 最大桁数を考慮してexamValueをセットする
  const decimalLength = firstExamItemDetail?.decimalLength ?? 0;
  const integerLength = firstExamItemDetail?.integerLength ?? 0;
  const maxDigits = decimalLength + integerLength;
  const setExamValueWithMaxDigits = (examValue: string) => {
    if (firstExamItemDetail?.integerLength) {
      setExamValue(examValue.slice(0, maxDigits));
    } else {
      setExamValue(examValue);
    }
  };

  // examItemsを更新して渡す処理
  // TODO:エラーレベルでコールバックを制御するか確認
  const updatedExamItems = () => {
    if (errMessages?.some((x) => x.errorLevel === InputErrorLevel.異常)) {
      return;
    }
    const newExamItems: InputExamItem = {
      ...examItems,
      examItemDetails: [
        ...(examItems[0].examItemDetails?.[0]
          ? [
              {
                ...examItems[0].examItemDetails[0],
                value: examValue,
              },
            ]
          : []),
        ...(examItems[0].examItemDetails?.slice(1) ?? []),
      ],
    };
    onChange(newExamItems);
  };

  // テキストボックス入力時の処理
  const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.currentTarget.value;
    value.replace(".", ""); // テキストボックスの値を参照するので、小数点を取り除く
    setExamValueWithMaxDigits(value);
    updatedExamItems();
  };
  // キーボード入力時の処理
  const handleKeyChange = (e: string) => {
    setExamValueWithMaxDigits(e);
    updatedExamItems();
  };

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Group w="11168" gap="md">
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
        <TextInput
          classNames={{
            input: `${styles["input-textbox"]} ${
              errMessages?.some((x) => x.errorLevel === InputErrorLevel.異常)
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
          value={formatDecimalValue(examValue)}
          onClick={() => setShowKeyboard(true)}
          onChange={(e) => {
            handleTextChange(e);
          }}
        />
        {/* TODO:前回値のマックス横幅設定 */}
        <Stack gap="0">
          <Text size="md" fw="700" maw={"271"}>
            {firstExamItemDetail?.prevValue
              ? `(前回: ${firstExamItemDetail?.prevValue})`
              : ""}
          </Text>
          <Text size="xs" fw="400">
            {firstExamItemDetail?.unit}
          </Text>
        </Stack>
        <Button
          w={154}
          h={64}
          size="lg"
          bg={"white"}
          variant="outline"
          onClick={() => setExamValue("")}
        >
          クリア
        </Button>
      </Group>
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
