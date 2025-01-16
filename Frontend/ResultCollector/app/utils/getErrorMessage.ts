//メッセージ一覧を参照
export const errorMessages = {
  required: "{0}入力必須項目です。",
  numericString: "{0}半角数字で入力してください。",
  fullwidthString: "{0}全角文字で入力してください。",
  halfwidthString: "{0}半角文字で入力してください。",
  alphaNumericString: "{0}半角英数字で入力してください。",
  upperAlphaNumericString: "{0}英大文字または数字で入力してください。",
  fullWidthKanaString: "{0}全角カナ文字で入力してください。",
  halfWidthKanaString: "{0}半角カナ文字で入力してください。",
  byteLength: "{0}{1}文字で入力してください。",
  stringLength: "{0}{1}文字で入力してください。",
  dateInvalid: "{0}正しい日付で入力してください。",
  dateRange: "{0}{1}から{2}までの範囲で入力してください。",
  numberInvalid:
    "{0}整数部：{1}桁、小数部：{2}桁までの範囲で入力してください。",
  numberRange: "{0}{1}から{2}までの範囲で入力してください。",
  minLength: "{0}{1}文字以上で入力してください。",
  maxLength: "{0}{1}文字以下で入力してください。",
  emailInvalid: "{0}eメールアドレスの形式で入力してください。",
  prohibited: '{0}入力禁止文字 "{1}" が含まれています。',
  invalid: "{0}の入力形式が間違っています。",
  noData: "{0}データが0件でした。",
  notFound: "{0}が存在しません。",
  serverError: "システム管理者にお問い合わせください。",
  accessDenied: "アクセスが拒否されました。入力された情報が正しくありません。",
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
