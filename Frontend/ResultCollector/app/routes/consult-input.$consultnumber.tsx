import CommonFooter from "~/components/CommonFooter";
import ExamineeHeader from "~/components/ExamineeHeader";
<<<<<<< HEAD
import ExamBP2 from "~/components/ExamBP2";
=======
import ExamNumeric from "~/components/ExamNumeric";
>>>>>>> b3f1bc51481586acd3aeee4bc6d82aa075db60ca
import ExamSelect from "~/components/ExamSelect";
import type { InputExamItem } from "~/domain/wellship.schemas";
import { errorMessages } from "~/utils/getErrorMessage";

export default function consultInput() {
<<<<<<< HEAD
  const data: InputExamItem[] = [
    {
      positionNumber: 1,
      examItemId: 101,
      name: "血圧1",
      examRegistResults: [
        { description: "APIエラー1:異常", errorLevel: 2 },
        { description: "APIエラー2:警告", errorLevel: 3 },
      ],
      examItemDetails: [
        {
          positionNumber: 1,
          examItemDetailId: 1001,
          name: "上",
          value: "120",
          prevValue: "120",
          unit: "cm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value1",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.1", "0.2", "0.3"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 3,
              minValue: 250.0,
              maxValue: 999.9,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
        {
          positionNumber: 2,
          examItemDetailId: 1002,
          name: "下",
          value: "60",
          prevValue: "50",
          unit: "mm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value2",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.4", "0.5", "0.6"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
      ],
    },
    {
      positionNumber: 2,
      examItemId: 102,
      name: "血圧2",
      examRegistResults: [
        {
          description: "APIエラー1:警告 脈圧が正常値ではありません。",
          errorLevel: 2,
        },
      ],
      examItemDetails: [
        {
          positionNumber: 1,
          examItemDetailId: 1003,
          name: "上",
          value: "130",
          prevValue: "130",
          unit: "cm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value3",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.1", "0.2", "0.3"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 3,
              minValue: 250.0,
              maxValue: 999.9,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
        {
          positionNumber: 2,
          examItemDetailId: 1004,
          name: "下",
          value: "70",
          prevValue: "60",
          unit: "mm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value4",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.4", "0.5", "0.6"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
      ],
    },
    {
      positionNumber: 3,
      examItemId: 103,
      name: "平均",
      examRegistResults: [],
      examItemDetails: [
        {
          positionNumber: 1,
          examItemDetailId: 1005,
          name: "上",
          value: "120",
          prevValue: "120",
          unit: "cm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value5",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.1", "0.2", "0.3"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 3,
              minValue: 250.0,
              maxValue: 999.9,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
        {
          positionNumber: 2,
          examItemDetailId: 1006,
          name: "下",
          value: "60",
          prevValue: "50",
          unit: "mm",
          type: 1,
          cancelReasonId: undefined,
          equipmentLabel: "★value6",
          integerLength: 3,
          decimalLength: 0,
          keyboard: {
            keyboardType: 1,
            values: ["0.4", "0.5", "0.6"],
          },
          examNormalValueRanges: [
            {
              errorLevel: 3,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 2,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
      ],
    },
  ];
=======
  const data = {
    examItems: [
      {
        positionNumber: 1,
        examItemId: 101,
        name: "検査項目検査項目",
        //どのような形式かは未確定
        errorMessages: [{ value: "エラー1" }, { value: "エラー2" }],
        examItemDetails: [
          {
            positionNumber: 1, //血圧の上なのか下なのかのテキストボックス位置を指定する
            examItemDetailId: 1001,
            examItemDetailName: "スピッツ",
            value: "10031", //前回値初期値化設定があれば、ここに入れておいてほしい   インクリメントの場合登録済みなら、登録済みの値、未登録なら次のインクリメント値をもらう
            prevValue: "1111111111",
            unit: "cm",
            examItemDetailType: "1", //1:入力、2:選択、3：演算値など ※ここのテーブル設定を知らないのでとりあえずの例
            isCancelled: true,
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
>>>>>>> b3f1bc51481586acd3aeee4bc6d82aa075db60ca

  const handleConfirm = () => {};

  return (
    <>
      <ExamineeHeader
        staffName="両備 花子"
        managerId={100001}
        name="リョウビ タロウ"
        gender={1}
        age={35}
      />
<<<<<<< HEAD
      <ExamBP2
        examItems={data}
=======
      <ExamSelect
        examItems={data.examItems[0]}
        onRegisterPressed={1}
        onClick={handleConfirm}
      />
      <ExamNumeric
        examItems={data.examItems}
>>>>>>> b3f1bc51481586acd3aeee4bc6d82aa075db60ca
        onRegisterPressed={true}
        onChange={handleConfirm}
      />
      <CommonFooter />
    </>
  );
}
