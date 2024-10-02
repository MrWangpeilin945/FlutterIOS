using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 基幹システム連携ユースケース
/// </summary>
public class IntegrationUsecase : IIntegrationUsecase
{
    private readonly IIntegrationRepository _integrationRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="integrationRepository">基幹システム連携リポジトリ</param>
    public IntegrationUsecase(IIntegrationRepository integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    /// <summary>
    /// 連携対象の検査結果を取得する
    /// </summary>
    public void GetIntegrationResults()
    {

    }

    /// <summary>
    /// 連携用に検査結果を出力する
    /// </summary>
    public void ExportResults()
    {

    }

    /// <summary>
    /// 検査結果の出力履歴を取得する
    /// </summary>
    public void GetExportHistory()
    {

    }
}
