import { Box, Button, Progress, Title, Text } from "@mantine/core";
import styles from "~/styles/common.module.css";

type progressData = {
	progress: {
		koumoku: string;
		examinee: {
			id: number;
			name: string;
			status: string;
		}[];
	};
};

type StatusCount = {
	[key: string]: number;
};

export default function ProgressDisplay({ progress }: progressData) {
	const statusCount = progress.examinee.reduce<StatusCount>((acc, examinee) => {
		acc[examinee.status] = (acc[examinee.status] || 0) + 1;
		return acc;
	}, {});

	console.log(statusCount);
	const data = {
		raijyo: statusCount.raijyo || 0, // 'raijyo'のカウント
		yotei: statusCount.yotei || 0, // 'yotei'のカウント
		completed: statusCount.completed || 0, // 'completed'のカウント
	};
	console.log(data);
	// 合計値を算出
	const total = data.completed + data.raijyo + data.yotei;

	// 各項目の割合を算出
	const sections = [
		{
			value: (data.completed / total) * 100,
			color: "rgb(143, 196, 96)",
			label: "completed",
		},
		{ value: (data.raijyo / total) * 100, color: "gray", label: "raijyo" },
		{ value: (data.yotei / total) * 100, color: "black", label: "yotei" },
	];

	return (
		<Box className={styles.border}>
			<Title
				h={40}
				order={3}
				className={styles["basic-blue"]}
				fw={550}
				pl={20}
				py={3}
			>
				{progress.koumoku}
			</Title>
			<Box p={10}>
				<table>
					<tbody>
						<tr>
							<td>【予定】</td>
							<td>
								<Button
									className={styles["progress-yotei"]}
									size="xs"
									radius="md"
									px={5}
								>
									<Text size="lg">来場</Text>
								</Button>
							</td>
							<td className={styles["progress-td"]}>{data.raijyo}</td>
							<td>
								<Button
									className={styles["progress-raijyo"]}
									size="xs"
									radius="md"
									px={5}
								>
									<Text size="lg">予定</Text>
								</Button>
							</td>
							<td className={styles["progress-td"]}>{data.yotei}</td>
						</tr>
						<tr>
							<td>【受診】</td>
							<td>
								<Button
									className={styles["progress-completed"]}
									size="xs"
									radius="md"
									px={5}
								>
									<Text size="lg">済</Text>
								</Button>
							</td>
							<td className={styles["progress-td"]}>{data.completed}</td>
						</tr>
					</tbody>
				</table>
				<Progress.Root size="lg" mt={10}>
					{sections.map((sections) => (
						<Progress.Section
							key={sections.label}
							value={sections.value}
							color={sections.color}
						/>
					))}
				</Progress.Root>
			</Box>
		</Box>
	);
}
