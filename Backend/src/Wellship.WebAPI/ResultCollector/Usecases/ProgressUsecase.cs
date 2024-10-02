using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 進捗ユースケース
/// </summary>
public class ProgressUsecase : IProgressUsecase
{
    private readonly IProgressRepository _progressRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="progressRepository">進捗リポジトリ</param>
    public ProgressUsecase(IProgressRepository progressRepository)
    {
        _progressRepository = progressRepository;
    }

    /// <summary>
    /// 進捗状況を取得する
    /// </summary>
    public void GetProgress()
    {

    }
}
