import axios, { type AxiosInstance } from "axios";

export const axiosInstance: AxiosInstance = axios.create({
  // todo baseURLは実際には設定から取得する
  baseURL: "http://localhost:3001/",
  timeout: 3000,
});

// interceptorの定義(処理内でログイン画面に遷移する)
export const setupAxiosInterceptors = (redirectToLogin: () => void) => {
  // requestの共通前処理
  axiosInstance.interceptors.request.use((config) => {
    console.log("-----------interceptors.request start-----------");

    // todo 認証処理
    const token = localStorage.getItem("authToken");
    if (!token) {
      // 未認証時はログイン画面に遷移(redirectToLoginはroot.tsxで定義)
      // axiosInstance内ではuseNavigate()が使用できないため
      redirectToLogin();
      return Promise.reject(
        new Error("No authentication token, redirecting to login")
      );
    }

    // 認証トークンを付与
    config.headers.Authorization = `Bearer ${token}`;

    console.log(`method:${config.method}`);
    console.log(`url:${config.url}`);
    console.log("-----------interceptors.request end-----------");

    return config;
  });

  axiosInstance.interceptors.response.use(
    // todo 共通エラー処理など
    // ステータスコード 2xx
    (response) => {
      console.log("-----------interceptors.response success start-----------");
      console.log(`status:${response.status}`);
      console.log(`method:${response.config.method}`);
      console.log(`url:${response.config.url}`);
      console.log(`data:${response.data}`);
      console.log("-----------interceptors.response success end-----------");
      return response;
    },
    // ステータスコード 2xx 以外
    (error) => {
      if (!error.response) {
        // ネットワークエラーの時
        console.log("network error");
        return Promise.reject(
          new Error("Network error: Unable to reach the server")
        );
      }

      console.log("-----------interceptors.response error start-----------");
      console.log(`status:${error.response.status}`);
      console.log(`method:${error.config.method}`);
      console.log(`url:${error.config.url}`);
      console.log("-----------interceptors.response error end-----------");
      if (error.response.status === 401) {
        // 認証エラーの時はログイン画面に遷移(redirectToLoginはroot.tsxで定義)
        // axiosInstance内ではuseNavigate()が使用できないため
        redirectToLogin();
        return new Promise(() => {});
      }

      return Promise.reject(error);
    }
  );
};
