import { getDefaultStore } from "jotai/vanilla";
import CryptoJS from "crypto-js";
import { clearAllState, serverTimeOffsetState } from "~/store/store";
import { authenticationRefresh } from "~/api/wellship";

const accessTokenName = "resultCollectorAccessToken";
// 暗号化のキー
const encryptionKey = "RyobiSystems1965";
// APIのバージョン
const apiVersion = "1";

// JWTのペイロードを格納するインターフェース
interface JwtPayload {
  iat: number;
  exp: number;
}

let refreshTokenPromise: Promise<boolean> | null = null;

// 認証周りの共通処理を定義
export const authUtil = {
  // アクセストークンを取得する
  getAccessToken: (): string | null => {
    const cryptToken = localStorage.getItem(accessTokenName);
    // 複合化して返却する
    return cryptToken ? authUtil.decrypt(cryptToken) : null;
  },

  // アクセストークンを保存する
  setAccessToken: (token: string): void => {
    // 暗号化して保存する
    const cryptToken = authUtil.encrypt(token);
    localStorage.setItem(accessTokenName, cryptToken);
  },

  // アクセストークンの有効期限チェックを行う
  isAccessTokenExpired: (token: string): boolean => {
    const decoded = authUtil.decodeJwt(token);
    if (!decoded) {
      // 有効期限が取得できない時は有効期限切れ
      return true;
    }
    // クライアントとサーバーの時間の差を考慮(時間の差はログイン時に取得)
    const store = getDefaultStore();
    const offset = store.get(serverTimeOffsetState) ?? 0;
    const now = Math.floor(Date.now() / 1000);
    // 有効期限が切れていたらtrue
    return decoded.exp + offset < now;
  },

  // アクセストークンを再取得する
  refreshAccessToken: async (): Promise<boolean> => {
    if (!refreshTokenPromise) {
      refreshTokenPromise = (async () => {
        try {
          // キャッシュを使用せず直接アクセストークンリフレッシュのAPIを呼び出す
          const accessToken = authUtil.getAccessToken();
          const response = await authenticationRefresh(apiVersion, {
            token: accessToken ?? "",
          });
          authUtil.setAccessToken(response.data.token ?? "");
          return true;
        } catch (error) {
          return false;
        }
      })();
    }
    return refreshTokenPromise;
  },

  // ログアウト処理
  logout: async (navigateToLogin: () => void): Promise<void> => {
    // アクセストークンを削除する
    localStorage.removeItem(accessTokenName);

    // 状態管理をクリアする
    authUtil.removeState();

    // 共通関数はReactのフックを使用できないので自力でログイン画面への遷移はできない
    // 呼び出し元からログイン画面に遷移するメソッドを渡す
    navigateToLogin();
  },

  // 状態管理をクリアする
  removeState: (): void => {
    const store = getDefaultStore();
    store.set(clearAllState);
  },

  // JWTを解析する
  decodeJwt: (token: string): JwtPayload | null => {
    try {
      // ペイロードを取得する
      const payloadBase64Url = token.split(".")[1];
      if (!payloadBase64Url) {
        return null;
      }

      // Base64URL をデコード
      const payloadJson = atob(
        payloadBase64Url.replace(/-/g, "+").replace(/_/g, "/"),
      );

      // JSONをオブジェクトに変換
      const payload: JwtPayload = JSON.parse(payloadJson);
      return payload;
    } catch (error) {
      return null;
    }
  },

  // クライアントとサーバーの時間の差を計算する
  // サーバーの時間はJWTのiatから取得する
  calculateTimeOffset: (token: string): number => {
    const clientCurrentTime = Math.floor(Date.now() / 1000);
    const decoded = authUtil.decodeJwt(token);
    if (!decoded) {
      return 0;
    }
    // クライアントの時間 - サーバーの時間
    return clientCurrentTime - decoded.iat;
  },

  // 暗号化を行う
  encrypt: (data: string): string => {
    return CryptoJS.AES.encrypt(data, encryptionKey).toString();
  },

  // 復号化を行う
  decrypt: (ciphertext: string): string => {
    const bytes = CryptoJS.AES.decrypt(ciphertext, encryptionKey);
    return bytes.toString(CryptoJS.enc.Utf8);
  },
};
