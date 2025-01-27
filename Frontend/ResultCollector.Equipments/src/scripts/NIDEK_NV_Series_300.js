/* NV-300 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x17").slice(3);

  const extractNumber = (s) => {
    if (s == null) return null;
    const substring = s?.substring(1, 3);
    if (substring == "FF") return 0;
    const result = parseInt(substring);
    return isNaN(result) ? null : result / 10;
  };

  const extractBool = (s) => {
    if (s == null) return null;
    const substring = s?.substring(1, 2);
    if (substring == "Y") return true;
    if (substring == "N") return false;
    return null;
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
    nakedLeftDV: dvWithCorrection === false ? leftDV : null,
    nakedRightDV: dvWithCorrection === false ? rightDV : null,
    nakedBothDV: dvWithCorrection === false ? bothDV : null,
    correctedLeftDV: dvWithCorrection === true ? leftDV : null,
    correctedRightDV: dvWithCorrection === true ? rightDV : null,
    correctedBothDV: dvWithCorrection === true ? bothDV : null,
    nakedLeftNV: nvWithCorrection === false ? leftNV : null,
    nakedRightNV: nvWithCorrection === false ? rightNV : null,
    nakedBothNV: nvWithCorrection === false ? bothNV : null,
    correctedLeftNV: nvWithCorrection === true ? leftNV : null,
    correctedRightNV: nvWithCorrection === true ? rightNV : null,
    correctedBothNV: nvWithCorrection === true ? bothNV : null,
  };
}
