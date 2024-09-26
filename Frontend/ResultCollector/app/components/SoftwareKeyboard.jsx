import { useEffect, useState } from 'react';
import "@mantine/core/styles.css";
import { MantineProvider, ColorSchemeScript, GridCol, Button, Grid, Box } from "@mantine/core";

export default function SoftwareKeyboard(props) {
    // Data
    const keyboardValues = [
        '7', '8', '9',
        '4', '5', '6',
        '1', '2', '3',
        'AC', '0', '確定',
    ];
    const [value, setValue] = useState(props.value);
    const [initialized, setInitialized] = useState(false);

    // Methods
    const handleInputChange = newValue => {
        const inputKeyboardValues = keyboardValues.filter(keyboardValue => { // 入力可能な値のみを抽出
            const invalidValues = ["AC"];
            return invalidValues.includes(keyboardValue) === false;
        });

        const pattern = new RegExp(`^(${inputKeyboardValues.join('|')})*$`);

        if (pattern.test(newValue) && newValue.length <= 10) { // 入力可能文字だけの場合
            setValue(newValue);
            if (typeof props.onChange === 'function') {
                props.onChange(newValue);
            }
        }

    };
    const handlerKeyboardClick = (keyboardValue) => {

        let beforeValue = value.toString();

        let middleValue = "";
        if (keyboardValue === "AC") { // クリア
            beforeValue = "";
        } else {
            middleValue = keyboardValue;
        }

        if (keyboardValue === "確定") {

        }

        const newValue = beforeValue + middleValue;
        handleInputChange(newValue);
    };

    // Effect
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

                <Box w={255} h={175} bg={"rgba(181, 181, 181, 1)"} style={{ padding: 3, borderRadius: 8 }}>
                    <div>
                        <Grid gutter={3}>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[0]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[0]}</Button> </GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[1]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[1]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[2]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[2]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[3]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[3]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[4]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[4]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[5]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[5]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[6]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[6]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[7]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[7]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[8]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[8]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="filled" color="rgb(110, 110, 110)" value={keyboardValues[9]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[9]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="white" color="rgb(0,0,0)" value={keyboardValues[10]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[10]}</Button></GridCol>
                            <GridCol span={4}><Button w={80} h={40} variant="filled" color="rgb(110, 110, 110)" value={keyboardValues[11]} onClick={(e) => handlerKeyboardClick(e.currentTarget.value)}>{keyboardValues[11]}</Button></GridCol>
                        </Grid>
                    </div>
                </Box>
            </MantineProvider>
        </>
    );
}