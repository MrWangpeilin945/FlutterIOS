using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 検査結果ユースケース
/// </summary>
public class ResultUsecase : IResultUsecase
{
    private readonly IResultRepository _resultRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="resultRepository">検査結果リポジトリ</param>
    public ResultUsecase(IResultRepository resultRepository)
    {
        _resultRepository = resultRepository;
    }

    /// <summary>
    /// 検査結果を検証する
    /// </summary>
    public void VerifyResults()
    {

    }

    /// <summary>
    /// 検査結果を登録する
    /// </summary>
    public void RegisterResults()
    {

    }
}
