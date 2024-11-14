import { Container } from "@mantine/core";
import type { MetaFunction } from "@remix-run/node";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import ProgressDisplay from "~/components/ProgressDisplay";

export const meta: MetaFunction = () => {
	return [{ title: "進捗" }];
};

export default function progress() {
	const data = {
		progressList: [
			{
				koumokuId:1,
				koumoku: "身長",
				examinee: [
					{ id: 1, name: "両備 太郎", status: "raijyo" },
					{ id: 2, name: "テスト テスト", status: "raijyo" },
					{ id: 3, name: "アニシ・フルドーガルベンゼウハンイタスルゲーレュシフルウ・イレブ・トーバー", status: "yotei" },
					{ id: 4, name: "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz", status: "completed" },
				],
			},
			{
				koumokuId:2,
				koumoku: "体重",
				examinee: [
					{ id: 1, name: "両備 太郎", status: "raijyo" },
					{ id: 2, name: "テスト テスト", status: "raijyo" },
					{ id: 3, name: "アニシ・フルドーガルベンゼウハンイタスルゲーレュシフルウ・イレブ・トーバー", status: "raijyo" },
					{ id: 4, name: "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz", status: "completed" },
				],
			},
		],
	};
	return (
		<>
			<CommonHeader screenName="進捗" staffName="両備 太郎" />
			<Container fluid>
				{data.progressList.map((data) => (
					<ProgressDisplay key={data.koumoku} progress={data} />
				))}
			</Container>
			<CommonFooter/>
		</>
	);
}