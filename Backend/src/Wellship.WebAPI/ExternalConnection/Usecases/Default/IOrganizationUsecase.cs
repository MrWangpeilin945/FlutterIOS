using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// EC2008_団体ユースケース インターフェース
/// </summary>
public interface IOrganizationUsecase
{
    /// <summary>
    /// EC2008_団体を登録する。
    /// </summary>
    /// <param name="organizations">団体リスト</param>
    /// <returns>エラーオブジェクト</returns>
    public Task<List<ErrorObject>> StoreOrganizationsAsync(List<Organization> organizations);
}
