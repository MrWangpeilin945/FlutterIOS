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
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
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
  const firstExamItem = examItems[0].examItemDetails;

  // 状態の宣言
  const [showKeyboard, setShowKeyboard] = useState<boolean[]>([false, false]);
  const [examValue, setExamValue] = useState<string[]>(
    firstExamItem.map((item) => item.value || "")
  );
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();
  const closeKeyBoard = useClickOutside(() => setShowKeyboard([false, false]));
  const handleConfirm: () => void = () => {
    setShowKeyboard([false, false]);
  };

  // APIのエラーメッセージの取得
  const getAPIErrorMessages = () => {
    // ExamNormalValueRangeを参照したエラーメッセージを追加
    examItems = setRangesErrorMessage(examItems);
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
        .concat(validationNumeric())
        .concat(validationRequire())
    );
    setErrMessages(result);
  }, [examValue, onRegisterPressed]);
}
