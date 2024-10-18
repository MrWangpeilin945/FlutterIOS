import { Modal, Title, Button, Center } from "@mantine/core";
import styles from "~/styles/common.module.css";

interface ErrorModalProps {
	isOpen: boolean; //表示・非表示用
	onClose: () => void; //閉じる
	errorMessage: string | null;
}

export function ErrorModal({ isOpen, onClose, errorMessage }: ErrorModalProps) {
	return (
		<Modal
			size={500}
			radius="md"
			opened={isOpen}
			onClose={onClose}
			withCloseButton={false} //headerなくす
			closeOnClickOutside={false} //modalの外クリックしても消えないように
			centered
		>
			<Title
				className={styles["text-wrap"]}
				maw={400} // 最大幅の制限
				mx="md"
				my="xl"
				order={2}
			>
				{errorMessage}
			</Title>
			<Center>
				<Button
					w={200}
					h={80}
					variant="outline"
					color="black"
					bg="rgb(175, 245, 152)"
					onClick={onClose}
					mt="xl"
				>
					<Title order={2}>閉じる</Title>
				</Button>
			</Center>
		</Modal>
	);
}
