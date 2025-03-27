import { useNavigate, useParams } from "react-router";
import type { MetaFunction } from "react-router";
import { useEffect, useState } from "react";
import { useDisclosure } from "@mantine/hooks";
import { useAtom } from "jotai";
import { staffState } from "~/store/store";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonHeader from "~/components/CommonHeader";
import CommonFooter from "~/components/CommonFooter";

export const meta: MetaFunction = () => {
  return [{ title: "検査内容確認" }];
};

/** 検査内容確認画面に必須のパスパラメータの$consultnumberが渡ってこない場合のエラー処理用ファイル */
export default function ExamOrderConfirm() {
  const navigate = useNavigate();
  // エラーメッセージ
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  // 共通ダイアログ表示制御
  const [showCommonDialog, { open: openCommonDialog }] = useDisclosure(false);

  // ログインしている職員の状態管理
  const [staff] = useAtom(staffState);

  // 引数を取得
  const params = useParams();
  const paramConsultNumber = params.consultnumber;

  useEffect(() => {
    if (!validateParameter()) {
      return;
    }
  }, []);

  // パラメータチェック
  const validateParameter = (): boolean => {
    if (!paramConsultNumber) {
      // 引数の受診番号なし
      startupCommonDialog("必要な受診番号がありません。");
      return false;
    }

    return true;
  };

  // ダイアログを閉じた時に受診番号入力画面に遷移する関数
  const navigateToConsultNumberInput = (): void => {
    navigate("/consultnumber-input");
  };

  // 共通ダイアログを起動する
  const startupCommonDialog = (message: string): void => {
    setErrorMessage(message);
    openCommonDialog();
  };

  return (
    <>
      <AuthWrapper>
        <>
          {/* 共通ヘッダー */}
          <CommonHeader staffName={staff?.name ?? ""} screenName="" />
          {/* 共通フッター */}
          <CommonFooter />
          {/* 共通ダイアログ */}
          <CommonDialog
            isOpen={showCommonDialog}
            onClose={navigateToConsultNumberInput}
            message={errorMessage ?? ""}
            buttonMessage="閉じる"
          />
        </>
      </AuthWrapper>
    </>
  );
}
