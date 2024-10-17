import { Box, Button, Grid, GridCol } from "@mantine/core";
import { useNavigate } from "@remix-run/react";

import styles from "~/styles/common.module.css";

type footerProps = {
	items: string[];
};

export default function Footer({ items }: footerProps) {
	const navigate = useNavigate();
	return (
		<>
			{/*横並びにボタンを4つ配置*/}
			<Box h={60} className={styles.footer}>
				<Grid className={styles.footer}>
					<GridCol span={3} p={0}>
						<Button
							className={styles["footer-button-text"]}
							fullWidth
							h={70}
							variant="outline"
							color="rgba(255, 255, 255, 1)"
							radius="0"
							onClick={() => navigate("")}
						>
							{items[0]}
						</Button>
					</GridCol>
					<GridCol span={3} p={0}>
						<Button
							className={styles["footer-button-text"]}
							fullWidth
							h={70}
							variant="outline"
							color="rgba(255, 255, 255, 1)"
							radius="0"
							onClick={() => navigate("")}
						>
							{items[1]}
						</Button>
					</GridCol>
					<GridCol span={3} p={0}>
						<Button
							className={styles["footer-button-text"]}
							fullWidth
							h={70}
							variant="outline"
							color="rgba(255, 255, 255, 1)"
							radius="0"
							onClick={() => navigate("")}
						>
							{items[2]}
						</Button>
					</GridCol>
					<GridCol span={3} p={0}>
						<Button
							className={styles["footer-button-text"]}
							fullWidth
							h={70}
							variant="outline"
							color="rgba(255, 255, 255, 1)"
							radius="0"
							onClick={() => navigate("")}
						>
							{items[3]}
						</Button>
					</GridCol>
				</Grid>
			</Box>
		</>
	);
}
