import { Container, LoadingOverlay, Space, Stack, Text } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "react-router";
import { useNavigate } from "react-router";
import { format, parse } from "date-fns";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { usePlaceScheduleGetTeamPlaceSchedules } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import HeadlineButton from "~/components/HeadlineButton";
import type { PlaceSchedulePlaces } from "~/domain/wellship.schemas";
import {
  examDateState,
  placeScheduleState,
  staffState,
  teamState,
} from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "会場選択" }];
};

export default function PlaceSelect() {
  const navigate = useNavigate();
  const [placesData, setPlacesData] = useState<PlaceSchedulePlaces>();
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [placeSchedule, setPlaceSchedule] = useAtom(placeScheduleState);
  const [team] = useAtom(teamState);
  const [staff] = useAtom(staffState);
  const [, setExamDate] = useAtom(examDateState);
  const [targetDate] = useState(format(new Date(), "yyyy-MM-dd"));
  const { isFetching, refetch } = usePlaceScheduleGetTeamPlaceSchedules(
    "1",
    { date: targetDate, teamId: team?.id },
    { query: { enabled: false } },
  );

  // AP1004_班を指定して会場日程を取得する
  useEffect(() => {
    const fetchPlaceSchedules = async () => {
      const result = await refetch();
      if (result.data) {
        setPlacesData(result.data.data);
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
    fetchPlaceSchedules();
  }, []);

  // jotaiに健診日、会場ID、会場名、会場日程ID、開始時刻を保存して遷移
  const callbackClick = (
    examDate?: string,
    placeId?: string,
    placeName?: string,
    placeScheduleId?: string,
    startTime?: string,
  ) => {
    if (examDate) {
      setExamDate(parse(examDate, "yyyy-MM-dd", new Date()));
    }

    if (placeId !== undefined && placeName && placeScheduleId !== undefined) {
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
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="会場選択" staffName={staff?.name || ""} />
        <Container fluid bg="background" pt={32} pb={32} pl={24} pr={24}>
          {!isFetching && (
            <>
              {placesData?.placeSchedules &&
              placesData.placeSchedules.length > 0 ? (
                <Stack gap={16}>
                  {placesData.placeSchedules.map((ps) => (
                    <HeadlineButton
                      key={ps.placeId}
                      title={ps.placeName || ""}
                      elements={ps.startTime ? [`${ps.startTime}開始`] : undefined}
                      selected={placeSchedule?.placeId === ps.placeId}
                      onClick={() =>
                        callbackClick(
                          placesData.examDate,
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
                  <Text size="sm" c="black01">
                    会場の設定がありませんでした。
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
        <Space h={100} />
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}
