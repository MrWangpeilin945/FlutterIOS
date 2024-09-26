import React from "react";
import type { MetaFunction } from "@remix-run/node";
import { useState } from "react";
import { Group, Title, Box, Center, Container } from "@mantine/core";
import Header from "~/components/header";
import Footer from "~/components/footer";
import Keyboard from "~/components/SoftwareKeyboard";

import styles from "~/styles/yoyaku.module.css";


export const meta: MetaFunction = () => {
	return [{ title: "予約No.入力" }];
};

export default function han() {
	const footerItems = ["健診メニュー","進捗","",""];
	const [yoyakuNo, setYoyakuNo] = useState("");
	const [showKeyboard, setshowKeyboard] = useState(false);
	return (
		<div>
			<Header title="予約No.入力" />
			<Center>
				<Box className={styles.yoyakuBox}>
					<Group justify="space-between" m={10}>
						<Title order={1}>予約No.</Title>
						<Title order={2} fw={500}>
							Jotai
						</Title>
					</Group>
					<input
						className={styles.yoyakuInput}
						value={yoyakuNo}
						onFocus={() => setshowKeyboard(true)}
                        onBlur={() => setshowKeyboard(false)}
						onChange={(e) => setYoyakuNo(e.target.value)}
					/>
				</Box>
			</Center>
			<Center>
				{showKeyboard && (
					<Keyboard
						value={yoyakuNo}
						onChange={(e: string) => {
							setYoyakuNo(e);
						}}
					/>
				)}
			</Center>
			<Footer items={footerItems} />
		</div>
	);
}
