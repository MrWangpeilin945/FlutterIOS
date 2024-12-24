import { expect, test } from "vitest";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";
test("required", () => {
  const result = getErrorMessage(errorMessages.required, "Aは");
  expect(result).toBe("Aは入力必須項目です");
});
