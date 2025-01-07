/* NV-300 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x17").slice(3);

  const extractNumber = (s) => {
    if (s == null) return null;
    const result = s?.substring(1, 3);
    return result == "FF" ? 0 : result / 10;
  };

  const extractBool = (s) => {
    if (s == null) return null;
    const result = s?.substring(1, 2);
    return result == "Y";
  };

  const rightDV = extractNumber(records.find((x) => x.startsWith("D")));
  const leftDV = extractNumber(records.find((x) => x.startsWith("E")));
  const bothDV = extractNumber(records.find((x) => x.startsWith("F")));
  const bothNV = extractNumber(records.find((x) => x.startsWith("G")));
  const dvWithCorrection = extractBool(records.find((x) => x.startsWith("I")));
  const nvWithCorrection = extractBool(records.find((x) => x.startsWith("J")));
  const rightNV = extractNumber(records.find((x) => x.startsWith("K")));
  const leftNV = extractNumber(records.find((x) => x.startsWith("L")));

  return {
    nakedRightDV: dvWithCorrection === false ? rightDV : null,
    nakedLeftDV: dvWithCorrection === false ? leftDV : null,
    nakedBothDV: dvWithCorrection === false ? bothDV : null,
    correctedRightDV: dvWithCorrection === true ? rightDV : null,
    correctedLeftDV: dvWithCorrection === true ? leftDV : null,
    correctedBothDV: dvWithCorrection === true ? bothDV : null,
    rightDV: rightDV,
    leftDV: leftDV,
    bothDV: bothDV,
    dvWithCorrection: dvWithCorrection,
    nakedRightNV: nvWithCorrection === false ? rightNV : null,
    nakedLeftNV: nvWithCorrection === false ? leftNV : null,
    nakedBothNV: nvWithCorrection === false ? bothNV : null,
    correctedRightNV: nvWithCorrection === true ? rightNV : null,
    correctedLeftNV: nvWithCorrection === true ? leftNV : null,
    correctedBothNV: nvWithCorrection === true ? bothNV : null,
    rightNV: rightNV,
    leftNV: leftNV,
    bothNV: bothNV,
    nvWithCorrection: nvWithCorrection,
  };
}
