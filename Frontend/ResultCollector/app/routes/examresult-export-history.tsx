import {
  Box,
  Button,
  Center,
  Container,
  Group,
  LoadingOverlay,
  Modal,
  Table,
  Text,
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
  useIntegrationUndoExportStatus,
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

// 結果出力確認ダイアログ用の状態型定義
type State = {
  isOpen: boolean; // 表示・非表示用
  resolve: (isOk: boolean) => void; // resolve格納用
};

// 結果出力確認ダイアログ用の状態初期値
const initialState: State = {
  isOpen: false,
  resolve: () => {},
};

// 結果出力確認ダイアログの開閉状態を管理するカスタムフック
const useResultsOutputConfirmState = () => {
  const [{ isOpen, resolve }, setState] = useState<State>(initialState);

  // 結果出力確認ダイアログを起動するための関数
  const resultsOutputConfirm = useCallback(
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
    resultsOutputConfirm,
    handleOk,
    handleCancel,
  };
};

// 親コンポーネントに公開する関数
type Handle = { resultsOutputConfirm: () => Promise<boolean> };

type Props = Omit<ComponentProps<typeof Modal>, "opened" | "onClose">;

// 結果出力確認ダイアログコンポーネント
const ResultsOutputConfirmDialog = forwardRef<Handle, Props>((props, ref) => {
  const { isOpen, resultsOutputConfirm, handleOk, handleCancel } =
    useResultsOutputConfirmState();
  const { children, title } = props;
  useImperativeHandle(ref, () => ({ resultsOutputConfirm }), [
    resultsOutputConfirm,
  ]);

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
        <Button w={200} h={80} variant="outline" onClick={handleCancel} mt="xl">
          <Title order={2}>戻る</Title>
        </Button>
        <Button w={200} h={80} variant="filled" onClick={handleOk} mt="xl">
          <Title order={2}>未出力確定</Title>
        </Button>
      </Center>
    </Modal>
  );
});

interface ResultsOutputConfirmDialogState {
  examDate?: string;
  placeName?: string;
  dataCount?: number;
}

export default function ExamresultExportHistory() {
  const [exportHistory, setExportHistory] = useState<ExportHistoryList>();
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [resultsOutputConfirmDialog, setResultsOutputConfirmDialog] =
    useState<ResultsOutputConfirmDialogState | null>(null);
  const [staff] = useAtom(staffState);
  const [opened, { open, close }] = useDisclosure(false);
  const { mutateAsync } = useIntegrationUndoExportStatus();
  const { isFetching, refetch } = useIntegrationGetExportHistory("1", {
    query: { enabled: false },
  });
  const ref = useRef<ComponentRef<typeof ResultsOutputConfirmDialog>>(null);

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

  // 結果出力確認ダイアログを表示する
  const showResultsOutputConfirmDialog = useCallback(
    async (exportId: string) => {
      // 結果出力確認ダイアログの結果を取得する
      const confirmResult = await ref.current?.resultsOutputConfirm();
      if (confirmResult) {
        setIsLoading(true);
        // POST時のリクエストボディを生成する
        const body = {
          exportId: exportId,
        };
        // 「未出力確定」ボタン押下時はAP1021_出力した結果を未出力に戻す
        const postMutateAsync = async () => {
          try {
            const result = await mutateAsync({
              version: "1",
              data: body,
            });
            if (result.status === 200) {
              setMessage("未出力状態に変更しました。");
              open();
            }
          } catch (error) {
            if (isAxiosError(error) && error.response) {
              if (error.response.status === 400) {
                setMessage(
                  getErrorMessage(errorMessages.invalid, "パラメータ"),
                );
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
        postMutateAsync();
      }
    },
    [],
  );

  const handleClick = (
    exportId?: string,
    examDate?: string,
    placeName?: string,
    dataCount?: number,
  ) => {
    const resultsOutputConfirmDialogData = {
      examDate,
      placeName,
      dataCount,
    };
    setResultsOutputConfirmDialog(resultsOutputConfirmDialogData);
    // 結果出力確認ダイアログを表示する
    showResultsOutputConfirmDialog(exportId || "");
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
                                eh.exportId,
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
                    {getErrorMessage(
                      errorMessages.notFound,
                      "出力済みのデータ",
                    )}
                  </Title>
                </>
              )}
              <CommonDialog
                isOpen={opened}
                onClose={close}
                message={message || ""}
                buttonMessage="閉じる"
              />
              <ResultsOutputConfirmDialog ref={ref} title="検査結果出力履歴">
                <Box>
                  <Group wrap="nowrap">
                    <Text size="xs">健診日：</Text>
                    <Text size="xs">
                      {resultsOutputConfirmDialog?.examDate &&
                        format(
                          parse(
                            resultsOutputConfirmDialog?.examDate,
                            "yyyy-MM-dd",
                            new Date(),
                          ),
                          "yyyy/MM/dd",
                        )}
                    </Text>
                  </Group>
                </Box>
                <Box>
                  <Group wrap="nowrap">
                    <Text size="xs" style={{ whiteSpace: "nowrap" }}>
                      会場：
                    </Text>
                    <Text size="xs">
                      {resultsOutputConfirmDialog?.placeName}
                    </Text>
                  </Group>
                </Box>
                <Box>
                  <Group wrap="nowrap">
                    <Text size="xs" style={{ whiteSpace: "nowrap" }}>
                      件数：
                    </Text>
                    <Text size="xs">
                      {resultsOutputConfirmDialog?.dataCount}
                    </Text>
                  </Group>
                </Box>
              </ResultsOutputConfirmDialog>
            </>
          )}
        </Container>
        <CommonFooter items={fotterItems} />
      </AuthWrapper>
    </>
  );
}
