using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// 受診者
    /// </summary>
    public class Examinee
    {
        /// <summary>
        /// 受診者コード
        /// </summary>
        public required string ExamineeCode { get; init; }

        /// <summary>
        /// 氏名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// カナ氏名
        /// </summary>
        public required string KanaName { get; init; }

        /// <summary>
        /// 性別
        /// </summary>
        public required Sex Sex { get; init; }

        /// <summary>
        /// 生年月日
        /// </summary>
        public required DateTime Birthdate { get; init; }

        /// <summary>
        /// 所属
        /// </summary>
        public required List<Affiliation> Affiliations { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        public required string InputNote { get; init; }
    }
}
