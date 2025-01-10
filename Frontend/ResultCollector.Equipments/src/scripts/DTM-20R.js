/* DTM-20R */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));
  const data = rawText?.substring(0, 5);

  if (data == null) return null;

  return {
    waist: Number(rawText?.substring(0, 5)),
  };
}
