import {
  Button,
  Center,
  Flex,
  Modal,
  Stack,
  Text,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";
import {
  IconAlertTriangleFilled,
  IconMessageFilled,
  IconCircleXFilled,
} from "@tabler/icons-react";
import { IconType } from "~/domain/enums";
import styles from "~/styles/common.module.css";

interface CommonDialogProps {
  message: string;
  buttonMessage?: string;
  iconType?: string;
  isOpen: boolean; // 表示・非表示用
  onClose: () => void; // 閉じる
}
export default function CommonDialog({
  message,
  buttonMessage,
  iconType,
  isOpen,
  onClose,
}: CommonDialogProps) {
  buttonMessage = buttonMessage || "OK";
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
        <Flex
          mih={50}
          gap="md"
          justify="center"
          align="center"
          direction="row"
          c={
            iconType === IconType.未設定
              ? undefined
              : iconType === IconType.正常
                ? "blue04"
                : iconType === IconType.警告
                  ? "warning"
                  : iconType === IconType.異常
                    ? "error"
                    : undefined
          }
        >
          {iconType !== IconType.未設定 &&
            (iconType === IconType.正常 ? (
              <IconMessageFilled size={96} />
            ) : iconType === IconType.警告 ? (
              <IconAlertTriangleFilled size={112} />
            ) : iconType === IconType.異常 ? (
              <IconCircleXFilled size={112} />
            ) : undefined)}
          <Text size="lg" c="black01" className={styles["text-wrap"]}>
            {message}
          </Text>
        </Flex>
        <Center>
          <Button
            w={214}
            h={75}
            variant="filled"
            bg="primary"
            color="primary"
            py={16}
            px={32}
            bd="2px solid"
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
