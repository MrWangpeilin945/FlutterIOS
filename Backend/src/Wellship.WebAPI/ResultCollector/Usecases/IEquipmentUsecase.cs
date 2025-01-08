using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 機器ユースケースのインターフェース
/// </summary>
public interface IEquipmentUsecase
{
    /// <summary>
    /// AP1012_機器連携設定を取得する
    /// </summary>
    public Task<EquipmentList> GetEquipmentsAsync(int examMenuId);
}