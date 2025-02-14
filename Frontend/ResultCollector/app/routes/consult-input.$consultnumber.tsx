import { useEffect, useRef, useState } from "react";
import {
  type MetaFunction,
  useNavigate,
  useParams,
  useSearchParams,
} from "@remix-run/react";
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

export const meta: MetaFunction = () => {
  return [{ title: "検査結果入力" }];
};

export default function ConsultInput() {
  const navigate = useNavigate();
  const examData = useRef<InputExamItems>();
  const [, setRendering] = useState(false);
  // パスパラメータの取得
  const consultNumber = useParams().consultnumber ?? undefined;
  // クエリパラメータの取得
  const [searchParams] = useSearchParams();
  const examMenuId = Number(searchParams.get("exammenuid"));
  // 登録ボタンフラグ
  const [isRegisterPressed, setIsRegisterPressed] = useState(false);
  //APIのバージョン
  const apiVersion = "1";
  //職員の状態管理
  const [staffData] = useAtom(staffState);
  //検査メニューの状態管理
  const [examMenus] = useAtom(examMenuState);
  //機器連携の状態管理
  const [connectionEquipment] = useAtom(connectionEquipmentState);
  const targetConnectionEquipment = connectionEquipment?.find(
    (item) => item.examMenuId === examMenuId,
  )?.equipment;
  //ローディング管理
  const [isLoading, setIsLoading] = useState(false);
  //共通ダイアログ表示管理
  const [openedCommon, { open: openCommon, close: closeCommon }] =
    useDisclosure(false);
  // 共通ダイアログで表示するメッセージ
  const [commonMessage, setCommonMessage] = useState<string>("");
  //共通ダイアログのボタンラベル
  const [commonButtonMessage, setCommonButtonMessage] = useState<string>("");
  //確認ダイアログ表示管理
  const [openedConfirm, { open: openConfirm, close: closeConfirm }] =
    useDisclosure(false);
  // 確認ダイアログで表示するメッセージ
  const [confirmMessage, setConfirmMessage] = useState<string>("");
  // 共通ダイアログのボタン押下時にブラウザバック処理を追加するフラグ
  const [commonBrowserbackFlag, setCommonBrowserbackFlag] = useState(false);
  // 通過用
  const [hasPass, setHasPass] = useState(false);
  const [passValue, setPassValue] = useState("");
  //測定ボタンの表示切り替え
  const [visibleRemeasurement, setVisibleRemeasurement] = useState(true);
  //測定ボタンの無効切り替え
  const [disabledRemeasurement, setDisabledRemeasurement] = useState(false);
  //機器連携用
  const localStorageKey = "measurementResult";
  const [isWatching, setIsWatching] = useState(false); // 監視状態を管理するフラグ
  const [initialDisplay, setInitialDisplay] = useState(true);

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
        examData.current = result.data.data;
        setRendering(true);
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

  const launchConnectionEquipment = () => {
    //ローカルストレージの監視を開始
    setIsWatching(true);
    // 機器連携アプリへ遷移
    window.location.href = `${targetConnectionEquipment?.appLaunchUrl}`;
  };

  useEffect(() => {
    if (!examData) return;

    //機器ラベルとvalueが空かをチェック
    const isValueEmptyForEquipmentLabel = (): boolean => {
      return (
        examData.current?.examItemGroups?.some((group) =>
          group.examItems?.some((item) =>
            item.examItemDetails?.some(
              (detail) => detail.equipmentLabel && detail.value === "",
            ),
          ),
        ) || false
      );
    };

    // 機器連携
    const handleRemeasurementCheck = () => {
      const isEmpty = isValueEmptyForEquipmentLabel();
      const isConnectionEquipment =
        !!targetConnectionEquipment &&
        !!targetConnectionEquipment.appLaunchUrl &&
        !!targetConnectionEquipment.processingScriptUrl;
      setVisibleRemeasurement(isConnectionEquipment); // 機器が選択されていないなら非表示
      setDisabledRemeasurement(!isEmpty); // valueが全て存在する場合無効化

      if (isConnectionEquipment && isEmpty && initialDisplay) {
        setInitialDisplay(false);
        launchConnectionEquipment();
      }
    };
    handleRemeasurementCheck();
  }, [examData, connectionEquipment]);

  //測定ボタン押下時
  const handleRemeasurement = () => {
    launchConnectionEquipment();
  };

  //解析js呼び出し
  const dynamicScriptExecute = async (data: string, scriptPath: string) => {
    try {
      const module = await import(/* @vite-ignore */ scriptPath);
      const output = module.decode(data);
      return output;
    } catch {
      return null; // エラー時は null を返す
    }
  };

  // 機器連携：監視用
  useEffect(() => {
    // StorageEventの変更を監視する関数
    const handleStorageChange = async (event: StorageEvent) => {
      if (event.key !== localStorageKey) return;

      setIsLoading(true);
      const base64Data = event.newValue ?? "";
      const scriptUrl = targetConnectionEquipment?.processingScriptUrl;

      // 解析js呼び出し処理
      const analyzedData = await dynamicScriptExecute(
        base64Data,
        scriptUrl ?? "",
      );

      if (analyzedData === null) {
        // 解析エラー発生時
        setIsWatching(false);
        setIsLoading(false);
        localStorage.removeItem(localStorageKey);
        return;
      }

      // value更新処理
      let isUpdated = false;
      let updatedExamData = { ...examData.current };

      for (const [key, value] of Object.entries(analyzedData)) {
        // valueがnullの場合は処理を行わない
        if (value === null) continue;
        let isUpdatedInThisKey = false;

        // equipmentLabelが一致するvalueを更新
        updatedExamData = {
          ...updatedExamData,
          examItemGroups: updatedExamData.examItemGroups?.map((group) => ({
            ...group,
            examItems: group.examItems?.map((item) => ({
              ...item,
              examItemDetails: item.examItemDetails?.map((detail) => {
                if (
                  !isUpdatedInThisKey &&
                  detail.equipmentLabel === key &&
                  detail.hasOrder &&
                  !detail.cancelReasonId &&
                  !detail.value
                ) {
                  isUpdated = true;
                  isUpdatedInThisKey = true;
                  return { ...detail, value: String(value) };
                }
                return detail;
              }),
            })),
          })),
        };
      }
      examData.current = updatedExamData;
      // 変更後に監視を解除
      if (isWatching) {
        setIsWatching(false);
        window.removeEventListener("storage", handleStorageChange);
      }

      // ローカルストレージのデータを削除
      localStorage.removeItem(localStorageKey);

      // 値を更新しなかった場合、ダイアログを表示
      if (!isUpdated) {
        setCommonMessage("表示可能な測定結果がありませんでした。");
        setCommonButtonMessage("閉じる");
        openCommon();
      }
      setIsLoading(false);
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
      examData.current = {
        ...examData.current,
        examItemGroups:
          examData.current?.examItemGroups?.map(
            (group, gIndex) =>
              gIndex === groupIndex
                ? { ...group, examItems: newExamItems } // 対象のグループの `examItems` を更新
                : group, // 他のグループはそのまま
          ) ?? [],
      };
    }
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
    let updatedExamData = { ...examData.current };
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
      setCommonMessage("エラーがあります。内容を確認してください");
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
            errorMessage = getErrorMessage(errorMessages.notFound, "受診番号");
          } else if (status === 422) {
            const response: VerifyExamItems = error.response.data;
            // 最大 errorLevel を取得
            const maxErrorLevel = Math.max(
              ...(response.examItemGroups?.flatMap(
                (group) =>
                  group.examItems?.flatMap(
                    (item) =>
                      item.examRegistResults?.map(
                        (result) => result.errorLevel ?? 0,
                      ) || [],
                  ) || [],
              ) || []),
              0, // データがない場合のデフォルト値
            );

            errorBranch(maxErrorLevel);
            examData.current = error.response.data;
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
    setIsRegisterPressed(true);
    setIsLoading(true);
    //AP1013_検査結果を検証する
    verifyResults();
    setIsLoading(false);
  };

  // 検査継続処理
  const continuingExam = () => {
    if (!examMenus) return;
    const currentIndex = examMenus.findIndex((menu) => menu.id === examMenuId);
    if (currentIndex !== -1 && currentIndex < examMenus.length - 1) {
      //次の検査メニューIDが存在する場合、検査内容確認画面へ遷移
      const nextExam = examMenus[currentIndex + 1];
      navigate(`/examorder-confirm/${consultNumber}?exammenuid=${nextExam.id}`);
    } else {
      //最後の検査メニューの場合、受診番号入力画面へ遷移
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
            errorMessage = getErrorMessage(errorMessages.notFound, "受診番号");
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

  // 登録処理
  const callbackRegister = () => {
    setIsLoading(true);
    // AP1014_検査結果を登録する
    registerResults();
    setIsLoading(false);
  };

  // 共通ダイアログ：閉じる処理
  const callbackCloseCommon = () => {
    closeCommon();
    if (commonBrowserbackFlag) {
      navigate(-1);
    }
  };

  // 確認ダイアログ：閉じる処理
  const callbackCloseConfirm = () => {
    closeConfirm();
    if (hasPass) {
      navigate(-1);
    }
  };

  // 検査項目コンポーネントのレンダリング
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
    // 通過フラグをリセット
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
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.選択:
        return (
          <ExamSelect
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onClick={handleChange}
          />
        );
      case ExamItemGroupType.血圧2回:
        return (
          <ExamBP2
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.自由入力:
        return (
          <ExamFreeInput
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.数値_左右:
        return (
          <ExamNumericLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.選択_左右:
        return (
          <ExamSelectLR
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onClick={handleChange}
          />
        );
      case ExamItemGroupType.身体計測:
        return (
          <ExamBody
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.視力:
        return (
          <ExamVision
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
            onChange={handleChange}
          />
        );
      case ExamItemGroupType.聴力:
        return (
          <ExamHearing
            key={groupIndex}
            examItems={examItems}
            onRegisterPressed={isRegisterPressed}
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
        const 実施済み = "1";
        const newValue = detailValue === 実施済み ? "" : 実施済み;
        setPassValue(newValue);

        // メッセージの設定
        if (detailValue === 実施済み) {
          setConfirmMessage("実施済みです。取消してよろしいですか。");
        } else {
          setConfirmMessage("登録します。よろしいですか。");
        }

        openConfirm();
        return;
      }
      default:
        return;
    }
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        {/* 受診者ヘッダー */}
        <ExamineeHeader
          staffName={staffData?.name ?? ""}
          managerNo={examData.current?.examinee?.ticketNumber ?? ""}
          name={examData.current?.examinee?.kanaName ?? ""}
          gender={examData.current?.examinee?.sex ?? 0}
          age={examData.current?.examinee?.examDateAge ?? 0}
        />
        {/* ブース特記 */}
        <BoothNote
          relatedExamItems={examData.current?.relatedExamItems ?? []}
        />
        <Stack align="center" gap={32} px={32} mt={32}>
          {/* 測定ボタン */}
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
              c="black"
              bd={`2px solid ${disabledRemeasurement ? "" : "gray03"}`}
              onClick={handleRemeasurement}
            >
              測定する
            </Button>
          )}
          {/* 検査項目コンポーネント */}
          {examData.current?.examItemGroups?.map(
            (examItemGroup: ExamItemGroup, index: number) => (
              <ExamItemRender
                key={index}
                groupIndex={index}
                examItemGroup={examItemGroup}
              />
            ),
          )}
          {/* 登録ボタン */}
          <Button w={860} h={75} mt={24} mb={83} onClick={handleVerify}>
            登録する
          </Button>
        </Stack>
        {/* 確認ダイアログ */}
        <ConfirmDialog
          message={confirmMessage}
          confirmButtonMessage={"OK"}
          isOpen={openedConfirm}
          onCancel={callbackCloseConfirm}
          onConfirm={callbackRegister}
        />
        {/* 共通ダイアログ */}
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
