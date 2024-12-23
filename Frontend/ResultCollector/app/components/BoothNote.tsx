import { Divider, Flex, Group, Paper, Text } from "@mantine/core";
import type { RelatedExamItem } from "~/domain/wellship.schemas";
import styles from "~/styles/common.module.css";

type BoothNoteProps = {
  relatedExamItems: RelatedExamItem[];
};

export default function BoothNote({ relatedExamItems }: BoothNoteProps) {
  // 配列の要素がない時は空を返す
  if (!relatedExamItems || relatedExamItems?.length === 0) {
    return null;
  }

  return (
    <>
      <Flex direction="column" style={{ width: "100%" }}>
        <Flex px={16} wrap="wrap" direction="row" gap={16}>
          {relatedExamItems.map((examItem, index) => (
            <Group key={index} mt={5} style={{ width: "calc(50% - 8px)" }}>
              <Flex gap={16} align="center" wrap="nowrap">
                <Paper
                  bg="gray03"
                  c="gray01"
                  radius="md"
                  w={210}
                  miw={210}
                  h={51}
                  py={8}
                >
                  <Text size="xs" fw={700} ta="center">
                    {examItem.examItemName?.slice(0, 8)}
                  </Text>
                </Paper>
                <Text
                  size="xs"
                  className={styles["text-wrap"]}
                  style={{ width: "calc(48vw - 242px)" }}
                >
                  {examItem.examResult}
                </Text>
              </Flex>
            </Group>
          ))}
        </Flex>
        <Divider mt={16} />
      </Flex>
    </>
  );
}
