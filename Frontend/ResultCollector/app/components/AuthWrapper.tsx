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
      // 認証チェックを行う

      // アクセストークンがない時はログアウト処理を行う
      const accessToken = authUtil.getAccessToken();
      if (!accessToken) {
        authUtil.logout(() => {
          navigate("/login");
        });
        return;
      }
      if (authUtil.isAccessTokenExpired(accessToken)) {
        // アクセストークンが期限切れの時はアクセストークンを再取得する
        if (!(await authUtil.refreshAccessToken())) {
          // アクセストークンが再取得できなかった時はログアウト処理を行う
          authUtil.logout(() => {
            navigate("/login");
          });
          return;
        }
      }

      // 認証チェックOK
      setIsAuthenticated(true);
    })();
  }, [navigate]);
  // 認証済みであれば子要素を描画する(未認証の時に一瞬子要素が描画されるのを防止)
  return isAuthenticated ? <>{children}</> : null;
}
