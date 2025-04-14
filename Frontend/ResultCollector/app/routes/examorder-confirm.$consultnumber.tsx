import { useNavigate, useParams, useSearchParams } from "react-router";
import type { MetaFunction } from "react-router";
import BoothNote from "~/components/BoothNote";
import CommonFooter from "~/components/CommonFooter";
import ExamineeHeader from "~/components/ExamineeHeader";
import ExamineeInfo from "~/components/ExamineeInfo";
import type {
  CancelReason,
  Equipment,
  ExamContent,
  ExamDetail,
  ExecutionRequest,
  ExecutionsRequest,
} from "~/domain/wellship.schemas";
import {
  Button,
  Center,
  Container,
  Divider,
  FocusTrap,
  Flex,
  LoadingOverlay,
  Modal,
  Paper,
  ScrollArea,
  Space,
  Stack,
  Switch,
  Text,
  getThemeColor,
  useMantineTheme,
} from "@mantine/core";
import {
  IconExclamationCircleFilled,
  IconSquareRoundedXFilled,
  IconWifi,
} from "@tabler/icons-react";
import { useEffect, useState } from "react";
import { useDisclosure } from "@mantine/hooks";
import { useAtom } from "jotai";
import {
  connectionEquipmentState,
  examMenuState,
  staffState,
} from "~/store/store";
import type {
  ConnectionEquipment,
  NumberIdNamedEntity,
} from "~/interfaces/interfaces";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";
import {
  useCancelReasonGetCancelReasons,
  useConsultGetExamItemsExaminee,
  useConsultRegisterExecutions,
  useEquipmentGetEquipmentSettings,
} from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonHeader from "~/components/CommonHeader";
import { isAxiosError } from "axios";
import { InputErrorLevel, Sex } from "~/domain/enums";

export const meta: MetaFunction = () => {
  return [{ title: "検査内容確認" }];
};

// 検査項目操作用のインターフェース
interface ExamDetailEdit {
  examItemId: number;
  examItemName: string;
  hasOrder: boolean;
  isPerforming: boolean;
  cancelReasonId?: number;
}

// 機器選択ダイアログ用のインターフェース
interface EquipmentDialogSetting {
  selectedId: number | undefined;
}

// 中止理由ダイアログ用のインターフェース
interface CancelReasonDialogSetting {
  examItemId: number;
  examItemName: string;
  cancelReasons: CancelReason[];
  selectedId: number | undefined;
}

export default function ExamOrderConfirm() {
  const theme = useMantineTheme();
  const navigate = useNavigate();
  // ローディング状態
  const [isLoading, setIsLoading] = useState(true);
  // 初期化処理でのエラー状態
  const [hasInitializeError, setHasInitializeError] = useState(false);
  // AP1010のレスポンス
  const [examContent, setExamContent] = useState<ExamContent | null>(null);
  // 検査項目
  const [examItems, setExamItems] = useState<ExamDetailEdit[]>([]);
  // AP1011のレスポンス(中止理由)
  const [cancelReasons, setCancelReasons] = useState<CancelReason[]>([]);
  // AP1012のレスポンス(検査機器)
  const [equipments, setEquipments] = useState<Equipment[]>([]);
  // 検査実施判断結果による実施不可能の判定
  const [isExamDecisionValid, setIsExamDecisionValid] = useState(false);
  // 機器選択ダイアログ用設定
  const [equipmentDialogSetting, setEquipmentDialogSetting] =
    useState<EquipmentDialogSetting | null>(null);
  // 中止理由ダイアログ用設定
  const [cancelReasonDialogSetting, setCancelReasonDialogSetting] =
    useState<CancelReasonDialogSetting | null>(null);
  // 機器選択ダイアログ表示制御
  const [
    showEquipmentDialog,
    { open: openEquipmentDialog, close: closeEquipmentDialog },
  ] = useDisclosure(false);
  // 中止理由ダイアログ表示制御
  const [
    showCancelReasonDialog,
    { open: openCancelReasonDialog, close: closeCancelReasonDialog },
  ] = useDisclosure(false);
  // 共通ダイアログに表示するメッセージ
  const [message, setMessage] = useState<string | null>(null);
  // 共通ダイアログ表示制御
  const [
    showCommonDialog,
    { open: openCommonDialog, close: closeCommonDialog },
  ] = useDisclosure(false);
  // 共通ダイアログを閉じた時の処理を切り替えるフラグ
  const [isNavigateOnClose, setIsNavigateOnClose] = useState(false);

  // 機器選択の状態管理
  const [connectionEquipments, setConnectionEquipments] = useAtom(
    connectionEquipmentState,
  );
  // 選択している検査メニューの状態管理
  const [selectedExamMenuState] = useAtom(examMenuState);
  // ログインしている職員の状態管理
  const [staff] = useAtom(staffState);

  // 検査実施判断結果の色定義
  const errorLevelColors = [
    {
      errorLevel: InputErrorLevel.警告,
      color: getThemeColor("warning", theme),
    },
    { errorLevel: InputErrorLevel.異常, color: getThemeColor("error", theme) },
  ];

  // 性別の色定義
  const sexThemeColors = [
    { sex: Sex.男, color: getThemeColor("malePrimary", theme) },
    { sex: Sex.女, color: getThemeColor("femalePrimary", theme) },
  ];

  // 中止理由未選択時のボタン文言
  const cancelReasonNotSelectCaption = "理由を選択してください";

  // API実行時に想定していない例外が返ってきた時のメッセージ
  const unexpectedErrorMessage = "予期しないエラーが発生しました。";

  // APIのバージョン
  const apiVersion = "1";

  // 引数を取得
  const params = useParams();
  const paramConsultNumber = params.consultnumber;
  const consultNumber = paramConsultNumber
    ? Number(paramConsultNumber)
    : undefined;
  const [query] = useSearchParams();
  // クエリパラメータをすべて小文字に変換する
  const queryParams = new Map(
    Array.from(query.entries()).map(([key, value]) => [
      key.toLocaleLowerCase(),
      value,
    ]),
  );
  const paramExamMenuId = queryParams.get("exammenuid");
  const examMenuId = paramExamMenuId ? Number(paramExamMenuId) : undefined;

  // AP1010_検査内容を取得する
  const {
    isFetching: isFetchingExamContent,
    refetch: refetchExamContentAsync,
  } = useConsultGetExamItemsExaminee(
    apiVersion,
    paramConsultNumber ?? "",
    { examMenuId: examMenuId },
    { query: { enabled: false } },
  );

  // AP1011_中止理由一覧を取得する
  const {
    isFetching: isFetchingCancelReasons,
    refetch: refetchCancelReasonsAsync,
  } = useCancelReasonGetCancelReasons(apiVersion, {
    query: { enabled: false },
  });

  // AP1012_検査機器一覧を取得する
  const { isFetching: isFetchingEquipments, refetch: refetchEquipmentsAsync } =
    useEquipmentGetEquipmentSettings(
      apiVersion,
      { examMenuId: examMenuId },
      { query: { enabled: false } },
    );

  // AP1022_検査の実施有無と中止理由を登録する
  const { mutateAsync: mutateConsultRegisterExecutionsAsync } =
    useConsultRegisterExecutions();

  useEffect(() => {
    if (!validateParameter()) {
      return;
    }

    // AP1010の呼び出し処理を定義
    const fetchConsultExamItemsAsync =
      async (): Promise<ExamContent | null> => {
        const result = await refetchExamContentAsync();
        if (result.error) {
          // エラー内容を判別する
          const errorMessage =
            result.error.status === 400
              ? getErrorMessage(errorMessages.invalid, "受診番号")
              : result.error.status === 404
                ? getErrorMessage(errorMessages.notFound, "該当する受診番号")
                : result.error.status === 500
                  ? getErrorMessage(errorMessages.serverError)
                  : unexpectedErrorMessage;
          // エラーメッセージをセットして共通ダイアログを表示する
          // 共通ダイアログを閉じた後、受診番号入力画面に遷移する
          startupCommonDialog(errorMessage, true);
          throw new Error("fetchConsultExamItemsAsync");
        }
        const data = result.data?.data ?? {};
        setExamContent(data);
        return data;
      };

    // AP1011の呼び出し処理を定義
    const fetchCancelReasonsAsync = async (): Promise<
      CancelReason[] | null
    > => {
      const result = await refetchCancelReasonsAsync();
      if (result.error) {
        // エラー内容を判別する
        const errorMessage =
          result.error.status === 400
            ? getErrorMessage(errorMessages.invalid, "検査項目ID")
            : result.error.status === 500
              ? getErrorMessage(errorMessages.serverError)
              : unexpectedErrorMessage;
        // エラーメッセージをセットして共通ダイアログを表示する
        // 共通ダイアログを閉じた後、受診番号入力画面に遷移する
        startupCommonDialog(errorMessage, true);
        throw new Error("fetchCancelReasonsAsync");
      }
      const data = result.data?.data.cancelReasons ?? [];
      setCancelReasons(data);
      return data;
    };

    // AP1012の呼び出し処理を定義
    const fetchEquipmentsAsync = async (): Promise<Equipment[] | null> => {
      const result = await refetchEquipmentsAsync();
      if (result.error) {
        // エラー内容を判別する
        const errorMessage =
          result.error.status === 400
            ? getErrorMessage(errorMessages.invalid, "検査メニューID")
            : result.error.status === 500
              ? getErrorMessage(errorMessages.serverError)
              : unexpectedErrorMessage;
        // エラーメッセージをセットして共通ダイアログを表示する
        // 共通ダイアログを閉じた後、受診番号入力画面に遷移する
        startupCommonDialog(errorMessage, true);
        throw new Error("fetchEquipmentsAsync");
      }
      const data = result.data?.data.equipments ?? [];
      setEquipments(data);
      return data;
    };

    // AP1010、AP1011、AP1012の取得処理がすべて終わってから画面初期化処理を行う
    const initializePageAsync = async () => {
      try {
        const [fetchExamContent, , fetchEquipmentData] = await Promise.all([
          fetchConsultExamItemsAsync(),
          fetchCancelReasonsAsync(),
          fetchEquipmentsAsync(),
        ]);

        // 検査項目に入力値用の項目を追加して保持
        const filteredExamItems = fetchExamContent?.examItems?.filter(
          Boolean,
        ) as ExamDetail[];
        const examItems: ExamDetailEdit[] = filteredExamItems?.map(
          (examDetail) => ({
            examItemId: examDetail.examItemId ?? 0,
            examItemName: examDetail.examItemName ?? "",
            hasOrder: examDetail.hasOrder ?? false,
            cancelReasonId: examDetail.cancelReasonId ?? undefined,
            // 実施する/しないの初期値
            isPerforming:
              // 実施可能かつ中止理由が未設定の時の初期値を「実施する」にする
              (examDetail.hasOrder ?? false) &&
              examDetail.cancelReasonId == null,
          }),
        );
        setExamItems(examItems);

        // 開始ボタンの使用可否を判断する
        setIsExamDecisionValid(
          // 健診実施判断結果のエラーレベルが異常のものがある場合は開始ボタンを使用できない
          !fetchExamContent?.examDecisionResults?.some(
            (x) => (x.errorLevel ?? 0) === InputErrorLevel.異常,
          ),
        );

        // 取得できた検査機器がひとつの時は自動で選択する
        if (!isEquipmentSelected() && fetchEquipmentData?.length === 1) {
          // 検査機器を選択していないかつ検査機器がひとつの時は自動で選択する
          updateConnectionEquipments(fetchEquipmentData[0]);
        }
      } catch {
        // エラー発生時
        setHasInitializeError(true);
      }
    };

    // 画面を初期化する
    initializePageAsync();
  }, [consultNumber, examMenuId]);

  // データ取得中はローディング表示する
  useEffect(() => {
    setIsLoading(
      isFetchingExamContent || isFetchingCancelReasons || isFetchingEquipments,
    );
  }, [isFetchingExamContent, isFetchingCancelReasons, isFetchingEquipments]);

  // パラメータチェック
  const validateParameter = (): boolean => {
    if (!paramConsultNumber) {
      // 引数の受診番号なし
      setHasInitializeError(true);
      startupCommonDialog("必要な受診番号がありません。", true);
      return false;
    }
    if (!paramExamMenuId) {
      // 引数の検査メニューIDなし
      setHasInitializeError(true);
      startupCommonDialog("必要な検査メニューIDがありません。", true);
      return false;
    }

    return true;
  };

  // ダイアログを閉じた時に受診番号入力画面に遷移する関数
  const navigateToConsultNumberInput = (): void => {
    navigate("/consultnumber-input");
  };

  // 実施有無変更時
  const handleSwitchChanged = (examItemId: number, checked: boolean): void => {
    const updatedExamItems = examItems?.map((item) => {
      if (item.examItemId === examItemId) {
        return { ...item, isPerforming: checked };
      }
      return item;
    });
    setExamItems(updatedExamItems);

    if (!checked) {
      // 「しない」にした時、中止理由ダイアログを起動する
      startCancelReasonDialog(examItemId);
    }
  };

  // 選択している検査機器があるかどうか判定する
  /** 選択している機器があるかどうか(敢えて選択していない場合はfalse) */
  const hasSelectedEquipment = (): boolean => {
    return (
      connectionEquipments?.some(
        (x) => x.examMenuId === examMenuId && x.equipment !== null,
      ) ?? false
    );
  };

  // 検査機器を選択しているかどうか判定する
  /** 機器を選択しているかどうか(敢えて選択していない場合も「選択している」と判断) */
  const isEquipmentSelected = (): boolean => {
    return (
      connectionEquipments?.some((x) => x.examMenuId === examMenuId) ?? false
    );
  };

  // スキップボタンクリック
  const handleSkipButtonClick = (): void => {
    // スキップ処理を行う
    examMenuSkip();
  };

  // 検査メニューをスキップする
  const examMenuSkip = (): void => {
    // 今の検査メニューをスキップして次の検査メニューに遷移する
    const selectedExamMenu = selectedExamMenuState?.filter(
      Boolean,
    ) as NumberIdNamedEntity[];

    const examMenuIndex = selectedExamMenu?.findIndex(
      (x) => x.id === examMenuId,
    );

    if (examMenuIndex === -1 || examMenuIndex === undefined) {
      // パラメータの検査メニューIDが状態管理で選択している検査メニューIDに存在しない時
      // 受診番号入力画面に遷移
      navigate("/consultnumber-input");
    } else if (examMenuIndex === selectedExamMenu?.length - 1) {
      // パラメータの検査メニューIDが状態管理で選択している検査メニューIDの最後の項目の時
      // 受診番号入力画面に遷移(受診番号をパラメータで渡す)
      navigate(`/consultnumber-input?consultnumber=${paramConsultNumber}`);
    } else {
      // パラメータの検査メニューIDが状態管理で選択している検査メニューIDの最後の項目ではない時
      const nextExamMenu = selectedExamMenu[examMenuIndex + 1];
      // 検査内容確認画面に次の検査メニューIDを渡して遷移
      navigate(
        `/examorder-confirm/${paramConsultNumber}?exammenuid=${nextExamMenu.id}`,
      );
    }
  };

  // 開始ボタンクリック
  const handleStartButtonClick = (): void => {
    // 入力チェック
    if (!validate()) {
      return;
    }

    setIsLoading(true);

    // POSTするデータを取得する
    const data: ExecutionRequest[] = examItems?.map((examItem) => ({
      examItemId: examItem.examItemId,
      isPerforming: examItem.isPerforming,
      cancelReasonId: examItem.isPerforming ? null : examItem.cancelReasonId,
    }));
    const postBody: ExecutionsRequest = {
      executions: data,
    };

    // AP1022の呼び出し処理を定義
    const postConsultExamExecutionsAsync = async () => {
      try {
        const result = await mutateConsultRegisterExecutionsAsync({
          version: apiVersion,
          consultNumber: paramConsultNumber ?? "",
          data: postBody,
        });

        // 正常終了時
        if (result.status === 200) {
          // 遷移先の画面を判定する
          if (examItems.some((x) => x.isPerforming)) {
            // 「実施する」が選択されている検査項目がある時
            // 検査結果入力画面に遷移する
            navigate(
              `/consult-input/${paramConsultNumber}?exammenuid=${paramExamMenuId}`,
            );
          } else {
            // 「実施する」が選択されている検査項目がない時
            // スキップボタンと同じ画面遷移を行う
            examMenuSkip();
          }
        }
      } catch (error) {
        // AP1022でエラーが発生した時
        let errorMessage = unexpectedErrorMessage;
        // AxiosErrorかどうかを確認
        if (isAxiosError(error) && error.response) {
          const status = error.response.status;
          errorMessage =
            status === 400
              ? getErrorMessage(errorMessages.invalid, "受診番号")
              : status === 403
                ? "会場ロック中です。管理者のみ更新可能です。"
                : status === 404
                  ? getErrorMessage(errorMessages.notFound, "該当する受診番号")
                  : status === 500
                    ? getErrorMessage(errorMessages.serverError)
                    : unexpectedErrorMessage;
        }
        startupCommonDialog(errorMessage, false);
      } finally {
        setIsLoading(false);
      }
    };

    // 検査実施有無と中止理由を登録する
    postConsultExamExecutionsAsync();
  };

  // 入力チェック
  const validate = (): boolean => {
    // 実施可否「可」かつ実施理由「しない」かつ中止理由未選択の項目がある時はエラー
    const errors = examItems
      .filter(
        (examItem) =>
          examItem.hasOrder &&
          !examItem.isPerforming &&
          examItem.cancelReasonId === undefined,
      )
      .map((examItem) =>
        getErrorMessage(
          errorMessages.required,
          `${examItem.examItemName}の中止理由は`,
        ),
      );

    if (errors.length === 0) {
      return true;
    }

    // エラーメッセージをセットして共通ダイアログを表示する
    startupCommonDialog(errors.join("\n"), false);

    return false;
  };

  // 機器選択ボタンクリック
  const handleEquipmentButtonClick = (): void => {
    // 状態管理から対象の検査メニューIDで選択している機器情報を取得する
    const connectionEquipment = connectionEquipments?.find(
      (x) => x.examMenuId === examMenuId,
    );

    const tempEquipmentDialogSetting: EquipmentDialogSetting = {
      selectedId: connectionEquipment?.equipment?.equipmentId,
    };

    setEquipmentDialogSetting(tempEquipmentDialogSetting);
    openEquipmentDialog();
  };

  // 機器選択ダイアログの検査機器ボタンクリック
  const handleEquipmentSubButtonClick = (id: number | undefined): void => {
    // equipmentDialogSettingがnullの時は抜ける
    if (!equipmentDialogSetting) {
      return;
    }

    const tempEquipmentDialogSetting = { ...equipmentDialogSetting };
    if (tempEquipmentDialogSetting.selectedId === id) {
      // 選択中の検査機器と同じ検査機器ボタンがクリックされた時は選択を解除
      tempEquipmentDialogSetting.selectedId = undefined;
    } else {
      // クリックされた検査機器ボタンのIDを保持
      tempEquipmentDialogSetting.selectedId = id;
    }
    setEquipmentDialogSetting(tempEquipmentDialogSetting);
  };

  // 機器選択ダイアログの決定ボタンクリック
  const handleEquipmentDecideButtonClick = (): void => {
    // 選択した検査機器を取得する
    const selectedEquipment =
      equipments.find(
        (x) => x.equipmentId === equipmentDialogSetting?.selectedId,
      ) ?? null;

    // 選択している検査機器を更新する
    updateConnectionEquipments(selectedEquipment);

    closeEquipmentDialog();
  };

  // 選択している検査機器を更新する
  const updateConnectionEquipments = (
    selectedEquipment: Equipment | null,
  ): void => {
    // 状態管理に検査メニューごとに検査機器を保持する
    const updatedEquipments = (() => {
      const connectionEquipmentList = connectionEquipments ?? [];

      // 状態管理に対象の検査メニューIDがあるかどうか判定する
      const connectionEquipmentIndex = connectionEquipmentList.findIndex(
        (x) => x.examMenuId === examMenuId,
      );

      if (connectionEquipmentIndex !== -1) {
        // 存在する場合は上書きする
        return connectionEquipmentList
          .map((connectionEquipment, index) => ({
            examMenuId: connectionEquipment.examMenuId,
            equipment:
              index === connectionEquipmentIndex
                ? selectedEquipment
                : connectionEquipment.equipment,
          }))
          .filter(Boolean) as ConnectionEquipment[];
      }

      // 存在しない場合は追加する
      return [
        ...connectionEquipmentList,
        { examMenuId: examMenuId, equipment: selectedEquipment },
      ].filter(Boolean) as ConnectionEquipment[];
    })();
    setConnectionEquipments(updatedEquipments);
  };

  // 中止理由ボタンクリック
  const handleCancelReasonButtonClick = (
    examItemId: number | undefined,
  ): void => {
    startCancelReasonDialog(examItemId);
  };

  // 中止理由ダイアログを起動する
  const startCancelReasonDialog = (examItemId: number | undefined): void => {
    if (examItemId === undefined) {
      return;
    }

    // 中止理由を対象の検査項目でフィルタする
    const tempCancelReasons = cancelReasons.filter(
      (x) => x.examItemId === examItemId,
    );

    const targetExamItem = examItems?.find((x) => x.examItemId === examItemId);
    const tempCancelReasonDialogSetting: CancelReasonDialogSetting = {
      examItemId: examItemId,
      examItemName: targetExamItem?.examItemName || "",
      cancelReasons: tempCancelReasons,
      selectedId: targetExamItem?.cancelReasonId || undefined,
    };

    setCancelReasonDialogSetting(tempCancelReasonDialogSetting);
    openCancelReasonDialog();
  };

  // 中止理由ダイアログの中止理由ボタンクリック
  const handleCancelReasonSubButtonClick = (id: number | undefined): void => {
    // cancelReasonDialogSettingがnullの時は抜ける
    if (!cancelReasonDialogSetting) {
      return;
    }

    const tempCancelReasonDialogSetting = { ...cancelReasonDialogSetting };
    if (tempCancelReasonDialogSetting.selectedId === id) {
      // 選択中の中止理由と同じ中止理由ボタンがクリックされた時は選択を解除
      tempCancelReasonDialogSetting.selectedId = undefined;
    } else {
      // クリックされた中止理由ボタンのIDを保持
      tempCancelReasonDialogSetting.selectedId = id;
    }
    setCancelReasonDialogSetting(tempCancelReasonDialogSetting);
  };

  // 中止理由ダイアログの決定ボタンクリック
  const handleCancelReasonDecideButtonClick = (): void => {
    // 中止理由を保持する
    const updatedExamItems = examItems?.map((item) => {
      if (item.examItemId === cancelReasonDialogSetting?.examItemId) {
        return {
          ...item,
          cancelReasonId: cancelReasonDialogSetting?.selectedId,
        };
      }
      return item;
    });
    setExamItems(updatedExamItems);

    closeCancelReasonDialog();
  };

  // 開始ボタンを強調するかどうか判断する
  const isEmphasizeStartButton = (): boolean => {
    return (
      // 検査実施判断結果にエラーとなるものがない
      // 検査項目に実施可能なものが存在する
      // 検査メニューが未実施
      isExamDecisionValid && hasExamItemOrder() && !examContent?.isComplete
    );
  };

  // 検査項目に「実施可能」なものが存在するかチェック
  const hasExamItemOrder = (): boolean => {
    return examItems?.some((x) => x.hasOrder);
  };

  // 共通ダイアログを起動する
  const startupCommonDialog = (message: string, navigateFlg: boolean): void => {
    setIsNavigateOnClose(navigateFlg);
    setMessage(message);
    openCommonDialog();
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isLoading} />
        <>
          {isFetchingExamContent ||
          isFetchingCancelReasons ||
          isFetchingEquipments ||
          hasInitializeError ? (
            <>
              {/* データ取得中またはエラー発生時は共通ヘッダーを画面名なしで表示する */}
              {/* 共通ヘッダー */}
              <CommonHeader staffName={staff?.name ?? ""} screenName="" />
            </>
          ) : (
            <>
              {/* 検査内容が取得できている時は画面を表示する */}
              {/* 受診者ヘッダー */}
              <ExamineeHeader
                managerNo={examContent?.examinee?.ticketNumber ?? ""}
                staffName={staff?.name ?? ""}
                name={examContent?.examinee?.kanaName ?? ""}
                gender={examContent?.examinee?.sex ?? 0}
                age={examContent?.examinee?.examDateAge ?? 0}
              />
              {/* 個人属性情報 */}
              <Flex mx={16}>
                <ExamineeInfo
                  name={examContent?.examinee?.name ?? ""}
                  birthday={examContent?.examinee?.birthdate ?? ""}
                  office={examContent?.examinee?.organizations ?? []}
                  note={examContent?.consultName}
                  namesake={examContent?.examinee?.sameNameAlert ?? false}
                />
              </Flex>
              {/* ブース特記 */}
              <Flex mx={16} my={16}>
                <BoothNote
                  relatedExamItems={examContent?.relatedExamItems ?? []}
                />
              </Flex>
              {/* 検査実施判断結果 */}
              {examContent?.examDecisionResults !== undefined &&
                examContent?.examDecisionResults?.length > 0 && (
                  <Container fluid>
                    <Stack gap={16}>
                      {examContent?.examDecisionResults?.map(
                        (examDecision, index) => (
                          <Flex
                            gap={16}
                            c={
                              errorLevelColors.find(
                                (x) =>
                                  x.errorLevel ===
                                  (examDecision.errorLevel ?? 0),
                              )?.color || "black01"
                            }
                            key={index}
                          >
                            {examDecision.errorLevel ===
                              InputErrorLevel.異常 && (
                              <IconSquareRoundedXFilled size={32} />
                            )}
                            {examDecision.errorLevel ===
                              InputErrorLevel.警告 && (
                              <IconExclamationCircleFilled size={32} />
                            )}
                            <Text size="sm" fw={700}>
                              {examDecision.description}
                            </Text>
                          </Flex>
                        ),
                      )}
                    </Stack>
                  </Container>
                )}
              <Container size="xs" mt={16} fluid>
                <Flex
                  gap={16}
                  direction="row"
                  align="flex-start"
                  justify="flex-start"
                >
                  {/* 受信ボタン */}
                  {equipments.length > 0 && (
                    <Flex direction="column" mx={16}>
                      <Button
                        variant="outline"
                        h={133}
                        w={80}
                        px={5}
                        bd="2px solid primary"
                        bg={
                          hasSelectedEquipment()
                            ? getThemeColor("green03", theme)
                            : getThemeColor("white01", theme)
                        }
                        onClick={() => handleEquipmentButtonClick()}
                      >
                        <Center>
                          <Flex direction="column">
                            <IconWifi size={50} />
                            <Text fw={700} size="xs">
                              受信
                            </Text>
                          </Flex>
                        </Center>
                      </Button>
                      <Center>
                        <Text
                          c={getThemeColor("primary", theme)}
                          size="xs"
                          fw={700}
                        >
                          {hasSelectedEquipment() ? <>ON</> : <>OFF</>}
                        </Text>
                      </Center>
                    </Flex>
                  )}

                  <Flex direction="column">
                    {/* 検査項目表示部 */}
                    {examItems?.map((examItem) => (
                      <Flex
                        gap={8}
                        mt={5}
                        mb={10}
                        direction="column"
                        key={examItem.examItemId}
                      >
                        <Flex gap="md" align="center" key={examItem.examItemId}>
                          <Paper
                            bg={getThemeColor("gray02", theme)}
                            c={getThemeColor("white01", theme)}
                            radius="sm"
                            px={3}
                            w={267}
                            miw={267}
                            h={56}
                            py={8}
                          >
                            <Text size="xs" fw={700} ta="center" mt={5}>
                              {examItem.examItemName.slice(0, 8)}
                            </Text>
                          </Paper>
                          {examItem.hasOrder && (
                            <Switch
                              size="xxl"
                              onLabel="する"
                              offLabel="しない"
                              checked={examItem.isPerforming}
                              onChange={(event) =>
                                handleSwitchChanged(
                                  examItem.examItemId,
                                  event.currentTarget.checked,
                                )
                              }
                            />
                          )}

                          {examItem.hasOrder && !examItem.isPerforming && (
                            <Button
                              variant="outline"
                              size="lg"
                              bg="white01"
                              w={500}
                              h={60}
                              onClick={() =>
                                handleCancelReasonButtonClick(
                                  examItem.examItemId,
                                )
                              }
                            >
                              {examItem.cancelReasonId == null
                                ? cancelReasonNotSelectCaption
                                : cancelReasons.find(
                                    (cancelReason) =>
                                      cancelReason.examItemId ===
                                        examItem.examItemId &&
                                      cancelReason.cancelReasonId ===
                                        examItem.cancelReasonId,
                                  )?.cancelReasonName || "理由が取得できません"}
                            </Button>
                          )}
                          {!examItem.hasOrder && (
                            <Flex
                              align="center"
                              c={getThemeColor("warning", theme)}
                              gap={16}
                            >
                              <IconExclamationCircleFilled size={32} />
                              <Text size="sm" fw={700}>
                                この項目は対象外です
                              </Text>
                            </Flex>
                          )}
                        </Flex>
                      </Flex>
                    ))}
                  </Flex>
                </Flex>
              </Container>
              <Divider mx={16} />
              <Container fluid mt={24}>
                <Center>
                  <Flex
                    gap={16}
                    wrap="wrap"
                    justify="center"
                    direction={{ base: "column", md: "row" }}
                  >
                    <Button
                      w={860}
                      h={75}
                      bd="4px solid primary"
                      variant={!isEmphasizeStartButton() ? "filled" : "outline"}
                      bg={!isEmphasizeStartButton() ? "" : "white01"}
                      size="md"
                      onClick={() => handleSkipButtonClick()}
                    >
                      この項目をスキップする
                    </Button>
                    <Button
                      w={860}
                      h={75}
                      bd="4px solid primary"
                      variant={isEmphasizeStartButton() ? "filled" : "outline"}
                      bg={isEmphasizeStartButton() ? "" : "white01"}
                      size="md"
                      disabled={!isExamDecisionValid || !hasExamItemOrder()}
                      onClick={() => handleStartButtonClick()}
                    >
                      開始する
                    </Button>
                  </Flex>
                </Center>
              </Container>
              {/* 未実施項目 */}
              {examContent?.unexaminedItems !== undefined &&
                examContent?.unexaminedItems?.length > 0 && (
                  <Container mx={16} mt={16} fluid>
                    <Flex gap={16} direction="column">
                      <Paper
                        bg={getThemeColor("green02", theme)}
                        radius="md"
                        px="xs"
                        h={42}
                        w={100}
                        miw={100}
                      >
                        <Text size="xs" ta="center" mt={5}>
                          未受診
                        </Text>
                      </Paper>
                      <Flex gap={0} align="center" wrap="wrap">
                        <Text size="xs">
                          {examContent.unexaminedItems
                            .map((x) => x.examMenuName)
                            .join("、")}
                        </Text>
                      </Flex>
                    </Flex>
                  </Container>
                )}
              {/* 共通フッター部分にコンテンツが重なるためスペースを入れておく */}
              <Space h={100} />
              {/* 機器選択ダイアログ */}
              <Modal
                padding={0}
                size={800}
                radius="md"
                opened={showEquipmentDialog}
                onClose={closeEquipmentDialog}
                withCloseButton={false}
                closeOnClickOutside={false}
                centered
              >
                <Modal.Header bg={getThemeColor("primary", theme)}>
                  <Container fluid>
                    <Text size="sm" c="white01" fw={700}>
                      機器受信
                    </Text>
                  </Container>
                </Modal.Header>
                <Modal.Body>
                  <FocusTrap.InitialFocus />
                  <Space h={16} />
                  <Flex direction="column">
                    <ScrollArea h={400} type="always">
                      <Flex gap={16} direction="column" mx={16}>
                        {equipments.length === 0 ? (
                          <>
                            <Space h={1} />
                            <Text size="sm">
                              検査機器の設定がありませんでした。
                            </Text>
                          </>
                        ) : (
                          equipments.map((equipment) => {
                            const isSelectedItem =
                              equipment.equipmentId ===
                              equipmentDialogSetting?.selectedId;
                            return (
                              <Button
                                h={64}
                                size="sm"
                                variant="outline"
                                color={
                                  isSelectedItem
                                    ? getThemeColor("primary", theme)
                                    : getThemeColor("gray01", theme)
                                }
                                bg={
                                  isSelectedItem
                                    ? getThemeColor("green03", theme)
                                    : getThemeColor("white01", theme)
                                }
                                key={equipment.equipmentId}
                                onClick={() =>
                                  handleEquipmentSubButtonClick(
                                    equipment.equipmentId,
                                  )
                                }
                              >
                                {equipment.equipmentName}
                              </Button>
                            );
                          })
                        )}
                      </Flex>
                    </ScrollArea>
                    <Space h={16} />
                    <Divider />
                    <Space h={24} />
                    <Container fluid>
                      <Flex gap={24} direction="row">
                        <Button
                          size="lg"
                          variant="outline"
                          bg="white01"
                          w={360}
                          h={64}
                          onClick={() => closeEquipmentDialog()}
                        >
                          戻る
                        </Button>
                        <Button
                          size="lg"
                          variant="filled"
                          w={360}
                          h={64}
                          onClick={() => handleEquipmentDecideButtonClick()}
                        >
                          決定
                        </Button>
                      </Flex>
                    </Container>
                  </Flex>
                  <Space h={24} />
                </Modal.Body>
              </Modal>
              {/* 中止理由ダイアログ */}
              <Modal
                padding={0}
                size={800}
                radius="md"
                opened={showCancelReasonDialog}
                onClose={closeCancelReasonDialog}
                withCloseButton={false}
                closeOnClickOutside={false}
                centered
              >
                <Modal.Header
                  bg={
                    sexThemeColors.find(
                      (x) => x.sex === (examContent?.examinee?.sex ?? 0),
                    )?.color || getThemeColor("green02", theme)
                  }
                >
                  <Container fluid>
                    <Text size="sm" fw={700}>
                      {cancelReasonDialogSetting?.examItemName}
                    </Text>
                  </Container>
                </Modal.Header>
                <Modal.Body>
                  <FocusTrap.InitialFocus />
                  <Space h={16} />
                  <Flex direction="column">
                    <ScrollArea h={400} type="always">
                      <Flex gap={16} direction="column" mx={16}>
                        {cancelReasonDialogSetting?.cancelReasons.length ===
                        0 ? (
                          <>
                            <Space h={1} />
                            <Text size="sm">
                              中止理由の設定がありませんでした。
                            </Text>
                          </>
                        ) : (
                          cancelReasonDialogSetting?.cancelReasons.map(
                            (cancelReason) => {
                              const isSelectedItem =
                                cancelReason.cancelReasonId ===
                                cancelReasonDialogSetting?.selectedId;
                              return (
                                <Button
                                  h={64}
                                  size="sm"
                                  variant="outline"
                                  color={
                                    isSelectedItem
                                      ? getThemeColor("primary", theme)
                                      : getThemeColor("gray01", theme)
                                  }
                                  bg={
                                    isSelectedItem
                                      ? getThemeColor("green03", theme)
                                      : getThemeColor("white01", theme)
                                  }
                                  key={cancelReason.cancelReasonId}
                                  onClick={() =>
                                    handleCancelReasonSubButtonClick(
                                      cancelReason.cancelReasonId,
                                    )
                                  }
                                >
                                  {cancelReason.cancelReasonName}
                                </Button>
                              );
                            },
                          )
                        )}
                      </Flex>
                    </ScrollArea>
                    <Space h={16} />
                    <Divider />
                    <Space h={24} />
                    <Container fluid>
                      <Flex gap={24} direction="row">
                        <Button
                          size="lg"
                          variant="outline"
                          bg="white01"
                          w={360}
                          h={64}
                          onClick={() => closeCancelReasonDialog()}
                        >
                          戻る
                        </Button>
                        <Button
                          size="sm"
                          variant="filled"
                          w={360}
                          h={64}
                          onClick={() => handleCancelReasonDecideButtonClick()}
                        >
                          決定
                        </Button>
                      </Flex>
                    </Container>
                  </Flex>
                  <Space h={24} />
                </Modal.Body>
              </Modal>
            </>
          )}
          {/* 共通フッター */}
          <CommonFooter />
          {/* 共通ダイアログ */}
          <CommonDialog
            isOpen={showCommonDialog}
            onClose={
              isNavigateOnClose
                ? navigateToConsultNumberInput
                : closeCommonDialog
            }
            message={message ?? ""}
            buttonMessage="閉じる"
          />
        </>
      </AuthWrapper>
    </>
  );
}
