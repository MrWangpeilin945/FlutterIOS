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
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type ExamNumericLRProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onChange: (newExamItems: InputExamItem[] | undefined) => void;
};

interface BackendValidation {
  itemPositionNumber: number;
  examRegistResults: ExamRegistResult[];
}

export default function ExamNumericLR({
  examItems,
  onRegisterPressed,
  onChange,
}: ExamNumericLRProps) {
  // 定数で定義
  const 左 = 1;
  const 右 = 2;

  // 引数のチェック
  if (!examItems || examItems.length === 0) {
    return null;
  }
  // positionNumberが1のexamItemの、examItemDetailsの中に
  // positionNumberが1か2のexamItemDetailが存在することをチェック
  const firstPosition = examItems.find((item) => item.positionNumber === 1);

  if (
    !firstPosition?.examItemDetails?.some(
      (detail) => detail.positionNumber === 左,
    ) &&
    !firstPosition?.examItemDetails?.some(
      (detail) => detail.positionNumber === 右,
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
  const [showKeyboards, setShowKeyboards] = useState<{
    left: boolean;
    right: boolean;
  }>({
    left: false,
    right: false,
  });

  // キーボードの表示/非表示をトグルする関数
  const toggleKeyboard = (detailPositionNumber: number) => {
    setShowKeyboards((prevKeyboards) => {
      // positionNumberによって切り替えるフラグを制御する
      if (detailPositionNumber === 左 || detailPositionNumber === 右) {
        return {
          ...prevKeyboards,
          left:
            detailPositionNumber === 左
              ? !prevKeyboards.left
              : prevKeyboards.left,
          right:
            detailPositionNumber === 右
              ? !prevKeyboards.right
              : prevKeyboards.right,
        };
      }
      return prevKeyboards;
    });
  };
  // キーボードの確定ボタン押下時にキーボードを非表示にする
  const handleConfirm = () => {
    setShowKeyboards({ left: false, right: false });
  };
  const closeKeyBoard = useClickOutside(() =>
    setShowKeyboards({ left: false, right: false }),
  );

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

  // 初回読み込み時にAPIのエラーメッセージを保存する
  useEffect(() => {
    const backendErrorMessages: BackendValidation[] = examItems.map((item) => ({
      itemPositionNumber: item.positionNumber ?? 0,
      examRegistResults: item.examRegistResults ?? [],
    }));
    setBackendValidation(backendErrorMessages);
  }, []);

  // APIのエラーメッセージ以外を削除する
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
    // コンポーネント由来のエラーメッセージを削除
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];
    // detailsのpositionNumberが1と2のものについてバリデーションチェックを行う
    for (const {
      name,
      positionNumber,
      value,
      hasOrder,
      cancelReasonId,
    } of item.examItemDetails ?? []) {
      if (positionNumber !== 左 && positionNumber !== 右) {
        continue; // 対象のpositionNumberでない場合、処理をスキップする
      }
      if (!hasOrder || !!cancelReasonId) {
        continue; // disableの場合、処理をスキップする
      }
      // 必須チェックと半角数字チェックを一度に行うスキーマ
      const schema = z
        .string()
        .min(
          1,
          getErrorMessage(errorMessages.required, `${item.name}:${name}は`),
        ) // 必須チェック
        .refine((value) => /^\d+(\.\d+)?$/.test(value), {
          message: getErrorMessage(
            errorMessages.numericString,
            `${item.name}:${name}は`,
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
    detailsPositionNumber?: number,
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
            value:
              detail.hasOrder && !detail.cancelReasonId ? value : detail.value,
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

  // positionNumberが1のexamItem
  const firstPositionItem = examItemsData.find(
    (item) => item.positionNumber === 1,
  );
  const { positionNumber, examItemDetails, examRegistResults, name } =
    firstPositionItem ?? {};

  // 対象のexamItemの中の、positionNumberが1のexamItemDetail
  let leftItemDetail = examItemDetails?.find(
    (detail) => detail.positionNumber === 左,
  );
  if (!leftItemDetail) {
    leftItemDetail = {
      positionNumber: 左,
      name: "左",
    };
  }
  //  対象のexamItemの中の、positionNumberが2のexamItemDetail
  let rightItemDetail = examItemDetails?.find(
    (detail) => detail.positionNumber === 右,
  );
  if (!rightItemDetail) {
    rightItemDetail = {
      positionNumber: 右,
      name: "右",
    };
  }
  const targetDetails = [leftItemDetail, rightItemDetail];

  return (
    <Flex justify="flex-start" align="flex-start" direction="column">
      <Stack>
        <Paper w={274} h={80} bg="gray02" c="white" radius="itemName" py={16}>
          <Text size="lg" fw={700} ta="center">
            {name?.slice(0, 8)}
          </Text>
        </Paper>
        <Group gap={16}>
          {targetDetails?.map((detail) => {
            const {
              hasOrder,
              positionNumber: detailPositionNumber,
              name: detailName,
              value,
              prevValue,
              unit,
              cancelReasonId,
            } = detail;
            const isDisabled = !hasOrder || !!cancelReasonId;
            return (
              <Stack key={detailPositionNumber}>
                <Paper w={524} h={51} bg="gray02" c="white" radius="itemName">
                  <Text size="lg" fw={700} ta="center">
                    {detailName}
                  </Text>
                </Paper>
                <Group>
                  <TextInput
                    classNames={{
                      input: `${styles["input-textbox"]} ${
                        isDisabled
                          ? ""
                          : firstPositionItem?.examRegistResults?.some(
                                (x) => x.errorLevel === InputErrorLevel.異常,
                              )
                            ? `${styles["input-error"]}`
                            : firstPositionItem?.examRegistResults?.some(
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
                    onClick={() => toggleKeyboard(detailPositionNumber ?? 0)}
                    onChange={(e) =>
                      handleChange(
                        e.currentTarget.value,
                        positionNumber ?? 0,
                        positionNumber ?? 0,
                      )
                    }
                    disabled={isDisabled}
                  />
                  <Stack gap="0">
                    <Text size="md" fw="700" maw={172}>
                      {prevValue ? `(前回: ${prevValue})` : ""}
                    </Text>
                    <Text size="xs" fw="400">
                      {unit}
                    </Text>
                  </Stack>
                </Group>
              </Stack>
            );
          })}
        </Group>
        <Button
          w={154}
          h={64}
          mt={40}
          ml={914}
          size="lg"
          bg="white01"
          variant="outline"
          bd={"2px,solid"}
          onClick={() => handleChange("", positionNumber)}
          tabIndex={-1}
        >
          クリア
        </Button>
        {/* エラーメッセージの表示 */}
        <Stack gap={0}>
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
        </Stack>
      </Stack>
      <Group>
        {targetDetails?.map((detail) => (
          <Stack key={detail.positionNumber}>
            <Box w={540}>
              {showKeyboards[
                detail.positionNumber === 左 ? "left" : "right"
              ] && (
                <div ref={closeKeyBoard}>
                  <NumericKeyboard
                    value={detail?.value ?? ""}
                    integerLength={detail.integerLength}
                    decimalLength={detail.decimalLength}
                    onChange={(newValue) =>
                      handleChange(
                        newValue,
                        positionNumber ?? 0,
                        detail.positionNumber ?? 0,
                      )
                    }
                    onConfirm={handleConfirm}
                  />
                </div>
              )}
            </Box>
          </Stack>
        ))}
      </Group>
    </Flex>
  );
}
