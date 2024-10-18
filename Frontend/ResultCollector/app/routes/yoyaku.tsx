import type React from "react";
import { useState } from "react";
import type { MetaFunction } from "@remix-run/node";
import { z } from "zod";
import { Group, Title, Box, Center } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import Header from "~/components/Header";
import Footer from "~/components/Footer";
import Keyboard from "~/components/SoftwareKeyboard";
import { ErrorModal } from "~/components/ErrorModal";

import styles from "~/styles/common.module.css";
import { useNavigate } from "@remix-run/react";

export const meta: MetaFunction = () => {
	return [{ title: "予約No.入力" }];
};

export default function yoyaku() {
	const navigate = useNavigate();
	const footerItems = ["健診メニュー", "進捗", "", ""];
	const [yoyakuNo, setYoyakuNo] = useState("");
	const [showKeyboard, setShowKeyboard] = useState(false);
	const [opened, { open, close }] = useDisclosure(false);
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	// キーボード以外の部分がクリックされると非表示に
	const ref = useClickOutside(() => setShowKeyboard(false));

	// バリデーションチェック
	const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const value = e.target.value;

		// 20文字以内
		if (value.length <= 20) {
			setYoyakuNo(value);
		}
	};

	// 確定処理関数（バリデーション）
	const reservationSchema = z.string()
		.min(1, "予約No.を入力してください。")
		.max(20, "予約No.は20文字以内で入力してください。")
		.regex(/^[a-zA-Z0-9]+$/, "予約No.は半角英数字で入力してください。");

	const handleConfirm = () => {
		console.log("確定処理実行: 現在の予約No.", yoyakuNo);

		// Zodでバリデーションチェック
		const result = reservationSchema.safeParse(yoyakuNo);

		if (!result.success) {
			setErrorMessage(result.error.errors[0].message); // 最初のエラーメッセージを取得
			open();
		} else {
			setErrorMessage(null);
			//APIで受診者情報取得
			navigate("/test-confirm");
		}
	};

	// Enterキーが押されたときの処理
	const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (e.key === "Enter") {
			handleConfirm();
		}
	};

	return (
		<div>
			<Header title="予約No.入力" />
			<Center>
				<Box w={1200} mt={20} mx={50}>
					<Group justify="space-between" m={10}>
						<Title order={1}>予約No.</Title>
						<Title order={2} fw={500}>
							Jotai
						</Title>
					</Group>
					<input
						className={styles["large-input"]}
						value={yoyakuNo}
						onFocus={() => setShowKeyboard(true)}
						onChange={(e) => handleInputChange(e)}
						onKeyDown={handleKeyDown}
					/>
				</Box>
			</Center>
			<Center>
				{showKeyboard && (
					<div ref={ref}>
						<Keyboard
							size={150}
							value={yoyakuNo}
							onChange={(e: string) => setYoyakuNo(e)}
							onConfirm={handleConfirm}
						/>
					</div>
				)}
			</Center>
			<ErrorModal isOpen={opened} onClose={close} errorMessage={errorMessage} />
			<Footer items={footerItems} />
		</div>
	);
}
