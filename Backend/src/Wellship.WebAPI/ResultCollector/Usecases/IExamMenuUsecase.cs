using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 検査メニューユースケースのインターフェース
/// </summary>
public interface IExamMenuUsecase
{
    /// <summary>
    /// AP1006_検査メニュー一覧を取得する
    /// </summary>
    public Task<ExamMenuList> GetExamMenusAsync();
}