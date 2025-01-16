import { z } from "zod";
import { useEffect, useState } from "react";
import { Group, Paper, Text, Textarea, Button, Flex } from "@mantine/core";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import type {
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamFreeInputProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

export default function ExamFreeInput({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamFreeInputProps) {
  // 引数のチェック
  if (
    !examItems ||
    examItems.length === 0 ||
    !examItems.some(
      (item) =>
        item.positionNumber === 1 &&
        item.examItemDetails?.some((detail) => detail.positionNumber === 1),
    )
  ) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);
  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);

  // 初回読み込み時にAPIのエラーメッセージを保存する
  useEffect(() => {
    const backendErrorMessages: BackendValidation[] = examItems.map((item) => ({
      itemPositionNumber: item.positionNumber ?? 0,
      examRegistResults: item.examRegistResults ?? [],
    }));
    setBackendValidation(backendErrorMessages);
  }, []);

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      // positionNumberが1のアイテムのみに対してバリデーションチェックを実行
      if (item.positionNumber === 1) {
        let validatedData: InputExamItem = item;
        if (onRegisterPressed) {
          validatedData = validationCheck(item).validateResult;
        }
        return validatedData;
      }
      return item;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  const firstPositionExamItem = examItemsData.find(
    (item) => item.positionNumber === 1,
  );
  const firstPositionDetail = firstPositionExamItem?.examItemDetails?.find(
    (detail) => detail.positionNumber === 1,
  );

  // エラーメッセージの並び替え
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

  // APIのエラーメッセージで更新する
  const resetErrorMessages = (item: InputExamItem) => {
    const targetError = backendValidation.find(
      (error) => error.itemPositionNumber === item.positionNumber,
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
    // 必須チェック行うスキーマ
    const schema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, item.name?`${item.name}は`:"")); // 必須チェック
    // バリデーション対象データを取得
    const valueToValidate =
      item.examItemDetails?.find((item) => item.positionNumber === 1)?.value ||
      "";
    const result = schema.safeParse(valueToValidate);

    // バリデーションが失敗した場合
    if (!result.success) {
      const error = result.error.errors[0];
      componentErrorMessage.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    }
    // 基準値によるエラーメッセージを追加
    componentErrorMessage.push(...setRangesErrorMessage(item));
    // コンポーネント由来のエラーメッセージに異常メッセージがあるかチェック
    const isCallback = !componentErrorMessage.some(
      (error) => error.errorLevel === 3,
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

  // 変更を保存し、コールバックする
  const handleChange = (value: string) => {
    const updatedExamItems = [...examItemsData];

    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = true;

    for (const item of updatedExamItems) {
      if (item.positionNumber === 1) {
        item.examItemDetails = item.examItemDetails?.map((detail) =>
          detail.positionNumber === 1 ? { ...detail, value: value } : detail,
        );
        // バリデーションチェックを実施
        const { validateResult, hasCallback } = validationCheck(item);
        if (!hasCallback) {
          // falseのexamItemがあればコールバックを行わない
          hasValidationError = false;
        }
        Object.assign(item, validateResult);
      }
    }
    // 値の保存
    setExamItemsData(updatedExamItems);

    // APIエラー以外で異常エラーが無い場合、コールバックを行う
    if (!hasValidationError) {
      onChange(updatedExamItems);
    }
  };

  const { name, examRegistResults = [] } = firstPositionExamItem || {};
  const isDisabled =
    !firstPositionDetail?.hasOrder || !!firstPositionDetail.cancelReasonId;

  return (
    <Flex mt={16} justify="flex-start" align="flex-start" direction="column">
      <Flex gap={16} justify="flex-start" align="flex-start">
        <Paper w={274} h={80} bg="gray02" c="white" radius="itemName" py={16}>
          <Text size="lg" fw={700} ta="center">
            {name?.slice(0, 8)}
          </Text>
        </Paper>
        <Textarea
          classNames={{
            input: `${styles["input-textarea"]} ${
              isDisabled
                ? ""
                : examRegistResults?.some(
                      (x) => x.errorLevel === InputErrorLevel.異常,
                    )
                  ? `${styles["input-error"]}`
                  : examRegistResults?.some(
                        (x) => x.errorLevel === InputErrorLevel.警告,
                      )
                    ? `${styles["input-warning"]}`
                    : ""
            }`,
          }}
          w={524}
          radius="md"
          size="sm"
          value={firstPositionDetail?.value}
          onChange={(e) => handleChange(e.currentTarget.value)}
          autosize
          minRows={1}
          maxRows={4}
          disabled={isDisabled}
        />
        <Button
          w={154}
          h={64}
          ml={48}
          size="lg"
          bg="white01"
          variant="outline"
          bd={"2px,solid"}
          tabIndex={-1}
          onClick={() => handleChange("")}
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
        {firstPositionDetail?.prevValue
          ? `(前回値)\n${firstPositionDetail?.prevValue}`
          : ""}
      </Text>
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
    </Flex>
  );
}
