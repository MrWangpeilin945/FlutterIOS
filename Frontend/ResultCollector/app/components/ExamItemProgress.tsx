import {
  Box,
  Button,
  Divider,
  Group,
  Progress,
  Text,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";
import { AggregatedProgressStatus } from "~/domain/enums";

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
  const theme = useMantineTheme();
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
    <Box bg="white01" py={24} px={32} style={{ borderRadius: 16 }}>
      <Text size="md" fw={700} c="black01" pb={16}>
        {progress.examItemName}
      </Text>
      <Divider
        size="xs"
        pb={16}
        style={{ "--divider-color": getThemeColor("gray03", theme) }}
      />
      <Group>
        {sections.map((sections) => (
          <Box key={sections.label}>
            <Button w={154} h={66} py={16} px={32} color={sections.color}>
              <Text size="lg" fw={700} c="white01">
                {sections.label}
              </Text>
            </Button>
            <Text size="lg" fw={700} c="black01" ta="center">
              {sections.count}
            </Text>
          </Box>
        ))}
      </Group>
      <Progress.Root size={30} radius={32} mt={16}>
        {sections.map((sections) => (
          <Progress.Section
            key={sections.label}
            value={sections.value}
            color={sections.color}
          />
        ))}
      </Progress.Root>
    </Box>
  );
}
