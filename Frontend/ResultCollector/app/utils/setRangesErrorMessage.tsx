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
  inputexamItem.examRegistResults = inputexamItem.examRegistResults || [];
  const matchExamRanges = getMaxErrorLevelsByRangeCheck(
    inputexamItem.examItemDetails
  );
  console.log(inputexamItem.examRegistResults);
  // 既に存在するExamNormalValueRangeによるエラーメッセージを削除
  inputexamItem.examRegistResults = inputexamItem.examRegistResults.filter(
    (error) => !error.description?.includes("正常値ではありません。")
  );
  console.log(inputexamItem.examRegistResults);
  if (matchExamRanges.length === 0) {
    return inputexamItem;
  }
  for (const { minValue, maxValue, errorLevel } of matchExamRanges) {
    const rangesErrorMessage: ExamRegistResult = {
      // TODO：エラーメッセージの詳細を決定
      description: `入力された値が${minValue}から${maxValue}であり、正常値ではありません。`,
      errorLevel: errorLevel,
    };
    inputexamItem.examRegistResults.push(rangesErrorMessage);
  }
  console.log(inputexamItem.examRegistResults);
  return inputexamItem;
};
