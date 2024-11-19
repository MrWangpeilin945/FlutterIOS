import { Divider, Flex, Group, Paper, Text } from "@mantine/core";
import type { RelatedExamItem } from "~/domain/wellship.schemas";
import styles from "~/styles/common.module.css";

type BoothNoteProps = {
  relatedExamItems: RelatedExamItem[];
};

export default function BoothNote({ relatedExamItems }: BoothNoteProps) {
  // 配列の要素がない時は空を返す
  if (relatedExamItems?.length === 0) {
    return null;
  }

  return (
    <>
      <Flex px={10} wrap="wrap" direction="row">
        {relatedExamItems.map((examItem, index) => (
          <Group key={index} mt={5} style={{ width: "calc(50% - 8px)" }}>
            <Flex gap="xs" align="flex-start" wrap="nowrap">
              <Paper
                className={styles["basic-grey"]}
                radius="md"
                px="xs"
                w={160}
                miw={160}
              >
                <Text size="xs" fw={700} ta="center">
                  {examItem.examItemName}
                </Text>
              </Paper>
              <Text size="xs" truncate="end" className={styles["text-wrap"]}>
                {examItem.examResult}
              </Text>
            </Flex>
          </Group>
        ))}
      </Flex>
      <Divider mt={5} mx={5} />
    </>
  );
}
