import { Button, Group, Text, Title } from "@mantine/core";


type Venue = {
	会場名: string;
};

type HanProps = {
	hanName: string;
	kaijyou: Venue[];
};

export default function Han(props: HanProps) {
	const { hanName, kaijyou } = props;

	return (
		<Group justify="flex-start">
			<Button
				fullWidth
				variant="outline"
				color="rgba(0, 0, 0, 1)"
				radius="md"
				justify="flex-start"
				h={90}
				m={5}
			>
				<div>
					<Title order={2} maw="md">
						【{hanName}】
					</Title>
					<Group align="left" mt={10}>
						{kaijyou.map((venue: Venue) => (
							<Text key={venue.会場名} size="md" fw={700}>
								{venue.会場名}
							</Text>
						))}
					</Group>
				</div>
			</Button>
		</Group>
	);
}
