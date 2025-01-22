// biome-ignore lint/style/useImportType: typeをつけるとaxiosInstanceが関数として認識されなくなるため
import axios, { AxiosInstance } from "axios";
import { authUtil } from "./authUtil";

export const axiosInstance: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: import.meta.env.VITE_API_TIMEOUT,
});

// 認証系エンドポイント
const authEndpoints = ["/staff/login", "/staff/login/refresh"];

// interceptorの定義(処理内でログイン画面に遷移する)
export const setupAxiosInterceptors = (handleLogout: () => void) => {
  // requestの共通前処理
  axiosInstance.interceptors.request.use(async (config) => {
    // "/api/v1"を除いたエンドポイントを取得する
    const endpoint = config.url?.replace(/^\/api\/v\d+/, "") ?? "";

    // アクセストークンを取得する
    let accessToken = authUtil.getAccessToken();

    // 認証系のエンドポイントの時は認証チェックをしない
    if (!authEndpoints.includes(endpoint)) {
      // 認証チェック

      // アクセストークンがない時はログアウト処理を行う
      if (!accessToken) {
        handleLogout();
        return Promise.reject(
          new Error("No authentication token, redirecting to login"),
        );
      }
      if (authUtil.isAccessTokenExpired(accessToken)) {
        // アクセストークンが期限切れの時はアクセストークンをリフレッシュする
        if (!(await authUtil.refreshAccessToken())) {
          // アクセストークンが再取得できなかった時はログアウト処理を行う
          handleLogout();
          return Promise.reject(
            new Error("No authentication token, redirecting to login"),
          );
        }
        // リフレッシュしたアクセストークンを取得しなおす
        accessToken = authUtil.getAccessToken();
      }
    }

    // 認証処理
    if (authEndpoints.includes(endpoint)) {
      // 認証系エンドポイントの時はCookieを付与する
      config.withCredentials = true;
    } else {
      // 認証系エンドポイント以外の時はアクセストークンを付与する
      config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
  });

  axiosInstance.interceptors.response.use(
    // 正常時
    (response) => {
      return response;
    },
    // 異常時
    (error) => {
      if (!error.response) {
        // 開発モードの時のみ、StrictModeの影響でキャンセルエラーが発生するのでここで抜ける
        if (error.code === "ERR_CANCELED") {
          return Promise.resolve();
        }
        // ネットワークエラーの時
        return Promise.reject(
          new Error("Network error: Unable to reach the server"),
        );
      }

      // ステータスコードによって処理を分岐する
      if (error.response.status === 401) {
        // "/api/v1"を除いたエンドポイントを取得する
        const endpoint = error.config.url?.replace(/^\/api\/v\d+/, "") ?? "";

        if (authEndpoints.includes(endpoint)) {
          // 認証系のエンドポイントの時はログアウト処理をせずそのままエラーを返す
          return Promise.reject(error);
        }
        // 認証系以外のエンドポイントで認証エラーが発生した時はログアウト処理を行う
        handleLogout();
        return new Promise(() => {});
      }

      return Promise.reject(error);
    },
  );
};
