using Ryobi.Wellship.APIModels.Requests;
using Ryobi.Wellship.APIModels.Responses;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Usecases;

/// <summary>
/// 受診ユースケースのインターフェース
/// </summary>
public interface IConsultUsecase
{
    /// <summary>
    /// AP1007_受診番号の受診が存在するか確認する
    /// </summary>
    /// <param name="consultNumberRequest">受診番号リクエスト</param>
    /// <returns>受診が存在するか</returns>
    public Task VerifyConsultNumberAsync(ConsultNumberRequest consultNumberRequest);

    /// <summary>
    /// AP1008_未受診の検査メニューを取得する
    /// </summary>
    public Task<UnexaminedMenuList> GetUnexaminedMenusAsync(string consultNumber);

    /// <summary>
    /// AP1010_検査内容を取得する
    /// </summary>
    public Task<ExamContent> GetExamItemsExamineeAsync(string consultNumber, int examMenuId);

    /// <summary>
    /// AP1022_検査の実施有無と中止理由を登録する
    /// </summary>
    public Task RegisterExecutionsAsync(string consultNumber, ExecutionsRequest request);

    /// <summary>
    /// 前提検査メニューを検証する
    /// </summary>
    public Task<IEnumerable<Domain.Models.ExamMenu>> ValidatePriorExamMenusAsync(string consultNumber, int examMenuId);

    /// <summary>
    /// AP1009_検査結果入力情報を取得する
    /// </summary>
    public Task<InputExamItems> GetInputExamItemsExamineeAsync(string consultNumber, int examMenuId);

    /// <summary>
    /// 検査結果相関ルールで検証する
    /// </summary>
    public Task<IEnumerable<Domain.Models.RuleError>> ValidateCorrelationRuleAsync(string consultNumber, ResultsRequest result);

    /// <summary>
    /// 検査正常値を検証する
    /// </summary>
    public Task<IEnumerable<Domain.Models.RangeError>> ValidateNormalValueRangeAsync(string consultNumber, ResultsRequest result);

    /// <summary>
    /// AP1014_検査結果を登録する
    /// </summary>
    public Task RegisterResultsAsync(string consultNumber, ResultsRequest results);

    /// <summary>
    /// AP1013_検査結果を検証する
    /// </summary>
    public Task<VerifyExamItems> VerifyResults(string consultNumber, ResultsRequest results);

    /// <summary>
    /// AP1025_検査結果を取り消す
    /// </summary>
    public Task BatchDeleteResultsAsync(string consultNumber, ResultDeleteRequest resultDeleteRequest);

    /// <summary>
    /// AP1026_検査結果を取得する
    /// </summary>
    public Task<ConsultAllExamResult> GetConsultAllResult(string consultNumber);
}
