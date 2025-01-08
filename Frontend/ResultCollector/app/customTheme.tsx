import {
  rem,
  Button,
  createTheme,
  Divider,
  Switch,
  colorsTuple,
  LoadingOverlay,
  getSize,
  getThemeColor,
} from "@mantine/core";

import styles from "./styles/theme.module.css";

export const customTheme = createTheme({
  colors: {
    // プライマリカラー
    primary: colorsTuple("#00979A"),
    // システムの背景色
    background: colorsTuple("#F2F2F2"),
    // 標準の背景色
    white01: colorsTuple("#FFFFFF"),
    // 標準の文字色
    black01: colorsTuple("#3F3A39"),
    // 警告色
    warning: colorsTuple("#EE6A00"),
    // エラー色
    error: colorsTuple("#E26264"),
    // 男性のプライマリカラー
    malePrimary: colorsTuple("#A0DDEA"),
    // 男性のセカンダリカラー
    maleSecondary: colorsTuple("#219FD1"),
    // 女性のプライマリカラー
    femalePrimary: colorsTuple("#FFBEB7"),
    // 女性のセカンダリカラー
    femaleSecondary: colorsTuple("#F56358"),
    // ストライプカラー
    stripe: colorsTuple("#EAF7F2"),
    // 緑系の色
    green01: colorsTuple("#38A676"),
    green02: colorsTuple("#98DBBA"),
    green03: colorsTuple("#CDFADB"),
    // 青系の色
    blue01: colorsTuple("#1C208D"),
    blue02: colorsTuple("#0D5DB9"),
    blue03: colorsTuple("#5694E6"),
    blue04: colorsTuple("#009DE6"),
    // 灰系の色
    gray01: colorsTuple("#6A6A6A"),
    gray02: colorsTuple("#949494"),
    gray03: colorsTuple("#CECECE"),
    gray04: colorsTuple("#EAEAEA"),
    // オレンジ系の色
    orange01: colorsTuple("#EB6200"),
  },
  // プライマリカラーの設定
  primaryColor: "primary",
  // 標準の背景色
  white: "#FFFFFF",
  // 標準の文字色
  black: "#3F3A39",
  // ベースとなるフォントサイズの設定
  fontSizes: {
    xs: "24px",
    sm: "26px",
    md: "28px",
    lg: "30px",
    xl: "32px",
    keyboard: "48px",
    inputComponent: "64px",
  },
  // フォントファミリー
  fontFamily:
    " -apple-system, BlinkMacSystemFont, Roboto, Helvetica, Arial, sans-serif, Apple Color Emoji, Segoe UI Emoji",
  // ベースとなる境界半径サイズの設定
  radius: {
    itemName: "10px",
  },
  // 各コンポーネントのカスタマイズ
  components: {
    Button: Button.extend({
      vars: () => {
        return { root: {} };
      },
      defaultProps: {
        radius: "50",
      },
      classNames: (theme, props) => ({
        // Buttonコンポーネントが使用不可の時に適用するスタイルを設定
        root: props.disabled ? styles["button-disabled"] : "",
      }),
    }),
    Divider: Divider.extend({
      vars: (theme) => {
        return { root: { "--divider-color": getThemeColor("gray03", theme) } };
      },
      defaultProps: {
        color: "var(--divider-color)",
      },
    }),
    Switch: Switch.extend({
      vars: (theme, props) => {
        // 検査内容確認画面用のスイッチ
        if (props.size === "xxl") {
          return {
            root: {
              "--switch-height": rem(60),
              "--switch-label-font-size": getSize("lg"),
              "--switch-thumb-size": rem(54),
              "--switch-track-label-padding": rem(1),
              "--switch-width": rem(188),
              "--switch-color": getThemeColor("green01", theme),
            },
          };
        }
        return { root: {} };
      },
      classNames: (theme, props) => ({
        // Switchコンポーネントがオフの時に適用するスタイルを設定
        root: !props.checked
          ? `${styles["switch-off"]} ${styles["switch-padding"]}`
          : `${styles["switch-padding"]}`,
      }),
    }),
    LoadingOverlay: LoadingOverlay.extend({
      defaultProps: {
        loaderProps: { size: "xxl" },
      },
      classNames: (theme, props) => ({
        root: props.loaderProps?.size === "xxl" ? styles["loading-large"] : "",
      }),
    }),
  },
});
