/* TM-2580 */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const records = rawText.split("\x1e");

  const extract = (s) => {
    const result = s?.substring(1, 4);
    return result?.replaceAll(" ", "");
  };

  const sys = extract(records[5]);
  const dia = extract(records[7]);
  const pul = extract(records[8]);

  return {
    sys: sys,
    dia: dia,
    pul: pul,
  };
}
