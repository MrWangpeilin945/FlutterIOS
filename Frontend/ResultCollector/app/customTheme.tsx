import {
  rem,
  Button,
  createTheme,
  Divider,
  Switch,
  colorsTuple,
  LoadingOverlay,
} from "@mantine/core";

import styles from "./styles/theme.module.css";

export const customTheme = createTheme({
  colors: {
    // プライマリカラー
    primary: colorsTuple("#00ADA4"),
    // システムの背景色
    background: colorsTuple("#F2F2F2"),
    // 標準の背景色
    white01: colorsTuple("#FFFFFF"),
    // 標準の文字色
    black01: colorsTuple("#3F3A39"),
    // 警告色
    warning: colorsTuple("#FF9100"),
    // エラー色
    error: colorsTuple("#E26264"),
    // 男性のプライマリカラー
    malePrimary: colorsTuple("#A0DDEA"),
    // 男性のセカンダリカラー
    maleSecondary: colorsTuple("#6DB5D1"),
    // 女性のプライマリカラー
    femalePrimary: colorsTuple("#FFBEB7"),
    // 女性のセカンダリカラー
    femaleSecondary: colorsTuple("#F3948E"),
    // 緑系の色
    green01: colorsTuple("#70CBA9"),
    green02: colorsTuple("#98DBBA"),
    green03: colorsTuple("#D5F0DF"),
    // 青系の色
    blue01: colorsTuple("#1C208D"),
    blue02: colorsTuple("#0D5DB9"),
    blue03: colorsTuple("#5694E6"),
    blue04: colorsTuple("#40B7EE"),
    // 灰系の色
    gray01: colorsTuple("#6A6A6A"),
    gray02: colorsTuple("#9A9A9A"),
    gray03: colorsTuple("#CECECE"),
    gray04: colorsTuple("#EAEAEA"),
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
      classNames: (theme, props, ctx) => ({
        // Buttonコンポーネントが使用不可の時に適用するスタイルを設定
        root: props.disabled ? styles["button-disabled"] : "",
      }),
    }),
    Divider: Divider.extend({
      vars: (theme) => {
        return { root: { "--divider-color": theme.colors.gray03[1] } };
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
              "--switch-height": rem(40),
              "--switch-label-font-size": rem(22),
              "--switch-thumb-size": rem(35),
              "--switch-track-label-padding": rem(1),
              "--switch-width": rem(120),
              "--switch-color": theme.colors.green01[1],
            },
          };
        }
        return { root: {} };
      },
      classNames: (theme, props, ctx) => ({
        // Switchコンポーネントがオフの時に適用するスタイルを設定
        root: !props.checked ? styles["switch-off"] : "",
      }),
    }),
    LoadingOverlay: LoadingOverlay.extend({
      defaultProps: {
        loaderProps: { size: "xxl" },
      },
      classNames: (theme, props, ctx) => ({
        root: props.loaderProps?.size === "xxl" ? styles["loading-large"] : "",
      }),
    }),
  },
});
