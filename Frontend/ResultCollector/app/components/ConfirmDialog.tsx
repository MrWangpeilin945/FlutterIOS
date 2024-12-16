import {
  Button,
  Center,
  Group,
  Modal,
  Stack,
  Text,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";

interface ConfirmDialogProps {
  message: string;
  cancelButtonMessage?: string;
  confirmButtonMessage?: string;
  isOpen: boolean; // 表示・非表示用
  onCancel: () => void; // 取り消し
  onConfirm: () => void; // 実行
}

export default function ConfirmDialog({
  message,
  cancelButtonMessage,
  confirmButtonMessage,
  isOpen,
  onCancel,
  onConfirm,
}: ConfirmDialogProps) {
  cancelButtonMessage = cancelButtonMessage || "キャンセル";
  confirmButtonMessage = confirmButtonMessage || "OK";
  const theme = useMantineTheme();

  return (
    <Modal
      size={668}
      radius={16}
      opened={isOpen}
      onClose={onCancel}
      withCloseButton={false} // headerなくす
      closeOnClickOutside={false} // modalの外クリックしても消えないように
      centered
      styles={{
        body: {
          background: getThemeColor("white01", theme),
          padding: "64px 64px 32px",
        },
      }}
    >
      <Stack gap={64}>
        <Text
          size="lg"
          c="black01"
          style={{
            wordBreak: "break-word",
            whiteSpace: "pre-wrap",
          }}
        >
          {message}
        </Text>
        <Center>
          <Group gap={24} wrap="nowrap">
            <Button
              w={214}
              h={64}
              variant="outline"
              bg="white01"
              color="primary"
              py={16}
              px={32}
              style={{ borderWidth: 2 }}
              onClick={onCancel}
            >
              <Text size="lg" fw={700} c="primary">
                {cancelButtonMessage}
              </Text>
            </Button>
            <Button
              w={214}
              h={64}
              variant="filled"
              bg="primary"
              color="primary"
              py={16}
              px={32}
              style={{ borderWidth: 2 }}
              onClick={onConfirm}
            >
              <Text size="lg" fw={700} c="white01">
                {confirmButtonMessage}
              </Text>
            </Button>
          </Group>
        </Center>
      </Stack>
    </Modal>
  );
}
