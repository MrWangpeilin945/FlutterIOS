using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.Entities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.PostgreSQL.RepositoryImpls;
/// <summary>
/// 受付を更新するRepository層
/// </summary>
public interface IConsultRepository
{
    /// <summary>
    /// 受診を更新する
    /// </summary>
    /// <param name="consults">登録する受診のリスト</param>
    /// <param name="createdAt">作成日時</param>
    /// <param name="createdBy">作成者</param>
    public Task UpsertConsultsAsync(List<string> consults, DateTime createdAt, string createdBy);

}
