using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
/// <summary>
/// EC2002_受付を更新する
/// </summary>
public interface IConsultUsecase
{
    /// <summary>
    /// EC2004_受診を更新する
    /// </summary>
    /// <param name="consults">更新する受診のリスト</param>
    /// <returns>エラーリスト</returns>
    public Task<List<ErrorObject>> StoreConsultAsync(List<Consult> consults);
}