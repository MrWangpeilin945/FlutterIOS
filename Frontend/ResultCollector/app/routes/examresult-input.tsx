import { Button } from "@mantine/core";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { type QueryClient, useQueryClient } from "@tanstack/react-query";
import ExamineeHeader from "~/components/ExamineeHeader";

export const meta: MetaFunction = () => {
  return [{ title: "検査結果入力" }];
};

export default function testInput() {
  const navigate = useNavigate();
  // キャッシュから取得
  // const queryClient: QueryClient = useQueryClient();
  // const data = queryClient.getQueryData(["取り出したいqueryKey"]);
  const data = {
    id: 123456,
    name: "ケンコウ オトコ",
    gender: "m",
    age: 30,
  };
  return (
    <>
      <ExamineeHeader
        id={data.id}
        name={data.name}
        gender={data.gender}
        age={data.age}
      />
    </>
  );
}
