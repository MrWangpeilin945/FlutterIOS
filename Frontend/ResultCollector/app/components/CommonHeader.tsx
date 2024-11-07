import { Box, Button, Center, Flex, Title } from "@mantine/core";
import { useNavigate } from "react-router-dom";
import styles from "~/styles/common.module.css";

type HeaderProps = {
	screenName: string;
	buttonType: string;
};

export default function CommonHeader({ screenName, buttonType }: HeaderProps) {
	const navigate = useNavigate();
	const getButtonConfig = () => {
		switch (buttonType) {
			case "1":
				return {
					label: "ホーム",
					action: () => navigate("/"),
				};
			case "2":
				return {
					label: "戻る",
					action: () => navigate("/login"),
				};
			default:
				return {
					label: "ホーム",
					action: () => navigate("/home"),
				};
		}
	};

	const { label, action } = getButtonConfig();
	return (
		<Box className={styles["basic-blue"]} py="7">
			<header>
				<Flex justify="space-between" align="center" px="md">
					<Box w={60} />

					{/* 班選択 */}
					<Center>
						<Title order={1} c="white" fw={500}>
							{screenName}
						</Title>
					</Center>

					{/* ホームボタン */}
					<Button w={80} h={40} variant="fill" bg="#396B9E" onClick={action}>
						{label}
					</Button>
				</Flex>
			</header>
		</Box>
	);
}
