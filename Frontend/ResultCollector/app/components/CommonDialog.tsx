import { Button, Center, Modal, Title } from "@mantine/core";
import styles from "~/styles/common.module.css";

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
  return (
    <Modal
      size={500}
      radius="md"
      opened={isOpen}
      onClose={onClose}
      withCloseButton={false} // headerなくす
      closeOnClickOutside={false} // modalの外クリックしても消えないように
      centered
    >
      <Title
        className={styles["text-wrap"]}
        size="lg"
        maw={400} // 最大幅の制限
        mx="md"
        my="xl"
        order={2}
      >
        {message}
      </Title>
      <Center>
        <Button w={200} h={80} variant="filled" onClick={onClose} mt="xl">
          <Title order={2}>{buttonMessage}</Title>
        </Button>
      </Center>
    </Modal>
  );
}
