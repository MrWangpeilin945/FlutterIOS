import { Button, Group, Text, Title } from "@mantine/core";
import styles from "~/styles/common.module.css";

type TeamProps = {
  teamId?: number;
  teamName?: string;
  places?: {
    placeId?: number;
    placeName?: string;
  }[];
  onClick: () => void;
};

export default function Team({
  teamName = "",
  places = [],
  onClick,
}: TeamProps) {
  if (places.length === 0) return null;

  return (
    <Group>
      <Button
        fullWidth
        variant="outline"
        color="rgba(0, 0, 0, 1)"
        radius="md"
        justify="flex-start"
        h={(places.length + 1) * 30 + 50}
        m={5}
        py="xs"
        onClick={onClick}
      >
        <div>
          <Title className={styles["text-left"]} order={1}>
            【{teamName}】
          </Title>
          {places.map((kaijyou) => (
            <Text
              className={styles["button-text"]}
              key={kaijyou.placeId}
              w={1000}
              size="lg"
              fw={700}
            >
              {kaijyou.placeName}
            </Text>
          ))}
        </div>
      </Button>
    </Group>
  );
}
