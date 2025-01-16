import type {
  InputExamItem,
  ExamItemDetail,
  ExamRegistResult,
  ExamNormalValueRange,
} from "~/domain/wellship.schemas";
import { InputErrorLevel } from "~/domain/enums";

// 受け取ったExamItemDetailsから
// それぞれの最も高いエラーレベルに該当するExamNormalValueRangeを返す
const getMaxErrorLevelsByRangeCheck = (examItemDetails: ExamItemDetail[]) => {
  return examItemDetails.reduce(
    (
      matchExamRanges: ExamNormalValueRange[],
      { value, examNormalValueRanges, hasOrder, cancelReasonId },
    ) => {
      const isDisable = !hasOrder || !!cancelReasonId;
      const numericValue =
        value !== undefined && value !== ""
          ? Number.parseFloat(value)
          : Number.NaN;
      if (isDisable) {
        return matchExamRanges; // isDisable の場合は処理をスキップ
      }
      if (!Number.isNaN(numericValue) && examNormalValueRanges) {
        const matchRange = examNormalValueRanges.find(
          ({ minValue, maxValue }) =>
            typeof minValue === "number" &&
            typeof maxValue === "number" &&
            numericValue >= minValue &&
            numericValue < maxValue,
        );
        if (matchRange) {
          matchExamRanges.push(matchRange);
        }
      }
      return matchExamRanges;
    },
    [],
  );
};

// 受け取ったInputexamitemの、該当するExamRegistResult[]を返す
export const setRangesErrorMessage = (inputexamItem: InputExamItem) => {
  if (!inputexamItem.examItemDetails) {
    return [];
  }
  inputexamItem.examRegistResults = inputexamItem.examRegistResults || [];
  const matchExamRanges = getMaxErrorLevelsByRangeCheck(
    inputexamItem.examItemDetails,
  );
  // 該当するエラーが無いときは空配列を返す
  if (matchExamRanges.length === 0) {
    return [];
  }
  // エラーレベルに応じたエラーメッセージを追加
  const hasError = matchExamRanges.some(
    ({ errorLevel }) => errorLevel === InputErrorLevel.異常,
  );
  const rangesErrorMessages: ExamRegistResult[] = [
    {
      description: hasError
        ? "入力に誤りがあります。"
        : "入力値を確認してください。",
      errorLevel: hasError ? InputErrorLevel.異常 : InputErrorLevel.警告,
    },
  ];
  return rangesErrorMessages;
};
