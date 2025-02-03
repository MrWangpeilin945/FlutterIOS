using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2001_受診者を登録する インターフェース
/// </summary>
public interface IExamineeUsecase
{
    /// <summary>
    /// EC2001_受診者を登録する
    /// </summary>
    /// <param name="examinees">受診者リスト</param>
    /// <returns>エラーオブジェクト</returns>
    public Task<List<ErrorObject>> StoreExamineesAsync(List<Examinee> examinees);
}
