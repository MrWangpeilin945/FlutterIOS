import { Box, Title } from "@mantine/core";
import styles from "~/styles/common.module.css";

type personalProps = {
  name: string;
  age: number;
  birthday: string;
  office: string;
};

export default function PersonalInfo({
  name,
  age,
  birthday,
  office,
}: personalProps) {
  return (
    <Box px={15}>
      <Title my={15}>{name}</Title>
      <Title order={2} fw={400}>
        生年月日：{birthday} （{age}）
      </Title>
      <Title order={2} fw={400}>
        事業所：{office}
      </Title>
    </Box>
  );
}
