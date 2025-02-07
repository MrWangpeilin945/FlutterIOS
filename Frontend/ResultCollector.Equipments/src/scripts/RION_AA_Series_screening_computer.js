/* 聴力（選別検査）：AA-58, AA-K1A （コンピュータ）*/
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));
  const offset = rawText.lastIndexOf(" \x02");

  const extract = (s, index) => {
    const result = s[index];
    return result == "P" ? "1" : result == "R" ? "2" : null;
  };

  return {
    screening1000HzLeft: extract(rawText, 36 + offset),
    screening1000HzRight: extract(rawText, 35 + offset),
    screening4000HzLeft: extract(rawText, 46 + offset),
    screening4000HzRight: extract(rawText, 45 + offset),
  };
}
