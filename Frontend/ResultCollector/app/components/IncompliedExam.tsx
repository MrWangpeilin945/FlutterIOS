import React from "react";
import { Box, Paper, Text } from "@mantine/core";
import styles from "~/styles/common.module.css";

type IncompliedExamProps = {
  name: string;
  incompliesExam: string[];
};

export default function IncompliedExam({
  name,
  incompliesExam,
}: IncompliedExamProps) {
  function concatItems(items: string[]): string {
    let resultSt = "";
    let count = 0;
    for (let i = 0; i < items.length; i++) {
      count += items[i].length + 1;
      // 足した後の文章が38文字を超えていた場合、改行
      if (count > 38) {
        resultSt += `\n${items[i]}、`;
        count = items[i].length + 1;
      } else {
        resultSt += `${items[i]}、`;
      }
    }
    return resultSt;
  }

  return (
    <Box p="md" w={924}>
      <Paper radius="lg" bg={"white"}>
        <Box
          style={{
            // Todo styleを指定して良いか確認
            borderTopLeftRadius: "inherit",
            borderTopRightRadius: "inherit",
          }}
          pl="sm"
          p="md"
          bg={"green02"}
        >
          <Text size="xs" c="black" fw="500">
            {name.length > 18
              ? `${name.slice(0, 18)}...さんの未受診検査項目はこちらです。`
              : `${name}さんの未受診検査項目はこちらです。`}
          </Text>
        </Box>
        <Box pl="sm" p="md">
          <Text
            className={styles["text-multiline"]}
            size="xs"
            c="black"
            fw="500"
          >
            {concatItems(incompliesExam)}
          </Text>
        </Box>
      </Paper>
    </Box>
  );
}
