import axios from "axios";

// エラーメッセージを取得する関数
export const getErrorMessage = (error: unknown): string => {
	if (axios.isAxiosError(error)) {
        const status = error.response?.status;
        if (status !== undefined) {
            if (status >= 500) {
                return "システム管理者にお問い合わせください。"; // サーバーエラー
            }
            return "班情報が指定されていません。"; // クライアントエラー
        }
    }
	return "不明なエラーが発生しました";
};
