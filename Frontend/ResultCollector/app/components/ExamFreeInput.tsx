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
import type { InputExamItem } from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";
import { IconExclamationCircleFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamFreeInputProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

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
        item.examItemDetails?.some((detail) => detail.positionNumber === 1)
    )
  ) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);

  const firstPositionExamItem = examItemsData.find(
    (item) => item.positionNumber === 1
  );
  const firstPositionDetail = firstPositionExamItem?.examItemDetails?.find(
    (detail) => detail.positionNumber === 1
  );

  // エラーメッセージの並び替え
  const handleErrorMessage = (item: InputExamItem): InputExamItem => {
    if (item.examRegistResults) {
      // エラーレベルが高い順にソート
      item.examRegistResults.sort((a, b) => {
        const levelA = a.errorLevel ?? 0;
        const levelB = b.errorLevel ?? 0;
        return levelB - levelA;
      });

      // 重複を除外
      item.examRegistResults = item.examRegistResults.filter(
        (result, index, self) =>
          index === self.findIndex((r) => r.description === result.description)
      );
    }
    return item;
  };

  const validationCheck = (item: InputExamItem) => {
    // 必須チェック行うスキーマ
    const schema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, `${item.name}は`)); // 必須チェック
    // バリデーション対象データを取得
    const valueToValidate =
      item.examItemDetails?.find((item) => item.positionNumber === 1)?.value ||
      "";
    const result = schema.safeParse(valueToValidate);

    // エラーメッセージを更新
    let updatedErrors = item.examRegistResults || [];

    // 必須エラーを削除
    const errorMessageRequired = getErrorMessage(
      errorMessages.required,
      `${item.name}は`
    );
    updatedErrors = updatedErrors.filter(
      (error) => error.description !== errorMessageRequired
    );

    // バリデーションが失敗した場合
    if (!result.success) {
      const error = result.error.errors[0];
      updatedErrors.push({
        description: error.message,
        errorLevel: InputErrorLevel.異常,
      });
    }
    const validatedItem = {
      ...item,
      examRegistResults: updatedErrors,
    };
    return handleErrorMessage(validatedItem);
  };

  useEffect(() => {
    const updatedItems = examItems.map((item) => {
      // positionNumberが1のアイテムのみに対してバリデーションチェックを実行
      if (item.positionNumber === 1) {
        let validatedData: InputExamItem = item;
        if (onRegisterPressed) {
          validatedData = validationCheck(item);
        }
        return validatedData;
      }
      return item;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

  // 変更を保存し、コールバックする
  const handleChange = (value: string) => {
    const updatedExamItems = [...examItemsData];
    for (const item of updatedExamItems) {
      if (item.positionNumber === 1) {
        item.examItemDetails = item.examItemDetails?.map((detail) =>
          detail.positionNumber === 1 ? { ...detail, value: value } : detail
        );
        const validatedItem = validationCheck(item);
        Object.assign(item, validatedItem);
      }
    }
    // 値の保存
    setExamItemsData(updatedExamItems);

    // 必須エラーの存在を確認
    const hasValidationError = updatedExamItems.some(
      (item) =>
        item.positionNumber === 1 &&
        item.examRegistResults?.some(
          (error) =>
            error.description ===
            getErrorMessage(
              errorMessages.required,
              `${firstPositionExamItem?.name}は`
            )
        )
    );

    // 必須エラーがない場合にのみコールバック
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
            {name}
          </Title>
        </Paper>
        <Textarea
          classNames={{
            input: `${styles["input-textarea"]} ${
              isDisabled
                ? ""
                : examRegistResults?.some(
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
          w={"524"}
          radius={"md"}
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
          bg={"white"}
          variant="outline"
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
          <IconExclamationCircleFilled size={"1.7rem"} />
          <Text>{error.description}</Text>
        </Group>
      ))}
    </Flex>
  );
}
