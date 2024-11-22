import { Button, Center, Modal, Title } from "@mantine/core";
import styles from "~/styles/common.module.css";

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

  return (
    <Modal
      size={500}
      radius="md"
      opened={isOpen}
      onClose={onCancel}
      withCloseButton={false} // headerなくす
      closeOnClickOutside={false} // modalの外クリックしても消えないように
      centered
    >
      <Title
        className={`${styles["text-wrap"]} ${styles["white-wrap"]}`}
        size="lg"
        maw={400} // 最大幅の制限
        mx="md"
        my="xl"
        order={2}
      >
        {message}
      </Title>
      <Center>
        <Button w={200} h={80} variant="outline" onClick={onCancel} mt="xl">
          <Title order={2}>{cancelButtonMessage}</Title>
        </Button>
        <Button w={200} h={80} variant="filled" onClick={onConfirm} mt="xl">
          <Title order={2}>{confirmButtonMessage}</Title>
        </Button>
      </Center>
    </Modal>
  );
}
