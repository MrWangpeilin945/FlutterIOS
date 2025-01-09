/* 心電図：FCP-8300, FCP-8321, FCP-8400 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));
  const offset = rawText.lastIndexOf("\x02");

  const extract = (s, index, length) => {
    const span = s?.substring(index, index + length);
    if (span == null || span == "") return null;
    const result = Number(span);
    return result != NaN ? result : null;
  };

  return {
    heartRate: extract(rawText, 42 + offset, 4),
  };
}
