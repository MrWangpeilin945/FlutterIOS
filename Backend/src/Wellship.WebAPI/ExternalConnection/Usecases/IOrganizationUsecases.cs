using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 団体ユースケースインターフェース
    /// </summary>
    public interface IOrganizationUsecases
    {
        /// <summary>
        /// 団体を登録する。
        /// </summary>
        /// <param name="organizations">団体リスト</param>
        /// <returns>エラーオブジェクト</returns>
        public Task<List<ErrorObject>> StoreOrganizationsAsync(List<Organization> organizations);
    }
}
