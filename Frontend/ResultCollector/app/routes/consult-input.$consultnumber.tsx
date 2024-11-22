import { Button, Center, Container, Flex } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useNavigate, useParams, useSearchParams } from "@remix-run/react";
import { useAtom } from "jotai";
import { useEffect, useState } from "react";
import BoothNote from "~/components/BoothNote";
import CommonFooter from "~/components/CommonFooter";
import { ErrorModal } from "~/components/ErrorModal";
import ExamineeHeader from "~/components/ExamineeHeader";
import ExamSelect from "~/components/ExamSelect";
import { examMenuState, staffState } from "~/store/store";
import { errorMessages, getErrorMessage } from "~/utils/getErrorMessage";
import { resultRegisterResults } from "~/api/wellship";
import { NamedEntity } from "~/interfaces/interfaces";

export default function consultInput() {
  const navigate = useNavigate();
  // パスパラメータの取得
  const { consultnumber } = useParams();
  // クエリパラメータの取得
  const [searchParams] = useSearchParams();
  const examMenuId = searchParams.get("exammenuid");
  //TODO:機器情報をJotaiから取得
  const [staffData] = useAtom(staffState);
  const [examMenus] = useAtom(examMenuState);
  const [opened, { open, close }] = useDisclosure(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!consultnumber) {
      setErrorMessage("必要な受診番号がありません");
      open();
      return;
    }
    if (!examMenuId) {
      setErrorMessage("必要な検査メニューIDがありません");
      open();
      return;
    }
    //TODO：API呼び出し、通過の判断処理
  }, [consultnumber, examMenuId, open]);

  const callbackCloseModal = () => {
    close();
    if (!consultnumber || !examMenuId) {
      navigate(-1);
    }
  };

  const callbackChangeValue = () => {
    //valueを更新
    //再計測ボタンのDisabledチェック処理
  }

  //TODO：機器連携

  //AP1013_検査結果を検証する
  const verifyResults = async () => {

  }

  //AP1014_検査結果を登録する
  //TODO:ボディ作成
  const registerResults = async () => {
    if(!consultnumber) return;
    await resultRegisterResults("1", consultnumber)
      .then(() => {
        // 成功時の処理
        
      })
      .catch((error) => {
        if (error.responce.status === 400) {
          setErrorMessage(getErrorMessage(errorMessages.invalid, "指定した受診番号"));
        } else if (error.responce.status === 404) {
          setErrorMessage(
            getErrorMessage(errorMessages.noData, "指定した受診情報が"),
          );
        } else if (error.responce.status === 500) {
          setErrorMessage(getErrorMessage(errorMessages.server));
        }
        open();
      });
  };

  //検査継続処理
  const continuingExam = () => {
    if(!examMenus) return;
    const currentIndex = examMenus.findIndex((menu) => menu === examMenuId);
    if (currentIndex !== -1 && currentIndex < examMenus.length - 1) {
      const nextExam = examMenus[currentIndex + 1];
      navigate(`/examorder-confirm/${consultnumber}?exammenuid=${nextExam}`);
    } else {
      navigate(`/consultnumber-input?consultnumber=${consultnumber}`);
    }
  }

  const handleRegister = () => {
    registerResults();
    continuingExam();
  
  };

  const data = {
    examItems: [
      {
        positionNumber: 1,
        examItemId: 101,
        examItemName: "スピッツ",
        //どのような形式かは未確定
        errorMessages: [{ value: "エラー1" }, { value: "エラー2" }],
        examItemDetails: [
          {
            positionNumber: 1, //血圧の上なのか下なのかのテキストボックス位置を指定する
            examItemDetailId: 1001,
            examItemDetailName: "スピッツ",
            value: "10031", //前回値初期値化設定があれば、ここに入れておいてほしい   インクリメントの場合登録済みなら、登録済みの値、未登録なら次のインクリメント値をもらう
            prevValue: "111112222333",
            unit: "cm",
            examItemDetailType: "1", //1:入力、2:選択、3：演算値など ※ここのテーブル設定を知らないのでとりあえずの例
            isCancelled: false,
            kikiDetail: "★value1", //検討中連携している機器のどのパラメータに該当するかのプロパティ的なもの
            afterDecimalPointDigit: 1, //小数点以下の入力 ※必要か？
            keyboard: {
              Type: 1, //0～9タイプか、カスタムか
              keys: ["0.1", "0.2", "0.3"], //0～9タイプの時は不要
            },
            selectors: [
              {
                selectorId: "10031", //表示順にソートしてもらう
                selectorName: "メガネ",
              },
              {
                selectorId: "10032",
                selectorName: "コンタクト",
              },
              {
                selectorId: "10033",
                selectorName: "テスト1",
              },
              {
                selectorId: "10034",
                selectorName: "テスト2",
              },
            ], //選択系
            ranges: [
              //エラーレベルの高い順でソートして渡してもらう
              {
                errorLevel: 4,
                numericMin: 0.0,
                numericMax: 50.5,
              },
              {
                errorLevel: 4,
                numericMin: 250.0,
                numericMax: 999.9,
              },
              {
                errorLevel: 3,
                numericMin: 50.0,
                numericMax: 100.0,
              },
            ],
          },
        ],
      },
    ],
  };

  const BoothData = [
    {
      examItemName: "視力",
      examResult: "1.0",
    },
    {
      examItemName: "テストテストテス",
      examResult: "150.7",
    },
  ];
  

  return (
    <>
      <ExamineeHeader
        staffName={staffData?.name}
        managerId={100001}
        name="リョウビ タロウ"
        gender={1}
        age={35}
      />
      <BoothNote relatedExamItems={BoothData} />
      <Flex align="center" direction="column" my={10}>
        <ExamSelect
          examItems={data.examItems[0]}
          onRegisterPressed={1}
          onClick={callbackChangeValue}
        />
        <Center>
          <Button w={500} h={50} my={30} onClick={handleRegister}>
            登録する
          </Button>
        </Center>
      </Flex>

      <ErrorModal
        isOpen={opened}
        onClose={callbackCloseModal}
        errorMessage={errorMessage}
      />
      <CommonFooter />
    </>
  );
}
