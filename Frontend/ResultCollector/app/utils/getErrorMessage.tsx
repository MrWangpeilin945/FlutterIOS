//メッセージ一覧を参照
export const errorMessages = {
  required: "{0}入力必須項目です。",
  numericString: "半角数字を入力してください。",
  fullwidthString: "全角文字を入力してください。",
  halfwidthString: "半角文字を入力してください。",
  alphaNumericString: "{0}半角英数字で入力してください。",
  upperAlphaNumericString: "英大文字または数字を入力してください。",
  fullwidthKanaString: "全角カナ文字を入力してください。",
  halfwidthKanaString: "半角カナ文字を入力してください。",
  byteLength: "文字で入力してください。",
  byteStringLength: "文字で入力してください。",
  date: "正しい日付で入力してください。",
  dateRange: "{0}から{1}までの範囲で入力してください。",
  number: "整数部：{0}桁、小数部：{1}桁までの範囲で入力してください。",
  numberRange: "{0}から{1}までの範囲で入力してください。",
  minLength: "{0}文字以上で入力してください。",
  maxLength: "{0}文字以下で入力してください。",
  email: "eメールアドレスの形式で入力してください。",
  prohibited: '入力禁止文字 "{0}" が含まれています。',
  invalid: "の入力形式が間違っています。",
  noData: "{0}データが0件でした。",
  notFound: "が存在しません。",
  server: "システム管理者にお問い合わせください。",
};

// メッセージ内の {0}, {1}, {2}... を動的に置き換える関数
export const getErrorMessage = (
  message: string,
  ...values: (string | number)[]
) => {
  return message.replace(/{(\d+)}/g, (match, index) => {
    return values[index] !== undefined ? values[index].toString() : match;
  });
};
