/* 聴力（閾値検査）：AA-58, AA-K1A （コンピュータ）*/
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));
  const offset = rawText.indexOf(" \x02");

  const extract = (s, indexStart, length) => {
    const substring = s.slice(indexStart, indexStart + length).replaceAll(" ", "");
    if (substring == "") return null;
    if (substring.startsWith("(") && substring.endsWith(")")) return 999;
    const result = parseInt(substring);
    return isNaN(result) ? substring : result;
  };

  return {
    monitoring125HzLeft: extract(rawText, 33 + offset, 5),
    monitoring125HzRight: extract(rawText, 28 + offset, 5),
    monitoring250HzLeft: extract(rawText, 44 + offset, 5),
    monitoring250HzRight: extract(rawText, 39 + offset, 5),
    monitoring500HzLeft: extract(rawText, 55 + offset, 5),
    monitoring500HzRight: extract(rawText, 50 + offset, 5),
    monitoring750HzLeft: extract(rawText, 66 + offset, 5),
    monitoring750HzRight: extract(rawText, 61 + offset, 5),
    monitoring1000HzLeft: extract(rawText, 77 + offset, 5),
    monitoring1000HzRight: extract(rawText, 72 + offset, 5),
    monitoring1500HzLeft: extract(rawText, 88 + offset, 5),
    monitoring1500HzRight: extract(rawText, 83 + offset, 5),
    monitoring2000HzLeft: extract(rawText, 99 + offset, 5),
    monitoring2000HzRight: extract(rawText, 94 + offset, 5),
    monitoring3000HzLeft: extract(rawText, 110 + offset, 5),
    monitoring3000HzRight: extract(rawText, 105 + offset, 5),
    monitoring4000HzLeft: extract(rawText, 121 + offset, 5),
    monitoring4000HzRight: extract(rawText, 116 + offset, 5),
    monitoring6000HzLeft: extract(rawText, 132 + offset, 5),
    monitoring6000HzRight: extract(rawText, 127 + offset, 5),
    monitoring8000HzLeft: extract(rawText, 143 + offset, 5),
    monitoring8000HzRight: extract(rawText, 138 + offset, 5),
  };
}
