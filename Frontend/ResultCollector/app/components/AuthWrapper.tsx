import { useNavigate } from "@remix-run/react";
import { useEffect, useState } from "react";
export default function AuthWrapper({
  children,
}: {
  children: React.ReactNode;
}) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const navigate = useNavigate();
  // ↓一時biomeエラー回避用
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    // todo 認証チェックを行う
    // const token = localStorage.getItem("authToken");
    // if (!token) {
    //   navigate("/login");
    // } else {
    //   setIsAuthenticated(true);
    // }
    // 認証処理は後ほど実装
    setIsAuthenticated(true);
  }, [navigate]);
  // 認証済みであれば子要素を描画する(未認証の時に一瞬子要素が描画されるのを防止)
  return isAuthenticated ? <>{children}</> : null;
}
