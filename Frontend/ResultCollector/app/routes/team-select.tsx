import { Container, LoadingOverlay, Stack, Text } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { format } from "date-fns";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { usePlaceScheduleGetTeams } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonHeader from "~/components/CommonHeader";
import HeadlineButton from "~/components/HeadlineButton";
import type { PlaceScheduleTeams } from "~/domain/wellship.schemas";
import { staffState, teamState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "班選択" }];
};

export default function Teams() {
  const navigate = useNavigate();
  const [teamsData, setTeamsData] = useState<PlaceScheduleTeams>();
  const [opened, { open, close }] = useDisclosure(false);
  const [team, setTeam] = useAtom(teamState);
  const [staff] = useAtom(staffState);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [targetDate] = useState(format(new Date(), "yyyy-MM-dd"));
  const { isFetching, refetch } = usePlaceScheduleGetTeams(
    "1",
    { date: targetDate },
    { query: { enabled: false } },
  );

  // AP1003_班を取得する
  useEffect(() => {
    const fetchTeams = async () => {
      const result = await refetch();
      if (result.data) {
        setTeamsData(result.data.data);
      } else if (result.error) {
        if (result.error.status === 400) {
          setErrorMessage(
            getErrorMessage(errorMessages.invalid, "必要なパラメータ"),
          );
        } else if (result.error.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    fetchTeams();
  }, []);

  //jotaiに班idと班名を保存して遷移
  const callbackClick = (teamId?: string, teamName?: string) => {
    if (teamId !== undefined && teamName) {
      const teamData = { id: teamId, name: teamName };
      setTeam(teamData);
    }
    navigate("/place-select");
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="班選択" staffName={staff?.name || ""} />
        <Container fluid bg="background" pt={32} pb={32} pl={24} pr={24}>
          {!isFetching && (
            <>
              {teamsData?.teams && teamsData.teams.length > 0 ? (
                <Stack gap={16}>
                  {teamsData.teams.map((tm) => (
                    <HeadlineButton
                      key={tm.teamId}
                      title={tm.teamName || ""}
                      elements={
                        tm.places
                          ? tm.places.map((p) => {
                              return p.placeName ? p.placeName : "";
                            })
                          : undefined
                      }
                      selected={team?.id === tm.teamId}
                      onClick={() => callbackClick(tm.teamId, tm.teamName)}
                    />
                  ))}
                </Stack>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Text size="sm" c="black01">
                    班の設定がありませんでした。
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
      </AuthWrapper>
    </>
  );
}
