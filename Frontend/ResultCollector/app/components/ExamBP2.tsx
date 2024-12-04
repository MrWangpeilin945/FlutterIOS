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
  // 引数に必要な値が存在するかのチェック
  if (
    ![0, 1, 2].every((index) =>
      examItems[index]?.examItemDetails?.some(
        (item) => item.positionNumber === 1 || item.positionNumber === 2
      )
    )
  ) {
    return null;
  }

  // 状態の宣言
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [examValue, setExamValue] = useState(firstExamItem?.value || "");
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));
  const handleConfirm: () => void = () => {
    setShowKeyboard(false);
  };
}
