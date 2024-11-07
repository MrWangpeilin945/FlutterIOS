import type React from "react";
import { useState } from "react";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { z } from "zod";
import { useAtom } from "jotai";
import { teamAtom } from "~/store/store";
import { Group, Title, Box, Center } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { consultVerifyConsultNumber } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonHeader from "~/components/CommonHeader";
import CommonFooter from "~/components/CommonFooter";
import Keyboard from "~/components/SoftwareKeyboard";
import { ErrorModal } from "~/components/ErrorModal";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { createNavigate } from "~/utils/screenMove";
import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
	return [{ title: "受診番号入力" }];
};

export default function consultNumberInput() {
	const [consultNo, setConsultNo] = useState("");
	const [teamData, setTeamData] = useAtom(teamAtom);
	const [showKeyboard, setShowKeyboard] = useState(false);
	const [opened, { open, close }] = useDisclosure(false);
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	//【CP0002】共通フッター設定
	const navigate = useNavigate();
	const SC = createNavigate(navigate);
	const footerItems = [
		{ label: "検査メニュー", action: SC.navigateExamMenu },
		{ label: "進捗", action: SC.navigateProgress },
		{ label: "", action: () => {} },
		{ label: "", action: () => {} },
	];

	// キーボード以外の部分がクリックされると非表示に
	const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));

	// テキストボックスのバリデーションチェック
	const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const value = e.target.value;

		// 20文字以内
		if (value.length <= 20) {
			setConsultNo(value);
		}
	};

	// バリデーションチェック
	const validationCheck = () => {
		// バリデーションチェックスキーマ
		//ここも共通化できるかも？
		const validationSchema = z
			.string()
			.min(1, getErrorMessage(errorMessages.required, "受診番号は"))
			.max(20, getErrorMessage(errorMessages.maxLength, "受診番号は", 20))
			.regex(
				/^[a-zA-Z0-9]+$/,
				getErrorMessage(errorMessages.alphaNumericString, "受診番号は"),
			);
		const result = validationSchema.safeParse(consultNo);
		if (!result.success) {
			setErrorMessage(result.error.errors[0].message);
			open();
		}
	};

	//AP1007_受診番号を検証する
	const verifyConsultNo = async () => {
		await consultVerifyConsultNumber("1", { consultNumber: consultNo })
			.then(() => {
				// 成功時の処理
				//TODO：【SC0007】検査内容確認画面のパスパラメータに設定する処理
			})
			.catch((error) => {
				// エラー時の処理
				setErrorMessage(error);
				open();
			});
	};

	// Enterキーが押されたときの処理
	const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (e.key === "Enter") {
			handleConfirm();
		}
	};

	// 確定処理
	const handleConfirm = () => {
		try {
			validationCheck();
			verifyConsultNo();
			// 全てのチェックが通ったら次の画面に遷移
			navigate("/examorder-confirm");
		} catch (error) {
			console.error("受診番号の検証中にエラーが発生しました:", error);
		}
	};

	return (
		<>
			<AuthWrapper>
				<CommonHeader screenName="受診番号入力" buttonType="1" />
				<Center>
					<Box w={1200} mt={20} mx={50}>
						<Group justify="space-between" m={10}>
							<Title order={1}>受診番号</Title>
							<Title order={2} fw={500}>
								{teamData?.name}
							</Title>
						</Group>
						<input
							className={styles["large-input"]}
							value={consultNo}
							onFocus={() => setShowKeyboard(true)}
							onChange={(e) => handleInputChange(e)}
							onKeyDown={handleKeyDown}
						/>
					</Box>
				</Center>
				{showKeyboard && (
					<Center>
						<div ref={closeKeyBoard}>
							<Keyboard
								size={150}
								value={consultNo}
								onChange={(e: string) => setConsultNo(e)}
								onConfirm={handleConfirm}
							/>
						</div>
					</Center>
				)}
				<ErrorModal
					isOpen={opened}
					onClose={close}
					errorMessage={errorMessage}
				/>
				<CommonFooter items={footerItems} />
			</AuthWrapper>
		</>
	);
}
