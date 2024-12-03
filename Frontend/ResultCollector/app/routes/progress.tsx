import { Container, LoadingOverlay, Stack, Title } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { format } from "date-fns";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { useProgressGetProgress } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import ExamItemProgress from "~/components/ExamItemProgress";
import PlaceSchedule from "~/components/PlaceSchedule";
import type { PlaceScheduleProgress } from "~/domain/wellship.schemas";
import { examDateState, placeScheduleState, staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "進捗" }];
};

export default function Progress() {
  const [progressData, setProgressData] = useState<PlaceScheduleProgress>();
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [staff] = useAtom(staffState);
  const [placeSchedule] = useAtom(placeScheduleState);
  const [examDate] = useAtom(examDateState);
  const [targetDate] = useState(
    examDate ? format(examDate, "yyyy/MM/dd") : null,
  );
  const { isFetching, refetch } = useProgressGetProgress(
    "1",
    placeSchedule?.placeScheduleId || 0,
    {
      query: { enabled: false },
    },
  );

  // AP1015_進捗状況を取得する
  useEffect(() => {
    const fetchProgress = async () => {
      const result = await refetch();
      if (result.data) {
        setProgressData(result.data.data);
      } else if (result.error) {
        if (result.error.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    fetchProgress();
  }, []);

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="進捗" staffName={staff?.name || ""} />
        <PlaceSchedule
          placeName={placeSchedule?.placeName || ""}
          examDate={targetDate || ""}
        />
        <Container fluid>
          {!isFetching && (
            <>
              {progressData?.progress ? (
                <Stack>
                  {progressData?.progress?.map((p) => (
                    <ExamItemProgress
                      key={p.examItemId}
                      progress={p}
                      onClick={() => {}}
                    />
                  ))}
                </Stack>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Title order={3}>
                    {getErrorMessage(errorMessages.notFound, "進捗データ")}
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
