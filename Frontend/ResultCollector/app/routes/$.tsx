import { Button, Center, Image, Space, Text } from "@mantine/core";
import { useNavigate } from "react-router";
import type { MetaFunction } from "react-router";
import { authUtil } from "~/utils/authUtil";

export const meta: MetaFunction = () => {
  return [{ title: "お探しのページは見つかりませんでした" }];
};

export default function NotFound404() {
  const navigate = useNavigate();

  return (
    <>
      <Center style={{ flexDirection: "column" }}>
        <Space h={144} />
        <Image src="./wellship-404.svg" w={484.93} />
        <Space h={32} />
        <Text size="lg">Not Found</Text>
        <Space h={32} />
        <Text size="xl" fw={700}>
          お探しのページは見つかりませんでした。
        </Text>
        <Space h={64} />
        <Button
          size="xl"
          w={924}
          h={78}
          onClick={() => authUtil.logout(() => navigate("/login"))}
        >
          ログイン画面へ
        </Button>
      </Center>
    </>
  );
}
