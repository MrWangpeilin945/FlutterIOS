using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Domain.Models
{
    /// <summary>
    /// パスワード認証情報
    /// </summary>
    /// <remarks>コンストラクタ</remarks>
    /// <param name="hash">ハッシュ</param>
    /// <param name="salt">ソルト</param>
    public readonly struct Password(IEnumerable<byte> hash, IEnumerable<byte> salt)
    {
        /// <summary>
        /// パスワードハッシュ
        /// </summary>
        public ImmutableArray<byte> Hash { get; init; } = hash.ToImmutableArray();
        /// <summary>
        /// パスワードソルト
        /// </summary>
        public ImmutableArray<byte> Salt { get; init; } = salt.ToImmutableArray();

        /// <summary>
        /// パスワード文字列からハッシュ・ソルトを生成します
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public static Password Create(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return new Password(hash, salt);
        }

        /// <summary>
        /// パスワード文字列を検証します
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Verify(string password)
        {
            if (password == "" || Hash.Length != 64 || Salt.Length != 128)
            {
                return false;
            }
            using var hmac = new HMACSHA512(Salt.ToArray());
            // NOTE: ストレッチングの要否は要件を確認した上で判断します
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return hash.SequenceEqual(Hash);
        }

    }
}
