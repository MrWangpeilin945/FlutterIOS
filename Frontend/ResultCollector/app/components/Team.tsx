import { Button, Group, Text, Title } from "@mantine/core";


type TeamProps = {
	teamName?: string;
	places?: {
		placeId?: number;
		placeName?: string;
	}[];
};

export default function team(team: TeamProps) {
	const { teamName, places } = team;
    if(places === undefined ) return;
	return (
        //班名と会場名を表示したボタンを作成
		<Group >
			<Button
				fullWidth
				variant="outline"
				color="rgba(0, 0, 0, 1)"
				radius="md"
				justify="flex-start"
				h={((places.length + 1) * 30) + 25}
				m={5}
                py="xs"
			>
				<div>
					<Title order={2} >
						【{teamName}】
					</Title>
                    {/* 会場を1つずつ取り出して配置 */}
						{places.map((kaijyou) => (
							<Text key={kaijyou.placeId} size="lg" fw={700}>
								{kaijyou.placeName}
							</Text>
						))}
				</div>
			</Button>
		</Group>
	);
}
