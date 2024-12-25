// biome-ignore lint/style/useImportType: typeをつけるとaxiosInstanceが関数として認識されなくなるため
import axios, { AxiosInstance } from "axios";
import { authUtil } from "./authUtil";

export const axiosInstance: AxiosInstance = axios.create({
  // .env.productionのVITE_API_BASE_URLは設定する必要がない
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 3000,
});

// interceptorの定義(処理内でログイン画面に遷移する)
export const setupAxiosInterceptors = (handleLogout: () => void) => {
  // requestの共通前処理
  axiosInstance.interceptors.request.use(async (config) => {
    // console.log("-----------interceptors.request start-----------");

    // "/api/v1"を除いたエンドポイントを取得する
    const endpoint = config.url?.replace(/^\/api\/v\d+/, "");
    // 認証系エンドポイント
    const authEndpoints = ["/staff/login", "/auth/refresh"];
    // if (authEndpoints.includes(endpoint || "")) {
    //   console.log("認証系エンドポイント");
    // } else {
    //   console.log("認証系以外のエンドポイント");
    // }

    // todo 認証チェック
    // 一旦処理は実装したがコメントアウトしておく

    // // アクセストークンを取得する
    // const accessToken = authUtil.getAccessToken();

    // // 認証系のエンドポイントの時は認証チェックをしない
    // if (!authEndpoints.includes(endpoint || "")) {
    //   // 認証チェック

    //   // アクセストークンがない時はログアウト処理を行う
    //   if (!accessToken) {
    //     handleLogout();
    //     return Promise.reject(
    //       new Error("No authentication token, redirecting to login"),
    //     );
    //   }
    //   if (authUtil.isAccessTokenExpired(accessToken)) {
    //     // アクセストークンが期限切れの時はアクセストークンを再取得する
    //     if (!(await authUtil.refreshAccessToken())) {
    //       // アクセストークンが再取得できなかった時はログアウト処理を行う
    //       handleLogout();
    //       return Promise.reject(
    //         new Error("No authentication token, redirecting to login"),
    //       );
    //     }
    //   }
    // }

    // // トークンリフレッシュのAPIを呼び出すときはCookieを使用する必要がある
    // // axiosの第三引数？に`{ withCredentials: true }`が必要
    // if (endpoint === "/auth/refresh") {
    //   // Cookieを付与する
    //   config.withCredentials = true;
    // } else {
    //   // Cookieを付与しない
    //   config.withCredentials = false;
    //   // 認証トークンを付与
    //   config.headers.Authorization = `Bearer ${accessToken}`;
    // }

    // console.log(`method:${config.method}`);
    // console.log(`url:${config.url}`);
    // console.log("-----------interceptors.request end-----------");

    return config;
  });

  axiosInstance.interceptors.response.use(
    // todo 共通エラー処理など
    // ステータスコード 2xx
    (response) => {
      // console.log("-----------interceptors.response success start-----------");
      // console.log(`status:${response.status}`);
      // console.log(`method:${response.config.method}`);
      // console.log(`url:${response.config.url}`);
      // console.log(`data:${response.data}`);
      // console.log("-----------interceptors.response success end-----------");
      return response;
    },
    // ステータスコード 2xx 以外
    (error) => {
      if (!error.response) {
        // ネットワークエラーの時
        console.log("network error");
        return Promise.reject(
          new Error("Network error: Unable to reach the server"),
        );
      }

      // console.log("-----------interceptors.response error start-----------");
      // console.log(`status:${error.response.status}`);
      // console.log(`method:${error.config.method}`);
      // console.log(`url:${error.config.url}`);
      // console.log("-----------interceptors.response error end-----------");
      if (error.response.status === 401) {
        // リクエストで認証エラーが発生した時はログアウト処理を行う
        handleLogout();
        return new Promise(() => {});
      }

      return Promise.reject(error);
    },
  );
};
