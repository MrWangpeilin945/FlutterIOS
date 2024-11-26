import { useEffect, useState } from "react";
import { GridCol, Button, Grid, Box } from "@mantine/core";

type KeyboardProps = {
  value?: string;
  keyboardValues: string[];
  onChange: (newValue: string) => void;
};

export default function CollectionKeyboard(props: KeyboardProps) {
  const [value, setValue] = useState(props.value);

  useEffect(() => {
    setValue(props.value);
  }, [props.value]);

  //押下時処理
  const handlerKeyboardClick = (keyboardValue: string) => {
    let newValue = value;

    if (keyboardValue === "AC") {
      newValue = ""; // クリア
    } else {
      newValue = keyboardValue;
    }
    setValue(newValue);
    props.onChange(newValue);
  };

  return (
    <>
      {/* キーボード(選択) */}
      <Box
        w={632}
        h={88 * Math.ceil(props.keyboardValues.length / 4) + 24}
        bg="gray03"
        p={16}
        style={{ borderRadius: 8 }}
      >
        <Grid gutter={8}>
          {props.keyboardValues.map((keyValue, index) => {
            const isLastButton = index === props.keyboardValues.length - 1;

            return (
              <GridCol
                span={isLastButton ? "auto" : 3}
                style={isLastButton ? { textAlign: "right" } : {}} // 最後のボタンを右端に配置
                key={index}
              >
                <Button
                  w={144}
                  h={80}
                  size="keyboard"
                  radius="md"
                  variant={
                    keyValue === "AC"
                      ? "filled"
                      : keyValue === value //選択済みか判定
                        ? "outline"
                        : "white"
                  }
                  color={
                    keyValue === "AC"
                      ? "gray01"
                      : keyValue === value //選択済みか判定
                        ? "primary"
                        : "black"
                  }
                  bg={keyValue === value ? "green03" : ""} //選択済みか判定
                  value={keyValue}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  {keyValue}
                </Button>
              </GridCol>
            );
          })}
        </Grid>
      </Box>
    </>
  );
}
