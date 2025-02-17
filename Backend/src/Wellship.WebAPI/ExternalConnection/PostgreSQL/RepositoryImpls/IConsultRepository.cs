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
    public Task UpsertConsultsAsync(List<ConsultEntity> consults, DateTimeOffset createdAt, string createdBy);

    /// <summary>
    /// 存在する検査メニュー特記コード情報（検査特記コード、検査特記名）を取得する
    /// </summary>
    /// <param name="codes">検査メニュー特記コードのリスト</param>
    public Task<List<ExamMenuNoteCodeEntity>> GetExamMenuNodeCodeInfoAsync(List<string> codes);

    /// <summary>
    /// 存在する外部検査項目明細情報（検査項目明細ID、外部コード検査項目明細CD）を取得する
    /// </summary>
    /// <param name="codes">外部コード検査項目明細コードのリスト</param>
    public Task<List<ExternalExamItemDetailEntity>> GetExternalExamItemDetailInfoAsync(List<string> codes);

    /// <summary>
    /// 存在する受診情報の外部連携キーを取得する
    /// </summary>
    /// <param name="codes">連携キーのリスト</param>
    public Task<List<ExternalConnectionCodeEntity>> GetExternalConnectionCodeAsync(List<string> codes);

    /// <summary>
    /// 受診番号に紐づけられた外部連携キーを取得する
    /// </summary>
    /// <param name="consultNumbers">受診番号のリスト</param>
    public Task<List<ExternalConnectionCodeEntity>> GetConsultExternalConnectionCodeAsync(List<string> consultNumbers);

}
