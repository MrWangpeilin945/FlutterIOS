import { Box, Button, Grid, GridCol } from "@mantine/core";
import { useNavigate } from "react-router-dom";
import type { LinksFunction } from "@remix-run/node";

import styles from "~/styles/footer.module.css";

type footerProps = {
	items: string[]; // titleをpropsとして受け取る
};

export default function footer({ items }: footerProps) {
	const navigate = useNavigate();
	return (
		<Box h={60} bg="#396B9E" className={styles.footer}>
			{/*ボタンの文字大きくしたい！！*/}
			<Grid className={styles.footer}>
				<GridCol span={3} p={0}>
					<Button
						className={styles.ButtonText}
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
						className={styles.ButtonText}
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
						className={styles.ButtonText}
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
						className={styles.ButtonText}
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
	);
}
