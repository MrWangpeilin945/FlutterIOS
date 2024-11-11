import type React from "react";
import { useState } from "react";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { z } from "zod";
import { useAtom } from "jotai";
import { teamState } from "~/store/store";
import {
  Group,
  Title,
  Box,
  Center,
  Paper,
  Textarea,
  Text,
} from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { consultVerifyConsultNumber } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonHeader from "~/components/CommonHeader";
import CommonFooter from "~/components/CommonFooter";
import Keyboard from "~/components/SoftwareKeyboard";
import { ErrorModal } from "~/components/ErrorModal";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import { createNavigate } from "~/utils/screenMove";
import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
  return [{ title: "受診番号入力" }];
};

export default function consultNumberInput() {
  const [consultNo, setConsultNo] = useState("");
  const [teamData, setTeamData] = useAtom(teamState);
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  //【CP0002】共通フッター設定
  const navigate = useNavigate();
  const SC = createNavigate(navigate);
  const fotterItems = [
    { label: "進捗", action: SC.navigateProgress },
    { label: "", action: () => {} },
    { label: "", action: () => {} },
  ];

  // キーボード以外の部分がクリックされると非表示に
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));

  // テキストボックスのバリデーションチェック
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;

    // 20文字以内
    if (value.length <= 20) {
      setConsultNo(value);
    }
  };

  // バリデーションチェック
  const validationCheck = async () => {
    // バリデーションチェックスキーマ
    //ここも共通化できるかも？
    const validationSchema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, "受診番号は"))
      .max(20, getErrorMessage(errorMessages.maxLength, "受診番号は", 20))
      .regex(
        /^[a-zA-Z0-9]+$/,
        getErrorMessage(errorMessages.alphaNumericString, "受診番号は")
      );
    const result = validationSchema.safeParse(consultNo);
    if (!result.success) {
      setErrorMessage(result.error.errors[0].message);
      open();
      throw new Error("入力された受診番号が正しくありません");
    }
  };

  //AP1007_受診番号を検証する
  const verifyConsultNo = async () => {
    await consultVerifyConsultNumber("1", { consultNumber: consultNo })
      .then(() => {
        // 成功時の処理
        //TODO：【SC0007】検査内容確認画面のパスパラメータに設定する処理
      })
      .catch((error) => {
        // エラー時の処理
        setErrorMessage(error);
        open();
        throw new Error(error);
      });
  };

  // Enterキーが押されたときの処理
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      handleConfirm();
    }
  };

  // 確定処理
  const handleConfirm = async () => {
    try {
      await validationCheck();
      await verifyConsultNo();
      // 全てのチェックが通ったら次の画面に遷移
      navigate("/examorder-confirm");
    } catch (error) {
      console.error("受診番号の検証中にエラーが発生しました:", error);
    }
  };

  return (
    <>
      <AuthWrapper>
        <Box h="100vh" className={styles["basic-grey2"]}>
          <CommonHeader screenName="受診番号入力" staffName="検査 完璧男" />
          <Group mt={30} ml={50} gap="ms">
            <Box className={styles["basic-green"]} w={10} h={70} />
            <Title order={2} fw={550}>
              両備システムズ 豊成事業所
              {/* {teamData?.name} */}
            </Title>
          </Group>
          <Center>
            <Group mt={150}>
              <Paper
                className={styles["basic-grey"]}
                radius="lg"
                px="xl"
                py="md"
              >
                <Title order={1} fw={500}>
                  受診番号
                </Title>
              </Paper>
              <Box>
                <input
                  className={styles["large-input"]}
                  value={consultNo}
                  onFocus={() => setShowKeyboard(true)}
                  onChange={(e) => handleInputChange(e)}
                  onKeyDown={handleKeyDown}
                />
              </Box>
            </Group>
          </Center>
          {showKeyboard && (
            <Center>
              <div ref={closeKeyBoard}>
                <Keyboard
                  size={150}
                  value={consultNo}
                  onChange={(e: string) => setConsultNo(e)}
                  onConfirm={handleConfirm}
                />
              </div>
            </Center>
          )}
          <ErrorModal
            isOpen={opened}
            onClose={close}
            errorMessage={errorMessage}
          />
          <CommonFooter items={fotterItems} />
        </Box>
      </AuthWrapper>
    </>
  );
}
