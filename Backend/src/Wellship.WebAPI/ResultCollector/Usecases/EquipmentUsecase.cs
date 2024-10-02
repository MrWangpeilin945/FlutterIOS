using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 機器ユースケース
/// </summary>
public class EquipmentUsecase : IEquipmentUsecase
{
    private readonly IEquipmentRepository _equipmentRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="equipmentRepository">機器リポジトリ</param>
    public EquipmentUsecase(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    /// <summary>
    /// 機器連携設定を取得する
    /// </summary>
    public void GetEquipmentSettings()
    {

    }
}
