import { useEffect, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "@remix-run/react";
import { Button, LoadingOverlay, Stack, Text } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useAtom } from "jotai";
import { type AxiosResponse, isAxiosError } from "axios";
import {
  useConsultGetInputExamItemsExaminee,
  useResultRegisterResults,
  useResultVerifyResults,
} from "~/api/wellship";
import type {
  ExamItemGroup,
  InputExamItem,
  InputExamItems,
  ResultsRequest,
  VerifyExamItems,
} from "~/domain/wellship.schemas";
import {
  connectionEquipmentState,
  examMenuState,
  staffState,
} from "~/store/store";
import AuthWrapper from "~/components/AuthWrapper";
import CommonFooter from "~/components/CommonFooter";
import ExamineeHeader from "~/components/ExamineeHeader";
import BoothNote from "~/components/BoothNote";
import CommonDialog from "~/components/CommonDialog";
import ConfirmDialog from "~/components/ConfirmDialog";
import ExamNumeric from "~/components/ExamNumeric";
import ExamSelect from "~/components/ExamSelect";
import ExamSelectLR from "~/components/ExamSelectLR";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

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
  const [openedCommon, { open: openCommon, close: closeCommon }] =
    useDisclosure(false);
  const [openedConfirm, { open: openConfirm, close: closeConfirm }] =
    useDisclosure(false);
  const [commonMessage, setCommonMessage] = useState<string>("");
  const [confirmMessage, setConfirmMessage] = useState<string>("");
  const [visibleRemeasurement, setVisibleRemeasurement] = useState(true);
  const [disabledRemeasurement, setDisabledRemeasurement] = useState(false);
  //機器連携用
  const [value, setValue] = useState<string>(
    localStorage.getItem("value") || "",
  ); //TODO:jsonにvalueセット方法どうするか
  const [isWatching, setIsWatching] = useState(false); // 監視状態を管理するフラグ

  //AP1009呼び出し用(GET系APIの定義)
  const { refetch } = useConsultGetInputExamItemsExaminee(
    "1",
    consultNumber, //stringになるかも
    { examMenuId: examMenuId },
    { query: { enabled: false } },
  );

  useEffect(() => {
    //受診番号の受け取り確認
    //TODO:仕様変更予定
    if (!consultnumber) {
      setCommonMessage("必要な受診番号がありません");
      openCommon();
      return;
    }
    //検査メニューIDの受け取り確認
    if (!examMenuId) {
      setCommonMessage("必要な検査メニューIDがありません");
      open();
      return;
    }
    setIsLoading(true);

    //AP1009_検査結果入力情報を取得する
    const inputExamItems = async () => {
      const result = await refetch();
      if (result.data) {
        setExamData(result.data.data);
      } else if (result.error) {
        if (result.error.status === 400) {
          setCommonMessage(
            getErrorMessage(errorMessages.invalid, "受診番号と検査項目"),
          );
        } else if (result.error.status === 404) {
          setCommonMessage(getErrorMessage(errorMessages.notFound, "検査項目"));
        } else if (result.error.status === 500) {
          setCommonMessage(getErrorMessage(errorMessages.serverError));
        }
        openCommon();
      }
    };

    inputExamItems();
    setIsLoading(false);
  }, [consultnumber, examMenuId]);

  useEffect(() => {
    if (!examData) return;

    const isValueEmptyForEquipmentLabel = (): boolean => {
      return (
        examData.examItemGroups?.some((group) =>
          group.examItems?.some((item) =>
            item.examItemDetails?.some(
              (detail) => detail.equipmentLabel && detail.value === "",
            ),
          ),
        ) || false
      );
    };

    const handleRemeasurementCheck = () => {
      const isEmpty = isValueEmptyForEquipmentLabel();

      setVisibleRemeasurement(!!connectionEquipmentState); // 機器が選択されていないなら非表示
      setDisabledRemeasurement(isEmpty); // valueが空なら無効化

      if (connectionEquipmentState && isEmpty) {
        setIsWatching(true);
        // TODO: 機器連携アプリへ遷移
      }
    };
    //TODO:初期表示の時だけ処理を走らせるか確認
    handleRemeasurementCheck();
  }, [examData, connectionEquipmentState]);

  //機器連携：監視用
  useEffect(() => {
    const initialValue = localStorage.getItem("value") || "";
    setValue(initialValue);
    // StorageEventの変更を監視する関数
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key === "value") {
        setValue(event.newValue || "");
        //TODO:valueにsetする処理　どこのvalueを更新するかの判定をどうするか

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
    newExamItems: InputExamItem[] | undefined,
    groupIndex: number,
  ) => {
    if (newExamItems) {
      setExamData((prevData) => {
        if (!prevData) {
          return {
            examItemGroups: [],
          };
        }

        // 初期値を設定
        const updatedExamItemGroups = (prevData.examItemGroups || []).map(
          (group, gIndex) => {
            if (gIndex !== groupIndex) {
              return group; // 他のグループはそのまま返す
            }
            return {
              ...group,
              examItems: newExamItems, // 対象のグループの examItems を更新
            };
          },
        );

        return {
          ...prevData,
          examItemGroups: updatedExamItemGroups, // 更新されたグループを反映
        };
      });
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
  const makeBody = (): ResultsRequest => {
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
  const errorBranch = (errorLevel?: number) => {
    if (errorLevel === 3) {
      setCommonMessage("エラーがあります。内容を確認してください"); //改行文字入れるかも
      openCommon();
    } else if (errorLevel === 2) {
      setConfirmMessage("ワーニングがありますが、登録します。よろしいですか。");
      openConfirm();
    } else {
      return;
    }
  };

  //AP1013_検査結果を検証する
  const verifyMutateAsync = useResultVerifyResults().mutateAsync;
  const verifyResults = async () => {
    let result: AxiosResponse<VerifyExamItems>;
    const resultsRequest = makeBody();
    if (!consultnumber) return;
    
    const postMutateAsync = async () => {
      try {
        result = await verifyMutateAsync({
          version: "1",
          consultNumber: consultnumber,
          data: resultsRequest,
        });
        if (result.status === 200) {
          // 正常時の処理
          setConfirmMessage("登録します。よろしいですか。");
          openConfirm();
        }
      } catch (error) {
        let errorMessage = "";
        // AxiosErrorかどうかを確認
        if (isAxiosError(error) && error.response) {
          const status = error.response.status;
          // エラー処理
          if (status === 400) {
            errorMessage = getErrorMessage(
              errorMessages.invalid,
              "受診番号と検査項目",
            );
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
        setCommonMessage(errorMessage);
        openCommon();
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
  const registMutateAsync = useResultRegisterResults().mutateAsync;
  const registerResults = async () => {
    let result: AxiosResponse;
    const resultsRequest = makeBody();
    if (!consultnumber) return;
    const postMutateAsync = async () => {
      try {
        result = await registMutateAsync({
          version: "1",
          consultNumber: consultnumber,
          data: resultsRequest,
        });
        if (result.status === 200) {
          // 正常時の処理
          continuingExam();
        }
      } catch (error) {
        let errorMessage = "";
        // AxiosErrorかどうかを確認
        if (isAxiosError(error) && error.response) {
          const status = error.response.status;
          // エラー処理
          if (status === 400) {
            errorMessage = getErrorMessage(
              errorMessages.invalid,
              "指定した受診番号",
            );
          } else if (status === 403) {
            errorMessage = "会場ロック中です。管理者のみ更新可能です。";
          } else if (status === 404) {
            errorMessage = getErrorMessage(
              errorMessages.notFound,
              "指定した受診情報",
            );
          } else if (status === 500) {
            errorMessage = getErrorMessage(errorMessages.serverError);
          }
        }
        // 共通ダイアログにエラーメッセージを表示
        setCommonMessage(errorMessage);
        openCommon();
      }
    };
    postMutateAsync();
  };

  //登録処理
  const callbackRegister = () => {
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
      return <Text>検査項目がありません</Text>;
    }

    // 共通のコールバック関数
    const handleChange = (updatedExamItem: InputExamItem[] | undefined) =>
      callbackChangeValue(updatedExamItem, groupIndex);

    // typeによるコンポーネントの切り替え
    switch (type) {
      case 1:
        return (
          <ExamNumeric
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 2:
        return (
          <ExamSelect
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onClick={handleChange}
          />
        );
      case 5:
        return (
          <ExamNumericLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 6:
        return (
          <ExamSelectLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onClick={handleChange}
          />
        );
      case 7:
        return (
          <ExamBody
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 9:
        return (
          <ExamBP2
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 11:
        return (
          <ExamVision
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 12:
        return (
          <ExamHearing
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case 13:
        setConfirmMessage("実施済みです。取消してよろしいですか。");
        return null; // UIのレンダリングをスキップ
      default:
        return <Text>未対応のタイプ: {type}</Text>;
    }
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <ExamineeHeader
          staffName={staffData?.name ?? ""}
          managerId={staffData?.id ?? 0}
          name={examData?.examinee?.kanaName ?? ""}
          gender={examData?.examinee?.sex ?? 0}
          age={examData?.examinee?.examDateAge ?? 0}
        />
        <BoothNote relatedExamItems={examData?.relatedExamItems ?? []} />
        <Stack align="center" gap={32} px={32} mt={32}>
          {visibleRemeasurement && (
            <Button
              ml="auto"
              disabled={disabledRemeasurement}
              w={184}
              h={75}
              size="lg"
              fw={700}
              variant="outline"
              bg="white"
              color="gray03"
              c="black"
              onClick={handleRemeasurement}
            >
              測定する
            </Button>
          )}
          {examData?.examItemGroups?.map(
            (examItemGroup: ExamItemGroup, index: number) => (
              <ExamItemRender
                key={index}
                groupIndex={index}
                examItemGroup={examItemGroup}
              />
            ),
          )}
          <Button w={860} h={75} my={30} onClick={handleVerify}>
            登録する
          </Button>
        </Stack>

        <ConfirmDialog
          message={confirmMessage}
          confirmButtonMessage={"登録する"}
          isOpen={openedConfirm}
          onCancel={closeConfirm}
          onConfirm={callbackRegister}
        />
        <CommonDialog
          message={commonMessage}
          buttonMessage={"確認する"}
          isOpen={openedCommon}
          onClose={closeCommon}
        />
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}
