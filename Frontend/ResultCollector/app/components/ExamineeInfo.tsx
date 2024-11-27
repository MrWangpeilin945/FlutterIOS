import { Box, Group, Paper, Text, Title, Tooltip } from "@mantine/core";
import { useState } from "react";
import styles from "~/styles/common.module.css";

type ExamineeInfoProps = {
  name: string;
  birthday: string;
  office: string[];
  note?: string;
  namesake: boolean;
};

export default function ExamineeInfo({
  name,
  birthday,
  office,
  note,
  namesake,
}: ExamineeInfoProps) {
  const [showOfficeTooltip, setShowOfficeTooltip] = useState(false);
  const [showNoteTooltip, setShowNoteTooltip] = useState(false);

  function concatOffices(offices: string[]): string {
    return offices.join("、");
  }

  return (
    <Box>
      <Group>
        <Title order={2} className={styles["text-wrap"]}>
          {name}
        </Title>
        {namesake && <Text>同姓同名の受診者がいます</Text>}
      </Group>
      <Group>
        <Paper
          className={styles["basic-grey"]}
          radius="md"
          px="xs"
          w={190}
          miw={190}
        >
          <Text size="xs" fw={700} ta="center">
            生年月日
          </Text>
        </Paper>
        <Text>{birthday}</Text>
        <Paper
          className={styles["basic-grey"]}
          radius="md"
          px="xs"
          w={190}
          miw={190}
        >
          <Text size="xs" fw={700} ta="center">
            団体
          </Text>
        </Paper>
        <Tooltip
          label={concatOffices(office)}
          w={600}
          opened={showOfficeTooltip}
          multiline
          withArrow
          position="top"
        >
          <Text
            w={400}
            onClick={() => setShowOfficeTooltip(!showOfficeTooltip)}
            style={{
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
          >
            {concatOffices(office)}
          </Text>
        </Tooltip>
      </Group>
      <Group>
        <Paper
          className={styles["basic-grey"]}
          radius="md"
          px="xs"
          w={190}
          miw={190}
        >
          <Text size="xs" fw={700} ta="center">
            備考
          </Text>
        </Paper>
        <Tooltip
          label={note}
          w={800}
          opened={showNoteTooltip}
          multiline
          withArrow
          position="bottom"
        >
          <Text
            w={600}
            onClick={() => setShowNoteTooltip(!showNoteTooltip)}
            style={{
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
          >
            {note}
          </Text>
        </Tooltip>
      </Group>
    </Box>
  );
}
