import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  useNavigate,
} from "@remix-run/react";
import { useEffect } from "react";
import "@mantine/core/styles.css";
import "./styles/global.css";
import { ColorSchemeScript, MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { customTheme } from "~/customTheme";
import { setupAxiosInterceptors } from "~/utils/axiosInstance";

const queryClient = new QueryClient();

export function Layout({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate();

  const redirectToLogin = () => {
    navigate("/login");
  };
  // Biomeのエラーが出るので、コメントアウトしています。
  // useEffect(() => {
  //   // API呼び出し時に401が返ってきたらログイン画面に遷移する処理をaxiosInstanceに引き渡す
  //   setupAxiosInterceptors(redirectToLogin);
  // }, []);

  return (
    <html lang="ja">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
        <ColorSchemeScript />
      </head>
      <body>
        <MantineProvider theme={customTheme}>
          <QueryClientProvider client={queryClient}>
            {children}
          </QueryClientProvider>
        </MantineProvider>
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

export default function App() {
  return <Outlet />;
}

export function HydrateFallback() {
  return <p>Loading...</p>;
}
