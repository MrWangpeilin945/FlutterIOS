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
      <Button variant="fill" onClick={() => navigate("/teams")}>
        班選択
      </Button>
      <Button variant="fill" onClick={() => navigate("/yoyaku")}>
        予約No
      </Button>
      <Button variant="fill" onClick={() => navigate("/progress")}>
        進捗
      </Button>
    </div>
  );
}
