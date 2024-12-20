import { expect, test } from "vitest";
import { dateUtil } from "~/utils/dateUtil";
test("令和_開始", () => {
  const result = dateUtil.toJapaneseEra("2019-05-01");
  expect(result).toBe("R1");
});
test("平成_終了", () => {
  const result = dateUtil.toJapaneseEra("2019-04-30");
  expect(result).toBe("H31");
});
test("平成_開始", () => {
  const result = dateUtil.toJapaneseEra("1989-01-08");
  expect(result).toBe("H1");
});
test("昭和_終了", () => {
  const result = dateUtil.toJapaneseEra("1989-01-07");
  expect(result).toBe("S64");
});
test("昭和_開始", () => {
  const result = dateUtil.toJapaneseEra("1926-12-25");
  expect(result).toBe("S1");
});
test("大正_終了", () => {
  const result = dateUtil.toJapaneseEra("1926-12-24");
  expect(result).toBe("T15");
});
test("大正_開始", () => {
  const result = dateUtil.toJapaneseEra("1912-07-30");
  expect(result).toBe("T1");
});
test("不正な日付", () => {
  const result = dateUtil.toJapaneseEra("2024-02-30");
  expect(result).toBe("");
});
test("大正より前の日付", () => {
  const result = dateUtil.toJapaneseEra("1800-01-01");
  expect(result).toBe("");
});
