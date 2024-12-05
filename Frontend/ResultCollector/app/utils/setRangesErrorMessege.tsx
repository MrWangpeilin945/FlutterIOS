import { range } from "@mantine/hooks";
import type {
  InputExamItem,
  ExamItemDetail,
  ExamRegistResult,
  ExamNormalValueRange,
} from "~/domain/wellship.schemas";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";

// 受け取ったExamItemDetailsから
// 最も高いエラーレベルに該当するExamNormalValueRangeを返す
const getMaxErrorLevel = (examItemDetails: ExamItemDetail[]) => {
  let matchExamRange: ExamNormalValueRange | null = null;
  for (const examItemDetail of examItemDetails) {
    const numericValue = Number.parseFloat(examItemDetail.value || "");
    const ranges = examItemDetail.examNormalValueRanges;
    if (!numericValue || !ranges) continue;
    // ValueもしくはexamNormalValueRangesが無い場合次のループに進む
    const valueRange = ranges?.find((range) => {
      const minValue = range.minValue;
      const maxValue = range.maxValue;
      if (typeof minValue === "number" && typeof maxValue === "number") {
        return numericValue >= minValue && numericValue <= maxValue;
      }
    });
    if (
      valueRange &&
      (matchExamRange === null ||
        valueRange.errorLevel > matchExamRange.errorLevel)
    ) {
      matchExamRange = valueRange;
    }
  }
  return matchExamRange;
};

// 受け取ったInputexamitem[]の、examRegistresultsにエラーを追加して
// Inputexamitem[]を返す
export const setRangesErrorMessage = (inputexamItems: InputExamItem[]) => {
  for (const examitem of inputexamItems) {
    if (!examitem.examItemDetails) continue;
    const matchExamRange = getMaxErrorLevel(examitem.examItemDetails);
    if (matchExamRange === null) continue;
    const rangesErrorMessage: ExamRegistResult = {
      description: getErrorMessage(
        errorMessages.numberRange,
        `${examitem.name}は`,
        `${matchExamRange.minValue}`,
        `${matchExamRange.maxValue}`
      ),
      errorLevel: matchExamRange.errorLevel,
    };
    if (!examitem.examRegistResults) {
      examitem.examRegistResults = [];
    }
    examitem.examRegistResults?.push(rangesErrorMessage);
  }
  return inputexamItems;
};
