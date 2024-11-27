import {
  Button,
  Container,
  Flex,
  LoadingOverlay,
  Space,
  Text,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import { useExamMenuGetExamMenus } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import type { ExamMenu, ExamMenuList } from "~/domain/wellship.schemas";
import type { NamedEntity } from "~/interfaces/interfaces";
import { examMenuState, staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "検査メニュー" }];
};

export default function ExamMenuSelect() {
  const navigate = useNavigate();
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [staff] = useAtom(staffState);
  const [menu] = useAtom(examMenuState);
  const [examMenuList, setExamMenuList] = useState<ExamMenuList>();
  const [selectExamMenuList, setSelectExamMenuList] = useState<ExamMenu[]>([]);

  // 選択している検査メニューの状態管理
  const [, setSelectedExamMenuState] = useAtom(examMenuState);

  //AP1006呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = useExamMenuGetExamMenus("1", {
    query: { enabled: false },
  });

  useEffect(() => {
    //AP1006_検査メニュー一覧を取得する
    const fetchExamMenuSelect = async () => {
      const result = await refetch();
      if (result.data) {
        setExamMenuList(result.data.data);

        // 初期表示時、jotaiに検査項目選択リスト(ID、Nameのリスト)が保持されている場合、
        //APIから取得した該当検査項目を選択済みとしてselectExamMenuListに入れておく
        if (!menu) return;
        const menuList: ExamMenu[] = menu.map((examMenu) => ({
          examMenuId: examMenu.id,
          examMenuName: examMenu.name,
        }));
        setSelectExamMenuList(menuList);
      } else if (result.error) {
        if (result.error.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    fetchExamMenuSelect();
  }, []);

  // 検査メニューボタンクリック
  const handleMenuButtonClick = (id: number | undefined) => {
    // 自メニューを取得
    const examMenu = examMenuList?.examMenus?.find((x) => x.examMenuId === id);
    if (!examMenu) {
      return;
    }
    // 押下されたメニューを選択済みか確認する
    const selIndex = selectExamMenuList?.findIndex((x) => x.examMenuId === id);
    if (selIndex === -1) {
      // 存在しない場合はリストに追加する
      setSelectExamMenuList([...selectExamMenuList, examMenu]);
    } else {
      // 存在する場合はリストから削除する
      const updateSelectExamMenuList = [
        ...selectExamMenuList.slice(0, selIndex),
        ...selectExamMenuList.slice(selIndex + 1),
      ];
      setSelectExamMenuList(updateSelectExamMenuList);
    }
  };

  const getSelectedIndex = (id: number | undefined): number => {
    return selectExamMenuList.findIndex(
      (selectExamMenu) => selectExamMenu.examMenuId === id,
    );
  };

  //検査項目名の表示文字数を最大8文字とし、9文字目以降は表示しない処理
  const getDispExamMenuName = (name: string) => {
    let res = "";
    if (name.length > 8) {
      res = name.substring(0, 8);
    } else {
      res = name;
    }
    return res;
  };

  // 開始ボタン押下時処理：選択された検査項目ID、検査項目名のリストをjotaiに保存して次画面へ遷移
  const callbackConfirm = () => {
    if (selectExamMenuList.length > 0) {
      const selectedMenuList: NamedEntity[] = selectExamMenuList.map(
        (examMenu) => ({
          id: examMenu.examMenuId,
          name: examMenu.examMenuName,
        }),
      );
      setSelectedExamMenuState(selectedMenuList);

      // 次画面(受診番号入力)に遷移する
      navigate("/consultnumber-input");
    } else {
      setErrorMessage("検査メニューが選択されていません。");
      open();
      //jotaiに保存している検査項目をクリアする
      setSelectedExamMenuState(null);
    }
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="検査メニュー" staffName={staff?.name || ""} />
        <Container fluid mt={20}>
          {!isFetching && (
            <>
              {examMenuList?.examMenus ? (
                <Flex columnGap={5} rowGap={3} wrap="wrap">
                  {examMenuList?.examMenus?.map((examMenu) => (
                    <Button
                      variant="outline"
                      bg={
                        getSelectedIndex(examMenu.examMenuId) >= 0
                          ? "green03"
                          : "white"
                      }
                      color={
                        getSelectedIndex(examMenu.examMenuId) >= 0
                          ? "primary"
                          : "gray02"
                      }
                      w={300}
                      key={examMenu.examMenuId}
                      onClick={() => handleMenuButtonClick(examMenu.examMenuId)}
                    >
                      <Flex gap={5} wrap="nowrap" align="center">
                        <Text w={30} fw={500} color="primary">
                          {(() => {
                            const index = getSelectedIndex(examMenu.examMenuId);
                            return index >= 0 ? <>{index + 1}</> : null;
                          })()}
                        </Text>
                        {getDispExamMenuName(examMenu?.examMenuName || "")}
                        <Space w={30} />
                      </Flex>
                    </Button>
                  ))}
                </Flex>
              ) : (
                <>
                  {/* エラーメッセージを表示(検査メニュー情報の取得データが0件だった場合) */}
                  <Title order={3}>
                    {getErrorMessage(
                      errorMessages.notFound,
                      "使用できる検査メニュー",
                    )}
                  </Title>
                </>
              )}
              <Space h={20} />
              <Button onClick={() => callbackConfirm()}>開始する</Button>
              <Space h={50} />
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
