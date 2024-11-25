import { Box, Group, Paper, Text, Title } from "@mantine/core";
import styles from "~/styles/common.module.css";

type ExamineeInfoProps = {
  name: string;
  birthday: string;
  age: number;
  office: string[];
  note?: string;
  namesake: boolean;
};

export default function ExamineeInfo({
  name,
  birthday,
  age,
  office,
  note,
  namesake,
}: ExamineeInfoProps) {
  return (
    <Box>
      <Group>
        <Title order={2} className={styles["text-wrap"]}>
          {name}
        </Title>
        {namesake && <Text>同性同名の受診者がいます</Text>}
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
        <Text className={styles["text-wrap"]}>
          {office.map((office) => {
            return office.concat("　");
          })}
        </Text>
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
        <Text className={styles["text-wrap"]}>{note}</Text>
      </Group>
    </Box>
  );
}
