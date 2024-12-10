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
  Flex,
  Box,
} from "@mantine/core";
import { useClickOutside } from "@mantine/hooks";
import Keyboard from "~/components/NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/tet";
import type {
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";
import Index from "~/routes/_index";

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
  const [examValues, setExamValues] = useState<string[]>(
    firstExamItemDetails.map((item) => item.value || "")
  );
  const [showKeyboards, setShowKeyboards] = useState<boolean[]>(
    firstExamItemDetails.map(() => false)
  );
  const closeKeyBoard = useClickOutside(() =>
    setShowKeyboards(Array(firstExamItemDetails.length).fill(false))
  );
  const [errMessages, setErrMessages] = useState<ExamRegistResult[]>();

  // テキストボックス押下時の動作
  const handleTextboxClick = (index: number) => {
    setShowKeyboards((prev) => {
      const updated = [...prev];
      updated[index] = true;
      return updated;
    });
  };

  // キーボードの確定キー押下時に非表示にする
  const handleConfirm: () => void = () => {
    setShowKeyboards(Array(firstExamItemDetails.length).fill(false));
  };

  // 表示時の小数点追加処理
  const formatDecimalValue = (value: string, index: number) => {
    const floatValue = Number.parseFloat(value);
    const afterDecimalDigit = firstExamItemDetails[index]?.decimalLength;
    if (afterDecimalDigit && !Number.isNaN(floatValue)) {
      const result = (floatValue / 10 ** afterDecimalDigit)
        .toFixed(afterDecimalDigit)
        .toString();
      return result;
    }
    return value;
  };

  // 最大桁数を考慮してexamValueをセットする
  const setExamValueWithMaxDigits = (examValue: string, index: number) => {
    const decimalLength = firstExamItemDetails[index]?.decimalLength ?? null;
    const integerLength = firstExamItemDetails[index]?.integerLength ?? 0;
    const updatedExamValue = [...examValues];
    if (decimalLength) {
      const maxDigits = decimalLength + integerLength;
      updatedExamValue[index] = examValue.slice(0, maxDigits);
    } else {
      updatedExamValue[index] = examValue;
    }
    setExamValues(updatedExamValue);
  };

  // examItemsを更新して渡す処理
  const updatedExamItems = () => {
    // if (errMessages?.some((x) => x.errorLevel === InputErrorLevel.異常)) {
    //   return;
    // }
    // TODO：どのときにコールバックしないかは確認
    const newExamItemDetails = firstExamItemDetails.map((item, index) => {
      return {
        ...item,
        value: examValues[index],
      };
    });
    const newExamItems: InputExamItem = {
      ...examItems[0],
      examItemDetails: newExamItemDetails,
    };
    onChange(newExamItems);
    return newExamItems;
  };

  // テキストボックス入力時の処理
  const handleTextChange = (
    e: React.ChangeEvent<HTMLInputElement>,
    index: number
  ) => {
    const value = e.currentTarget.value;
    value.replace(".", ""); // テキストボックスの値を参照するので、小数点を取り除く
    setExamValueWithMaxDigits(value, index);
    updatedExamItems();
  };

  // キーボード入力時の処理
  const handleKeyChange = (e: string, index: number) => {
    setExamValueWithMaxDigits(e, index);
    updatedExamItems();
  };

  // APIのエラーメッセージの取得
  const getAPIErrorMessages = () => {
    const updatedExamItem = updatedExamItems();
    // TODO:共通関数の追加 ExamNormalValueRangeを参照したエラーメッセージを追加
    const examItem = setRangesErrorMessage(updatedExamItem);
    const APIerror = examItem.examRegistResults || [];
    console.log(examItem.examItemDetails);
    console.log(updatedExamItem.examItemDetails);
    console.log(examValues);
    return APIerror || [];
  };

  // 半角数字のチェック
  const validationNumeric = () => {
    const numericMessages: ExamRegistResult[] = [];
    examValues.forEach((examValue, Index) => {
      if (!examValue) {
        return [];
      }
      const validationSchema = z
        .string()
        .regex(
          /^[0-9]+$/,
          getErrorMessage(
            errorMessages.numericString,
            `${examItems[0].name}の${firstExamItemDetails[Index].name}は`
          )
        );
      const result = validationSchema.safeParse(examValue);
      if (!result.success) {
        const numericMessage: ExamRegistResult = {
          description: result.error.errors[0].message,
          errorLevel: InputErrorLevel.異常,
        };
        numericMessages.push(numericMessage);
      }
    });
    return numericMessages;
  };
  // 必須バリデーションチェック
  const validationRequire = () => {
    const requireSchema = z
      .string()
      .min(
        1,
        getErrorMessage(errorMessages.required, `${examItems[0].name}は`)
      );
    for (const examValue of examValues) {
      const result = requireSchema.safeParse(examValue);
      if (!result.success && onRegisterPressed) {
        const numericMessage: ExamRegistResult = {
          description: result.error.errors[0].message,
          errorLevel: InputErrorLevel.異常,
        };
        return [numericMessage];
      }
      return [];
    }
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
      getAPIErrorMessages().concat(validationNumeric())
      // .concat(validationRequire())
    );
    setErrMessages(result);
  }, [examValues, onRegisterPressed]);

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
        <Group gap={16}>
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
                  value={formatDecimalValue(examValues[index], index)}
                  onClick={() => handleTextboxClick(index)}
                  onChange={(e) => {
                    handleTextChange(e, index);
                  }}
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
        <Group>
          {firstExamItemDetails?.map((detail, index) => (
            <Stack key={index}>
              <Box w={540}>
                {showKeyboards[index] && (
                  <div ref={closeKeyBoard}>
                    <Keyboard
                      value={examValues[index]}
                      onChange={(e: string) => {
                        handleKeyChange(e, index);
                      }}
                      onConfirm={handleConfirm}
                    />
                  </div>
                )}
              </Box>
            </Stack>
          ))}
        </Group>
      </Stack>
    </Flex>
  );
}
