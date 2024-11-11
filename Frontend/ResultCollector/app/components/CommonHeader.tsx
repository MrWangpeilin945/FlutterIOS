import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Button, Center, Flex, Title, Dialog } from "@mantine/core";
import { useDisclosure, useClickOutside } from "@mantine/hooks";
import { IconUserFilled, IconHomeFilled } from "@tabler/icons-react";
import styles from "~/styles/common.module.css";

type HeaderProps = {
  screenName: string;
  staffName: string;
};

export default function CommonHeader({ screenName, staffName }: HeaderProps) {
  const navigate = useNavigate();
  const [opened, { toggle, close }] = useDisclosure(false);

  // ダイアログ以外の部分がクリックされると非表示に
  const closeKeyBoard = useClickOutside(close);

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
          onClick={() => navigate("/")}
        >
          ホーム
        </Button>
      </Flex>
      <div ref={closeKeyBoard}>
        <Dialog
          opened={opened}
          title="ダイアログ"
          position={{ top: 50, left: 10 }}
          onClose={close}
          w={200}
        >
          {staffName}
          {/* ログアウトボタン */}
          {/* ToDo:ログアウト時の処理は未実装 */}
          <Button
            className={styles[""]}
            color="grey"
            variant="outline"
            w={150}
            onClick={() => navigate("/login")}
          >
            ログアウト
          </Button>
        </Dialog>
      </div>
    </Box>
  );
}
