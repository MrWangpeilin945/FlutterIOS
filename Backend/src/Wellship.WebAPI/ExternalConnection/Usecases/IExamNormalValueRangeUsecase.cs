using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases
{
    /// <summary>
    /// 基準値範囲ユーズケースインターフェース
    /// </summary>
    public interface IExamNormalValueRangeUsecase
    {
        /// <summary>
        /// 基準値（範囲）を登録する
        /// </summary>
        /// <param name="examNormalValueRanges"></param>
        /// <returns></returns>
        public Task<List<ErrorObject>> StoreExamNormalValueRangeAsync(List<ExamNormalValueRange> examNormalValueRanges);
    }
}
