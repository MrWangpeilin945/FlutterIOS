import {
  Button,
  Center,
  Flex,
  Grid,
  GridCol,
  LoadingOverlay,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useNavigate, useParams, useSearchParams } from "@remix-run/react";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import BoothNote from "~/components/BoothNote";
import CommonFooter from "~/components/CommonFooter";
import { ErrorModal } from "~/components/ErrorModal";
import ExamineeHeader from "~/components/ExamineeHeader";
import ExamNumeric from "~/components/ExamNumeric";
import ExamSelect from "~/components/ExamSelect";
import {
  connectionEquipmentState,
  examMenuState,
  staffState,
} from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";
import {
  resultRegisterResults,
  resultVerifyResults,
  useConsultGetInputExamItemsExaminee,
  useResultVerifyResults,
} from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import type {
  ExamItemDetailOption,
  ExamItemGroup,
  InputExamItem,
  InputExamItems,
  ResultRequest,
  ResultsRequest,
  VerifyExamItems,
} from "~/domain/wellship.schemas";
import { type AxiosResponse, isAxiosError } from "axios";

export default function consultInput() {
  const navigate = useNavigate();
  const [examData, setExamData] = useState<InputExamItems>();
  // パスパラメータの取得
  const { consultnumber } = useParams();
  const [consultNumber] = useState(Number(consultnumber));
  // クエリパラメータの取得
  const [searchParams] = useSearchParams();
  const examMenuId = Number(searchParams.get("exammenuid"));
  const [staffData] = useAtom(staffState);
  const [examMenus] = useAtom(examMenuState);
  const [ConnectionEquipment] = useAtom(connectionEquipmentState);
  const [isLoading, setIsLoading] = useState(false);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [showRemeasurement, setShowRemeasurement] = useState<boolean>(false);
  //機器連携用
  const [value, setValue] = useState<string>(
    localStorage.getItem("value") || "",
  ); //jsonにvalueセット方法どうするか
  const [isWatching, setIsWatching] = useState(false); // 監視状態を管理するフラグ

  useEffect(() => {
    setIsLoading(true);
    //受診番号の受け取り確認
    if (!consultnumber) {
      setErrorMessage("必要な受診番号がありません");
      open();
      return;
    }
    //検査メニューIDの受け取り確認
    if (!examMenuId) {
      setErrorMessage("必要な検査メニューIDがありません");
      open();
      return;
    }
    //AP1009_検査結果入力情報を取得する
    const inputExamItems = async () => {
      const { refetch } = useConsultGetInputExamItemsExaminee(
        "1",
        consultNumber, //stringになるかも
        { examMenuId: examMenuId },
        { query: { enabled: false } },
      );

      const result = await refetch();
      if (result.data) {
        setExamData(result.data.data);
      } else if (result.error) {
        if (result.error.status === 400) {
          setErrorMessage(
            getErrorMessage(errorMessages.invalid, "受診番号と検査項目"),
          );
        } else if (result.error.status === 404) {
          setErrorMessage(getErrorMessage(errorMessages.notFound, "検査項目"));
        } else if (result.error.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      }
    };
    inputExamItems();
    setIsLoading(false);
  }, [consultnumber, examMenuId]);

  const callbackCloseModal = () => {
    close();
    if (!consultnumber || !examMenuId) {
      navigate(-1);
    }
  };

  //機器連携：監視用
  useEffect(() => {
    const initialValue = localStorage.getItem("value") || "";
    setValue(initialValue);
    // StorageEventの変更を監視する関数
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key === "value") {
        setValue(event.newValue || "");
        //TODO:valueにsetする処理　どこのvalueを更新するかの判定をどうするか
        console.log(event);

        // 初回の変更後に監視を解除
        if (isWatching) {
          setIsWatching(false);
        }
      }
    };

    // isWatchingがtrueのときに監視を開始
    if (isWatching) {
      window.addEventListener("storage", handleStorageChange);
    }

    // クリーンアップ処理
    return () => {
      window.removeEventListener("storage", handleStorageChange);
    };
  }, [isWatching]); // isWatchingが変わるたびに監視の開始/停止

  //値変更用
  const callbackChangeValue = (
    examItem: InputExamItem | undefined,
    index: number,
    groupIndex: number,
  ) => {
    if (examItem) {
      setExamData((prevData) => {
        if (!prevData) {
          return {
            examItemGroups: [],
          };
        }

        // 初期値を設定しつつ安全に処理
        const updatedExamItemGroups = (prevData.examItemGroups || []).map(
          (group, gIndex) => {
            if (gIndex !== groupIndex) {
              return group;
            }
            return {
              ...group,
              examItems: (group.examItems || []).map((item, itemIndex) => {
                // 指定された index のみ更新
                return itemIndex === index ? examItem : item;
              }),
            };
          },
        );

        return {
          ...prevData,
          examItemGroups: updatedExamItemGroups,
        };
      });
    }
    // 再計測ボタンのDisabledチェック処理
    if (!connectionEquipmentState /* && TODO:equipmentNameの仕様を確認*/) {
      setShowRemeasurement(true);
    } else {
      setIsWatching(true);
      if (!showRemeasurement) {
        // TODO:機器連携アプリへ遷移
      }
    }
  };

  //TODO：機器連携
  //再計測ボタン押下時
  const handleRemeasurement = () => {
    setIsWatching(true);
    //TODO:機器連携アプリへ遷移
    //TODO:js呼び出し処理以降
  };

  //リクエストボディ作成
  const makeBody = ():ResultsRequest => {
    if (examData) {
      const converted = {
        examMenuId: examMenuId,
        examResults: examData.examItemGroups
          ? examData.examItemGroups.flatMap(
              (group) =>
                group.examItems?.map((examItem) => ({
                  examItemId: examItem.examItemId,
                  examItemDetails: examItem.examItemDetails
                    ? examItem.examItemDetails.map((itemDetail) => ({
                        examItemDetailId: itemDetail.examItemDetailId,
                        value: itemDetail.value || "",
                      }))
                    : [],
                })) ?? [],
            )
          : [],
      };
      return converted;
    }
    return {};
  };

  //エラーレベルによる分岐処理
  const errorBranch = (errorLevel?:number) => {
    if(errorLevel === 3){
      setErrorMessage("エラーがあります。内容を確認してください"); //改行文字入れるかも
      open();
    }else if(errorLevel === 2){
      //TODO:確認ダイアログ呼び出し　OKの時に検査結果確認画面　キャンセルでclose
    }else{
      return;
    }
  };

  //AP1013_検査結果を検証する
  const verifyResults = async () => {
    let result:AxiosResponse<VerifyExamItems>;
    const resultsRequest = makeBody();
    if (!consultnumber) return;
    const { mutateAsync } = useResultVerifyResults();
    const postMutateAsync = async () => {
      try {
        result = await mutateAsync({
          version: "1",
          consultNumber: consultnumber,
          data: resultsRequest,
        });
        if (result.status === 200) {
          // 正常時の処理
          //TODO:検査結果確認画面へ遷移
        }
      } catch (error) {
        let errorMessage = "";
        // AxiosErrorかどうかを確認
        if (isAxiosError(error) && error.response) {
          const status = error.response.status;
          // エラー処理
          if (status === 400) {
            errorMessage =  getErrorMessage(errorMessages.invalid, "受診番号と検査項目");
          } else if (status === 404) {
            errorMessage = getErrorMessage(errorMessages.notFound, "検査項目");
          } else if (status === 422) {
            setExamData(result.data);
            errorBranch(result.data.errorLevel);
          } else if (status === 500) {
            errorMessage = getErrorMessage(errorMessages.serverError);
          }
        }
        // 共通ダイアログにエラーメッセージを表示
        setErrorMessage(errorMessage);
        open();
      } 
    };
    postMutateAsync();
  };

  //検証処理
  const handleVerify = () => {
    setIsLoading(true);
    verifyResults();
    setIsLoading(false);
  };

  //検査継続処理
  const continuingExam = () => {
    if (!examMenus) return;
    const currentIndex = examMenus.findIndex((menu) => menu === examMenuId);
    if (currentIndex !== -1 && currentIndex < examMenus.length - 1) {
      const nextExam = examMenus[currentIndex + 1];
      navigate(`/examorder-confirm/${consultnumber}?exammenuid=${nextExam}`);
    } else {
      navigate(`/consultnumber-input?consultnumber=${consultnumber}`);
    }
  };

  //AP1014_検査結果を登録する
  const registerResults = async () => {
    const resultsRequest = makeBody();
    if (!consultnumber) return;
    await resultRegisterResults("1", consultnumber, resultsRequest)
      .then(() => {
        // 成功時の処理
        continuingExam();
      })
      .catch((error) => {
        if (error.responce.status === 400) {
          setErrorMessage(
            getErrorMessage(errorMessages.invalid, "指定した受診番号"),
          );
        } else if (error.responce.status === 403) {
          setErrorMessage("会場ロック中です。管理者のみ更新可能です。");
        } else if (error.responce.status === 404) {
          setErrorMessage(
            getErrorMessage(errorMessages.notFound, "指定した受診情報が"),
          );
        } else if (error.responce.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.serverError));
        }
        open();
      });
  };

  //登録処理
  const handleRegister = () => {
    setIsLoading(true);
    registerResults();
    setIsLoading(false);
  };

  const ExamItemRender = ({
    groupIndex,
    examItemGroup,
  }: {
    groupIndex: number;
    examItemGroup: ExamItemGroup;
  }) => {
    const { type, examItems } = examItemGroup;

    // 検査項目が存在しない場合のチェック
    if (!examItems || examItems.length === 0) {
      return <div>検査項目がありません</div>;
    }

    // `examItems` をループして表示
    const renderedItems: JSX.Element[] = [];

    for (let index = 0; index < examItems.length; index++) {
      const item = examItems[index];

      // 単一選択の場合
      if (type === 2) {
        renderedItems.push(
          <ExamSelect
            key={index} // key を設定
            examItems={item}
            onRegisterPressed={1}
            onClick={(updatedExamItem) =>
              callbackChangeValue(updatedExamItem, index, groupIndex)
            }
          />,
        );
      }

      // 通過（typeが13の場合）: 処理を中断する
      else if (type === 13) {
        //TODO:検査結果確認画面へ遷移
        break;
      }

      // 他のtypeに対する処理
      else {
        renderedItems.push(
          <div key={index}>
            未対応のタイプ: {type}（項目ID: {index}）
          </div>,
        );
      }
    }

    return <div>{renderedItems}</div>;
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <ExamineeHeader
          staffName={staffData?.name ? staffData.name : ""}
          managerId={100001}
          name="リョウビ タロウ"
          gender={1}
          age={35}
        />
        <BoothNote relatedExamItems={examData?.relatedExamItems ?? []} />
        <Grid>
          <GridCol span="auto" />
          <GridCol span="content">
            <Flex align="center" direction="column" my={10}>
              {examData?.examItemGroups?.map(
                (examItemGroup: ExamItemGroup, index: number) => (
                  <ExamItemRender
                    key={index}
                    groupIndex={index}
                    examItemGroup={examItemGroup}
                  />
                ),
              )}
              <Center>
                <Button w={860} h={75} my={30} onClick={handleVerify}>
                  登録する
                </Button>
              </Center>
            </Flex>
          </GridCol>
          <GridCol span="auto">
            <Button
              disabled={showRemeasurement}
              w={100}
              onClick={handleRemeasurement}
            >
              再計測
            </Button>
          </GridCol>
        </Grid>
        {/* TODO:確認ダイアログの実装、onclickで登録処理呼び出し */}
        <ErrorModal
          isOpen={opened}
          onClose={callbackCloseModal}
          errorMessage={errorMessage}
        />
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}

  );
}
