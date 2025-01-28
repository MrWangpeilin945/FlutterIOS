/* DTM-20R */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));
  const substring = rawText?.substring(0, 5);

  if (substring == null) return null;

  const result = parseFloat(substring);

  return {
    waist: isNaN(result) ? null : result,
  };
}
