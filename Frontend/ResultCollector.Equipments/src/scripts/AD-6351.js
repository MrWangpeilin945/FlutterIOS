/* AD-6351(暫定) */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x03");

  const heightSection = records
    .filter((x) => x.startsWith("\x02SY"))[0]
    ?.split(",");
  const weightSection = records
    .filter((x) => x.startsWith("\x02TZ"))[0]
    ?.split(",");

  return {
    height: heightSection[1]?.substring(2, 7).replaceAll(" ", ""),
    weight: weightSection[1]?.substring(2, 7).replaceAll(" ", ""),
  };
}
