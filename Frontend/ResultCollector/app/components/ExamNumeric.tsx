import { z } from "zod";
import { useEffect, useState } from "react";
import {
  Group,
  Stack,
  Paper,
  Text,
  TextInput,
  Flex,
  Box,
  Button,
} from "@mantine/core";
import { useClickOutside } from "@mantine/hooks";
import NumericKeyboard from "~/components/NumericKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import type {
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

export default function ExamNumeric({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericProps) {
  // 引数のチェック
  // examItemのチェック
  if (!examItems || examItems.length === 0) {
    return null;
  }
  const firstPosition = examItems.find((item) => item.positionNumber === 1);
  // positionNumberが1のexamItemの中に
  // positionNumberが1のexamItemDetailがあるかチェック
  if (
    !firstPosition?.examItemDetails?.some(
      (detail) => detail.positionNumber === 1
    )
  ) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);

  // キーボードの表示インデックスを状態として管理する
  const [showKeyboards, setShowKeyboards] = useState(false);

  // キーボードのACボタン押下時にキーボードを非表示にする
  const handleConfirm = () => {
    setShowKeyboards(false);
  };
  const closeKeyBoard = useClickOutside(() => setShowKeyboards(false));

  const sortErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      // エラーレベルが高い順にソート
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });
    }
    return item;
  };

  // APIからのエラーメッセージを保存
  const APIErrors = examItems.map((item) => ({
    positionNumber: item.positionNumber,
    examRegistResults: item.examRegistResults || [],
  }));

  // 引数のexamItemのpositionNumberを参照し、エラーメッセージを初期化する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = APIErrors.find(
      (error) => error.positionNumber === item.positionNumber
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };

  const validationCheck = (item: InputExamItem) => {
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // detailsのpositionNumberが1のものについてバリデーションチェックを行う
    for (const { name, positionNumber, value } of item.examItemDetails ?? []) {
      if (positionNumber !== 1) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }
      // 必須チェックと半角数字チェックを一度に行うスキーマ
      const schema = z
        .string()
        .min(1, getErrorMessage(errorMessages.required, `${item.name}は`)) // 必須チェック
        .refine((value) => /^\d+(\.\d+)?$/.test(value), {
          message: getErrorMessage(
            errorMessages.numericString,
            `${item.name}は`
          ),
        });

      // バリデーション対象データを取得
      const valueToValidate = value;
      const result = schema.safeParse(valueToValidate);

      // バリデーションが失敗した場合
      if (!result.success) {
        const error = result.error.errors[0]; // 最初のエラーだけ取得
        componentErrorMessage.push({
          description: error.message,
          errorLevel: InputErrorLevel.異常,
        });
      }
    }
    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...setRangesErrorMessage(item));
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック
    const isCallback = !componentErrorMessage.some(
      (error) => error.errorLevel === InputErrorLevel.異常
    );
    // エラーメッセージをexamItemに保存
    const resultItem: InputExamItem = {
      ...item,
      examRegistResults: item.examRegistResults
        ? item.examRegistResults.concat(componentErrorMessage)
        : componentErrorMessage,
    };
    // バリデーションチェックを行ったexamItemと、
    // コールバックを判断するフラグを返す
    const ValidationResult = {
      validateResult: sortErrorMessage(resultItem),
      hasCallback: isCallback,
    };
    return ValidationResult;
  };

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      let validatedData = item;
      // onRegisterPressedがtrueの場合のみvalidationCheckを実行
      if (onRegisterPressed) {
        validatedData = validationCheck(validatedData).validateResult;
      }

      return validatedData;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  //変更イベント
  const handleChange = (
    value: string,
    positionNumber: number | undefined,
    detailsPositionNumber?: number
  ) => {
    // 対象のpositionNumberか確認
    if (positionNumber !== 1) {
      return null;
    }
    const updatedExamItems = [...examItemsData];
    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = true;

    // 該当するアイテムを更新
    for (const item of updatedExamItems) {
      if (item.positionNumber === positionNumber) {
        // クリアボタン用の処理
        if (detailsPositionNumber === undefined) {
          item.examItemDetails = item.examItemDetails?.map((detail) => ({
            ...detail,
            value: value,
          }));
        } else {
          // 該当するdetailの値を更新
          item.examItemDetails = item.examItemDetails?.map((detail) =>
            detail.positionNumber === detailsPositionNumber
              ? { ...detail, value: value }
              : detail
          );
        }
      }
      // バリデーションチェックを実施
      const { validateResult, hasCallback } = validationCheck(item);
      if (!hasCallback) {
        // falseのexamItemがあればコールバックを行わない
        hasValidationError = false;
      }
      // バリデーション結果を反映
      Object.assign(item, validateResult);
    }
    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);

    if (hasValidationError) {
      onChange(updatedExamItems);
    }
  };

  // positionNumberが1のexamItemを描写
  const firstPositionItem = examItemsData.find(
    (item) => item.positionNumber === 1
  );
  const { positionNumber, examItemDetails, examRegistResults, name } =
    firstPositionItem ?? {};

  // positionNumberが1のexamItemDetailを描写
  const targetDetails = examItemDetails?.find(
    (detail) => detail.positionNumber === 1
  );
  const {
    positionNumber: detailPositionNumber,
    prevValue,
    unit,
    value,
    integerLength,
    decimalLength,
  } = targetDetails ?? {};
  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Group w="11168" gap={16}>
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
          <Text size="lg" fw={700}>
            {name}
          </Text>
        </Paper>
        <TextInput
          classNames={{
            input: `${styles["input-textbox"]} ${
              examRegistResults?.some(
                (x) => x.errorLevel === InputErrorLevel.異常
              )
                ? `${styles["input-error"]}`
                : examRegistResults?.some(
                    (x) => x.errorLevel === InputErrorLevel.警告
                  )
                ? `${styles["input-warning"]}`
                : ""
            }`,
          }}
          w={340}
          radius="md"
          size="inputComponent"
          value={value}
          onClick={() => setShowKeyboards(true)}
          onChange={(e) =>
            handleChange(
              e.currentTarget.value,
              positionNumber ?? 0,
              detailPositionNumber ?? 0
            )
          }
        />
        <Stack gap="0">
          <Text size="md" fw="700" maw={271}>
            {prevValue ? `(前回: ${prevValue})` : ""}
          </Text>
          <Text size="xs" fw="400">
            {unit}
          </Text>
        </Stack>
        <Button
          w={154}
          h={64}
          size="lg"
          bg="white01"
          variant="outline"
          bd={"2px,solid"}
          tabIndex={-1}
          onClick={() => handleChange("", positionNumber)}
        >
          クリア
        </Button>
      </Group>
      {/* エラーメッセージを表示する。 */}
      {(examRegistResults || []).map((error, index) => (
        <Group
          key={index}
          c={error.errorLevel === InputErrorLevel.異常 ? "error" : "warning"}
        >
          <IconExclamationCircleFilled size={32} />
          <Text>{error.description}</Text>
        </Group>
      ))}
      <Box ml={220} mt={50}>
        {showKeyboards && (
          <div ref={closeKeyBoard}>
            <NumericKeyboard
              value={value ?? ""}
              integerLength={integerLength}
              decimalLength={decimalLength}
              onChange={(newValue) =>
                handleChange(
                  newValue,
                  positionNumber ?? 0,
                  detailPositionNumber ?? 0
                )
              }
              onConfirm={handleConfirm}
            />
          </div>
        )}
      </Box>
    </Flex>
  );
}
