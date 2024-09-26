import { Button } from "@mantine/core";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "react-router-dom";

export const meta: MetaFunction = () => {
  return [
    { title: "結果収集" },
    { name: "description", content: "" },
  ];
};

export default function Index() {
  const navigate = useNavigate()
  return (
    <div>
      <Button
        variant="fill"
        onClick={() => navigate("/selectHan")}
      >
        班選択
      </Button>
      <Button
        variant="fill"
        onClick={() => navigate("/yoyaku")}
      >
        予約No
      </Button>
    </div>
  );
}
