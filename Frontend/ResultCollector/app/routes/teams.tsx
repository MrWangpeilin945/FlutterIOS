import type { MetaFunction } from "@remix-run/node";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { Container, Stack, Title } from "@mantine/core";
import type { PlaceScheduleTeams } from "~/api/models/teamItems";
import config from "~/domain/config.json";
import Header from "~/components/Header";
import Team from "~/components/Team";
import { getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
	return [{ title: "班選択" }];
};

export default function teams() {
	const baseURL = config.baseURL;
	const date = config.date;

	// // APIから班データを取得する
	// const fetchTeams = async (): Promise<PlaceScheduleTeams> => {
	// 	const { data } = await axios.get(`${baseURL}/placeSchedules/teams?date=${date}`);
	// 	return data;
	// };

	// // TanStack Queryを使用してAPIデータを取得
	// // queryKey: キャッシュのキー（"teams"）
	// // queryFn: データを取得する関数（fetchTeams）
	// const { data, isLoading, error } = useQuery({
	// 	queryKey: ["teams"],
	// 	queryFn: fetchTeams,
	// });

	// if(data === undefined){
	//     return;
	// }

	// // ローディング中の表示
	// if (isLoading) return <div>Loading...</div>;

	// // エラーが発生した場合の処理
	// let errorMessage = "";
	// if (error) {
	//     errorMessage = getErrorMessage(error);
	// }
	// if (!data || (data.teams && data.teams.length === 0)) {
	//     errorMessage = "班情報データが０件でした。";
	// }

	const teamsData = {
		teams: [
			{
				teamId: 1,
				teamName: "1班",
				places: [
					{
						placeId: 1,
						placeName: "会場A",
					},
					{
						placeId: 2,
						placeName: "会場B",
					},
					{
						placeId: 3,
						placeName: "会場A",
					},
					{
						placeId: 4,
						placeName: "会場A",
					},
					{
						placeId: 5,
						placeName: "会場A",
					},
					{
						placeId: 6,
						placeName: "会場A",
					},
				],
			},
			{
				teamId: 200,
				teamName: "10班",
				places: [
					{
						placeId: 4,
						placeName: "会場F",
					},
				],
			},
		],
	};

	return (
		<div>
			<Header title="班選択" />

			{/* 班データをリストにして表示*/}
			<Container fluid mt={20}>
				<Stack>
					{teamsData.teams.map((teams) => (
						// teamに班名と会場情報を渡す
						<Team
							key={teams.teamId}
							teamName={teams.teamName}
							places={teams.places}
						/> 
					))}
				</Stack>
			</Container>

			{/* エラーメッセージを表示 */}
			{/* <Title order={3}>{errorMessage}</Title> */}
		</div>
	);
}
