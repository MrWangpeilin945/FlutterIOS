using System.Text.Json.Serialization;
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
        [JsonPropertyName("examineeCode")]
        public required string ExamineeCode { get; init; }

        /// <summary>
        /// 氏名
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// カナ氏名
        /// </summary>
        [JsonPropertyName("kanaName")]
        public required string KanaName { get; init; }

        /// <summary>
        /// 性別
        /// </summary>
        [JsonPropertyName("sex")]
        public required Sex Sex { get; init; }

        /// <summary>
        /// 生年月日
        /// </summary>
        [JsonPropertyName("birthdate")]
        public required DateOnly Birthdate { get; init; }

        /// <summary>
        /// 所属
        /// </summary>
        [JsonPropertyName("affiliations")]
        public required List<Affiliation> Affiliations { get; init; }

        /// <summary>
        /// エラーオブジェクトに登録する入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }
    }
}
