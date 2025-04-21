import { type JSX, useEffect, useState } from "react";
import { useNavigate, useSearchParams, type MetaFunction } from "react-router";
import {
  Badge,
  Button,
  Center,
  Container,
  Divider,
  Flex,
  LoadingOverlay,
  Paper,
  Space,
  Stack,
  Table,
  Text,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import {
  IconArrowsSort,
  IconSortAscending,
  IconSortDescending,
} from "@tabler/icons-react";
import { format, parseISO } from "date-fns";
import { useAtom } from "jotai";
import { placeScheduleState, staffState } from "~/store/store";
import { useExamineeGetConsultExaminees } from "~/api/wellship";
import type { ConsultExamineeList } from "~/domain/wellship.schemas";
import { Sex } from "~/domain/enums";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import ExamItemProgress from "~/components/ExamItemProgress";
import PlaceSchedule from "~/components/PlaceSchedule";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";
import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
  return [{ title: "受診者一覧" }];
};

type SortKey = "ticketNumber" | "kanaName" | null;
type SortOrder = "asc" | "desc" | null;

export default function Examinees() {
  const navigate = useNavigate();
  const [examineesData, setExamineesData] = useState<ConsultExamineeList>();
  // クエリパラメータの取得
  const [searchParams] = useSearchParams();
  const examMenuId = Number(searchParams.get("exammenuid"));
  const status = Number(searchParams.get("status"));
  const [selectedStatus,setSelectedStatus] = useState<number>(status);
  //職員の状態管理
  const [staffData] = useAtom(staffState);
  //会場の状態管理
  const [placeData] = useAtom(placeScheduleState);
  //ローディング管理
  const [isLoading, setIsLoading] = useState(true);
  //共通ダイアログ表示管理
  const [openedCommon, { open: openCommon, close: closeCommon }] =
    useDisclosure(false);
  // 共通ダイアログで表示するメッセージ
  const [commonMessage, setCommonMessage] = useState<string>("");
  //共通ダイアログのボタンラベル
  const [commonButtonMessage, setCommonButtonMessage] = useState<string>("");
  // 共通ダイアログのボタン押下時にブラウザバック処理を追加するフラグ
  const [commonBrowserbackFlag, setCommonBrowserbackFlag] = useState(false);
  //ソート制御
  const [sortKey, setSortKey] = useState<SortKey>(null);
  const [sortOrder, setSortOrder] = useState<SortOrder>(null);

  //共通ダイアログ表示
  const openCommonDialog = (message: string, buttonMessage: string) => {
    setCommonMessage(message);
    setCommonButtonMessage(buttonMessage);
    openCommon();
  };

  //AP1024呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = useExamineeGetConsultExaminees(
    "1",
    placeData?.placeScheduleId ?? "",
    examMenuId,
    { status: selectedStatus },
    { query: { enabled: false } },
  );

  //AP1024_受診者一覧を取得する
  const getExamineesData = async () => {
    const result = await refetch();
    if (result.data) {
      const responseData = result.data.data;
      setExamineesData(responseData);
      setCommonBrowserbackFlag(false); //正常処理のため、ブラウザバックフラグをfalseに
    } else if (result.error) {
      let message = "";
      if (result.error.status === 404) {
        message = getErrorMessage(errorMessages.notFound, "該当する進捗状況");
      } else if (result.error.status === 500) {
        message = getErrorMessage(errorMessages.serverError);
      }
      openCommonDialog(message, "閉じる");
    }
  };

  useEffect(() => {
    setIsLoading(isFetching);
  }, [isFetching]);

  useEffect(() => {
    setCommonBrowserbackFlag(true);
    //検査メニューIDの受け取り確認
    if (!examMenuId) {
      openCommonDialog("必要な検査メニューIDがありません。", "閉じる");
      return;
    }
    //進捗状況の受け取り確認
    if (!status) {
      openCommonDialog("必要な進捗状況がありません。", "閉じる");
      return;
    }
    getExamineesData();
  }, [examMenuId, selectedStatus]);

  //進捗ステータスボタン押下時
  const refetchExamineeList = (status: number) => {
    //進捗状況を再設定
    setSelectedStatus(status);
  };

  //共通ダイアログのボタン押下時
  const closeCommonDialog = () => {
    closeCommon();
    if (commonBrowserbackFlag) {
      navigate(-1);
    }
  };

  // ソートトグル関数
  const toggleSort = (key: SortKey) => {
    if (sortKey !== key) {
      setSortKey(key);
      setSortOrder("asc");
    } else {
      if (sortOrder === "asc") setSortOrder("desc");
      else if (sortOrder === "desc") {
        setSortOrder(null);
        setSortKey(null);
      } else setSortOrder("asc");
    }
  };

  // ソート処理
  const sorted = [...(examineesData?.examinees ?? [])].sort((a, b) => {
    if (!sortKey || !sortOrder) return 0;

    const aVal = a[sortKey] ?? "";
    const bVal = b[sortKey] ?? "";

    if (sortOrder === "asc") return aVal.localeCompare(bVal);
    if (sortOrder === "desc") return bVal.localeCompare(aVal);
    return 0;
  });

  //ソートアイコン設定
  const getSortIcon = (key: SortKey): JSX.Element => {
    if (sortKey !== key) return <IconArrowsSort size={32} />;

    switch (sortOrder) {
      case "asc":
        return <IconSortAscending size={32} />;
      case "desc":
        return <IconSortDescending size={32} />;
      default:
        return <IconArrowsSort size={32} />;
    }
  };

  const targetStatus = examineesData?.progress?.details?.find(
    (d) => d.status === selectedStatus,
  );
  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <CommonHeader
          screenName="受診者一覧"
          staffName={staffData?.name ?? ""}
        />
        {!isFetching && (
          <Container fluid bg="background" py={32} px={0}>
            <Stack px={24} gap={24} mb={24}>
              <PlaceSchedule
                placeName={examineesData?.placeName ?? ""}
                examDate={examineesData?.examDate ?? ""}
              />
              <ExamItemProgress
                progress={examineesData?.progress ?? {}}
                onClick={refetchExamineeList}
              />
            </Stack>
            <Stack gap={0}>
              <Flex justify="space-between" align="flex-end" px={32}>
                <Paper
                  w={200}
                  h={60}
                  bg="gray02"
                  c="white"
                  radius="itemName"
                  py={8}
                >
                  <Text size="lg" fw={700} ta="center">
                    {targetStatus?.statusName}
                  </Text>
                </Paper>
                <Text mr={10}>{examineesData?.examinees?.length}件表示中</Text>
              </Flex>
              <Center>
                <Divider w="97%" bd="2px solid gray03" my={8} ta="center" />
              </Center>
              {examineesData?.examinees &&
              examineesData.examinees.length > 0 ? (
                <Table bg="white01" striped="even" stripedColor="stripe">
                  <Table.Thead
                    h={80}
                    bg="background"
                    fz="xs"
                    fw={700}
                    c="black01"
                  >
                    <Table.Tr>
                      <Table.Th ta="center">受診番号</Table.Th>
                      <Table.Th ta="center">性別</Table.Th>
                      <Table.Th ta="center">
                        <Button
                          variant="subtle"
                          color="black01"
                          onClick={() => toggleSort("ticketNumber")}
                          rightSection={getSortIcon("ticketNumber")}
                        >
                          受付番号
                        </Button>
                      </Table.Th>
                      <Table.Th ta="center">
                        <Button
                          variant="subtle"
                          color="black01"
                          onClick={() => toggleSort("kanaName")}
                          rightSection={getSortIcon("kanaName")}
                        >
                          名前
                        </Button>
                      </Table.Th>
                      <Table.Th ta="center">受付日時</Table.Th>
                    </Table.Tr>
                  </Table.Thead>
                  <Table.Tbody h={118} fz="xs" c="black01">
                    {sorted?.map((examinee) => (
                      <Table.Tr
                        key={examinee.consultNumber}
                        onClick={() => {
                          navigate(
                            `/examorder-confirm/${examinee.consultNumber}?exammenuid=${examMenuId}&status=${selectedStatus}`,
                          );
                        }}
                      >
                        <Table.Td w="15%" ta="center">
                          {examinee.consultNumber}
                        </Table.Td>
                        <Table.Td w="15%" ta="center">
                          <Badge
                            w={90}
                            h={45}
                            c="black01"
                            color={
                              examinee.sex === Sex.男
                                ? "malePrimary"
                                : examinee.sex === Sex.女
                                  ? "femalePrimary"
                                  : "green02"
                            }
                          >
                            <Text size="20">
                              {examinee.sex === Sex.男
                                ? "男性"
                                : examinee.sex === Sex.女
                                  ? "女性"
                                  : "その他"}
                            </Text>
                          </Badge>
                        </Table.Td>
                        <Table.Td w="15%" ta="center">
                          {examinee.ticketNumber}
                        </Table.Td>
                        <Table.Td
                          w="27.5%"
                          pl="15"
                          className={styles["text-wrap"]}
                        >
                          {examinee.kanaName}
                        </Table.Td>
                        <Table.Td w="27.5%" ta="center">
                          {examinee.checkedInAt
                            ? format(
                                parseISO(examinee.checkedInAt),
                                "yyyy/M/d HH:mm:ss",
                              )
                            : ""}
                        </Table.Td>
                      </Table.Tr>
                    ))}
                  </Table.Tbody>
                </Table>
              ) : (
                <>
                  {/* エラーメッセージを表示 */}
                  <Text size="sm" c="black01" ml={32} mt={24}>
                    {getErrorMessage(errorMessages.notFound, "受診者情報")}
                  </Text>
                </>
              )}
            </Stack>
          </Container>
        )}
        <Space h={100} />
        <CommonFooter />
        <CommonDialog
          message={commonMessage}
          buttonMessage={commonButtonMessage}
          isOpen={openedCommon}
          onClose={closeCommonDialog}
        />
      </AuthWrapper>
    </>
  );
}
