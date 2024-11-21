import { Container, LoadingOverlay, Stack, Title } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { format } from "date-fns";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { placeScheduleGetTeamPlaceSchedules } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import HeadlineButton from "~/components/HeadlineButton";
import type { PlaceSchedulePlaces } from "~/domain/wellship.schemas";
import { placeScheduleState, staffState, teamState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "会場選択" }];
};

export default function placeSelect() {
  const navigate = useNavigate();
  const [placesData, setPlacesData] = useState<PlaceSchedulePlaces>();
  const [isLoading, setIsLoading] = useState(false);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [placeSchedule, setPlaceSchedule] = useAtom(placeScheduleState);
  const [team] = useAtom(teamState);
  const [staff] = useAtom(staffState);

  // AP1004_班を指定して会場日程を取得する
  // biome-ignore lint/correctness/useExhaustiveDependencies: <explanation>
  useEffect(() => {
    setIsLoading(true);
    const fetchPlaceSchedules = async () => {
      await placeScheduleGetTeamPlaceSchedules("1", {
        date: format(new Date(), "yyyy-MM-dd"),
        teamId: team?.id,
      })
        .then((result) => {
          setPlacesData(result.data);
        })
        .catch((error) => {
          if (error.response.status === 400) {
            setErrorMessage(
              getErrorMessage(errorMessages.invalid, "必要なパラメータ"),
            );
          } else if (error.response.status === 500) {
            setErrorMessage(getErrorMessage(errorMessages.server));
          }
          open();
        })
        .finally(() => {
          setIsLoading(false);
        });
    };
    fetchPlaceSchedules();
  }, []);

  // jotaiに会場ID、会場名、会場日程ID、開始時刻を保存して遷移
  const callbackClick = (
    placeId?: number,
    placeName?: string,
    placeScheduleId?: number,
    startTime?: string,
  ) => {
    if (placeId && placeName && placeScheduleId) {
      const placeScheduleData = {
        placeId,
        placeName,
        placeScheduleId,
        startTime,
      };
      setPlaceSchedule(placeScheduleData);
    }
    navigate("/home");
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <CommonHeader screenName="会場選択" staffName={staff?.name || ""} />
        <Container fluid mt={20}>
          {!isLoading && (
            <>
              {placesData?.placeSchedules ? (
                <Stack>
                  {placesData.placeSchedules.map((ps) => (
                    <HeadlineButton
                      key={ps.placeId}
                      title={ps.placeName || ""}
                      elements={ps.startTime ? [ps.startTime] : undefined}
                      selected={placeSchedule?.placeId === ps.placeId}
                      onClick={() =>
                        callbackClick(
                          ps.placeId,
                          ps.placeName,
                          ps.placeScheduleId,
                          ps.startTime,
                        )
                      }
                    />
                  ))}
                </Stack>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Title order={3}>
                    {getErrorMessage(errorMessages.noData, "該当する会場")}
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
