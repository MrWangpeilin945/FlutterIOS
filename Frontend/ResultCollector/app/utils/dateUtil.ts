import { parse, isValid, format } from "date-fns";

interface EraInfo {
  name: string;
  alpha: string;
  start: string;
}

// 元号と開始日の定義
// 降順に定義する
const eras: EraInfo[] = [
  { name: "令和", alpha: "R", start: "2019-05-01" },
  { name: "平成", alpha: "H", start: "1989-01-08" },
  { name: "昭和", alpha: "S", start: "1926-12-25" },
  { name: "大正", alpha: "T", start: "1912-07-30" },
];

export const dateUtil = {
  // 和暦元号と年を取得する
  toJapaneseEra: (dateStr: string): string => {
    const parsedDate = parse(dateStr, "yyyy-MM-dd", new Date());

    if (!isValid(parsedDate)) {
      // 不正な日付の時は空文字を返す
      return "";
    }

    // 和暦元号と年を計算
    for (const era of eras) {
      const eraStartDate = parse(era.start, "yyyy-MM-dd", new Date());
      if (parsedDate >= eraStartDate) {
        const year =
          Number(format(parsedDate, "yyyy")) -
          Number(format(eraStartDate, "yyyy")) +
          1;
        return `${era.alpha}${year}`;
      }
    }

    // 該当する元号が存在しない時は空文字を返す
    return "";
  },

  // 和暦元号付きで日付をフォーマットする
  formatDateWithJapaneseEra: (dateStr: string): string => {
    const parsedDate = parse(dateStr, "yyyy-MM-dd", new Date());

    if (!isValid(parsedDate)) {
      // 不正な日付の時は空文字を返す
      return "";
    }

    const era = dateUtil.toJapaneseEra(dateStr);
    if (!era) {
      // 和暦変換できなかった時は空文字を返す
      return "";
    }

    // 和暦元号のアルファベットを全角に変換
    const eraFullWidth = era.replace(/[A-Z]/, (char) =>
      String.fromCharCode(char.charCodeAt(0) + 0xfee0),
    );

    // 日付をフォーマット
    const year = format(parsedDate, "yyyy");
    const month = format(parsedDate, "MM");
    const day = format(parsedDate, "dd");
    return `(${eraFullWidth}) ${year}/${month}/${day}`;
  },
};
