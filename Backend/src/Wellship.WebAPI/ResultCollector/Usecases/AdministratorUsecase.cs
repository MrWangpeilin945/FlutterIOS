using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 管理者ユースケース
/// </summary>
public class AdministratorUsecase : IAdministratorUsecase
{
    private readonly IAdministratorRepository _administratorRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="administratorRepository">管理者リポジトリ</param>
    public AdministratorUsecase(IAdministratorRepository administratorRepository)
    {
        _administratorRepository = administratorRepository;
    }

    /// <summary>
    /// 管理者の情報を取得する
    /// </summary>
    public void GetAdministrator()
    {
        
    }
}
