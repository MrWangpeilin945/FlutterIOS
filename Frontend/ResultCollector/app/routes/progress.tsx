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
					{ id: 1, name: "丸々 盛盛", status: "raijyo" },
					{ id: 2, name: "不思議 種", status: "raijyo" },
					{ id: 3, name: "銭 亀", status: "yotei" },
					{ id: 4, name: "ヒト カゲ", status: "completed" },
					{ id: 5, name: "令和 五右衛門", status: "completed" },
					{ id: 6, name: "MukiMuki Mussle", status: "completed" },
				],
			},
			{
				koumokuId:2,
				koumoku: "体重",
				examinee: [
					{ id: 1, name: "丸々 盛盛", status: "completed"},
					{ id: 2, name: "不思議 種", status: "raijyo" },
					{ id: 3, name: "ヒト カゲ", status: "raijyo" },
					{ id: 4, name: "令和 五右衛門", status: "raijyo" },
					{ id: 5, name: "MukiMuki Mussle", status: "raijyo" },
				],
			},
		],
	};
	return (
		<>
			<CommonHeader screenName="進捗" buttonType="1"/>
			<Container fluid>
				{data.progressList.map((data) => (
					<ProgressDisplay key={data.koumoku} progress={data} />
				))}
			</Container>
			<CommonFooter/>
		</>
	);
}
