import { useEffect, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "@remix-run/react";
import { Button, LoadingOverlay, Stack, Text } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useAtom } from "jotai";
import { type AxiosResponse, isAxiosError } from "axios";
import {
  useConsultGetInputExamItemsExaminee,
  useConsultRegisterResults,
  useConsultVerifyResults,
} from "~/api/wellship";
import type {
  ExamItemGroup,
  InputExamItem,
  InputExamItems,
  ResultsRequest,
  VerifyExamItems,
} from "~/domain/wellship.schemas";
import { InputErrorLevel, ExamItemGroupType } from "~/domain/enums";
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
import ExamBP2 from "~/components/ExamBP2";
import ExamFreeInput from "~/components/ExamFreeInput";
import ExamNumericLR from "~/components/ExamNumericLR";
import ExamSelectLR from "~/components/ExamSelectLR";
import ExamBody from "~/components/ExamBody";
import ExamVision from "~/components/ExamVision";
import ExamHearing from "~/components/ExamHearing";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export default function ConsultInput() {
  const navigate = useNavigate();
  const [examData, setExamData] = useState<InputExamItems>();
  // パスパラメータの取得
  const consultNumber = useParams().consultnumber ?? undefined;
  // クエリパラメータの取得
  const [searchParams] = useSearchParams();
  const examMenuId = Number(searchParams.get("exammenuid"));
  //APIのバージョン
  const apiVersion = "1";
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
  const [commonButtonMessage, setCommonButtonMessage] = useState<string>("");
  const [commonBrowserbackFlag, setCommonBrowserbackFlag] = useState(false);
  // 通過用
  const [hasPass, setHasPass] = useState(false);
  const [passValue, setPassValue] = useState("");
  //測定ボタン用
  const [visibleRemeasurement, setVisibleRemeasurement] = useState(true);
  const [disabledRemeasurement, setDisabledRemeasurement] = useState(false);
  //機器連携用
  const localStorageKey = "value"; //TODO:Keyは未確定
  const [value, setValue] = useState<string>(
    localStorage.getItem(localStorageKey) || "",
  ); //TODO:jsonにvalueセット方法どうするか
  const [isWatching, setIsWatching] = useState(false); // 監視状態を管理するフラグ

  //AP1009呼び出し用(GET系APIの定義)
  const { refetch } = useConsultGetInputExamItemsExaminee(
    apiVersion,
    consultNumber ?? "",
    { examMenuId: examMenuId },
    { query: { enabled: false } },
  );

  useEffect(() => {
    setCommonBrowserbackFlag(true);
    //受診番号の受け取り確認
    if (!consultNumber) {
      setCommonMessage("必要な受診番号がありません");
      setCommonButtonMessage("閉じる");
      openCommon();
      return;
    }
    //検査メニューIDの受け取り確認
    if (!examMenuId) {
      setCommonMessage("必要な検査メニューIDがありません");
      setCommonButtonMessage("閉じる");
      openCommon();
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
          setCommonMessage(getErrorMessage(errorMessages.invalid, "受診番号"));
        } else if (result.error.status === 404) {
          setCommonMessage(getErrorMessage(errorMessages.notFound, "検査項目"));
        } else if (result.error.status === 500) {
          setCommonMessage(getErrorMessage(errorMessages.serverError));
        }
        setCommonButtonMessage("閉じる");
        openCommon();
      }
    };

    inputExamItems();
    setIsLoading(false);
    setCommonBrowserbackFlag(false);
  }, [consultNumber, examMenuId]);

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
    const initialValue = localStorage.getItem(localStorageKey || "");
    setValue(initialValue ?? "");
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

  //通過のvalueを更新
  const updatedPassValue = (examData: InputExamItems) => {
    const updatedInputExamItems: InputExamItems = {
      ...examData, // 最新のexamDataを参照
      examItemGroups: examData?.examItemGroups?.map((group) => {
        if (group.type === ExamItemGroupType.通過) {
          return {
            ...group,
            examItems: group.examItems?.map((item) => {
              if (item.positionNumber === 1) {
                return {
                  ...item,
                  examItemDetails: item.examItemDetails?.map((detail) =>
                    detail.positionNumber === 1
                      ? {
                          ...detail,
                          value: passValue, // 新しい値をセット
                        }
                      : detail,
                  ),
                };
              }
              return item;
            }),
          };
        }
        return group;
      }),
    };
    return updatedInputExamItems;
  };

  //リクエストボディ作成
  const makeBody = (): ResultsRequest => {
    let updatedExamData = { ...examData };
    //通過が存在する場合、通過のvalueを更新
    if (hasPass) {
      updatedExamData = updatedPassValue(updatedExamData);
    }
    if (updatedExamData) {
      const converted = {
        examMenuId: examMenuId,
        examResults: updatedExamData.examItemGroups
          ? updatedExamData.examItemGroups?.flatMap(
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
    if (errorLevel === InputErrorLevel.異常) {
      setCommonMessage("エラーがあります。内容を確認してください"); //改行文字入れるかも
      setCommonButtonMessage("閉じる");
      openCommon();
    } else if (errorLevel === InputErrorLevel.警告) {
      setConfirmMessage("ワーニングがありますが、登録します。よろしいですか。");
      openConfirm();
    } else {
      return;
    }
  };

  //AP1013_検査結果を検証する
  const verifyMutateAsync = useConsultVerifyResults().mutateAsync;
  const verifyResults = async () => {
    let result: AxiosResponse<VerifyExamItems>;
    const resultsRequest = makeBody();
    if (!consultNumber) return;

    const postMutateAsync = async () => {
      try {
        result = await verifyMutateAsync({
          version: apiVersion,
          consultNumber: consultNumber,
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
            errorMessage = getErrorMessage(errorMessages.invalid, "受診番号");
          } else if (status === 404) {
            errorMessage = getErrorMessage(errorMessages.notFound, "検査項目");
          } else if (status === 422) {
            setExamData(result.data);
            // 最大 errorLevel を取得
            const maxErrorLevel = Math.max(
              0, // デフォルト値として 0 を指定
              ...(result?.data?.examItemGroups?.flatMap(
                (group) =>
                  group.examItems?.flatMap(
                    (item) =>
                      item.examRegistResults
                        ?.map((result) => result.errorLevel)
                        .filter(
                          (level): level is number => level !== undefined,
                        ) || [],
                  ) || [],
              ) || []),
            );
            errorBranch(maxErrorLevel);
          } else if (status === 500) {
            errorMessage = getErrorMessage(errorMessages.serverError);
          }
        }
        // 共通ダイアログにエラーメッセージを表示
        setCommonMessage(errorMessage);
        setCommonButtonMessage("閉じる");
        openCommon();
      }
    };
    postMutateAsync();
  };

  //検証処理
  const handleVerify = () => {
    setIsLoading(true);
    //AP1013_検査結果を検証する
    verifyResults();
    setIsLoading(false);
  };

  // 検査継続処理
  const continuingExam = () => {
    if (!examMenus) return;
    const currentIndex = examMenus.findIndex((menu) => menu === examMenuId);
    if (currentIndex !== -1 && currentIndex < examMenus.length - 1) {
      const nextExam = examMenus[currentIndex + 1];
      navigate(`/examorder-confirm/${consultNumber}?exammenuid=${nextExam}`);
    } else {
      navigate(`/consultnumber-input?consultnumber=${consultNumber}`);
    }
  };

  //AP1014_検査結果を登録する
  const registMutateAsync = useConsultRegisterResults().mutateAsync;
  const registerResults = async () => {
    let result: AxiosResponse;
    const resultsRequest = makeBody();
    if (!consultNumber) return;
    const postMutateAsync = async () => {
      try {
        result = await registMutateAsync({
          version: apiVersion,
          consultNumber: consultNumber,
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
            errorMessage = getErrorMessage(errorMessages.invalid, "回答登録の");
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
        setCommonButtonMessage("閉じる");
        openCommon();
      }
    };
    postMutateAsync();
  };

  //登録処理
  const callbackRegister = () => {
    setIsLoading(true);
    //AP1014_検査結果を登録する
    registerResults();
    setIsLoading(false);
  };

  //共通ダイアログ：閉じる処理
  const callbackCloseCommon = () => {
    closeCommon();
    if (commonBrowserbackFlag) {
      navigate(-1);
    }
  };

  //確認ダイアログ：閉じる処理
  const callbackCloseConfirm = () => {
    closeConfirm();
    if (hasPass) {
      navigate(-1);
    }
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
    //通過フラグをリセット
    setHasPass(false);
    // 共通のコールバック関数
    const handleChange = (updatedExamItem: InputExamItem[] | undefined) =>
      callbackChangeValue(updatedExamItem, groupIndex);

    // typeによるコンポーネントの切り替え
    switch (type) {
      case ExamItemGroupType.数値:
        return (
          <ExamNumeric
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.選択:
        return (
          <ExamSelect
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onClick={handleChange}
          />
        );
      case ExamItemGroupType.血圧2回:
        return (
          <ExamBP2
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.自由入力:
        return (
          <ExamFreeInput
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.数値_左右:
        return (
          <ExamNumericLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.選択_左右:
        return (
          <ExamSelectLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onClick={handleChange}
          />
        );
      case ExamItemGroupType.身体計測:
        return (
          <ExamBody
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.視力:
        return (
          <ExamVision
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.聴力:
        return (
          <ExamHearing
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={true}
            onClick={handleChange}
          />
        );
      case ExamItemGroupType.通過: {
        setHasPass(true);
        //valueを取得
        const detailValue = examItems
          ?.find((item) => item.positionNumber === 1)
          ?.examItemDetails?.find(
            (detail) => detail.positionNumber === 1,
          )?.value;
        // 設定するvalueをset
        const newValue = detailValue === "1" ? "" : "1";
        setPassValue(newValue);

        // メッセージの設定
        if (detailValue === "1") {
          setConfirmMessage("実施済みです。取消してよろしいですか。");
        } else {
          setConfirmMessage("登録します。よろしいですか。");
        }

        openConfirm();
        return null;
      }
      default:
        return;
    }
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <ExamineeHeader
          staffName={staffData?.name ?? ""}
          managerNo={examData?.examinee?.ticketNumber ?? ""}
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
          confirmButtonMessage={"OK"}
          isOpen={openedConfirm}
          onCancel={callbackCloseConfirm}
          onConfirm={callbackRegister}
        />
        <CommonDialog
          message={commonMessage}
          buttonMessage={commonButtonMessage}
          isOpen={openedCommon}
          onClose={callbackCloseCommon}
        />
        <CommonFooter />
      </AuthWrapper>
    </>
  );
}
