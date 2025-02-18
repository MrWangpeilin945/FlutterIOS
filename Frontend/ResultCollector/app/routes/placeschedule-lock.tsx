import {
  Button,
  Container,
  Flex,
  LoadingOverlay,
  Space,
  Text,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useSearchParams } from "@remix-run/react";
import { isAxiosError } from "axios";
import { format } from "date-fns";
import { useAtom } from "jotai";
import { useEffect, useRef, useState } from "react";
import {
  usePlaceScheduleGetPlaceScheduleLockingStatus,
  usePlaceScheduleUpdatePlaceScheduleLockingStatus,
} from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import ConfirmDialog from "~/components/ConfirmDialog";
import PlaceSchedule from "~/components/PlaceSchedule";
import { PlaceScheduleLockingStatus } from "~/domain/enums";
import type {
  PlaceSchedule as PlaceScheduleType,
  PlaceScheduleLocking,
  PlaceScheduleLockingRequest,
} from "~/domain/wellship.schemas";
import { examDateState, placeScheduleState, staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "会場ロック" }];
};

export default function PlaceScheduleLock() {
  const [searchParams] = useSearchParams();
  const placeScheduleId = searchParams.get("placescheduleid");
  const [isLoading, setIsLoading] = useState(false);
  const [staff] = useAtom(staffState);
  const [placeSchedule] = useAtom(placeScheduleState);
  const firstPlaceSchedule = useRef<PlaceScheduleType>();
  const [examDate] = useAtom(examDateState);
  const [opened, { open, close }] = useDisclosure(false);
  const [message, setMessage] = useState<string | null>(null);
  const [openedConfirm, { open: openConfirm, close: closeConfirm }] =
    useDisclosure(false);
  const [processStatus, setProcessStatus] =
    useState<PlaceScheduleLockingStatus | null>();
  const [targetDate] = useState(examDate ? format(examDate, "yyyy-MM-dd") : "");
  const [placeScheduleLock, setPlaceScheduleLock] =
    useState<PlaceScheduleLocking>();

  const statusButton = [
    {
      placeScheduleLockingStatus: PlaceScheduleLockingStatus.検査完了,
      buttonName: "会場ロック",
    },
    {
      placeScheduleLockingStatus: PlaceScheduleLockingStatus.検査中,
      buttonName: "会場ロック解除",
    },
  ];

  //AP1016呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = usePlaceScheduleGetPlaceScheduleLockingStatus(
    "1",
    placeScheduleId || firstPlaceSchedule.current?.placeScheduleId || "",
    { query: { enabled: false } },
  );

  //AP1017呼び出し用(POST系APIの定義)
  const { mutateAsync } = usePlaceScheduleUpdatePlaceScheduleLockingStatus();

  //AP1016_会場ロック状態を取得する
  const fetchGetPlaceScheduleLocking = async () => {
    const result = await refetch();
    if (result.data) {
      setPlaceScheduleLock(result.data.data);
    } else if (result.error) {
      if (result.error.status === 400) {
        setMessage(getErrorMessage(errorMessages.invalid, "パラメータ"));
      } else if (result.error.status === 404) {
        setMessage(getErrorMessage(errorMessages.notFound, "該当する会場日程"));
      } else if (result.error.status === 500) {
        setMessage(getErrorMessage(errorMessages.serverError));
      }
      open();
      throw new Error();
    }
  };

  useEffect(() => {
    firstPlaceSchedule.current = placeSchedule ?? {};
    fetchGetPlaceScheduleLocking();
  }, []);

  // ボタン(会場ロック, 会場ロック解除)クリック
  const handleButtonClick = async (
    lockingStatus: PlaceScheduleLockingStatus,
  ) => {
    if (lockingStatus === PlaceScheduleLockingStatus.検査中) {
      setMessage("会場ロックの解除を行ってもよろしいですか？");
    } else if (lockingStatus === PlaceScheduleLockingStatus.検査完了) {
      setMessage("会場ロックを行ってもよろしいですか？");
    }

    setProcessStatus(lockingStatus);
    //確認ダイアログ表示
    openConfirm();
  };

  //AP1017_会場ロック状態を更新する
  const fetchUpdateLockingStatus = async () => {
    // POST時のリクエストボディを生成する
    const body: PlaceScheduleLockingRequest = {
      placeScheduleId: firstPlaceSchedule.current?.placeScheduleId || "",
      placeScheduleLockingStatus: processStatus || 0,
    };

    const postMutateAsync = async () => {
      setIsLoading(true);
      try {
        const result = await mutateAsync({
          version: "1",
          placeScheduleId: placeSchedule?.placeScheduleId || "",
          data: body,
        });
        if (result.status === 200) {
          // AP1016を実行して画面再取得
          await fetchGetPlaceScheduleLocking();
          setMessage("登録が完了しました。");
          open();
        }
      } catch (error) {
        if (isAxiosError(error) && error.response) {
          if (error.response.status === 400) {
            setMessage(getErrorMessage(errorMessages.invalid, "パラメータ"));
          } else if (error.response.status === 404) {
            setMessage(
              getErrorMessage(errorMessages.notFound, "該当する会場日程"),
            );
          } else if (error.response.status === 500) {
            setMessage(getErrorMessage(errorMessages.serverError));
          }
          open();
        }
      } finally {
        setIsLoading(false);
      }
    };
    postMutateAsync();
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching || isLoading} />
        <CommonHeader screenName="会場ロック" staffName={staff?.name || ""} />
        <Container fluid>
          {!isFetching && (
            <>
              <PlaceSchedule
                placeName={placeSchedule?.placeName || ""}
                examDate={targetDate}
              />
              {placeScheduleLock ? (
                <>
                  <Space h={40} />
                    <Container ta="center" mt={40} mb={300}>
                      {statusButton?.map((button) => (
                        <Button
                          w={860}
                          h={75}
                          px={32}
                          py={16}
                          bd="2px solid"
                          variant="outline"
                          bg={
                            placeScheduleLock.placeScheduleLockingStatus ===
                            button.placeScheduleLockingStatus
                              ? "green03"
                              : "white"
                          }
                          color={
                            placeScheduleLock.placeScheduleLockingStatus ===
                            button.placeScheduleLockingStatus
                              ? "primary"
                              : "gray03"
                          }
                          key={button.placeScheduleLockingStatus}
                          onClick={() =>
                            handleButtonClick(button.placeScheduleLockingStatus)
                          }
                          mb={
                            button.placeScheduleLockingStatus ===
                            PlaceScheduleLockingStatus.検査完了
                              ? 32
                              : 0
                          }
                        >
                          <Text
                            size="xl"
                            fw={700}
                            c={
                              placeScheduleLock.placeScheduleLockingStatus ===
                              button.placeScheduleLockingStatus
                                ? "primary"
                                : "black01"
                            }
                          >
                            {button.buttonName}
                          </Text>
                        </Button>
                      ))}
                    </Container>
                  <Flex justify="flex-end">
                    <Text size="xs" c="black01" ta="right">
                      最終更新者：{placeScheduleLock.updatedBy}（
                      {placeScheduleLock.updatedAt
                        ? format(
                            new Date(placeScheduleLock.updatedAt),
                            "yyyy/MM/dd HH:mm",
                          )
                        : ""}
                      ）&nbsp;
                    </Text>
                  </Flex>
                </>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Text size="sm" c="black01">
                    {getErrorMessage(
                      errorMessages.notFound,
                      "該当する会場日程",
                    )}
                  </Text>
                </>
              )}

              <CommonDialog
                message={message || ""}
                buttonMessage="閉じる"
                isOpen={opened}
                onClose={close}
              />

              <div>
                <ConfirmDialog
                  message={message || ""}
                  cancelButtonMessage="キャンセル"
                  confirmButtonMessage="OK"
                  isOpen={openedConfirm}
                  onCancel={closeConfirm}
                  onConfirm={() => {
                    closeConfirm();
                    fetchUpdateLockingStatus();
                  }}
                />
              </div>
            </>
          )}
        </Container>
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}
