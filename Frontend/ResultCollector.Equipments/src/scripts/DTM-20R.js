/* DTM-20R */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  return {
    waist: rawText?.substring(0, 5)?.replaceAll(" ", ""),
  };
}
