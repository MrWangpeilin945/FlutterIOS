using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 検査メニューユースケース
/// </summary>
public class ExamMenuUsecase : IExamMenuUsecase
{
    private readonly IExamMenuRepository _examMenuRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="examMenuRepository">検査メニューリポジトリ</param>
    public ExamMenuUsecase(IExamMenuRepository examMenuRepository)
    {
        _examMenuRepository = examMenuRepository;
    }

    /// <summary>
    /// 検査メニュー一覧を取得する
    /// </summary>
    public void GetExamMenus()
    {

    }
}
