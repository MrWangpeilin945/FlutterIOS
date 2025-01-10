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

    /// <summary>
    /// 存在する検査メニュー特記コード情報（検査特記コード、検査特記名）を取得する
    /// </summary>
    /// <param name="codes">検査メニュー特記コードのリスト</param>
    public Task<List<ExamMenuNoteCodeEntity>> GetExamMenuNodeCodesAsync(List<string> codes);

}
