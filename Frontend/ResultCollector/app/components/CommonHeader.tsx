import { Box, Button, Center, Dialog, Flex, Text } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { useNavigate } from "@remix-run/react";
import { IconHomeFilled, IconUserFilled } from "@tabler/icons-react";

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
    <Box h={67} bg="primary" c="white01" pt={8} pb={8} pl={24} pr={24}>
      <Flex justify="space-between" align="center">
        {/* ユーザアイコン */}
        <Button
          bg="white01"
          c="primary"
          h="auto"
          pt={8}
          pb={8}
          pl={32}
          pr={32}
          onClick={toggle}
        >
          <IconUserFilled size={"24"} />
        </Button>

        {/* 画面名 */}
        <Center>
          <Text size="lg" fw={700} c="white01" ta="center">
            {screenName}
          </Text>
        </Center>

        {/* ホームボタン */}
        <Button
          bg="white01"
          c="primary"
          h="auto"
          pt={8}
          pb={8}
          pl={32}
          pr={32}
          onClick={() => navigate("/home")}
        >
          <IconHomeFilled size={"24"} />
          <Text size="xs" fw={700} c="primary" pl={10}>
            ホーム
          </Text>
        </Button>
      </Flex>
      <div ref={closeMenu}>
        <Dialog
          opened={opened}
          position={{ top: 67, left: 0 }}
          onClose={hide}
          w={253}
          h={174}
          p={32}
          style={{ borderBottomLeftRadius: 8, borderBottomRightRadius: 8 }}
          bg="white01"
        >
          <Box>
            <Text size="xs" c="black01" pb={24} truncate="end">
              {staffName}
            </Text>
            {/* ログアウトボタン */}
            <Button
              h="auto"
              bg="white01"
              c="primary"
              variant="outline"
              style={{ borderWidth: 2 }}
              pt={8}
              pb={8}
              pl={32}
              pr={32}
              onClick={() => navigate("/login")}
            >
              <Text size="xs" fw={700} c="primary">
                ログアウト
              </Text>
            </Button>
          </Box>
        </Dialog>
      </div>
    </Box>
  );
}
