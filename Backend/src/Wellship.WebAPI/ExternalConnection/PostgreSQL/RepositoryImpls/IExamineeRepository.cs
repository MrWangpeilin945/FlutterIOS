using System.Data.Common;
using System.Transactions;

using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls
{
    /// <summary>
    /// 受診者リポジトリインターフェース
    /// </summary>
    public interface IExamineeRepository
    {
        /// <summary>
        /// 受診者を登録する。
        /// </summary>
        /// <param name="examineeEntities">受診者エンティティリスト</param>
        /// <param name="createdAt">作成日時</param>
        /// <param name="createdBy">作成者</param>
        /// <returns></returns>
        public Task UpsertExamineesAsync(List<ExamineeEntity> examineeEntities, DateTime createdAt, string createdBy);

        /// <summary>
        /// 存在する受診者情報（ID、コード）を取得する
        /// </summary>
        /// <param name="examinees">受診者コードのリスト</param>
        /// <returns></returns>
        public Task<List<ExamineeEntity>> GetExamineeInfoAsync(List<string> examinees);
    }
}
