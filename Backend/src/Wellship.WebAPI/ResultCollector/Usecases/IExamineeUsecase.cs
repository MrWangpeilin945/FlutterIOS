using Ryobi.Wellship.APIModels.Responses;

using Ryobi.Wellship.Core.Enums;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診者ユースケースのインターフェース
/// </summary>
public interface IExamineeUsecase
{
    /// <summary>
    /// AP1024_受診者一覧を取得する
    /// </summary>
    public Task<ConsultExamineeList> GetConsultExamineesAsync(Guid placeScheduleId, int examMenuId, AggregatedProgressStatus status);
}