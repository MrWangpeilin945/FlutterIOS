import { date } from "zod";
import CommonFooter from "~/components/CommonFooter";
import ExamineeHeader from "~/components/ExamineeHeader";
import ExamNumricLR from "~/components/ExamNumericLR";
import ExamSelect from "~/components/ExamSelect";
import type { InputExamItem } from "~/domain/wellship.schemas";
import { errorMessages } from "~/utils/getErrorMessage";

export default function consultInput() {
  const testdata: InputExamItem[] = [
    {
      positionNumber: 1,
      examItemId: 101,
      name: "スピッツ",
      //どのような形式かは未確定
      examRegistResults: [
        { description: "エラー1", errorLevel: 2 },
        { description: "エラー2", errorLevel: 3 },
      ],
      examItemDetails: [
        {
          positionNumber: 1, //血圧の上なのか下なのかのテキストボックス位置を指定する
          examItemDetailId: 1001,
          name: "左",
          value: "1000", //前回値初期値化設定があれば、ここに入れておいてほしい   インクリメントの場合登録済みなら、登録済みの値、未登録なら次のインクリメント値をもらう
          prevValue: "120",
          unit: "cm",
          type: 1, //1:入力、2:選択、3：演算値など ※ここのテーブル設定を知らないのでとりあえずの例
          cancelReasonId: undefined,
          equipmentLabel: "★value1", //検討中連携している機器のどのパラメータに該当するかのプロパティ的なもの
          decimalLength: 1, //小数点以下の入力 ※必要か？
          integerLength: 3,
          keyboard: {
            keyboardType: 1, //0～9タイプか、カスタムか
            values: ["0.1", "0.2", "0.3"], //0～9タイプの時は不要
          },
          examItemDetailOptions: undefined, //選択系
          examNormalValueRanges: [
            //エラーレベルの高い順でソートして渡してもらう
            {
              errorLevel: 4,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 4,
              minValue: 250.0,
              maxValue: 999.9,
            },
            {
              errorLevel: 3,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
        {
          positionNumber: 2, //血圧の上なのか下なのかのテキストボックス位置を指定する
          examItemDetailId: 1001,
          name: "右",
          value: "752", //前回値初期値化設定があれば、ここに入れておいてほしい   インクリメントの場合登録済みなら、登録済みの値、未登録なら次のインクリメント値をもらう
          prevValue: "800",
          unit: "kg",
          type: 1, //1:入力、2:選択、3：演算値など ※ここのテーブル設定を知らないのでとりあえずの例
          cancelReasonId: undefined,
          equipmentLabel: "★value1", //検討中連携している機器のどのパラメータに該当するかのプロパティ的なもの
          decimalLength: 2, //小数点以下の入力 ※必要か？
          integerLength: 4,
          keyboard: {
            keyboardType: 1, //0～9タイプか、カスタムか
            values: ["0.1", "0.2", "0.3"], //0～9タイプの時は不要
          },
          examItemDetailOptions: undefined, //選択系
          examNormalValueRanges: [
            //エラーレベルの高い順でソートして渡してもらう
            {
              errorLevel: 4,
              minValue: 0.0,
              maxValue: 50.5,
            },
            {
              errorLevel: 4,
              minValue: 250.0,
              maxValue: 999.9,
            },
            {
              errorLevel: 3,
              minValue: 50.0,
              maxValue: 100.0,
            },
          ],
        },
      ],
    },
  ];

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
      <ExamNumricLR
        examItems={testdata}
        onRegisterPressed={true}
        onChange={handleConfirm}
      />
      <CommonFooter />
    </>
  );
}
