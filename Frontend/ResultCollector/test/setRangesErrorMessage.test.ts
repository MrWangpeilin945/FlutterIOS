import { expect, test } from "vitest";
import { ExamItemDetailType, InputErrorLevel } from "~/domain/enums";
import { setRangesErrorMessage } from "~/utils/setRangesErrorMessage";

test("examItemDetailsが空の場合", () => {
  //Arrange:検査項目明細が空のデータを設定
  const data = { examItemDetails: [] };
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("examItemDetailsがundefinedの場合", () => {
  //Arrange:検査項目明細がundefinedのデータを設定
  const data = { examItemDetails: undefined };
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("hasOrderがfalseの場合", () => {
  //Arrange:検査項目明細のhasOrderがfalseのデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("cancelReasonIdが存在する場合", () => {
  //Arrange:検査項目明細のcancelReasonIdが存在するデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("typeが選択の場合", () => {
  //Arrange:検査項目明細のtypeが選択のデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("数値が正常範囲内に収まる場合", () => {
  //Arrange:検査項目明細のvalueがexamNormalValueRangesの範囲内ではないデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([]);
});

test("数値が範囲外で警告レベルのエラーが発生する場合", () => {
  //Arrange:検査項目明細のvalueがexamNormalValueRangesの範囲内のデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([
    {
      description: "入力値を確認してください。",
      errorLevel: InputErrorLevel.警告,
    },
  ]);
});

test("数値が範囲外で異常レベルのエラーが発生する場合", () => {
  //Arrange:検査項目明細のvalueがexamNormalValueRangesの範囲内のデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([
    {
      description: "入力に誤りがあります。",
      errorLevel: InputErrorLevel.異常,
    },
  ]);
});

test("複数の基準値チェックが存在し、異常が優先される場合", () => {
  //Arrange:検査項目明細のvalueに対して、examNormalValueRangesに複数の基準値エラーがあるデータを設定
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
  //Act:seRangesErrorMessageメソッドを呼び出す
  const result = setRangesErrorMessage(data);
  //Assert:結果を検証
  expect(result).toStrictEqual([
    {
      description: "入力に誤りがあります。",
      errorLevel: InputErrorLevel.異常,
    },
  ]);
});
