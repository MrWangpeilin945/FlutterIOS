import type {
  InputExamItem,
  ExamItemDetail,
  ExamRegistResult,
  ExamNormalValueRange,
} from "~/domain/wellship.schemas";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";

// 受け取ったExamItemDetailsから
// それぞれの最も高いエラーレベルに該当するExamNormalValueRangeを返す
const getMaxErrorLevelsByRangeCheck = (examItemDetails: ExamItemDetail[]) => {
  const matchExamRanges: ExamNormalValueRange[] = [];
  for (const { value, examNormalValueRanges } of examItemDetails) {
    const numericValue = Number.parseFloat(value || "");
    if (!numericValue || !examNormalValueRanges) continue;
    // errorLevelの高い順にソート
    const sortedRanges = [...examNormalValueRanges].sort(
      (a, b) => (b.errorLevel || 0) - (a.errorLevel || 0)
    );
    // 範囲にマッチする最初の値を返す
    const matchRange = sortedRanges.find(
      ({ minValue, maxValue }) =>
        typeof minValue === "number" &&
        typeof maxValue === "number" &&
        numericValue >= minValue &&
        numericValue <= maxValue
    );
    if (matchRange) {
      matchExamRanges.push(matchRange);
    }
  }
  return matchExamRanges;
};

// 受け取ったInputexamitemの、examRegistresultsにエラーを追加して
// Inputexamitemを返す
export const setRangesErrorMessage = (inputexamItem: InputExamItem) => {
  if (!inputexamItem.examItemDetails) {
    return inputexamItem;
  }
  const matchExamRanges = getMaxErrorLevelsByRangeCheck(
    inputexamItem.examItemDetails
  );
  if (matchExamRanges.length === 0) {
    return inputexamItem;
  }
  if (!inputexamItem.examRegistResults) {
    inputexamItem.examRegistResults = [];
  }
  for (const { minValue, maxValue, errorLevel } of matchExamRanges) {
    const rangesErrorMessage: ExamRegistResult = {
      description: getErrorMessage(
        errorMessages.numberRange,
        `${inputexamItem.name}は`,
        `${minValue}`,
        `${maxValue}`
      ),
      errorLevel: errorLevel,
    };
    inputexamItem.examRegistResults.push(rangesErrorMessage);
  }
  return inputexamItem;
};
