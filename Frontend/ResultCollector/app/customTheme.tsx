import { Button, createTheme, rem, TextInput } from "@mantine/core";

export const customTheme = createTheme({
  primaryColor: "primary",
  colors: {
    primary: [
      "#ebfffe",
      "#d7fdfb",
      "#aafdf8",
      "#7cfdf5",
      "#60fcf3",
      "#54fdf1",
      "#00ada4",
      "#3ee1d6",
      "#2ec8be",
      "#4bfdf1",
    ],
  },
  components: {
    Button: Button.extend({
      defaultProps: {
        h: "40px",
        radius: "xl",
      },
      styles: {
        root: {
          fontSize: "20px",
        },
      },
    }),
  },
});
