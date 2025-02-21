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
import CollectionKeyboard from "./CollectionKeyboard";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";
import type {
  InputExamItem,
  ExamRegistResult,
} from "~/domain/wellship.schemas";
import { InputErrorLevel, KeyboardType } from "~/domain/enums";
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

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
      (detail) => detail.positionNumber === 1,
    )
  ) {
    return null;
  }

  const [examItemsData, setExamItemsData] = useState(examItems);
  // APIからのエラーメッセージを保存する
  const [backendValidation, setBackendValidation] = useState<
    BackendValidation[]
  >([]);

  // キーボードの表示インデックスを状態として管理する
  const [showKeyboards, setShowKeyboards] = useState(false);

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
        const validatedData = validationCheck(item).validateResult;
        return validatedData;
      }
      return item;
    });
    setExamItemsData(updatedItems);
  }, [onRegisterPressed, examItems]);

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

  // 引数のexamItemのpositionNumberを参照し、エラーメッセージを初期化する
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
    // detailsのpositionNumberが1のものについてバリデーションチェックを行う
    for (const {
      positionNumber,
      value,
      hasOrder,
      cancelReasonId,
    } of item.examItemDetails ?? []) {
      if (positionNumber !== 1) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }
      if (!hasOrder || !!cancelReasonId) {
        continue; // 対象がdisableの場合、処理をスキップする
      }
      // [登録する]が押されたときは必須・半角数字チェック、その他は半角数字チェックのみ行う。
      const schema = onRegisterPressed
        ? z
            .string()
            .min(
              1,
              getErrorMessage(
                errorMessages.required,
                item.name ? `${item.name}は` : "",
              ), // 必須チェック
            )
            .refine((value) => /^\d+(\.\d+)?$/.test(value), {
              message: getErrorMessage(
                errorMessages.numericString,
                item.name ? `${item.name}は` : "",
              ),
            })
        : z.string().refine((value) => /^(\d+(\.\d+)?|)$/.test(value), {
            message: getErrorMessage(
              errorMessages.numericString,
              item.name ? `${item.name}は` : "",
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
      (error) => error.errorLevel === InputErrorLevel.異常,
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

  //変更イベント
  const handleChange = (
    value: string,
    positionNumber: number | undefined,
    detailsPositionNumber?: number,
  ) => {
    // 対象のpositionNumberか確認
    if (positionNumber !== 1) {
      return null;
    }
    const updatedExamItems = [...examItemsData];
    // examItemsに異常エラーメッセージがあるかをチェックするフラグ変数
    let hasValidationError = false;

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
              : detail,
          );
        }
      }
      // バリデーションチェックを実施
      const { validateResult, hasCallback } = validationCheck(item);
      if (!hasCallback) {
        // falseのexamItemがあればコールバックを行わない
        hasValidationError = true;
      }
      // バリデーション結果を反映
      Object.assign(item, validateResult);
    }
    // 更新されたデータをステートに設定
    setExamItemsData(updatedExamItems);

    if (!hasValidationError) {
      onChange(updatedExamItems);
    }
  };

  // positionNumberが1のexamItemを描写
  const firstPositionItem = examItemsData.find(
    (item) => item.positionNumber === 1,
  );
  const { positionNumber, examItemDetails, examRegistResults, name } =
    firstPositionItem ?? {};

  // positionNumberが1のexamItemDetailを描写
  const targetDetails = examItemDetails?.find(
    (detail) => detail.positionNumber === 1,
  );
  const {
    positionNumber: detailPositionNumber,
    prevValue,
    unit,
    value,
    hasOrder,
    cancelReasonId,
    integerLength,
    decimalLength,
    keyboard,
  } = targetDetails ?? {};
  const isDisabled = !hasOrder || !!cancelReasonId;
  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Group w={1036} gap={16}>
        <Paper w={274} h={80} bg="gray02" c="white" radius="itemName" py={16}>
          <Text size="lg" fw={700} ta="center">
            {name?.slice(0, 8)}
          </Text>
        </Paper>
        <TextInput
          classNames={{
            input: `${styles["input-textbox"]}
             ${
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
          w={340}
          radius="md"
          size="inputComponent"
          value={value}
          disabled={isDisabled}
          onClick={() => setShowKeyboards(true)}
          onChange={(e) =>
            handleChange(
              e.currentTarget.value,
              positionNumber ?? 0,
              detailPositionNumber ?? 0,
            )
          }
        />
        <Stack w={173} h={80} gap={4} justify="space-between">
          <Box>
            {prevValue && (
              <Text fw={700} mt={0}>
                (前回：{prevValue})
              </Text>
            )}
          </Box>
          <Box>
            <Text size="xs" mb={0}>
              {unit}
            </Text>
          </Box>
        </Stack>
        <Button
          w={154}
          h={64}
          size="lg"
          bg="white01"
          variant="outline"
          bd={"2px,solid"}
          disabled={isDisabled}
          tabIndex={-1}
          onClick={() => handleChange("", positionNumber)}
        >
          クリア
        </Button>
      </Group>
      {/* エラーメッセージを表示する。 */}
      {(examRegistResults || []).map((error, index) => {
        const isWarning = error.errorLevel === InputErrorLevel.警告;
        return (
          <Group key={index} c={isWarning ? "warning" : "error"}>
            {isWarning ? (
              <IconExclamationCircleFilled size={32} />
            ) : (
              <IconSquareRoundedXFilled size={32} />
            )}
            <Text size="sm" fw={700}>
              {error.description}
            </Text>
          </Group>
        );
      })}
      <Box ml={220}>
        {showKeyboards && (
          <div ref={closeKeyBoard}>
            {keyboard?.keyboardType === KeyboardType.テンキー ||
            keyboard?.keyboardType === undefined ? (
              <NumericKeyboard
                value={value ?? ""}
                integerLength={integerLength}
                decimalLength={decimalLength}
                onChange={(newValue) =>
                  handleChange(
                    newValue,
                    positionNumber ?? 0,
                    detailPositionNumber ?? 0,
                  )
                }
                onConfirm={handleConfirm}
              />
            ) : (
              <CollectionKeyboard
                value={value ?? ""}
                keyboardValues={keyboard?.values ?? []}
                onChange={(newValue) =>
                  handleChange(newValue, positionNumber, detailPositionNumber)
                }
              />
            )}
          </div>
        )}
      </Box>
    </Flex>
  );
}
