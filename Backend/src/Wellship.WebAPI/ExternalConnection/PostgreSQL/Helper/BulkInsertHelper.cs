using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;

using Dapper;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Helper
{
    /// <summary>
    /// BulkInsertヘルパー
    /// </summary>
    public static class BulkInsertHelper
    {
        /// <summary>
        /// BulkInsert実行用メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">登録データ</param>
        /// <param name="valueSelector">セレクタ</param>
        /// <param name="tableName">挿入テーブル</param>
        /// <param name="connection">IDbConnection</param>
        public static async Task BulkInsert<T>(IEnumerable<T> source, Func<T, string> valueSelector, string tableName, IDbConnection connection)
        {
            // 登録データを分割
            var chunks = SplitIntoChunks(source);

            // 分割サイズ毎に実行
            foreach (var chunk in chunks)
            {
                // 実行クエリ生成
                var sql = new StringBuilder($"insert into {tableName} values ");
                sql.Append(string.Join(",", chunk.Select(valueSelector)));
                sql.Append(";");

                // クエリ実行
                await connection.ExecuteAsync(sql.ToString());
            }
        }

        /// <summary>
        /// 挿入データの分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<IEnumerable<T>> SplitIntoChunks<T>(IEnumerable<T> source)
        {
            // 1,000件毎に分割
            int chunkSize = 1000;
            return source.Select((item, index) => new { item, index }).GroupBy(x => x.index / chunkSize).Select(g => g.Select(x => x.item)).ToList();
        }
    }
}
