import { useEffect, useState } from "react";
import "@mantine/core/styles.css";
import {
  MantineProvider,
  GridCol,
  Button,
  Grid,
  Box,
  Title,
} from "@mantine/core";

type keyboardProps = {
  size: number;
  value: string;
  onChange: (newValue: string) => void;
  onConfirm: () => void;
};

export default function SoftwareKeyboard(props: keyboardProps) {
  const size = props.size;
  const keyboardValues = [
    "7","8","9",
    "4","5","6",
    "1","2","3",
    "AC","0","確定",
  ];
  const [value, setValue] = useState(props.value);
  const [initialized, setInitialized] = useState(false);

  const handleInputChange = (newValue: string) => {
    const inputKeyboardValues = keyboardValues.filter((keyboardValue) => {
      // 入力可能な値のみを抽出
      const invalidValues = ["AC"];
      return invalidValues.includes(keyboardValue) === false;
    });

    const pattern = new RegExp(`^(${inputKeyboardValues.join("|")})*$`);

    if (pattern.test(newValue) && newValue.length <= 10) {
      // 入力可能文字だけの場合
      setValue(newValue);
      if (typeof props.onChange === "function") {
        props.onChange(newValue);
      }
    }
  };
  const handlerKeyboardClick = (keyboardValue: string) => {
    let beforeValue = value.toString();

    let middleValue = "";
    if (keyboardValue === "AC") {
      // クリア
      beforeValue = "";
    } else {
      middleValue = keyboardValue;
    }

    if (keyboardValue === "確定") {
      // 確定ボタンが押された時の処理を呼び出す
      props.onConfirm();
      return;
    }

    const newValue = beforeValue + middleValue;
    handleInputChange(newValue);
  };

  useEffect(() => {
    if (initialized === true) {
      //モード変更
      // const mode = "";
      // switch (mode) {
      //     case "マイナス":
      //         keyboardValues[11] = "-"
      //         break;
      //     case "少数":
      //         keyboardValues[11] = "."
      //     default:
      //         break;
      // }
    } else {
      setInitialized(true);
    }
    setValue(props.value);
  });

  return (
    <>
      <MantineProvider>
        {/* ソフトウェアキーボード */}

        <Box
          w={size * 3 + 15}
          h={size * 2 + 15}
          bg={"rgba(181, 181, 181, 1)"}
          m="md"
          style={{ padding: 3, borderRadius: 8 }}
        >
          <div>
            <Grid gutter={3}>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[0]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[0]}</Title>
                </Button>{" "}
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[1]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[1]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[2]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[2]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[3]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[3]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[4]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[4]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[5]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[5]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[6]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[6]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[7]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[7]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[8]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[8]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="filled"
                  color="rgb(110, 110, 110)"
                  value={keyboardValues[9]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[9]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="white"
                  color="rgb(0,0,0)"
                  value={keyboardValues[10]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[10]}</Title>
                </Button>
              </GridCol>
              <GridCol span={4}>
                <Button
                  w={size}
                  h={size / 2}
                  variant="filled"
                  color="rgb(110, 110, 110)"
                  value={keyboardValues[11]}
                  onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                >
                  <Title order={2}>{keyboardValues[11]}</Title>
                </Button>
              </GridCol>
            </Grid>
          </div>
        </Box>
      </MantineProvider>
    </>
  );
}
