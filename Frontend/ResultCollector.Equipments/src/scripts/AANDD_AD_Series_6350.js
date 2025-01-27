/* 身体計測：AD-6224A, AD-6350 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x03");

  const heightSection = records.find((x) => x.startsWith("\x02SY"))?.split(",");
  const weightSection = records.find((x) => x.startsWith("\x02TZ"))?.split(",");

  const height = parseFloat(heightSection[1]?.substring(2, 7));
  const weight = parseFloat(weightSection[1]?.substring(2, 7));

  return {
    height: isNaN(height) ? null : height,
    weight: isNaN(weight) ? null : weight,
  };
}
