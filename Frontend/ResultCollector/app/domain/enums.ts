/**
 * 性別
 * @description 受診者の生物学的性別を表現するコード。国際規格であるISO 5218に則る。
 */
export enum Sex {
  男 = 1,
  女 = 2,
}

/**
 * 対象性別
 * @description 基準値などの設定に使用します。
 */
export enum TargetSex {
  男 = 1,
  女 = 2,
  両方 = 3,
}

/**
 * 受診進捗状況
 * @description 受診単位の進捗ステータスです。
 */
export enum ConsultProgressStatus {
  来場待ち = 11,
  検査中 = 21,
  キャンセル = 41,
}

/**
 * 検査進捗状況
 * @description 検査項目単位の進捗ステータスです。
 */
export enum ExamItemProgressStatus {
  未実施 = 11,
  検査済み = 41,
  検査中止 = 51,
}

/**
 * 集計検査進捗状況
 * @description 進捗画面で取り扱うステータスです。
 */
export enum AggregatedProgressStatus {
  予定 = 11,
  来場 = 21,
  済 = 41,
  中止 = 51,
}

/**
 * 会場日程状況
 * @description 会場ロックのステータスです。
 */
export enum PlaceScheduleLockingStatus {
  検査中 = 21,
  検査完了 = 31,
}

/**
 * 会場日程データ出力状況
 * @description 会場日程単位のデータ出力ステータスです。
 */
export enum PlaceScheduleResultExportStatus {
  未出力 = 11,
  出力済み = 31,
  出力エラー = 41,
}

/**
 * 受診データ出力状況
 * @description 受診単位のデータ出力ステータスです。
 */
export enum ConsultResultExportStatus {
  未出力 = 11,
  出力保留 = 21,
  出力済み = 31,
}

/**
 * キーボード種別
 * @description ソフトウェアキーボードのタイプです。
 */
export enum KeyboardType {
  テンキー = 1,
  選択肢 = 2,
}

/**
 * 検査項目グループ種別
 * @description コンポーネントを出し分けるためのタイプです。 ざっくり下一桁と上N桁で分類していますが、アプリケーションとしては単なる識別子として使います。
 */
export enum ExamItemGroupType {
  自由入力 = 11,
  数値 = 21,
  数値_左右 = 22,
  選択 = 31,
  選択_左右 = 32,
  身体計測 = 41,
  血圧2回 = 51,
  視力 = 61,
  聴力 = 71,
  通過 = 81,
}

/**
 * 検査項目明細種別
 * @description フロントエンドで入力や選択を出し分けるためのタイプです。 検査項目種別ごとに設定します。
 */
enum ExamItemDetailType {
  入力 = 1,
  選択 = 2,
  演算値 = 3,
}

/**
 * 入力エラーレベル
 * @description 入力した結果値に対するエラーを表示するときのレベルです。
 */
export enum InputErrorLevel {
  正常 = 1,
  警告 = 2,
  異常 = 3,
}

/**
 * ロール
 * @description 職員に対して付与する役割です。権限グループとして使います。
 */
export enum Role {
  一般 = 10,
  管理者 = 20,
}
