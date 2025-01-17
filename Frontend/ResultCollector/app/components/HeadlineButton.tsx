import { Box, Button, Group, Text } from "@mantine/core";
import styles from "~/styles/common.module.css";
interface HeadlineButtonProps {
  title: string;
  elements?: string[];
  selected: boolean;
  onClick: () => void;
}

export default function HeadlineButton({
  title,
  elements,
  selected,
  onClick,
}: HeadlineButtonProps) {
  return (
    <Group>
      <Button
        fullWidth
        h="auto"
        bd="2px solid"
        radius={24}
        bg={selected ? "green03" : "white01"}
        color={selected ? "primary" : "gray03"}
        py={16}
        px={32}
        variant="outline"
        justify="flex-start"
        onClick={onClick}
      >
        <Box>
          <Text
            size="md"
            fw={700}
            c={selected ? "primary" : "black01"}
            ta="left"
            className={styles["text-wrap"]}
          >
            {title}
          </Text>
          {elements?.map((elem, index) => (
            <Text
              key={index}
              size="sm"
              c={selected ? "primary" : "black01"}
              pt={index === 0 ? 16 : 8}
              ta="left"
              className={styles["text-wrap"]}
            >
              {elem}
            </Text>
          ))}
        </Box>
      </Button>
    </Group>
  );
}
