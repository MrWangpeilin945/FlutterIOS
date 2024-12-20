import { Stack, Text } from "@mantine/core";
import { format, parse } from "date-fns";

type PlaceScheduleProps = {
  placeName: string;
  examDate: string;
};

export default function PlaceSchedule({
  placeName,
  examDate,
}: PlaceScheduleProps) {
  return (
    <Stack gap={16}>
      <Text
        size="lg"
        fw={700}
        c="black01"
        style={{
          wordBreak: "break-word",
          whiteSpace: "pre-wrap",
        }}
      >
        {placeName}
      </Text>
      <Text size="sm" c="black01">
        {examDate !== "" &&
          format(parse(examDate, "yyyy-MM-dd", new Date()), "yyyy/MM/dd")}
      </Text>
    </Stack>
  );
}
