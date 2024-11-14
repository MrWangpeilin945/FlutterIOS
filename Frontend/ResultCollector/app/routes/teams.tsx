import type { MetaFunction } from "@remix-run/node";
import { useEffect, useState } from "react";
import { Container, Stack, Title } from "@mantine/core";
import { useNavigate } from "@remix-run/react";
import { useAtom } from "jotai";
import { teamState } from "~/store/store";
import { placeScheduleGetTeams } from "~/api/wellship";
import type { PlaceScheduleTeams } from "~/domain/wellship.schemas";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import Team from "~/components/Team";
import CommonHeader from "~/components/CommonHeader";
import CommonFooter from "~/components/CommonFooter";

import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
	return [{ title: "班選択" }];
};

export default function teams() {
	const navigate = useNavigate();
	const [teamsData, setTeamsData] = useState<PlaceScheduleTeams>();
	const [isLoading, setIsLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [team, setTeam] = useAtom(teamState);


	useEffect(() => {
		const fetchTeams = async () => {
			try {
				const result = await placeScheduleGetTeams("1", { date: "2024-09-27" });
				setTeamsData(result.data);
			} catch (err) {
				console.error(err);
				setError("Failed to fetch teams data");
			} finally {
				setIsLoading(false);
			}
		};
		fetchTeams();
	},[]);
	
	//
	if (isLoading) return <div>Loading...</div>;
	if (error) return <div>{error}</div>;

	//jotaiに班idと班名を保存して遷移
	const buttonClickEvent = (id?: number, name?: string) => {
		if (id && name) {
			const teamData = {id,name};
			setTeam(teamData);
			console.log(teamState);
		}
		navigate("/consultnumber-input")
		//エラー処理
	};

	if(!teamsData?.teams){
		return (
			<>
				<CommonHeader screenName="班選択" staffName="両備 太郎" />
				<Container fluid mt={20}>
					{/* エラーメッセージを表示 */}
					<Title order={3}>{getErrorMessage(errorMessages.noData, "該当する班")}</Title>
				</Container>
				<CommonFooter />
			</>
		);
	}

	if (teamsData?.teams) {
		return (
			<div>
				<CommonHeader screenName="班選択" staffName="両備 太郎" />

				{/* 班データをリストにして表示*/}
				<Container className={styles["footer-padding"]}fluid mt={20}>
					<Stack>
						{teamsData.teams.map((teams) => (
								// teamに班名と会場情報を渡す
								<Team
									key={teams.teamId}
									teamId={teams.teamId}
									teamName={teams.teamName}
									places={teams.places}
									onClick={() => buttonClickEvent(teams.teamId, teams.teamName)}
								/>
							))}
					</Stack>
				</Container>
				<CommonFooter />
			</div>
		);
	} 
}
