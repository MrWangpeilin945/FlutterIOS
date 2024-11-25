using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository)
    {
        _consultRepository = consultRepository;
    }

    /// <summary>
    /// 受診番号の受診が存在するか確認する
    /// </summary>
    /// <param name="consultNumberRequest">受診番号リクエスト</param>
    /// <returns>受診が存在するか</returns>
    public async Task VerifyConsultNumberAsync(ConsultNumberRequest consultNumberRequest)
    {
        var consultNumber = consultNumberRequest.ConsultNumber;
        var consultExists = await _consultRepository.ConsultExistsAsync(consultNumber);

        if (!consultExists)
        {
            throw new ConsultNumberNotFoundException("受診番号が存在しません。");
        }
    }

    /// <summary>
    /// 未受診の検査項目を取得する
    /// </summary>
    public void GetUnexaminedItems()
    {

    }

    /// <summary>
    /// 簡易な受診者情報を取得する
    /// </summary>
    public void GetSimpleExaminee()
    {

    }

    /// <summary>
    /// 検査内容を取得する
    /// </summary>
    public void GetExamItemsExaminee()
    {

    }
}
