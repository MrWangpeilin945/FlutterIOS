import { Box, Button, Center, Flex, Popover, Text } from "@mantine/core";
import { useNavigate } from "@remix-run/react";
import { IconHomeFilled, IconUserFilled } from "@tabler/icons-react";
import { authUtil } from "~/utils/authUtil";

type HeaderProps = {
  screenName: string;
  staffName: string;
};

export default function CommonHeader({ screenName, staffName }: HeaderProps) {
  const navigate = useNavigate();

  return (
    <Box
      h={67}
      bg="primary"
      c="white01"
      py={8}
      px={24}
      pos="sticky"
      top={0}
      style={{ zIndex: 10 }}
    >
      <Flex justify="space-between" align="center">
        {/* ユーザアイコン */}
        <Popover position="bottom">
          <Popover.Target>
            <Button bg="white01" c="primary" h="auto" py={8} px={32}>
              <IconUserFilled size={24} />
            </Button>
          </Popover.Target>
          <Popover.Dropdown
            w={253}
            h={174}
            p={32}
            top={67}
            left={0}
            pos="fixed"
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
                py={8}
                px={32}
                onClick={() => authUtil.logout(() => navigate("/login"))}
              >
                <Text size="xs" fw={700} c="primary">
                  ログアウト
                </Text>
              </Button>
            </Box>
          </Popover.Dropdown>
        </Popover>

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
          py={8}
          px={32}
          onClick={() => navigate("/home")}
        >
          <IconHomeFilled size={24} />
          <Text size="xs" fw={700} c="primary" pl={10}>
            ホーム
          </Text>
        </Button>
      </Flex>
    </Box>
  );
}
