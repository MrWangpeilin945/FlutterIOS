import { Button, Dialog, Flex, Text } from "@mantine/core";
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

  const displayableName = name.length > 13 ? `${name.slice(0, 13)}…` : name;

  return (
    <>
      <Flex
        h={67}
        bg={gender === 1 ? "malePrimary" : "femalePrimary"}
        justify="space-between"
        align="center"
        px={24}
        py={8}
        pos="sticky"
        top={0}
        w="100%"
        style={{ zIndex: 10 }}
      >
        {/* ユーザアイコン */}
        <Button
          w={88}
          h={40}
          bg="white"
          c={gender === 1 ? "maleSecondary" : "femaleSecondary"}
          onClick={toggle}
        >
          <IconUserFilled size={"24px"} />
        </Button>
        {/* 受付番号、受診者名、年齢 */}
        <Text size="lg" fw={700}>
          {managerId} {displayableName}({age})
        </Text>
        {/* ホームボタン */}
        <Button
          w={170}
          h={51}
          bg="white"
          size="xs"
          fw={700}
          c={gender === 1 ? "maleSecondary" : "femaleSecondary"}
          leftSection={<IconHomeFilled size={"24px"} />}
          onClick={() => navigate("/home")}
        >
          ホーム
        </Button>
      </Flex>
      <div ref={closeMenu}>
        <Dialog
          opened={opened}
          title="ダイアログ"
          position={{ top: 67 }}
          onClose={hide}
          w="auto"
          radius="md"
          p={0}
        >
          <Flex w={331} h={174} direction="column" gap={24} p={32}>
            <Text size="xs">管理者　{staffName}</Text>
            {/* ログアウトボタン */}
            {/* ToDo:ログアウト時の処理は未実装 */}
            <Button
              w={184}
              h={51}
              color="primary"
              variant="outline"
              radius="xl"
              onClick={() => navigate("/login")}
            >
              ログアウト
            </Button>
          </Flex>
        </Dialog>
      </div>
    </>
  );
}
