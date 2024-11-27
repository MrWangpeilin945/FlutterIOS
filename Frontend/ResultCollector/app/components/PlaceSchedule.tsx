import { Box, Text } from "@mantine/core";
import styles from "~/styles/common.module.css";

type PlaceScheduleProps = {
  placeName: string;
  examDate: string;
};

export default function PlaceSchedule({
  placeName,
  examDate,
}: PlaceScheduleProps) {
  return (
    <Box className={styles.border}>
      <Text>{placeName}</Text>
      <Text>{examDate}</Text>
    </Box>
  );
}
