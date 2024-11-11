import { Button, createTheme } from "@mantine/core";
const baseColor = "#00ADA4";
export const customTheme = createTheme({
  colors: {
    // todo 色は取り急ぎ設定
    primary: [
      baseColor,
      baseColor,
      baseColor,
      baseColor,
      baseColor,
      baseColor,
      baseColor, // この色がボタンの色になる
      baseColor,
      baseColor,
      baseColor,
    ],
  },
  // プライマリカラーの設定
  primaryColor: "primary",
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
        h: "40",
      },
    }),
  },
});
