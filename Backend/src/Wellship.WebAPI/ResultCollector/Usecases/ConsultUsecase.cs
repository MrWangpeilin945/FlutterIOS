using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;
using Ryobi.Wellship.Core.Exceptions;
using Ryobi.Wellship.WebAPI.ResultCollector.Domain.Repositories;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケース
/// </summary>
public class ConsultUsecase : IConsultUsecase
{
    private readonly IConsultRepository _consultRepository;
    private readonly IExamineeRepository _examineeRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
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
    public async Task<UnexaminedItemList> GetUnexaminedItemsAsync(string consultNumber)
    {
        // 受診リポジトリから受診を取得する
        // 受診.受診者IDをもとに氏名などを取る
        var dummyId = 3;

        var examinee = await _examineeRepository.GetExamineeAsync(dummyId);

        return new UnexaminedItemList()
        {
            ConsultId = 1,
            ExamineeId = examinee.ExamineeId,
            ExamineeName = examinee.Name,
            UnexaminedItems = [
                new(){ExamItemId = 1,ExamItemName = ""}
            ]
        };
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
