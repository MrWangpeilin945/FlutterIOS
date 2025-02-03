import { expect, test } from "vitest";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

  test("required", () => {
    const result = getErrorMessage(errorMessages.required, "Aは");
    expect(result).toBe("Aは入力必須項目です。");
  });

  test("numericString", () => {
    const result = getErrorMessage(errorMessages.numericString, "Aは");
    expect(result).toBe("Aは半角数字で入力してください。");
  });

  test("fullwidthString", () => {
    const result = getErrorMessage(errorMessages.fullwidthString, "名前は");
    expect(result).toBe("名前は全角文字で入力してください。");
  });

  test("halfwidthString", () => {
    const result = getErrorMessage(errorMessages.halfwidthString, "IDは");
    expect(result).toBe("IDは半角文字で入力してください。");
  });

  test("alphaNumericString", () => {
    const result = getErrorMessage(errorMessages.alphaNumericString, "コードは");
    expect(result).toBe("コードは半角英数字で入力してください。");
  });

  test("upperAlphaNumericString", () => {
    const result = getErrorMessage(errorMessages.upperAlphaNumericString, "キーは");
    expect(result).toBe("キーは英大文字または数字で入力してください。");
  });

  test("fullWidthKanaString", () => {
    const result = getErrorMessage(errorMessages.fullWidthKanaString, "フリガナは");
    expect(result).toBe("フリガナは全角カナ文字で入力してください。");
  });

  test("halfWidthKanaString", () => {
    const result = getErrorMessage(errorMessages.halfWidthKanaString, "カナは");
    expect(result).toBe("カナは半角カナ文字で入力してください。");
  });

  test("byteLength", () => {
    const result = getErrorMessage(errorMessages.byteLength, "名前は", "10");
    expect(result).toBe("名前は10文字で入力してください。");
  });

  test("stringLength", () => {
    const result = getErrorMessage(errorMessages.stringLength, "名前は", "15");
    expect(result).toBe("名前は15文字で入力してください。");
  });

  test("dateInvalid", () => {
    const result = getErrorMessage(errorMessages.dateInvalid, "日付は");
    expect(result).toBe("日付は正しい日付で入力してください。");
  });

  test("dateRange", () => {
    const result = getErrorMessage(
      errorMessages.dateRange,
      "日付は",
      "2024/01/01",
      "2024/12/31"
    );
    expect(result).toBe("日付は2024/01/01から2024/12/31までの範囲で入力してください。");
  });

  test("numberInvalid", () => {
    const result = getErrorMessage(errorMessages.numberInvalid, "値は", 5, 2);
    expect(result).toBe("値は整数部：5桁、小数部：2桁までの範囲で入力してください。");
  });

  test("numberRange", () => {
    const result = getErrorMessage(errorMessages.numberRange, "値は", "1", "100");
    expect(result).toBe("値は1から100までの範囲で入力してください。");
  });

  test("minLength", () => {
    const result = getErrorMessage(errorMessages.minLength, "パスワードは", "8");
    expect(result).toBe("パスワードは8文字以上で入力してください。");
  });

  test("maxLength", () => {
    const result = getErrorMessage(errorMessages.maxLength, "名前は", "20");
    expect(result).toBe("名前は20文字以下で入力してください。");
  });

  test("emailInvalid", () => {
    const result = getErrorMessage(errorMessages.emailInvalid, "メールアドレスは");
    expect(result).toBe("メールアドレスはeメールアドレスの形式で入力してください。");
  });

  test("prohibited", () => {
    const result = getErrorMessage(errorMessages.prohibited, "テキストは", "×");
    expect(result).toBe('テキストは入力禁止文字 "×" が含まれています。');
  });

  test("serverError", () => {
    const result = getErrorMessage(errorMessages.serverError);
    expect(result).toBe("システム管理者にお問い合わせください。");
  });

  test("prohibited", () => {
    const result = getErrorMessage(errorMessages.accessDenied);
    expect(result).toBe("アクセスが拒否されました。入力された情報が正しくありません。");
  });