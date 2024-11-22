import { Button, Group, Text, Title } from "@mantine/core";

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
        variant="outline"
        color={selected ? "lime.4" : "rgba(0, 0, 0, 1)"} // TODO：見出しボタンの背景色を変更
        radius="md"
        justify="flex-start"
        h={(elements ? elements.length + 1 : 1) * 30 + 50}
        m={5}
        py="xs"
        onClick={onClick}
      >
        <div>
          <Title order={1}>【{title}】</Title>
          {elements?.map((elem, index) => (
            <Text key={index} size="lg" fw={700}>
              {elem}
            </Text>
          ))}
        </div>
      </Button>
    </Group>
  );
}
