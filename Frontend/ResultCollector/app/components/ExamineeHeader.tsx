import { Box, Button, Dialog, Flex, Title } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { useNavigate } from "@remix-run/react";
import { IconHomeFilled, IconUserFilled } from "@tabler/icons-react";

type HeaderProps = {
  staffName: string;
  managerId: number;
  name: string;
  age: number;
  gender: number;
};

export default function ExamineeHeader({
  staffName,
  managerId,
  name,
  age,
  gender,
}: HeaderProps) {
  const navigate = useNavigate();
  const [opened, { toggle, close: hide }] = useDisclosure(false);

  // ダイアログ以外の部分がクリックされると非表示に
  const closeMenu = useClickOutside(hide);

  return (
    <Box bg={gender === 1 ? "malePrimary" : "femalePrimary"} py="10">
      <Flex justify="space-between" align="center" px="md">
        {/* ユーザアイコン */}
        <Button
          bg="white"
          c={gender === 1 ? "maleSecondary" : "femaleSecondary"}
          onClick={toggle}
        >
          <IconUserFilled size={"2.3rem"} />
        </Button>
        {/* 受診番号、受診者名、年齢 */}
        <Title fw={500}>
          {managerId} {name}({age})
        </Title>
        {/* ホームボタン */}
        <Button
          bg="white"
          c={gender === 1 ? "maleSecondary" : "femaleSecondary"}
          leftSection={<IconHomeFilled size={"1.7rem"} />}
          w={150}
          onClick={() => navigate("/")}
        >
          ホーム
        </Button>
      </Flex>
      <div ref={closeMenu}>
        <Dialog
          opened={opened}
          title="ダイアログ"
          position={{ top: 50, left: 10 }}
          onClose={hide}
          w={200}
        >
          {staffName}
          {/* ログアウトボタン */}
          {/* ToDo:ログアウト時の処理は未実装 */}
          <Button
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
