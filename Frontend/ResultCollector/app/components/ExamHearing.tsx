import { useEffect, useState } from "react";
import { Flex, Box, Paper, Text, Button, Stack, Group } from "@mantine/core";
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
} from "@tabler/icons-react";
import { InputErrorLevel } from "~/domain/enums";
import type {
  ExamItemDetail,
  ExamRegistResult,
  InputExamItem,
} from "~/domain/wellship.schemas";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

type HearingProps = {
  examItems: InputExamItem[];
  onRegisterPressed: boolean;
  onClick: (updatedExamItem: InputExamItem[] | undefined) => void;
};

export default function ExamHearing({
  examItems,
  onRegisterPressed,
  onClick,
}: HearingProps) {
  if (!examItems || examItems.length === 0) {
    return null;
  }
  const firstItem = examItems.find((item) => item.positionNumber === 1);
  if (!firstItem) {
    return null;
  }

  const 左1000Hz = 1;
  const 左4000Hz = 2;
  const 右1000Hz = 3;
  const 右4000Hz = 4;

  // 要素が1つも存在しない場合に return
  const hearingItemPositionNumbers = [左1000Hz, 左4000Hz, 右1000Hz, 右4000Hz];
  const noPositionExists = hearingItemPositionNumbers.every((position) =>
    firstItem.examItemDetails?.every(
      (detail) => detail.positionNumber !== position,
    ),
  );

  if (noPositionExists) {
    return null;
  }

  const [examItemData, setExamItemData] = useState<InputExamItem>(firstItem);

  useEffect(() => {
    // positionNumberが1のアイテムを1つだけ取り出してバリデーションチェック
    const updatedItem = firstItem;
    let validatedData: InputExamItem = { ...updatedItem };

    // onRegisterPressedがtrueの場合のみバリデーションを実行
    if (onRegisterPressed) {
      validatedData = validationCheck(updatedItem).validateResult;
    }
    // 状態を更新
    setExamItemData(validatedData); // 配列にラップして1つだけセット
  }, [onRegisterPressed, examItems]);

  const handleErrorMessage = (item: InputExamItem): InputExamItem => {
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
      (error) => error.positionNumber === item.positionNumber,
    );
    if (targetError) {
      item.examRegistResults = targetError.examRegistResults;
    }
    return item;
  };

  // 必須チェック
  const validationCheck = (item: InputExamItem) => {
    // APIエラーメッセージで初期化
    resetErrorMessages(item);
    // コールバック判断用のコンポーネントのエラーメッセージ
    const componentErrorMessage: ExamRegistResult[] = [];

    const leftErrorMessage: ExamRegistResult = {
      description: getErrorMessage(errorMessages.required, "左は"),
      errorLevel: InputErrorLevel.異常,
    };
    const left1000 = item.examItemDetails?.find(
      (detail) => detail.positionNumber === 左1000Hz,
    );
    const left4000 = item.examItemDetails?.find(
      (detail) => detail.positionNumber === 左4000Hz,
    );
    const isLeft1000Disabled = checkDisabled(left1000);
    const isLeft4000Disabled = checkDisabled(left4000);
    if (
      (!isLeft1000Disabled && !left1000?.value) || // 有効な left1000 が空
      (!isLeft4000Disabled && !left4000?.value) // 有効な left4000 が空
    ) {
      componentErrorMessage.push(leftErrorMessage);
    }

    const rightErrorMessage: ExamRegistResult = {
      description: getErrorMessage(errorMessages.required, "右は"),
      errorLevel: InputErrorLevel.異常,
    };
    const right1000 = item.examItemDetails?.find(
      (detail) => detail.positionNumber === 右1000Hz,
    );
    const right4000 = item.examItemDetails?.find(
      (detail) => detail.positionNumber === 右4000Hz,
    );
    const isRight1000Disabled = checkDisabled(right1000);
    const isRight4000Disabled = checkDisabled(right4000);
    if (
      (!isRight1000Disabled && !right1000?.value) || // 有効な right1000 が空
      (!isRight4000Disabled && !right4000?.value) // 有効な right4000 が空
    ) {
      componentErrorMessage.push(rightErrorMessage);
    }

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
      validateResult: handleErrorMessage(resultItem),
      hasCallback: isCallback,
    };
    return ValidationResult;
  };

  // 所見なしボタン押下時
  const setNoFindings = (value: string, isGroup: string) => {
    // 値を更新
    const updatedExamItem: InputExamItem = {
      ...examItemData,
      examItemDetails: examItemData.examItemDetails?.map((detail) => {
        // 左の要素に値を設定
        if (
          isGroup === "左" &&
          (detail.positionNumber === 左1000Hz || detail.positionNumber === 左4000Hz)
        ) {
          return {
            ...detail,
            value: value, // 1000Hz と 4000Hz の positionNumber に value を代入
          };
        }
        // 右の要素に値を設定
        if (
          isGroup === "右" &&
          (detail.positionNumber === 右1000Hz || detail.positionNumber === 右4000Hz)
        ) {
          return {
            ...detail,
            value: value, // 1000Hz と 4000Hz の positionNumber に value を代入
          };
        }
        return detail;
      }),
    };
    const { validateResult, hasCallback } = validationCheck(updatedExamItem);
    setExamItemData(validateResult);
    if (hasCallback) {
      onClick([validateResult]);
    }
  };

  // 所見ありボタン押下時
  const setFindings = (value: string, detailNumber: number) => {
    const selectedDetail = examItemData.examItemDetails?.find(
      (detail) => detail.positionNumber === detailNumber,
    );
    // 選択/未選択処理
    const selectedValue = value === selectedDetail?.value ? "" : value;

    // 値を更新
    const updatedExamItem: InputExamItem = {
      ...examItemData,
      examItemDetails: examItemData.examItemDetails?.map((detail) =>
        detail.positionNumber === detailNumber
          ? {
              ...detail,
              value: selectedValue,
            }
          : detail,
      ),
    };

    const { validateResult, hasCallback } = validationCheck(updatedExamItem);
    setExamItemData(validateResult);
    if (hasCallback) {
      onClick([validateResult]);
    }
  };

  const details = examItemData.examItemDetails ?? [];
  const left1000Detail = details.find(
    (detail) => detail.positionNumber === 左1000Hz,
  );
  const left4000Detail = details.find(
    (detail) => detail.positionNumber === 左4000Hz,
  );
  const right1000Detail = details.find(
    (detail) => detail.positionNumber === 右1000Hz,
  );
  const right4000Detail = details.find(
    (detail) => detail.positionNumber === 右4000Hz,
  );
  // 左右で配列を作成
  const groupDetails = [
    [left1000Detail, left4000Detail],
    [right1000Detail, right4000Detail],
  ];

  // disabled判定
  const checkDisabled = (detail: ExamItemDetail | undefined) => {
    return !detail || !detail.hasOrder || !!detail.cancelReasonId;
  };

  //指定したorderNumberのcodeを取得
  const getOptionCode = (detail: ExamItemDetail, orderNumber: number) => {
    return (
      detail.examItemDetailOptions?.find(
        (option) => option.orderNumber === orderNumber,
      )?.code ?? ""
    );
  };

  //選択済み判定
  const checkSelected = (detail: ExamItemDetail, orderNumber: number) => {
    return detail.value === getOptionCode(detail, orderNumber);
  };

  // 所見ありの前回値が存在するかを判定
  const checkPrev = (detail: ExamItemDetail | undefined) => {
    if (!detail) return false;
    const matchingOption = detail.examItemDetailOptions?.find(
      (option) => option.orderNumber === 2,
    );
    return matchingOption?.code === detail.prevValue;
  };

  return (
    <>
      <Flex justify="flex-start" align="flex-start" direction="column">
        <Box mb={16}>
          <Paper
            w={274}
            h={80}
            bg="gray02"
            c="white"
            radius="itemName"
            px={32}
            py={16}
          >
            <Text size="lg" fw={700} ta="center">
              {examItemData.name}
            </Text>
          </Paper>
        </Box>
        <Group gap={40}>
          {groupDetails?.map((group, index) => {
            const h1000 = group[0];
            const h4000 = group[1];
            const isGroupDisabled =
              checkDisabled(h1000) && checkDisabled(h4000);
            const isGroupSelected =
              (checkDisabled(h1000) || checkSelected(h1000 ?? {}, 1)) &&
              (checkDisabled(h4000) || checkSelected(h4000 ?? {}, 1));
            return (
              <Stack key={index} gap={0}>
                <Paper w={524} bg="gray02" c="white" radius="itemName">
                  <Text size="lg" fw={700} ta="center">
                    {index === 0 ? "左" : "右"}
                  </Text>
                </Paper>
                <Box ml="auto" h={43.4}>
                  {(h1000?.prevValue || h4000?.prevValue) && (
                    <Text fw={700} maw={500}>
                      (前回：
                      {checkPrev(h1000) ? "1000Hz" : ""}
                      {(h1000?.prevValue && !h4000?.prevValue) ||
                      (!h1000?.prevValue && h4000?.prevValue)
                        ? ""
                        : "/"}
                      {checkPrev(h4000) ? "4000Hz" : ""})
                    </Text>
                  )}
                </Box>
                <Button
                  w={524}
                  h={78}
                  mb={16}
                  variant="outline"
                  bd={`2px solid ${isGroupDisabled ? "" : isGroupSelected ? "primary" : "gray03"}`}
                  bg={
                    isGroupDisabled
                      ? "gray03"
                      : isGroupSelected
                        ? "green03"
                        : "white"
                  }
                  c={
                    isGroupDisabled
                      ? "gray02"
                      : isGroupSelected
                        ? "primary"
                        : "black"
                  }
                  size="xl"
                  fw={700}
                  disabled={isGroupDisabled}
                  onClick={() => {
                    setNoFindings(
                      getOptionCode(h1000 ?? h4000 ?? {}, 1),
                      index === 0 ? "左" : "右",
                    );
                  }}
                >
                  所見なし
                </Button>
                <Stack>
                  {/* 1000Hz */}
                  {(() => {
                    const isDisabled = checkDisabled(h1000);
                    const isSelected = checkSelected(h1000 ?? {}, 2);
                    const buttonValue = getOptionCode(h1000 ?? {}, 2);

                    return (
                      <Button
                        w={524}
                        h={78}
                        variant="outline"
                        bd={`2px solid ${isDisabled ? "" : isSelected ? "primary" : "gray03"}`}
                        bg={
                          isDisabled
                            ? "gray03"
                            : isSelected
                              ? "green03"
                              : "white"
                        }
                        c={
                          isDisabled
                            ? "gray02"
                            : isSelected
                              ? "primary"
                              : "black"
                        }
                        size="xl"
                        fw={700}
                        value={buttonValue}
                        disabled={isDisabled}
                        onClick={(e) =>
                          setFindings(
                            e.currentTarget.value,
                            index === 0 ? 左1000Hz : 右1000Hz,
                          )
                        }
                      >
                        1000Hz
                      </Button>
                    );
                  })()}

                  {/* 4000Hz */}
                  {(() => {
                    const isDisabled = checkDisabled(h4000);
                    const isSelected = checkSelected(h4000 ?? {}, 2);
                    const buttonValue = getOptionCode(h4000 ?? {}, 2);

                    return (
                      <Button
                        w={524}
                        h={78}
                        variant="outline"
                        bd={`2px solid ${isDisabled ? "" : isSelected ? "primary" : "gray03"}`}
                        bg={
                          isDisabled
                            ? "gray03"
                            : isSelected
                              ? "green03"
                              : "white"
                        }
                        c={
                          isDisabled
                            ? "gray02"
                            : isSelected
                              ? "primary"
                              : "black"
                        }
                        size="xl"
                        fw={700}
                        value={buttonValue}
                        disabled={isDisabled}
                        onClick={(e) =>
                          setFindings(
                            e.currentTarget.value,
                            index === 0 ? 左4000Hz : 右4000Hz,
                          )
                        }
                      >
                        4000Hz
                      </Button>
                    );
                  })()}
                </Stack>
              </Stack>
            );
          })}
        </Group>
        {examItemData.examRegistResults?.map((error, index) => {
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
      </Flex>
    </>
  );
}
