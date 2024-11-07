import { Button } from "@mantine/core";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import ExamineeHeader from "~/components/ExamineeHeader";
import CommonFooter from "~/components/CommonFooter";
import PersonalInfo from "~/components/PersonalInfo";
import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
	return [{ title: "検査内容確認" }];
};

export default function examOrderConfirm() {
	const navigate = useNavigate();
	//api共通化（orval)
	//tanstackqueryでキャッシュ
	const data = {
		id: 123456,
		name: "健康 男",
		kanaName: "ケンコウ オトコ",
		gender: "m",
		age: 30,
		birthDay:"2024-01-21",
		office:"マレーシア クアラルンプール"
	};
	return (
		<>
			<ExamineeHeader
				id={data.id}
				name={data.kanaName}
				gender={data.gender}
				age={data.age}

			/>
			<PersonalInfo name={data.name} age={data.age} birthday={data.birthDay} office={data.office}/>
			<hr className={styles.hr} />
			<Button fullWidth onClick={() => navigate("/exam-input")}>
				計測開始
			</Button>
            <CommonFooter items={undefined} />
		</>
	);
}
