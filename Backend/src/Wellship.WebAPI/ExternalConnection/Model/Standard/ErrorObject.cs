using System.Text.Json.Serialization;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    /// <summary>
    /// エラーオブジェクト
    /// </summary>
    public class ErrorObject
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonPropertyName("message")] 
        public required string Message { get; init; }

        /// <summary>
        /// 入力項目Noなど
        /// </summary>
        [JsonPropertyName("inputNote")]
        public required string InputNote { get; init; }

        /// <summary>
        /// オブジェクトの比較のためEqualsメソッドをオーバーライドする
        /// </summary>
        /// <param name="obj">比較対象</param>
        public override bool Equals(object? obj)
        {
            if (obj is ErrorObject other)
            {
                return Code == other.Code &&
                    Message == other.Message &&
                    InputNote == other.InputNote;
            }
            return false;
        }

        /// <summary>
        /// オブジェクトの比較のためGetHashCodeをオーバーライドする
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Code, Message, InputNote);
        }        
    }
}
