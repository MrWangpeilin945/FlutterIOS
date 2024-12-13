import {
  Box,
  Button,
  Container,
  LoadingOverlay,
  Paper,
  Stack,
  Text,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { useHomeMenuGetHomeMenuSettings } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import { PlaceScheduleLockingStatus } from "~/domain/enums";
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

  //availableConditions[]に、条件:placeScheduleSelected が存在するか
  const isPlaceSelected = (conditions: string[]) => {
    return conditions.some((element) => element === "placeScheduleSelected");
  };

  //availableConditions[]に、条件:placeScheduleUnlocked が存在するか
  const isPlaceUnlocked = (conditions: string[]) => {
    return conditions.some((element) => element === "placeScheduleUnlocked");
  };

  //ホームメニューボタンの名称を取得
  const getMenuName = (menus: HomeMenu, placeSchedulelocking: number) => {
    let name = "";
    if (!team || !placeSchedule) {
      //jotaiの班、会場のどちらかがない場合
      name = `${menus.menuName}【班と会場を選択してください】`;
    } else if (
      isPlaceSelected(menus?.availableConditions || []) &&
      !placeSchedule
    ) {
      //availableConditions[placeScheduleSelected]有りで、jotaiのplaceScheduleStateに値がない場合
      name = `${menus.menuName}【班と会場を選択してください】`;
    } else if (
      isPlaceUnlocked(menus?.availableConditions || []) &&
      placeSchedulelocking === PlaceScheduleLockingStatus.検査完了
    ) {
      //availableConditions[placeScheduleUnlocked]有りで、placeSchedulelockingStatusの値が(検査完了)である場合
      name = `${menus.menuName}【会場ロック中】`;
    } else {
      name = menus?.menuName || "";
    }
    return name;
  };

  //ホームメニューボタンのdisabled(押下可能/不可)を取得
  const getDisabled = (menus: HomeMenu) => {
    let disabled = false;
    if (!team || !placeSchedule) {
      //jotaiの班、会場のどちらかがない場合
      disabled = true;
    } else if (
      isPlaceSelected(menus?.availableConditions || []) &&
      !placeSchedule
    ) {
      //availableConditions[placeScheduleSelected]有りで、jotaiのplaceScheduleStateに値がない場合
      disabled = true;
    }
    return disabled;
  };

  //メニュー項目ボタン
  interface BaseButtonProps {
    menuName: string;
    disabled: boolean;
    onClick: () => void;
  }
  const ButtonComponent = ({
    menuName,
    disabled,
    onClick,
  }: BaseButtonProps) => {
    return (
      <Button
        fullWidth
        justify="flex-start"
        key={menuName}
        bg="white01"
        c="black"
        m={10}
        onClick={onClick}
        disabled={disabled}
      >
        <Text size="xs" fw="500">
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
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="ホーム" staffName={staff?.name || ""} />
        <Container fluid mt={20}>
          {!isFetching && (
            <>
              {homeMenuGroupData?.homeMenuGroups ? (
                <>
                  <Stack>
                    <Box variant="outline" key={team?.id}>
                      <Paper radius="lg" bg={"white"}>
                        <Box
                          style={{
                            borderTopLeftRadius: "inherit",
                            borderTopRightRadius: "inherit",
                          }}
                          pl="sm"
                          p="md"
                          bg="#f8f7f6"
                        >
                          {/* 班名ボタン */}
                          <ButtonComponent
                            key={team?.id}
                            menuName={team?.name || "【班を選択してください】"}
                            disabled={false}
                            onClick={() => navigate("/team-select")}
                          />

                          {/* 会場名ボタン */}
                          <div>
                            <ButtonComponent
                              key={placeSchedule?.placeScheduleId}
                              menuName={
                                placeSchedule?.placeName ||
                                "【会場を選択してください】"
                              }
                              disabled={!team}
                              onClick={() => navigate("/place-select")}
                            />
                          </div>
                        </Box>
                      </Paper>
                    </Box>
                  </Stack>
                  <Stack>
                    {/* ホームメニュー一覧 */}
                    {homeMenuGroupData?.homeMenuGroups?.map((homeMenuGroup) => (
                      <Box variant="outline" key={homeMenuGroup.groupName}>
                        <Paper radius="lg" bg={"white"}>
                          <Box
                            style={{
                              borderTopLeftRadius: "inherit",
                              borderTopRightRadius: "inherit",
                            }}
                            pl="sm"
                            p="md"
                            bg={"gray02"}
                          >
                            <Text size="xs" c="white" fw="500">
                              {homeMenuGroup.groupName || ""}
                            </Text>
                          </Box>
                          <Box pl="sm" p="md" bg="#f8f7f6">
                            {homeMenuGroup.menus?.map((menus) => (
                              <ButtonComponent
                                key={menus.menuName}
                                menuName={getMenuName(
                                  menus,
                                  homeMenuGroupData.placeScheduleLockingStatus ||
                                    0,
                                )}
                                disabled={getDisabled(menus)}
                                onClick={() => navigate(`/${menus.path}`)}
                              />
                            ))}
                          </Box>
                        </Paper>
                      </Box>
                    ))}
                  </Stack>
                </>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Title order={3}>
                    使用できるホームメニューの設定がありませんでした。
                  </Title>
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
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}
