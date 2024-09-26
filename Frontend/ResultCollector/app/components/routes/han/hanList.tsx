import { EventHandler, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import Han from "~/components/routes/han/han";
import { Container, Group, Button, Center } from "@mantine/core";

// const fetchData = async () => {
// 	const response = await axios.get(
// 		"https://jsonplaceholder.typicode.com/posts",
// 	);
//     const jsonString = JSON.stringify(response.data);
// 	return jsonString;
// };

export default function HanList() {
	// const [errorMessage, setErrorMessage] = useState("");
	// const { data, error, isLoading } = useQuery({
	// 	queryKey: ["hans"], // キャッシュのためのキー
	// 	queryFn: fetchData,
	// });
    
	// if (isLoading) return <div>Loading...</div>;
	// if (error instanceof Error) return <div>Error: {error.message}</div>;
	const data = [
		{
			班: "A班",
			会場: [
				{
					会場名: "健康センターA1",
					住所: "東京都新宿区1-1-1",
					日付: "2024-10-01",
					時間: "09:00 - 12:00",
				},
				{
					会場名: "健康センターA2",
					住所: "東京都新宿区1-2-2",
					日付: "2024-10-02",
					時間: "13:00 - 16:00",
				},
			],
		},
		{
			班: "B班",
			会場: [
				{
					会場名: "健康センターB1",
					住所: "東京都渋谷区2-2-2",
					日付: "2024-10-03",
					時間: "09:00 - 12:00",
				},
				{
					会場名: "健康センターB2",
					住所: "東京都渋谷区2-3-3",
					日付: "2024-10-04",
					時間: "13:00 - 16:00",
				},
			],
		},
	];

	//要素一つずつ取り出して、hanに渡す
	return (
		<Container fluid m={15}>
			{data.map((hanData) => (
				<Han key={hanData.班} hanName={hanData.班} kaijyou={hanData.会場} /> // Hanに班名と会場情報を渡す
			))}
		</Container>
	);
}
