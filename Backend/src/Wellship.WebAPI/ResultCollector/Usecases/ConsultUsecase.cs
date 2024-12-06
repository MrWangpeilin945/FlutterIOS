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
    private readonly IExamMenuRepository _examMenuRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="consultRepository">受診リポジトリ</param>
    /// <param name="examineeRepository">受診者リポジトリ</param>
    /// <param name="examMenuRepository">検査メニューリポジトリ</param>
    public ConsultUsecase(IConsultRepository consultRepository, IExamineeRepository examineeRepository, IExamMenuRepository examMenuRepository)
    {
        _consultRepository = consultRepository;
        _examineeRepository = examineeRepository;
        _examMenuRepository = examMenuRepository;
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
    /// 未受診の検査メニューを取得する
    /// </summary>
    public async Task<UnexaminedMenuList> GetUnexaminedMenusAsync(string consultNumber)
    {
        // 受診単位に紐づく未受診の検査項目を取得する
        // 検査項目明細単位の依頼に対して、検査結果あるいは検査中止のレコードが存在すれば受診済みとする
        // 未受診の検査項目明細が1つ以上存在する検査メニューを「未受診の検査メニュー」として返す

        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examinee = await _examineeRepository.GetExamineeAsync(consult.ExamineeId);

        // 未受診の検査メニューを取得する
        var unexaminedConsults = await _consultRepository.GetUnexaminedConsultsAsync([consult.ConsultNumber]);

        // 未受診の検査メニューがない受診の場合は、空リストを返す
        if (!unexaminedConsults.Any())
        {
            return new UnexaminedMenuList()
            {
                ConsultId = consult.ConsultId,
                ExamineeId = examinee.ExamineeId,
                ExamineeName = examinee.Name,
                UnexaminedMenus = []
            };
        }

        // 未受診項目がある受診の場合は、検査メニュー単位の未受診リストを返す
        var unexaminedConsult = unexaminedConsults.Single();
        return new UnexaminedMenuList()
        {
            ConsultId = unexaminedConsult.ConsultId,
            ExamineeId = examinee.ExamineeId,
            ExamineeName = examinee.Name,
            UnexaminedMenus = unexaminedConsult.UnexaminedExamMenus.Select(x => new ExamMenu()
            {
                ExamMenuId = x.ExamMenuId,
                ExamMenuName = x.ExamMenuName
            }).ToArray()
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

    /// <summary>
    /// 検査の実施有無と中止理由を登録する
    /// </summary>
    public async Task RegisterExecutionsAsync(string consultNumber, ExecutionsRequest request)
    {
        // NOTE: 中止理由の登録ルール
        // 前提：リクエストは検査項目単位、DBは検査項目明細単位
        // 
        // [1] isPerforming（検査実施する）：true & 中止理由：null
        //   - a. 中止レコードがある => 中止レコードを削除する
        //   - b. 中止レコードがない => 処理しない
        //
        // [2] isPerforming（検査実施する）：false & 中止理由：あり
        //   - a. 中止レコードがある => 中止レコードを更新する
        //   - b. 中止レコードがない => 中止レコードを挿入する

        var consult = await _consultRepository.GetConsultAsync(consultNumber);
        var examCancels = await _consultRepository.GetExamCancelsAsync(consult.ConsultId);

        // [1] isPerforming（検査実施する）：true & 中止理由：null
        var performingList = request.Executions.Where(x => x.IsPerforming && x.CancelReasonId is null).ToArray();

        // [2] isPerforming（検査実施する）：false & 中止理由：あり
        var notPerformingList = request.Executions.Where(x => !x.IsPerforming && x.CancelReasonId is not null).ToArray();

        // [1]-a 削除対象
        // 保存済みの中止レコードに対して検査項目IDで突合して、削除対象の検査項目明細IDを取得する
        var removeTargets = performingList.SelectMany(req => examCancels.ExamItemDetailCancels
                                                                .Where(x => x.ExamItemId == req.ExamItemId)
                                                                .Select(x => x.ExamItemDetailId)
                                                     ).ToArray();

        // [2]-a,b
        // UPSERTはリポジトリに任せる
        var toSave = notPerformingList.Select(x => new Domain.Models.ExamItemCancel()
        {
            ExamItemId = x.ExamItemId,
            CancelReasonId = (int)x.CancelReasonId!,
        }).ToArray();

        await _consultRepository.RemoveExamCancelsAsync(consult.ConsultId, removeTargets);
        await _consultRepository.SaveExamCancelsAsync(consult.ConsultId, toSave);
    }

    /// <summary>
    /// 前提検査メニューを検証する
    /// </summary>
    public async Task<IEnumerable<Domain.Models.ExamMenu>> ValidatePriorExamMenus(string consultNumber, int examMenuId)
    {
        // 未受診の検査メニューを取得する
        var unexamined = await _consultRepository.GetUnexaminedConsultAsync(consultNumber);
        var unexaminedMenuIds = unexamined.UnexaminedExamMenus.Select(x => x.ExamMenuId).ToArray();

        // 前提検査メニューの設定を取得する
        var priorExamMenus = await _examMenuRepository.GetPriorExamMenusAsync();
        var priorExamMenu = priorExamMenus.SingleOrDefault(x => x.CurrentExamMenuId == examMenuId);

        // 現在の検査メニューについて、前提検査メニューの設定がない場合はエラーなし
        if (priorExamMenu is null)
        {
            return [];
        }

        // 前提検査が必要な検査メニューを返す
        var missingPriorMenus = priorExamMenu.GetMissingPriorMenus(unexaminedMenuIds);
        return unexamined.UnexaminedExamMenus.Where(x => missingPriorMenus.Contains(x.ExamMenuId))
                                             .Select(x => new Domain.Models.ExamMenu() { MenuId = x.ExamMenuId, MenuName = x.ExamMenuName })
                                             .ToArray();
    }
}
