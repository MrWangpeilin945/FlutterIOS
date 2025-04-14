import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  useNavigate,
} from "react-router";
import { useEffect } from "react";
import "@mantine/core/styles.css";
import "./styles/global.css";
import { ColorSchemeScript, MantineProvider } from "@mantine/core";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { customTheme } from "~/customTheme";
import { setupAxiosInterceptors } from "~/utils/axiosInstance";
import { authUtil } from "./utils/authUtil";

// TanStackQueryのリトライ回数を設定
// query系:3回、mutation系：リトライしない
const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 3 }, mutations: { retry: false } },
});

export function Layout({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate();

  const handleLogout = () => {
    authUtil.logout(() => {
      navigate("/login");
    });
  };
  useEffect(() => {
    // 画面のスケールを計算して適用する
    applyDisplayScale();

    // API呼び出し時に401が返ってきたらログアウトするための関数をaxiosInstanceに引き渡す
    setupAxiosInterceptors(handleLogout);
  }, []);

  const applyDisplayScale = () => {
    // DPRからスケールを求める
    const scale = 1 / window.devicePixelRatio;

    // metaタグのviewportのscaleを置き換える
    let viewportMeta = document.querySelector("meta[name=viewport]");
    if (!viewportMeta) {
      viewportMeta = document.createElement("meta");
      (viewportMeta as HTMLMetaElement).name = "viewport";
      document.head.appendChild(viewportMeta);
    }

    // scaleが0未満の時、デリミタが","だとiPadで反応しないので";"にする
    (
      viewportMeta as HTMLMetaElement
    ).content = `width=device-width; initial-scale=${scale}; user-scalable=no`;
  };

  return (
    <html lang="ja" suppressHydrationWarning>
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
        <ColorSchemeScript />
        <base href="/ResultCollector/" />
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
  return (
    <>
      <div className="loading-div">
        <span className="loading-span" />
      </div>
    </>
  );
}
