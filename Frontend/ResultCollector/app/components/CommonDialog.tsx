import {
  Button,
  Center,
  Modal,
  Stack,
  Text,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";

interface CommonDialogProps {
  message: string;
  buttonMessage: string;
  isOpen: boolean; // 表示・非表示用
  onClose: () => void; // 閉じる
}
export default function CommonDialog({
  message,
  buttonMessage,
  isOpen,
  onClose,
}: CommonDialogProps) {
  const theme = useMantineTheme();

  return (
    <Modal
      radius="16"
      size={668}
      opened={isOpen}
      onClose={onClose}
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
          <Button
            w={214}
            h={64}
            variant="filled"
            bg="primary"
            color="primary"
            py={16}
            px={32}
            style={{ borderWidth: 2 }}
            onClick={onClose}
          >
            <Text size="lg" fw={700} c="white01">
              {buttonMessage}
            </Text>
          </Button>
        </Center>
      </Stack>
    </Modal>
  );
}
