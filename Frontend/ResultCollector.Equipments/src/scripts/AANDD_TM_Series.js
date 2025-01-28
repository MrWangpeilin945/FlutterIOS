/* TM-2580 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x1e");

  const extract = (s) => {
    if (s == null) return null;
    const result = parseInt(s?.substring(1, 4));
    return isNaN(result) ? null : result;
  };

  return {
    systolicBP: extract(records[5]),
    meanBP: extract(records[6]),
    diastolicBP: extract(records[7]),
    pulseRate: extract(records[8]),
  };
}
