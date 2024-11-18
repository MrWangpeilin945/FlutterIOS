import {
  rem,
  Button,
  createTheme,
  Divider,
  Switch,
  colorsTuple,
} from "@mantine/core";

export const customTheme = createTheme({
  colors: {
    // プライマリカラー
    primary: colorsTuple("#00ADA4"),
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
  },
  // プライマリカラーの設定
  primaryColor: "primary",
  // 標準の背景色
  white: "#FFFFFF",
  // 標準の文字色
  black: "#3F3A39",
  // ベースとなるフォントサイズの設定
  fontSizes: { xs: "22px", sm: "24px", md: "28px", lg: "30px", xl: "46px" },
  // 各コンポーネントのカスタマイズ
  components: {
    Button: Button.extend({
      vars: () => {
        return { root: {} };
      },
      defaultProps: {
        radius: "50",
      },
    }),
    Divider: Divider.extend({
      vars: () => {
        return { root: {} };
      },
      defaultProps: {
        // todo テーマから取得する
        color: "#CECECE",
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
              // todo テーマから取得する
              "--switch-color": "#70CBA9",
            },
          };
        }
        return { root: {} };
      },
    }),
  },
});
