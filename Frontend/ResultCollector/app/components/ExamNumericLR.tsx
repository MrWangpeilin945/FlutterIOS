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
}
