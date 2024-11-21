import { Box, Button, Center, Dialog, Flex, Title } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { useNavigate } from "@remix-run/react";
import { IconHomeFilled, IconUserFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type HeaderProps = {
  screenName: string;
  staffName: string;
};

export default function CommonHeader({ screenName, staffName }: HeaderProps) {
  const navigate = useNavigate();
  const [opened, { toggle, close: hide }] = useDisclosure(false);

  // ダイアログ以外の部分がクリックされると非表示に
  const closeMenu = useClickOutside(hide);

  return (
    <Box className={styles["basic-green"]} py="15">
      <Flex justify="space-between" align="center" px="md">
        {/* ユーザアイコン */}
        <Button className={styles["reverse-green-button"]} onClick={toggle}>
          <IconUserFilled size={"2.3rem"} />
        </Button>

        {/* 画面名 */}
        <Center>
          <Title order={1} c="white" fw={550}>
            {screenName}
          </Title>
        </Center>

        {/* ホームボタン */}
        <Button
          className={styles["reverse-green-button"]}
          leftSection={<IconHomeFilled size={"1.7rem"} />}
          w={150}
          onClick={() => navigate("/home")}
        >
          ホーム
        </Button>
      </Flex>
      <div ref={closeMenu}>
        <Dialog
          opened={opened}
          title=""
          position={{ top: 50, left: 10 }}
          onClose={hide}
          w={200}
        >
          {staffName}
          {/* ログアウトボタン */}
          <Button
            className={styles[""]}
            color="grey"
            variant="outline"
            w={150}
            radius="ms"
            onClick={() => navigate("/login")}
          >
            ログアウト
          </Button>
        </Dialog>
      </div>
    </Box>
  );
}
