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
      { value, examNormalValueRanges }
    ) => {
      const numericValue =
        value !== undefined && value !== ""
          ? Number.parseFloat(value)
          : Number.NaN;
      if (!Number.isNaN(numericValue) && examNormalValueRanges) {
        const matchRange = examNormalValueRanges.find(
          ({ minValue, maxValue }) =>
            typeof minValue === "number" &&
            typeof maxValue === "number" &&
            numericValue >= minValue &&
            numericValue < maxValue
        );
        if (matchRange) {
          matchExamRanges.push(matchRange);
        }
      }
      return matchExamRanges;
    },
    []
  );
};

// 受け取ったInputexamitemの、該当するExamRegistResult[]を返す
export const setRangesErrorMessage = (inputexamItem: InputExamItem) => {
  if (!inputexamItem.examItemDetails) {
    return [];
  }
  inputexamItem.examRegistResults = inputexamItem.examRegistResults || [];
  const matchExamRanges = getMaxErrorLevelsByRangeCheck(
    inputexamItem.examItemDetails
  );

  // 該当するエラーが無いときは空配列を返す
  if (matchExamRanges.length === 0) {
    return [];
  }
  // エラーレベルに応じたエラーメッセージを追加
  const rangesErrorMessages: ExamRegistResult[] = matchExamRanges
    .filter(
      ({ errorLevel }) =>
        errorLevel === InputErrorLevel.警告 ||
        errorLevel === InputErrorLevel.異常
    )
    .map(({ errorLevel }) => ({
      description:
        errorLevel === InputErrorLevel.警告
          ? "入力値を確認してください。"
          : "入力に誤りがあります。",
      errorLevel,
    }));
  return rangesErrorMessages;
};
