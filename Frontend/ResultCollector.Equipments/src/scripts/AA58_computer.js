/* AA-58, AA-K1A （コンピュータ） */
export function decode(base64UrlString) {
  const rawText = atob(base64UrlString.replace(/-/g, "+").replace(/_/g, "/"));

  const extract = (s, indexStart, length) => {
    const result = s.slice(indexStart, indexStart + length);
    return result.replaceAll(" ", "");
  };

  return {
    "125hzLeft": extract(rawText, 28, 5),
    "125hzRight": extract(rawText, 33, 5),
    "250hzLeft": extract(rawText, 39, 5),
    "250hzRight": extract(rawText, 44, 5),
    "500hzLeft": extract(rawText, 50, 5),
    "500hzRight": extract(rawText, 55, 5),
    "750hzLeft": extract(rawText, 61, 5),
    "750hzRight": extract(rawText, 66, 5),
    "1000hzLeft": extract(rawText, 72, 5),
    "1000hzRight": extract(rawText, 77, 5),
    "1500hzLeft": extract(rawText, 83, 5),
    "1500hzRight": extract(rawText, 88, 5),
    "2000hzLeft": extract(rawText, 94, 5),
    "2000hzRight": extract(rawText, 99, 5),
    "3000hzLeft": extract(rawText, 105, 5),
    "3000hzRight": extract(rawText, 110, 5),
    "4000hzLeft": extract(rawText, 116, 5),
    "4000hzRight": extract(rawText, 121, 5),
    "6000hzLeft": extract(rawText, 127, 5),
    "6000hzRight": extract(rawText, 132, 5),
    "8000hzLeft": extract(rawText, 138, 5),
    "8000hzRight": extract(rawText, 143, 5),
  };
}
