import { Button } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { useAtom } from "jotai";
import { useState } from "react";
import { z } from "zod";
import { authenticationLogin, staffGetStaff } from "~/api/wellship";
import CommonDialog from "~/components/CommonDialog";
import type { StaffLoginRequest } from "~/domain/wellship.schemas";
import type { NamedEntity } from "~/interfaces/interfaces";
import { staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "ログイン" }];
};

export default function Login() {
  const navigate = useNavigate();
  const [password, setPassword] = useState("");
  const [loginId, setLoginId] = useState("");
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [, setStaff] = useAtom(staffState);

  // バリデーションチェック
  const validationCheck = () => {
    setErrorMessage(null);
    // バリデーションチェックスキーマ
    const validationSchemaId = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, "IDは"))
      .max(20, getErrorMessage(errorMessages.maxLength, "IDは", 20))
      .regex(
        /^[a-zA-Z0-9]+$/,
        getErrorMessage(errorMessages.alphaNumericString, "IDは"),
      );

    const validationSchemaPw = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, "パスワードは"));

    // ID項目：Zodでバリデーションチェック実行
    const resultId = validationSchemaId.safeParse(loginId);
    if (!resultId.success) {
      setErrorMessage(resultId.error.errors[0].message);
      open();
      return false;
    }

    // パスワード項目：Zodでバリデーションチェック実行
    const resultPw = validationSchemaPw.safeParse(password);
    if (!resultPw.success) {
      setErrorMessage(resultPw.error.errors[0].message);
      open();
      return false;
    }
    return true;
  };

  //AP1001_ログインする( 認証情報を渡してJWTを取得する)
  const fetchlogin = async () => {
    const loginData: StaffLoginRequest = { loginId, password };

    await authenticationLogin("1", loginData)
      .then(() => {
        // 成功時の処理
      })
      .catch((error) => {
        if (error.response.status === 403) {
          setErrorMessage(getErrorMessage(errorMessages.accesDenied));
        } else if (error.response.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.server));
        }
        open();
        throw new Error(error);
      });
  };

  //AP1002_職員の情報を取得する(ログイン中の職員の名前などを取得する)
  const fetchStaff = async () => {
    await staffGetStaff("1")
      .then((result) => {
        // 成功時の処理
        // サーバから取得した情報.staffId、staffNameを設定する
        const jotaiData: NamedEntity = {
          id: result.data.staffId,
          name: result.data.staffName,
        };
        setStaff(jotaiData);
      })
      .catch((error) => {
        if (error.response.status === 404) {
          setErrorMessage(
            getErrorMessage(errorMessages.noData, "該当IDの職員情報"),
          );
        } else if (error.response.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.server));
        }
        open();
        throw new Error(error);
      });
  };

  // ログイン実行処理
  const handleConfirm = async () => {
    try {
      if (validationCheck()) {
        await fetchlogin();
        await fetchStaff();

        // 次の画面に遷移
        navigate("/team-select");
      }
    } catch (error) {
      // console.error("ログイン実行中にエラーが発生しました:", error);
    }
  };

  return (
    <div>
      <div>
        <h1>ログイン画面</h1>
        <label>
          利用者ID&nbsp;
          <input
            type="text"
            value={loginId}
            onChange={(e) => setLoginId(e.target.value)}
            maxLength={20}
          />
          &nbsp;&nbsp;
        </label>
        <br />
        <label>
          パスワード&nbsp;
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </label>
        <br />
        <Button variant="primary" onClick={() => handleConfirm()}>
          ログイン
        </Button>
        <CommonDialog
          message={errorMessage || ""}
          buttonMessage="閉じる"
          isOpen={opened}
          onClose={close}
        />
      </div>
    </div>
  );
}
