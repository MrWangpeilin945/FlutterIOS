import { Button } from "@mantine/core";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";

export const meta: MetaFunction = () => {
  return [{ title: "結果収集" }];
};

export default function Index() {
  const navigate = useNavigate();
  return (
    <div>
      <Button variant="outline" onClick={() => navigate("/teams")}>
        班選択
      </Button>
      <Button onClick={() => navigate("/consultnumber-input")}>
        受診番号入力
      </Button>
      <Button onClick={() => navigate("/progress")}>進捗</Button>
      <Button onClick={() => navigate("/consult-input/1")}>検査結果入力</Button>
    </div>
  );
}
