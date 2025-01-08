import { useEffect, useState } from "react";
import { GridCol, Button, Grid, Box } from "@mantine/core";

type KeyboardProps = {
  value: string;
  keyboardValues: string[];
  onChange: (newValue: string) => void;
};

export default function CollectionKeyboard(props: KeyboardProps) {
  if (!props.keyboardValues.length) return null;

  const [value, setValue] = useState(props.value);
  const [buttons, setButtons] = useState(props.keyboardValues);

  useEffect(() => {
    setValue(props.value);
  }, [props.value]);

  //ACを配列の最後に結合
  useEffect(() => {
    const newKeyboardValues = [...props.keyboardValues, "AC"];
    setButtons(newKeyboardValues);
  }, [props.keyboardValues]);

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
        h={88 * Math.ceil(buttons.length / 4) + 24}
        bg="gray03"
        p={16}
        style={{ borderRadius: 8 }}
      >
        <Grid gutter={8}>
          {buttons.map((keyValue, index) => {
            const buttonValue = String(keyValue);
            return (
              <GridCol
                span={buttonValue === "AC" ? "auto" : 3}
                style={buttonValue === "AC" ? { textAlign: "right" } : {}} // 最後のボタンを右端に配置
                key={index}
              >
                <Button
                  w={144}
                  h={80}
                  size="keyboard"
                  radius="md"
                  variant={
                    buttonValue === "AC"
                      ? "filled"
                      : buttonValue === value //選択済みか判定
                        ? "outline"
                        : "white"
                  }
                  color={
                    buttonValue === "AC"
                      ? "gray01"
                      : buttonValue === value //選択済みか判定
                        ? "primary"
                        : "black"
                  }
                  bg={buttonValue === value ? "green03" : ""} //選択済みか判定
                  value={buttonValue}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                  tabIndex={-1}
                >
                  {buttonValue}
                </Button>
              </GridCol>
            );
          })}
        </Grid>
      </Box>
    </>
  );
}
