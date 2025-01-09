/* 身体計測：AD-6224A, AD-6350 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x03");

  const heightSection = records.find((x) => x.startsWith("\x02SY"))?.split(",");
  const weightSection = records.find((x) => x.startsWith("\x02TZ"))?.split(",");

  return {
    height: Number(heightSection[1]?.substring(2, 7)),
    weight: Number(weightSection[1]?.substring(2, 7)),
  };
}
