import { useNavigate } from "@remix-run/react";
import { useEffect, useState } from "react";
import { authUtil } from "~/utils/authUtil";
export default function AuthWrapper({
  children,
}: {
  children: React.ReactNode;
}) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      // todo 認証チェックを行う
      // 一旦処理は実装したがコメントアウトしておく

      // // アクセストークンがない時はログイン画面に遷移する
      // const accessToken = authUtil.getAccessToken();
      // if (!accessToken) {
      //   authUtil.logout(() => {
      //     navigate("/login");
      //   });
      //   return;
      // }
      // if (authUtil.isAccessTokenExpired(accessToken)) {
      //   // アクセストークンが期限切れの時はアクセストークンを再取得する
      //   if (!(await authUtil.refreshAccessToken())) {
      //     // アクセストークンが再取得できなかった時はログイン画面に遷移する
      //     authUtil.logout(() => {
      //       navigate("/login");
      //     });
      //     return;
      //   }
      // }

      // 認証チェックOK
      setIsAuthenticated(true);
    })();
  }, [navigate]);
  // 認証済みであれば子要素を描画する(未認証の時に一瞬子要素が描画されるのを防止)
  return isAuthenticated ? <>{children}</> : null;
}
