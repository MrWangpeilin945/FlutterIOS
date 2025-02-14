import {
  Box,
  Button,
  Container,
  LoadingOverlay,
  Stack,
  Text,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { useHomeMenuGetHomeMenuSettings } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonHeader from "~/components/CommonHeader";
import { PlaceScheduleLockingStatus, Role } from "~/domain/enums";
import type { HomeMenu, HomeMenuGroupList } from "~/domain/wellship.schemas";
import { placeScheduleState, staffState, teamState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "ホーム" }];
};

export default function Home() {
  const navigate = useNavigate();
  const [homeMenuGroupData, setHomeMenuGroupData] =
    useState<HomeMenuGroupList>();
  const [team] = useAtom(teamState);
  const [placeSchedule] = useAtom(placeScheduleState);
  const [staff] = useAtom(staffState);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [opened, { open, close }] = useDisclosure(false);

  //AP1005呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = useHomeMenuGetHomeMenuSettings(
    "1",
    { placeScheduleId: placeSchedule?.placeScheduleId },
    { query: { enabled: false } },
  );

  //availableConditions[]に、条件:PlaceScheduleSelected が存在するか
  const isPlaceSelected = (conditions: string[]) => {
    return conditions.some((element) => element === "PlaceScheduleSelected");
  };

  //availableConditions[]に、条件:PlaceScheduleUnlocked が存在するか
  const isPlaceUnlocked = (conditions: string[]) => {
    return conditions.some((element) => element === "PlaceScheduleUnlocked");
  };

  //ホームメニューボタンの名称を取得
  const getMenuName = (menus: HomeMenu, placeSchedulelocking: number) => {
    let name = "";
    if (
      isPlaceSelected(menus?.availableConditions || []) &&
      (!team || !placeSchedule)
    ) {
      //availableConditions[PlaceScheduleSelected]有りで、jotaiの班／jotaiの会場 のどれか値がない場合
      name = `${menus.menuName}【班と会場を選択してください】`;
    } else if (
      isPlaceUnlocked(menus?.availableConditions || []) &&
      placeSchedulelocking === PlaceScheduleLockingStatus.検査完了
    ) {
      //availableConditions[PlaceScheduleUnlocked]有りで、placeSchedulelockingStatusの値が(検査完了)である場合
      name = `${menus.menuName}【会場ロック中】`;
    } else {
      name = menus?.menuName || "";
    }
    return name;
  };

  //ホームメニューボタンのdisabled(押下可能/不可)を取得
  const getDisabled = (menus: HomeMenu) => {
    let disabled = false;
    if (
      isPlaceSelected(menus?.availableConditions || []) &&
      (!team || !placeSchedule)
    ) {
      //availableConditions[PlaceScheduleSelected]有りで、jotaiの班／jotaiの会場 のどれか値がない場合
      disabled = true;
    }
    return disabled;
  };

  //メニュー項目ボタン
  interface BaseButtonProps {
    menuName: string;
    disabled: boolean;
    onClick: () => void;
    marginBottom: number;
  }
  const ButtonComponent = ({
    menuName,
    disabled,
    onClick,
    marginBottom,
  }: BaseButtonProps) => {
    return (
      <Button
        fullWidth
        justify="flex-start"
        key={menuName}
        onClick={onClick}
        disabled={disabled}
        py={16}
        px={32}
        bd="2px solid"
        variant="outline"
        bg="white01"
        color="gray03"
        mb={marginBottom}
        h="auto"
        radius={48}
      >
        <Text
          c="black01"
          size="xl"
          fw={700}
          style={{
            textOverflow: "ellipsis",
            whiteSpace: "nowrap",
            overflow: "hidden",
          }}
        >
          {menuName}
        </Text>
      </Button>
    );
  };

  useEffect(() => {
    const fetchHomeMenuGroup = async () => {
      // AP1005_ホームメニュー項目を取得する
      const result = await refetch();
      if (result.data) {
        setHomeMenuGroupData(result.data.data);
      } else if (result.error) {
        if (result.error.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    fetchHomeMenuGroup();
  }, []);

  return (
    <AuthWrapper>
      <Box h="100vh" bg="white01">
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="ホーム" staffName={staff?.name || ""} />
        <Container fluid py={32} px={24}>
          {!isFetching && (
            <>
              {homeMenuGroupData?.homeMenuGroups &&
              homeMenuGroupData.homeMenuGroups.length > 0 ? (
                <>
                  <Stack pb={24}>
                    <Box
                      variant="outline"
                      bg="background"
                      py={24}
                      px={32}
                      style={{ borderRadius: 16 }}
                      pb={16}
                      key={team?.id}
                    >
                      {/* 班名ボタン */}
                      <ButtonComponent
                        key={team?.id}
                        menuName={team?.name || "【班を選択してください】"}
                        disabled={false}
                        onClick={() => navigate("/team-select")}
                        marginBottom={16}
                      />

                      {/* 会場名ボタン */}
                      <ButtonComponent
                        key={placeSchedule?.placeScheduleId}
                        menuName={
                          placeSchedule?.placeName ||
                          "【会場を選択してください】"
                        }
                        disabled={!team}
                        onClick={() => navigate("/place-select")}
                        marginBottom={0}
                      />
                    </Box>
                  </Stack>
                  <Stack gap={24}>
                    {/* ホームメニュー一覧 */}
                    {homeMenuGroupData?.homeMenuGroups?.map((homeMenuGroup) => (
                      <Box variant="outline" key={homeMenuGroup.groupName}>
                        <Box
                          style={{
                            borderTopLeftRadius: 16,
                            borderTopRightRadius: 16,
                          }}
                          bg={"gray02"}
                          py={12}
                          px={24}
                        >
                          <Text c="white01" size="md" fw={700}>
                            {homeMenuGroup.groupName || ""}
                          </Text>
                        </Box>
                        <Box
                          bg="background"
                          py={24}
                          px={32}
                          style={{
                            borderBottomLeftRadius: 16,
                            borderBottomRightRadius: 16,
                          }}
                        >
                          {homeMenuGroup.menus?.map((menus, index) => (
                            <ButtonComponent
                              key={menus.menuName}
                              menuName={getMenuName(
                                menus,
                                homeMenuGroupData.placeScheduleLockingStatus ||
                                  0,
                              )}
                              disabled={getDisabled(menus)}
                              onClick={() => navigate(`/${menus.path}`)}
                              marginBottom={
                                homeMenuGroup.menus
                                  ? homeMenuGroup.menus.length - 1 === index
                                    ? 0
                                    : 16
                                  : 0
                              }
                            />
                          ))}
                        </Box>
                      </Box>
                    ))}
                  </Stack>
                </>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Text size="sm" c="black01">
                    使用できるホームメニューの設定がありませんでした。
                  </Text>
                </>
              )}
              <CommonDialog
                message={errorMessage || ""}
                buttonMessage="閉じる"
                isOpen={opened}
                onClose={close}
              />
            </>
          )}
        </Container>
      </Box>
    </AuthWrapper>
  );
}
