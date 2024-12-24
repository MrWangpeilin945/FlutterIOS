/// <reference types="vitest/config" />

import { vitePlugin as remix } from "@remix-run/dev";
import { defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";

export default defineConfig({
  plugins: [
    remix({
      ssr: false,
      future: {
        v3_fetcherPersist: true,
        v3_relativeSplatPath: true,
        v3_throwAbortReason: true,
      },
      basename: "/ResultCollector/",
    }),
    tsconfigPaths(),
  ],
  base: "/ResultCollector/",
  resolve: {
    mainFields: ["browser", "module", "main"],
    alias: {
      // 開発環境でも、アイコンを静的にエクスポートするように設定
      "@tabler/icons-react": "@tabler/icons-react/dist/esm/icons/index.mjs",
    },
  },
  test: {
    globals: true,
  },
});
