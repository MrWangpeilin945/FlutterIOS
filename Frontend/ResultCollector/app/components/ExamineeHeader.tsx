import { Box, Button, Flex, Popover, Text } from "@mantine/core";
import { useNavigate } from "react-router";
import { IconHomeFilled, IconUserFilled } from "@tabler/icons-react";
import { Sex } from "~/domain/enums";
import { authUtil } from "~/utils/authUtil";

type HeaderProps = {
  staffName: string;
  managerNo: string;
  name: string;
  age: number;
  gender: number;
};

export default function ExamineeHeader({
  staffName,
  managerNo,
  name,
  age,
  gender,
}: HeaderProps) {
  const navigate = useNavigate();

  const displayableExamineeName =
    name.length > 14 ? `${name.slice(0, 13)}…` : name;
  const displayableStaffName =
    staffName.length > 8 ? `${staffName.slice(0, 7)}…` : staffName;
  const isAgeNumber: boolean = typeof age === "number";

  return (
    <>
      <Flex
        h={67}
        bg={
          gender === Sex.男
            ? "malePrimary"
            : gender === Sex.女
              ? "femalePrimary"
              : "green02"
        }
        justify="space-between"
        align="center"
        pos="sticky"
        px={24}
        py={8}
        top={0}
        style={{ zIndex: 10 }}
      >
        {/* ユーザアイコン */}
        <Popover position="bottom">
          <Popover.Target>
            <Button
              w={88}
              h={40}
              bg="white"
              c={
                gender === Sex.男
                  ? "maleSecondary"
                  : gender === Sex.女
                    ? "femaleSecondary"
                    : "green01"
              }
            >
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
              <Text size="xs" c="black01" pb={24}>
                {displayableStaffName}
              </Text>
              <Button
                h="auto"
                bg="white01"
                c="primary"
                variant="outline"
                py={8}
                px={32}
                onClick={() => authUtil.logout(() => navigate("/login"))}
                bd="2px,solid"
              >
                <Text size="xs" fw={700} c="primary" w={120} h={35}>
                  ログアウト
                </Text>
              </Button>
            </Box>
          </Popover.Dropdown>
        </Popover>

        {/* 受付番号、受診者名、年齢 */}
        <Text size="lg" fw={700}>
          {managerNo} {displayableExamineeName}
          {isAgeNumber && `(${age})`}
        </Text>
        {/* ホームボタン */}
        <Button
          w={170}
          h={51}
          bg="white01"
          size="xs"
          fw={700}
          c={
            gender === Sex.男
              ? "maleSecondary"
              : gender === Sex.女
                ? "femaleSecondary"
                : "green01"
          }
          pl={10}
          leftSection={<IconHomeFilled size={24} />}
          onClick={() => navigate("/home")}
        >
          ホーム
        </Button>
      </Flex>
    </>
  );
}
