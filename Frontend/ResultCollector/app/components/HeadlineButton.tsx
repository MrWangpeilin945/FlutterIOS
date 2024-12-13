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
        style={{ borderWidth: 2 }}
        radius={24}
        bg={selected ? "green03" : "white01"}
        color={selected ? "primary" : "gray03"}
        pt={16}
        pb={16}
        pl={32}
        pr={32}
        variant="outline"
        justify="flex-start"
        onClick={onClick}
      >
        <Box>
          <Text
            className={styles["text-multiline"]}
            size="md"
            fw={700}
            c={selected ? "primary" : "black01"}
            ta="left"
          >
            {title}
          </Text>
          {elements?.map((elem, index) => (
            <Text
              className={styles["text-multiline"]}
              key={index}
              size="sm"
              c={selected ? "primary" : "black01"}
              pt={index === 0 ? 16 : 8}
              ta="left"
            >
              {elem}
            </Text>
          ))}
        </Box>
      </Button>
    </Group>
  );
}
