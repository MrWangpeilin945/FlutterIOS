import { expect, test } from "vitest";
import { ExamItemDetailType, InputErrorLevel } from "~/domain/enums";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";

test("examItemDetailsが空の場合", () => {
  const data = { examItemDetails: [] };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("examItemDetailsがundefinedの場合", () => {
  const data = { examItemDetails: undefined };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("hasOrderがfalseの場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: false,
        value: "10",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("cancelReasonIdが存在する場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        cancelReasonId: 123,
        value: "10",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("typeが選択の場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        value: "10",
        type: ExamItemDetailType.選択,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("数値が正常範囲内に収まる場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        value: "16",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([]);
});

test("数値が範囲外で警告レベルのエラーが発生する場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        value: "10",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([
    {
      description: "入力値を確認してください。",
      errorLevel: InputErrorLevel.警告,
    },
  ]);
});

test("数値が範囲外で異常レベルのエラーが発生する場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        value: "5",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.異常 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([
    {
      description: "入力に誤りがあります。",
      errorLevel: InputErrorLevel.異常,
    },
  ]);
});

test("複数の明細が存在し、異常が優先される場合", () => {
  const data = {
    examItemDetails: [
      {
        hasOrder: true,
        value: "10",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.異常 },
        ],
      },
      {
        hasOrder: true,
        value: "10",
        type: ExamItemDetailType.入力,
        examNormalValueRanges: [
          { minValue: 5, maxValue: 15, errorLevel: InputErrorLevel.警告 },
        ],
      },
    ],
  };
  const result = setRangesErrorMessage(data);
  expect(result).toStrictEqual([
    {
      description: "入力に誤りがあります。",
      errorLevel: InputErrorLevel.異常,
    },
  ]);
});