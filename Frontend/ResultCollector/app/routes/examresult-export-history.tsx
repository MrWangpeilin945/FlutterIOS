import {
  Button,
  Center,
  Container,
  LoadingOverlay,
  Modal,
  Table,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { isAxiosError } from "axios";
import { format, parse } from "date-fns";
import { useAtom } from "jotai";
import {
  type ComponentProps,
  type ComponentRef,
  forwardRef,
  useCallback,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import {
  useIntegrationGetExportHistory,
  usePlaceScheduleUpdatePlaceScheduleResultExportStatus,
} from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import type { ExportHistoryList } from "~/domain/wellship.schemas";
import { staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "検査結果出力履歴" }];
};

// 確認ダイアログ用の状態型定義
type State = {
  isOpen: boolean; // 表示・非表示用
  resolve: (isOk: boolean) => void; // resolve格納用
};

// 確認ダイアログ用の状態初期値
const initialState: State = {
  isOpen: false,
  resolve: () => {},
};

// 確認ダイアログの開閉状態を管理するカスタムフック
const useConfirmState = () => {
  const [{ isOpen, resolve }, setState] = useState<State>(initialState);

  // 確認ダイアログを起動するための関数
  const confirm = useCallback(
    () =>
      new Promise<boolean>((resolve) => {
        setState({ isOpen: true, resolve });
      }),
    [],
  );

  // 「未出力確定」ボタン用の関数
  const handleOk = useCallback(() => {
    resolve(true);
    setState(initialState);
  }, [resolve]);

  // 「キャンセル」ボタン用の関数
  const handleCancel = useCallback(() => {
    resolve(false);
    setState(initialState);
  }, [resolve]);

  return {
    isOpen,
    confirm,
    handleOk,
    handleCancel,
  };
};

// 親コンポーネントに公開する関数
type Handle = { confirm: () => Promise<boolean> };

type Props = Omit<ComponentProps<typeof Modal>, "opened" | "onClose">;

// 確認ダイアログコンポーネント
const ConfirmDialog = forwardRef<Handle, Props>((props, ref) => {
  const { isOpen, confirm, handleOk, handleCancel } = useConfirmState();
  const { children, title } = props;
  useImperativeHandle(ref, () => ({ confirm }), [confirm]);

  return (
    <Modal
      size="md"
      radius="md"
      opened={isOpen}
      onClose={handleCancel}
      title={title}
      closeOnClickOutside={false} // modalの外クリックしても消えないように
      withCloseButton={false} // closeボタンを消す
      centered
    >
      {children}
      <Center>
        <div>
          <Button w={400} h={80} variant="filled" onClick={handleOk} mt="xl">
            <Title order={2}>未出力確定</Title>
          </Button>
        </div>
      </Center>
      <Center>
        <Button w={400} h={40} variant="outline" onClick={handleCancel} mt="xl">
          <Title order={2}>戻る</Title>
        </Button>
      </Center>
    </Modal>
  );
});

interface ConfirmResult {
  examDate?: string;
  placeName?: string;
  dataCount?: number;
}

export default function ExamresultExportHistory() {
  const [exportHistory, setExportHistory] = useState<ExportHistoryList>();
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [confirmResult, setConfirmResult] = useState<ConfirmResult | null>(
    null,
  );
  const [staff] = useAtom(staffState);
  const [opened, { open, close }] = useDisclosure(false);
  const { mutateAsync } =
    usePlaceScheduleUpdatePlaceScheduleResultExportStatus();
  const { isFetching, refetch } = useIntegrationGetExportHistory("1", {
    query: { enabled: false },
  });
  const ref = useRef<ComponentRef<typeof ConfirmDialog>>(null);

  //【CP0002】共通フッター設定
  const navigate = useNavigate();
  const fotterItems = [
    { label: "", action: () => {} },
    { label: "", action: () => {} },
    { label: "出力", action: () => navigate("/examresult-export") },
  ];

  // AP1020_検査結果の出力履歴を取得する
  useEffect(() => {
    const fetchExportHistory = async () => {
      const result = await refetch();
      if (result.data) {
        setExportHistory(result.data.data);
      } else if (result.error) {
        if (result.error.status === 400) {
          setMessage(getErrorMessage(errorMessages.invalid, "パラメータ"));
        } else if (result.error.status === 500) {
          setMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    fetchExportHistory();
  }, []);

  // 確認ダイアログを表示する
  const showConfirmDialog = useCallback(async (placeScheduleId: number) => {
    // 確認ダイアログの結果を取得する
    const confirmResult = await ref.current?.confirm();
    if (confirmResult) {
      setIsLoading(true);
      // PUT時のリクエストボディを生成する
      const body = {
        placeScheduleId: placeScheduleId,
        placeScheduleResultExportStatus: 11,
      };
      // 「未出力確定」ボタン押下時はAP1021_検査結果の連携状態を変更する
      const putMutateAsync = async () => {
        try {
          const result = await mutateAsync({
            version: "1",
            placeScheduleId: placeScheduleId,
            data: body,
          });
          if (result.status === 200) {
            setMessage("未出力状態に変更しました。");
            open();
          }
        } catch (error) {
          if (isAxiosError(error) && error.response) {
            if (error.response.status === 400) {
              setMessage(getErrorMessage(errorMessages.invalid, "パラメータ"));
            } else if (error.response.status === 404) {
              setMessage(getErrorMessage(errorMessages.notFound, "変更情報"));
            } else if (error.response.status === 500) {
              setMessage(getErrorMessage(errorMessages.serverError));
            }
            open();
          }
        } finally {
          setIsLoading(false);
        }
      };
      putMutateAsync();
    }
  }, []);

  const handleClick = (
    placeScheduleId?: number,
    examDate?: string,
    placeName?: string,
    dataCount?: number,
  ) => {
    const confirmResultData = {
      examDate,
      placeName,
      dataCount,
    };
    setConfirmResult(confirmResultData);
    // 確認ダイアログを表示する
    showConfirmDialog(placeScheduleId || 0);
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching || isLoading} />
        <CommonHeader
          screenName="検査結果出力履歴"
          staffName={staff?.name || ""}
        />
        <Container fluid mt={20}>
          {!isFetching && (
            <>
              {exportHistory?.exportHistories ? (
                <Table>
                  <Table.Thead>
                    <Table.Tr>
                      <Table.Th>出力者</Table.Th>
                      <Table.Th>健診日</Table.Th>
                      <Table.Th>会場</Table.Th>
                      <Table.Th>件数</Table.Th>
                    </Table.Tr>
                  </Table.Thead>
                  <Table.Tbody>
                    {exportHistory?.exportHistories?.map((eh) => (
                      <Table.Tr key={eh.placeScheduleId}>
                        <Table.Td>{eh.exportedBy}</Table.Td>
                        <Table.Td>
                          {eh.examDate &&
                            format(
                              parse(eh.examDate, "yyyy-MM-dd", new Date()),
                              "yyyy/MM/dd",
                            )}
                        </Table.Td>
                        <Table.Td>{eh.placeName}</Table.Td>
                        <Table.Td>{eh.dataCount}</Table.Td>
                        <Table.Td>
                          <Button
                            variant="outline"
                            onClick={() =>
                              handleClick(
                                eh.placeScheduleId,
                                eh.examDate,
                                eh.placeName,
                                eh.dataCount,
                              )
                            }
                            mt="xs"
                          >
                            <Title order={2}>未出力</Title>
                          </Button>
                        </Table.Td>
                      </Table.Tr>
                    ))}
                  </Table.Tbody>
                </Table>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Title order={3}>
                    {getErrorMessage(errorMessages.notFound, "出力済みのデータ")}
                  </Title>
                </>
              )}
              <CommonDialog
                isOpen={opened}
                onClose={close}
                message={message || ""}
                buttonMessage="閉じる"
              />
              <ConfirmDialog ref={ref} title="検査結果出力履歴">
                <div>
                  健診日：
                  {confirmResult?.examDate &&
                    format(
                      parse(confirmResult?.examDate, "yyyy-MM-dd", new Date()),
                      "yyyy/MM/dd",
                    )}
                </div>
                <div>会場：{confirmResult?.placeName}</div>
                <div>件数：{confirmResult?.dataCount}</div>
              </ConfirmDialog>
            </>
          )}
        </Container>
        <CommonFooter items={fotterItems} />
      </AuthWrapper>
    </>
  );
}
