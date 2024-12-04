import {
  Box,
  Button,
  Group,
  Progress,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { AggregatedProgressStatus } from "~/domain/enums";
import styles from "~/styles/common.module.css";

type ProgressData = {
  examItemId?: number;
  examItemName?: string;
  details?: {
    status?: number;
    statusName?: string;
    count?: number;
  }[];
};

interface ExamItemProgressProps {
  progress: ProgressData;
  onClick: () => void;
}

export default function ExamItemProgress({
  progress,
  onClick,
}: ExamItemProgressProps) {
  // 合計値を算出
  const total =
    progress.details?.reduce((sum, detail) => sum + (detail.count || 0), 0) ||
    0;

  const statusColors = [
    { status: AggregatedProgressStatus.済, color: "blue01" },
    { status: AggregatedProgressStatus.中止, color: "blue04" },
    { status: AggregatedProgressStatus.来場, color: "blue02" },
    { status: AggregatedProgressStatus.予定, color: "blue03" },
  ];

  // 各項目の設定と割合を算出
  const sections = statusColors.map(({ status, color }) => {
    const detail = progress.details?.find((d) => d.status === status);
    const count = detail?.count || 0;
    return {
      value: (count / total) * 100,
      color,
      label: detail?.statusName,
      count,
    };
  });

  return (
    <Box className={styles.border}>
      <Title
        h={40}
        order={3}
        className={styles["basic-blue"]}
        fw={550}
        pl={20}
        py={3}
      >
        {progress.examItemName}
      </Title>
      <Box>
        <Group>
          {sections.map((sections) => (
            <Stack key={sections.label} align="center">
              <Button w={100} h={40} color={sections.color}>
                <Text size="lg">{sections.label}</Text>
              </Button>
              <Text size="lg">{sections.count}</Text>
            </Stack>
          ))}
        </Group>
        <Progress.Root size="lg" mt={10}>
          {sections.map((sections) => (
            <Progress.Section
              key={sections.label}
              value={sections.value}
              color={sections.color}
            />
          ))}
        </Progress.Root>
      </Box>
    </Box>
  );
}
