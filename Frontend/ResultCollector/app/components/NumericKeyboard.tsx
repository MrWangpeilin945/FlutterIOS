import { useEffect, useState } from "react";
import {
  GridCol,
  Button,
  Grid,
  Box,
} from "@mantine/core";

type KeyboardProps = {
  value: string;
  onChange: (newValue: string) => void;
  onConfirm: () => void;
};

export default function NumericKeyboard(props: KeyboardProps) {
  const keyboardValues = [
    "7", "8", "9",
    "4", "5", "6",
    "1", "2", "3",
    "AC", "0", "確定",
  ];

  const [value, setValue] = useState(props.value);

  useEffect(() => {
    setValue(props.value);
  }, [props.value]);

  //バリデーションチェック(整数)
  const handleInputChange = (newValue: string) => {
    const inputKeyboardValues = keyboardValues.filter((keyboardValue) => {
      const invalidValues = ["AC", "確定"];
      return !invalidValues.includes(keyboardValue);
    });

    const pattern = new RegExp(`^(${inputKeyboardValues.join("|")})*$`);

    if (pattern.test(newValue)) {
      setValue(newValue);
      if (typeof props.onChange === "function") {
        props.onChange(newValue);
      }
    }
  };

  //押下時処理
  const handlerKeyboardClick = (keyboardValue: string) => {
    let beforeValue = value.toString();
    let middleValue = "";

    if (keyboardValue === "AC") {
      beforeValue = ""; // クリア
    } else if (keyboardValue === "確定") {
      props.onConfirm(); // 確定処理
      return;
    } else {
      middleValue = keyboardValue;
    }

    const newValue = beforeValue + middleValue;
    handleInputChange(newValue);
  };


  return (
    <>
      {/* ソフトウェアキーボード */}
      <Box
        w={480}
        h={376}
        bg="gray03"
        p={16}
        style={{borderRadius: 8 }}
      >
          <Grid gutter={8}>
            {keyboardValues.map((keyValue, index) => (
              <GridCol span={4} key={index}>
                <Button
                  w={144}
                  h={80}
                  size="keyboard"
                  radius="md"
                  variant={keyValue === "AC" || keyValue === "確定" ? "filled" : "white"}
                  color={keyValue === "AC" || keyValue === "確定" ? "gray01" : "black"}
                  value={keyValue}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  {keyValue}
                </Button>
              </GridCol>
            ))}
          </Grid>
      </Box>
    </>
  );
}
