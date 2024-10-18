import { Box, Button, Center, Flex, Grid, GridCol, Title } from "@mantine/core";
import { useNavigate } from "react-router-dom";
import styles from "~/styles/common.module.css";

type HeaderProps = {
	id: number;
	name: string;
	gender:string;
	age: number;
};

export default function ExamineeHeader({ id, name, gender, age }: HeaderProps) {
	const navigate = useNavigate();
	return (
		<Box className={styles["basic-blue"]} py="7">
			<header>
				<Grid justify="space-between" align="center" px="md" gutter={0}>
					{/* 管理番号 */}
					<GridCol span="content" offset={2} >
						<Title h={40}  order={2} fw={500} bg="rgba(29, 163, 132, 1)" px={10}>{id}</Title>
					</GridCol>
					{/* 受診者名 */}
					<GridCol span={4}><Title fw={500}><Center>{name}({age})</Center></Title></GridCol>
					{/* ホームボタン */}
					<GridCol span="content" offset={2}>
					{/* ホームボタン */}
                    <Button
                        w={80}
                        h={40}
                        variant="fill"
                        bg="#396B9E"
                        onClick={() => navigate("/")}
                    >
                        ホーム
                    </Button>
					</GridCol>
				</Grid>
			</header>
		</Box>
	);
}
