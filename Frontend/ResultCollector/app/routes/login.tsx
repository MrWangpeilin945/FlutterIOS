import {
  Box,
  Button,
  Center,
  Container,
  Flex,
  Group,
  Image,
  LoadingOverlay,
  PasswordInput,
  Space,
  Text,
  TextInput,
} from "@mantine/core";

import { useDisclosure } from "@mantine/hooks";
import type { MetaFunction } from "react-router";
import { useNavigate } from "react-router";
import { isAxiosError } from "axios";
import { useAtom } from "jotai";
import { useState } from "react";
import { z } from "zod";
import { useAuthenticationLogin, useStaffGetStaff } from "~/api/wellship";
import CommonDialog from "~/components/CommonDialog";
import type { StringIdNamedEntity } from "~/interfaces/interfaces";
import { serverTimeOffsetState, staffState } from "~/store/store";
import { authUtil } from "~/utils/authUtil";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "ログイン" }];
};

export default function Login() {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [password, setPassword] = useState("");
  const [loginId, setLoginId] = useState("");
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [, setStaff] = useAtom(staffState);
  const [, setServerTimeOffset] = useAtom(serverTimeOffsetState);

  //AP1001呼び出し用(POST系APIの定義)
  const { mutateAsync } = useAuthenticationLogin();

  //AP1002呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = useStaffGetStaff("1", {
    query: { enabled: false },
  });

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
    // POST時のリクエストボディを生成する
    const body = {
      loginId: loginId,
      password: password,
    };

    const postMutateAsync = async () => {
      setIsLoading(true);
      try {
        const result = await mutateAsync({
          version: "1",
          data: body,
        });
        if (result.status === 200) {
          // 成功時の処理
          // 取得したアクセストークンとサーバー時間との差を保存する
          const responseToken = result.data.token ?? "";
          authUtil.setAccessToken(responseToken);
          const offset = authUtil.calculateTimeOffset(responseToken);
          setServerTimeOffset(offset);

          await fetchStaff();
        }
      } catch (error) {
        if (isAxiosError(error) && error.response) {
          if (error.response.status === 401) {
            setErrorMessage(getErrorMessage(errorMessages.accessDenied));
          } else if (error.response.status === 500) {
            setErrorMessage(getErrorMessage(errorMessages.serverError));
          }
          open();
        }
      } finally {
        setIsLoading(false);
      }
    };
    postMutateAsync();
  };

  //AP1002_職員の情報を取得する(ログイン中の職員の名前などを取得する)
  const fetchStaff = async () => {
    const result = await refetch();
    if (result.data) {
      // 成功時
      // サーバから取得した情報.staffId、staffNameを設定する
      const jotaiData: StringIdNamedEntity = {
        id: result.data.data.staffId,
        name: result.data.data.staffName,
      };
      setStaff(jotaiData);

      // 次の画面に遷移
      navigate("/team-select");
    } else if (result.error) {
      if (result.error.status === 404) {
        setErrorMessage(
          getErrorMessage(errorMessages.notFound, "該当IDの職員情報"),
        );
      } else if (result.error.status === 500) {
        setErrorMessage(getErrorMessage(errorMessages.serverError));
      }
      open();
      throw new Error();
    }
  };

  // ログイン実行処理
  const handleConfirm = async () => {
    try {
      if (validationCheck()) {
        await fetchlogin();
      }
    } catch (error) {
      // console.error("ログイン実行中にエラーが発生しました:", error);
    }
  };

  return (
    <>
      <LoadingOverlay visible={isFetching || isLoading} />
      <Container fluid bg="background">
        <Center style={{ flexDirection: "column" }}>
          <Space h={144} />
          <Image src="./wellship-logo.svg" w={566} h={131} />
          <Space h={128} />
          <Flex columnGap={16}>
            <Group w="490" justify="flex-end">
              <Text size="sm" c="black01" ta="right">
                ログインID
              </Text>
              <TextInput
                value={loginId}
                autoComplete="off"
                onChange={(e) => setLoginId(e.target.value)}
                maxLength={20}
                inputMode="email"
                styles={{
                  input: {
                    height: "auto",
                    width: 340,
                    padding: "16px 32px",
                  },
                }}
              />
            </Group>
          </Flex>
          <Space h={16} />
          <Flex columnGap={16}>
            <Group w="490" justify="flex-end">
              <Text size="sm" c="black01" ta="right">
                パスワード
              </Text>
              <PasswordInput
                value={password}
                autoComplete="one-time-code"
                onChange={(e) => setPassword(e.target.value)}
                maxLength={20}
                rightSectionWidth={70}
                inputMode="email"
                styles={{
                  input: {
                    height: 68,
                    width: 340,
                  },
                  innerInput: {
                    padding: "16px 32px",
                  },
                  visibilityToggle: {
                    transform: "scale(2)",
                  },
                }}
              />
            </Group>
          </Flex>
          <Space h={128} />
          <Box>
            <Button
              w={860}
              h={75}
              bg="primary"
              onClick={() => handleConfirm()}
              px={32}
              py={16}
            >
              <Text size="lg" fw={700} c="white01">
                ログイン
              </Text>
            </Button>
          </Box>
          <CommonDialog
            message={errorMessage || ""}
            buttonMessage="閉じる"
            isOpen={opened}
            onClose={close}
          />
        </Center>
      </Container>
    </>
  );
}
