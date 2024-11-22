import React from "react";
import { Flex, Box, Paper, Text } from "@mantine/core";
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
    let result = "";
    let count = 0;
    for (const item of items) {
      const newCount = count + item.length + 1;
      if (newCount > 38) {
        result += "\n";
        count = 0;
      }
      count += item.length + 1;
      result += `${item}、`;
    }
    // 最後の"、"を削除
    if (result.length > 0) {
      result = result.slice(0, -1);
    }
    return result;
  }

  return (
    <Flex align="flex-start" direction="column">
      <Box p="md" w={924}>
        <Paper radius="lg" bg={"white"}>
          <Box
            style={{
              borderTopLeftRadius: "inherit",
              borderTopRightRadius: "inherit",
            }}
            pl="sm"
            p="md"
            bg={"green02"}
          >
            <Text size="xs" c="black" fw="500">
              {name?.length > 18 ? `${name.slice(0, 18)}...` : name}
              さんの未受診検査項目はこちらです。
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
    </Flex>
  );
}
