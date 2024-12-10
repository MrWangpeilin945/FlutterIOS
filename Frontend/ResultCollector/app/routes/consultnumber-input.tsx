import {
  Box,
  Center,
  Group,
  LoadingOverlay,
  Paper,
  TextInput,
  Title,
} from "@mantine/core";
import { useClickOutside, useDisclosure } from "@mantine/hooks";
import { useFocusTrap } from "@mantine/hooks";
import type { MetaFunction } from "@remix-run/node";
import { useNavigate } from "@remix-run/react";
import { isAxiosError } from "axios";
import { useAtom } from "jotai";
import type React from "react";
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { z } from "zod";
import { useConsultGetUnexaminedMenus } from "~/api/wellship";
import { useConsultVerifyConsultNumber } from "~/api/wellship";
import AuthWrapper from "~/components/AuthWrapper";
import CommonDialog from "~/components/CommonDialog";
import CommonFooter from "~/components/CommonFooter";
import CommonHeader from "~/components/CommonHeader";
import IncompliedExam from "~/components/IncompliedExam";
import Keyboard from "~/components/NumericKeyboard";
import { examMenuState, placeScheduleState, staffState } from "~/store/store";
import styles from "~/styles/common.module.css";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";

export const meta: MetaFunction = () => {
  return [{ title: "受診番号入力" }];
};

export default function consultNumberInput() {
  const navigate = useNavigate();
  const focusTrapRef = useFocusTrap();
  const [placeSchedule] = useAtom(placeScheduleState);
  const [showKeyboard, setShowKeyboard] = useState(false);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [staff] = useAtom(staffState);
  const [beforeIncompData, setBeforeIncompData] = useState<string[]>([]);
  const [beforeExamName, setBeforeExamName] = useState("");
  const [isBeforeNum, setIsBeforeNum] = useState(false);
  const [menu] = useAtom(examMenuState);

  // URLのパスパラメータ
  const [searchParams] = useSearchParams();
  const [consultNumber, setConsultNumber] = useState(
    searchParams.get("consultnumber") || "",
  );

  //AP1008呼び出し用(GET系APIの定義)
  const { isFetching, refetch } = useConsultGetUnexaminedMenus(
    "1",
    consultNumber || "",
    { query: { enabled: false } },
  );

  //AP1007呼び出し用(POST系APIの定義)
  const { mutateAsync } = useConsultVerifyConsultNumber();

  //【CP0002】共通フッター設定
  const footerItems = [
    { label: "進捗", action: () => navigate("/progress") },
    { label: "", action: () => {} },
    { label: "", action: () => {} },
  ];

  useEffect(() => {
    //AP1008_未受診の検査項目を取得する
    if (consultNumber) {
      const fetchUnexaminedItemsSelect = async () => {
        const result = await refetch();
        if (result.data) {
          //成功時
          setIsBeforeNum(true);

          const examineeName = result.data.data.examineeName || "";
          setBeforeExamName(examineeName);

          //未実施検査項目の取得
          if (result.data.data.unexaminedMenus) {
            const incompliedList: string[] =
              result.data.data.unexaminedMenus.map(
                (ExamMenu) => ExamMenu.examMenuName || "",
              );
            setBeforeIncompData(incompliedList);
          }
        } else if (result.error) {
          if (result.error.status === 400) {
            setErrorMessage(getErrorMessage(errorMessages.invalid, "受診番号"));
          } else if (result.error.status === 404) {
            setErrorMessage(
              getErrorMessage(errorMessages.noData, "該当の受診番号のデータ"),
            );
          } else if (result.error.status === 500) {
            setErrorMessage(getErrorMessage(errorMessages.serverError));
          }
          open();
        }
      };
      fetchUnexaminedItemsSelect();
    }
  }, []);

  // キーボード以外の部分がクリックされると非表示に
  const closeKeyBoard = useClickOutside(() => setShowKeyboard(false));

  // テキストボックスのバリデーションチェック
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // 20文字以内
    if (value.length <= 20) {
      setConsultNumber(value);
    }
  };

  // バリデーションチェック
  const validationCheck = () => {
    // バリデーションチェックスキーマ
    const validationSchema = z
      .string()
      .min(1, getErrorMessage(errorMessages.required, "受診番号は"))
      .max(20, getErrorMessage(errorMessages.maxLength, "受診番号は", 20))
      .regex(
        /^[a-zA-Z0-9]+$/,
        getErrorMessage(errorMessages.alphaNumericString, "受診番号は"),
      );

    const result = validationSchema.safeParse(consultNumber);
    if (!result.success) {
      setErrorMessage(result.error.errors[0].message);
      open();
      return false;
    }
    return true;
  };

  //AP1007_受診番号を検証する
  const verifyConsultNumber = async () => {
    // POST時のリクエストボディを生成する
    const body = {
      consultNumber: consultNumber || "",
    };

    const postMutateAsync = async () => {
      try {
        const result = await mutateAsync({
          version: "1",
          data: body,
        });
        if (result.status === 200) {
          //受診番号ありの場合
          if (consultNumber) {
            //検査メニューIDをパスパラメータへセット
            let exammenuid = 0;
            if (menu) {
              exammenuid = menu[0]?.id || 0;
            }
            //次の画面に遷移
            navigate(
              `/examorder-confirm/${consultNumber}?exammenuid=${exammenuid}`,
            );
          }
        }
      } catch (error) {
        if (isAxiosError(error) && error.response) {
          if (error.response.status === 400) {
            setErrorMessage(getErrorMessage(errorMessages.invalid, "受診番号"));
          } else if (error.response.status === 404) {
            setErrorMessage(
              getErrorMessage(errorMessages.noData, "該当の受診番号のデータ"),
            );
          } else if (error.response.status === 500) {
            setErrorMessage(getErrorMessage(errorMessages.serverError));
          }
          open();
        }
      }
    };
    postMutateAsync();
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
      await verifyConsultNumber();
    }
  };

  return (
    <>
      <AuthWrapper>
        <LoadingOverlay visible={isFetching} />
        <CommonHeader screenName="受診番号入力" staffName={staff?.name || ""} />
        {!isFetching && (
          <>
            <Group mt={30} ml={50} gap="ms">
              <Box className={styles["basic-green"]} w={10} h={70} />
              <Title order={2} fw={550}>
                {placeSchedule?.placeName || ""}
              </Title>
            </Group>
            <Center>
              <div ref={focusTrapRef}>
                <Group mt={50}>
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
                    <TextInput
                      data-autofocus
                      size="xl"
                      value={consultNumber || ""}
                      onFocus={() => setShowKeyboard(true)}
                      onChange={(e) => handleInputChange(e)}
                      onKeyDown={handleKeyDown}
                    />
                  </Box>
                </Group>
              </div>
            </Center>
            {showKeyboard && (
              <Center>
                <div ref={closeKeyBoard}>
                  <Keyboard
                    value={consultNumber || ""}
                    onChange={(e: string) => setConsultNumber(e)}
                    onConfirm={handleConfirm}
                  />
                </div>
              </Center>
            )}
            {isBeforeNum && (
              <Box mt="50">
                <IncompliedExam
                  name={beforeExamName}
                  incompliedExams={beforeIncompData}
                />
              </Box>
            )}
            <CommonDialog
              message={errorMessage || ""}
              buttonMessage="閉じる"
              isOpen={opened}
              onClose={close}
            />
          </>
        )}
        <CommonFooter items={footerItems} />
      </AuthWrapper>
    </>
  );
}
