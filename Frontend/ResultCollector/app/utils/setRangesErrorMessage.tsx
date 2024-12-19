import type {
  InputExamItem,
  ExamItemDetail,
  ExamRegistResult,
  ExamNormalValueRange,
} from "~/domain/wellship.schemas";

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
            numericValue <= maxValue
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
  // 既に存在するExamNormalValueRangeによるエラーメッセージを削除
  inputexamItem.examRegistResults = inputexamItem.examRegistResults.filter(
    (error) => {
      const description = error.description || "";
      return !(
        description === "入力値を確認してください。" ||
        description === "入力に誤りがあります。"
      );
    }
  );
  // 該当するエラーが無いときはそのまま返す
  if (matchExamRanges.length === 0) {
    return inputexamItem;
  }
  // エラーレベルに応じたエラーメッセージを追加
  const rangesErrorMessages: ExamRegistResult[] = matchExamRanges
    .filter(({ errorLevel }) => errorLevel === 2 || errorLevel === 3)
    .map(({ errorLevel }) => ({
      description:
        errorLevel === 2
          ? "入力値を確認してください。"
          : "入力に誤りがあります。",
      errorLevel,
    }));
  return rangesErrorMessages;
};
