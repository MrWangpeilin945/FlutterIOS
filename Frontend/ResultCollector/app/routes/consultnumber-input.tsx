import type React from "react";
import { useState } from "react";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { z } from "zod";
import { useAtom } from "jotai";
import { placeScheduleState } from "~/store/store";
import { Group, Title, Box, Center, Paper, TextInput } from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { consultVerifyConsultNumber } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonHeader from "~/components/CommonHeader";
import CommonFooter from "~/components/CommonFooter";
import Keyboard from "~/components/SoftwareKeyboard";
import IncompliedExam from "~/components/IncompliedExam";
import { ErrorModal } from "~/components/ErrorModal";
import { getErrorMessage, errorMessages } from "~/utils/getErrorMessage";
import styles from "~/styles/common.module.css";

export const meta: MetaFunction = () => {
  return [{ title: "受診番号入力" }];
};

export default function consultNumberInput() {
  const [consultNo, setConsultNo] = useState("");
  const [placeSchedule, setPlaceSchedule] = useAtom(placeScheduleState);
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  //【CP0002】共通フッター設定
  const navigate = useNavigate();
  const fotterItems = [
    { label: "進捗", action: () => navigate("/progress") },
    { label: "", action: () => {} },
    { label: "", action: () => {} },
  ];

  // Todo 後に消す【CP0011】未受診検査項目の確認用の設定
  const incompliedExams = [
    "胸囲",
    "検尿",
    "視力",
    "診察",
    "胸部X線",
    "心電図",
    "マーゲン",
    "肺活量",
    "MRI",
    "CT3",
    "3",
    "身長体重",
    "診察",
    "問診",
    "健康指導",
    "胃カメラ",
    "エコーエコーエコーエコーエコーエコーエコーエコーエコーエコー",
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
  const validationCheck = () => {
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
      return false;
    }
    return true;
  };

  //AP1007_受診番号を検証する
  const verifyConsultNo = async () => {
    await consultVerifyConsultNumber("1", { consultNumber: consultNo })
      .then(() => {
        // 成功時の処理
        //TODO：【SC0007】検査内容確認画面のパスパラメータに設定する処理
      })
      .catch((error) => {
        if (error.responce.status === 400) {
          setErrorMessage(getErrorMessage(errorMessages.invalid, "受診番号"));
        } else if (error.responce.status === 404) {
          setErrorMessage(
            getErrorMessage(errorMessages.noData, "該当の受診番号のデータ")
          );
        } else if (error.responce.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.server));
        }
        open();
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
    if (validationCheck()) {
      await verifyConsultNo();
      // 全てのチェックが通ったら次の画面に遷移
      navigate("/examorder-confirm");
    }
  };

  return (
    <>
      <AuthWrapper>
        <CommonHeader screenName="受診番号入力" staffName="両備 太郎" />
        <Group mt={30} ml={50} gap="ms">
          <Box className={styles["basic-green"]} w={10} h={70} />
          <Title order={2} fw={550}>
            両備システムズ 豊成事業所
            {/* {placeSchedule?.placeName} */}
          </Title>
        </Group>
        <Center>
          <Group mt={50}>
            <Paper className={styles["basic-grey"]} radius="lg" px="xl" py="md">
              <Title order={1} fw={500}>
                受診番号
              </Title>
            </Paper>
            <Box>
              <TextInput
                size="xl"
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
        <Box mt="50">
          {/* TODO 確認用のため、後で変更 */}
          <IncompliedExam
            name="あいうえおかきくけこさしすせそたちつてとなにぬねの"
            incompliesExam={incompliedExams}
          />
        </Box>
        <ErrorModal
          isOpen={opened}
          onClose={close}
          errorMessage={errorMessage}
        />
        <CommonFooter items={fotterItems} />
      </AuthWrapper>
    </>
  );
}
