namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
/// <summary>
/// ファイル取り込みを実行する インターフェース
/// </summary>
public interface IDataImportUsecase
{
    /// <summary>
    /// C1001_ファイル取り込みを実行する_随時
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="fileKey">オブジェクト名</param>
    /// <returns>エラーリスト</returns>
    public Task<Boolean> StoreConstantlyDataAsync(string bucketName, string fileKey);

    /// <summary>
    /// EC1002_ファイル取り込みを実行する_日次
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <returns>エラーリスト</returns>
    public Task<Boolean> StoreDailyDataAsync(string bucketName);
}
