import {
  Box,
  Group,
  Paper,
  Stack,
  Text,
  Tooltip,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";
import { IconAlertCircle } from "@tabler/icons-react";
import { useState } from "react";
import { dateUtil } from "~/utils/dateUtil";
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
  const theme = useMantineTheme();

  function concatOffices(offices: string[]): string {
    return offices.join("、");
  }

  return (
    <Stack
      bg="white01"
      p={16}
      gap={16}
      style={{ borderBottomLeftRadius: 16, borderBottomRightRadius: 16 }}
    >
      <Group display="table">
        <Text
          size="lg"
          fw={700}
          c="black01"
          display="table-cell"
          ta="left"
          className={styles["text-wrap"]}
        >
          {name}
        </Text>
        {namesake && (
          <Box display="table-cell">
            <Group gap={10} justify="flex-end" wrap="nowrap">
              <IconAlertCircle
                size={32}
                fill={getThemeColor("warning", theme)}
                color={getThemeColor("white01", theme)}
              />
              <Text
                size="sm"
                fw={700}
                c="warning"
                style={{ textWrap: "nowrap" }}
              >
                同姓同名の受診者がいます
              </Text>
            </Group>
          </Box>
        )}
      </Group>
      <Group gap={24} wrap="nowrap">
        <Group gap={16} wrap="nowrap">
          <Paper miw={168} radius={8} bg="gray04" py={8}>
            <Text size="xs" fw={700} c="black01" ta="center">
              生年月日
            </Text>
          </Paper>
          <Text size="sm" c="black01" style={{ whiteSpace: "nowrap" }}>
            {dateUtil.formatDateWithJapaneseEra(birthday)}
          </Text>
        </Group>
        <Group
          gap={16}
          wrap="nowrap"
          style={{
            overflow: "hidden",
          }}
        >
          <Paper miw={168} radius={8} bg="gray04" py={8}>
            <Text size="xs" fw={700} c="black01" ta="center">
              団体
            </Text>
          </Paper>
          <Tooltip
            bg="green02"
            c="black01"
            label={concatOffices(office)}
            opened={showOfficeTooltip}
            multiline
            withArrow
            arrowPosition="side"
            arrowOffset={20}
            position="top-end"
            className={styles["text-wrap"]}
          >
            <Text
              size="sm"
              c="black01"
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
      </Group>
      <Group gap={16} wrap="nowrap">
        <Paper miw={168} radius={8} bg="gray04" py={8}>
          <Text size="xs" fw={700} c="black01" ta="center">
            備考
          </Text>
        </Paper>
        <Tooltip
          bg="green02"
          c="black01"
          label={note}
          opened={showNoteTooltip}
          multiline
          withArrow
          arrowPosition="side"
          arrowOffset={20}
          position="top-end"
          className={styles["text-wrap"]}
        >
          <Text
            size="sm"
            c="black01"
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
    </Stack>
  );
}
