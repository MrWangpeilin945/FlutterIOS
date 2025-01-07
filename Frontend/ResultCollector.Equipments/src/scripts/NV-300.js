/* NV-300 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x17").slice(3);

  const extractNumber = (s) => {
    if (s == null) return s;
    const result = s?.substring(1, 3);
    return result == "FF" ? result : result?.[0] + "." + result?.[1];
  };

  const extractBool = (s) => {
    if (s == null) return s;
    const result = s?.substring(1, 2);
    return result == "Y";
  };

  const rightVA = records.filter(
    (x) => x.startsWith("D") || x.startsWith("K")
  )[0];
  const leftVA = records.filter(
    (x) => x.startsWith("E") || x.startsWith("L")
  )[0];
  const bothVA = records.filter(
    (x) => x.startsWith("F") || x.startsWith("G")
  )[0];
  const vaCorrection = records.filter(
    (x) => x.startsWith("I") || x.startsWith("J")
  )[0];

  return {
    rightVA: extractNumber(rightVA),
    leftVA: extractNumber(leftVA),
    bothVA: extractNumber(bothVA),
    vaCorrection: extractBool(vaCorrection),
  };
}
