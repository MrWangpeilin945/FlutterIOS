/// <reference types="vitest/config" />

import { reactRouter } from "@react-router/dev/vite";
import { defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";

export default defineConfig({
  plugins: [
    reactRouter(),
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
