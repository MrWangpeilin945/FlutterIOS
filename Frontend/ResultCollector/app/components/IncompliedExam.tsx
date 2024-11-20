import React from "react";
import { Box, Paper, Title, Text, Flex, Group } from "@mantine/core";
import styles from "~/styles/common.module.css";

type incompliedExamProps = {
  name: string;
  incompliesExam: string[];
};

export default function IncompliedExam({
  name,
  incompliesExam,
}: incompliedExamProps) {
  function concatItems(items: string[]): string {
    const resultArray = [];

    for (let i = 0; i < items.length; i++) {
      resultArray.push(items[i]);

      if ((i + 1) % 6 === 0 && i !== items.length - 1) {
        // 6要素ごとに改行文字を追加
        resultArray.push("、\n");
      } else if (i !== items.length - 1) {
        // 要素の間で句読点追加
        resultArray.push("、");
      }
    }

    return resultArray.join("");
  }

  return (
    <Box p="md" w={800}>
      <Paper radius="lg" bg={"white"}>
        <Box
          className={styles["custome-box"]}
          pl="sm"
          p="md"
          bg={"green02"}
          // Todo CSS増やして良いか確認
        >
          <Text size="sm" c="black" fw="550">
            {name.length > 10
              ? `${name.slice(0, 10)}...さんの未受診検査項目はこちらです。`
              : `${name}さんの未受診検査項目です。`}
          </Text>
        </Box>
        <Box pl="sm" p="md">
          <Text className={styles["white-wrap"]} size="sm" c="black" fw="500">
            {concatItems(incompliesExam)}
          </Text>
        </Box>
      </Paper>
    </Box>
  );
}
