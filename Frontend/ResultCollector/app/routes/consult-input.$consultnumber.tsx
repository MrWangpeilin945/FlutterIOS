import CommonFooter from "~/components/CommonFooter";
import ExamineeHeader from "~/components/ExamineeHeader";
import ExamBP2 from "~/components/ExamBP2";
import ExamSelect from "~/components/ExamSelect";
import type { InputExamItem } from "~/domain/wellship.schemas";
import { errorMessages } from "~/utils/getErrorMessage";

export default function consultInput() {
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
        { description: "APIエラー1:異常", errorLevel: 2 },
        { description: "APIエラー2:警告", errorLevel: 3 },
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
      <ExamBP2
        examItems={data}
        onRegisterPressed={true}
        onChange={handleConfirm}
      />
      <CommonFooter />
    </>
  );
}
