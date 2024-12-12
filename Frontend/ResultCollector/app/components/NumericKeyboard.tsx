import { useEffect, useState } from "react";
import { GridCol, Button, Grid, Box } from "@mantine/core";

type KeyboardProps = {
  value: string;
  integerLength?: number;
  decimalLength?: number;
  onChange: (newValue: string) => void;
  onConfirm: () => void;
};

export default function NumericKeyboard(props: KeyboardProps) {
  const keyboardValues = [
    "7",
    "8",
    "9",
    "4",
    "5",
    "6",
    "1",
    "2",
    "3",
    "AC",
    "0",
    "確定",
  ];

  // 小数点と先頭の0を除去して数値部分だけを取得する処理
  const convertValue = (value: string): string => {
    // 小数点を取り除く
    const withoutDecimal = value.replace(".", "");
    // 先頭の0を除去
    const withoutLeadingZero = withoutDecimal.replace(/^0+/, "");
    return withoutLeadingZero || "0"; // 空になった場合は "0" を返す
  };
  const [value, setValue] = useState(convertValue(props.value));

  useEffect(() => {
    setValue(convertValue(props.value));
  }, [props.value]);

  // 小数点追加処理
  const formatDecimalValue = (
    value: string,
    integerLength: number,
    decimalLength: number,
  ): string => {
    if (!value || integerLength <= 0 || decimalLength < 0) return value;
    const totalLength = integerLength + decimalLength;
    const paddedValue = value.padStart(totalLength, "0");
    let integerPart = paddedValue.slice(0, integerLength);
    const decimalPart = paddedValue.slice(integerLength, totalLength);
    // 整数部が不足する場合、0で補填
    if (integerPart.length < integerLength) {
      integerPart =
        "0".repeat(integerLength - integerPart.length) + integerPart;
    }
    let formattedValue = `${integerPart}.${decimalPart}`;
    // 整数部が1未満の場合、整数部の先頭ゼロは除去しない
    if (Number.parseInt(formattedValue) < 1) {
      formattedValue = `0.${decimalPart}`;
    } else {
      // 整数部の先頭にゼロがついている場合、それを除去
      formattedValue = formattedValue.replace(/^0+/, "");
    }
    return formattedValue;
  };

  //バリデーションチェック(整数)
  const handleInputChange = (newValue: string) => {
    const inputKeyboardValues = keyboardValues.filter((keyboardValue) => {
      const invalidValues = ["AC", "確定"];
      return !invalidValues.includes(keyboardValue);
    });

    const pattern = new RegExp(`^(${inputKeyboardValues.join("|")})*$`);

    if (pattern.test(newValue)) {
      setValue(newValue);
      //小数点追加処理
      if (props.integerLength && props.decimalLength) {
        const formatValue = formatDecimalValue(
          newValue,
          props.integerLength,
          props.decimalLength,
        );
        props.onChange(formatValue);
      } else {
        props.onChange(newValue.replace(/^0+/, ""));
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
      <Box w={480} h={376} bg="gray03" p={16} style={{ borderRadius: 8 }}>
        <Grid gutter={8}>
          {keyboardValues.map((keyValue, index) => (
            <GridCol span={4} key={index}>
              <Button
                w={144}
                h={80}
                size="keyboard"
                radius="md"
                variant={
                  keyValue === "AC" || keyValue === "確定" ? "filled" : "white"
                }
                color={
                  keyValue === "AC" || keyValue === "確定" ? "gray01" : "black"
                }
                value={keyValue}
                onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}
                tabIndex={-1}
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
