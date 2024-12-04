import React from "react";
import { Flex, Box, Paper, Text } from "@mantine/core";
import styles from "~/styles/common.module.css";

type IncompliedExamProps = {
  name: string;
  incompliedExams: string[];
};

export default function IncompliedExam({
  name,
  incompliedExams,
}: IncompliedExamProps) {
  function concatItems(items: string[]): string {
    if (items.length === 0) {
      return "未受診の検査項目はありません。";
    }
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
      <Box w={924}>
        <Paper radius="lg" bg={"white"}>
          <Box
            style={{
              borderTopLeftRadius: "inherit",
              borderTopRightRadius: "inherit",
            }}
            px="24"
            py={"sm"}
            bg={"green02"}
          >
            <Text size="xs" c="black" fw="500">
              {name?.length > 18 ? `${name.slice(0, 18)}...` : name}
              さんの未受診検査項目はこちらです。
            </Text>
          </Box>
          <Box px="24" py={"sm"}>
            <Text
              className={styles["text-multiline"]}
              size="xs"
              c="black"
              fw="500"
            >
              {concatItems(incompliedExams)}
            </Text>
          </Box>
        </Paper>
      </Box>
    </Flex>
  );
}
