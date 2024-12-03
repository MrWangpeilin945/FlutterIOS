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
  const total = progress.details?.reduce(
    (sum, detail) => sum + (detail.count || 0),
    0,
  );

  // 各項目の設定と割合を算出
  const sections = [
    {
      value:
        (progress.details?.find((d) => d.status === AggregatedProgressStatus.済)
          ?.count || 0 / (total || 0)) * 100,
      color: "blue01",
      label: progress.details?.find(
        (d) => d.status === AggregatedProgressStatus.済,
      )?.statusName,
      count:
        progress.details?.find((d) => d.status === AggregatedProgressStatus.済)
          ?.count || 0,
    },
    {
      value:
        (progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.中止,
        )?.count || 0 / (total || 0)) * 100,
      color: "blue04",
      label: progress.details?.find(
        (d) => d.status === AggregatedProgressStatus.中止,
      )?.statusName,
      count:
        progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.中止,
        )?.count || 0,
    },
    {
      value:
        (progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.来場,
        )?.count || 0 / (total || 0)) * 100,
      color: "blue02",
      label: progress.details?.find(
        (d) => d.status === AggregatedProgressStatus.来場,
      )?.statusName,
      count:
        progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.来場,
        )?.count || 0,
    },
    {
      value:
        (progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.予定,
        )?.count || 0 / (total || 0)) * 100,
      color: "blue03",
      label: progress.details?.find(
        (d) => d.status === AggregatedProgressStatus.予定,
      )?.statusName,
      count:
        progress.details?.find(
          (d) => d.status === AggregatedProgressStatus.予定,
        )?.count || 0,
    },
  ];

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
            <>
              <Stack key={sections.label} align="center">
                <Button w={100} h={40} color={sections.color}>
                  <Text size="lg">{sections.label}</Text>
                </Button>
                <Text size="lg">{sections.count}</Text>
              </Stack>
            </>
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
