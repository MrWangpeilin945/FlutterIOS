using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Controllers.V1
{
    /// <summary>
    /// 動作確認用コントローラ
    /// </summary>
    [OpenApiIgnore]
    public class PlaceTestController : ControllerBase
    {
        private readonly IPlaceUsecase _administratorUsecase;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="placeUsecaseUsecase">会場ユースケース</param>
        public PlaceTestController(IPlaceUsecase placeUsecaseUsecase)
        {
            _administratorUsecase = placeUsecaseUsecase;
        }

        /// <summary>
        /// 会場登録テスト
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Place))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/place/profile")]
        public async Task<IActionResult> TestPlaceAsync()
        {
            // 投入データ作成
            List<Place> places;
            places = CreateTestPlaces(10000);
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StorePlacesAsync(places);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }

        /// <summary>
        /// 会場登録テスト（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Place))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("api/v{version:apiVersion}/place_upsert/profile")]
        public async Task<IActionResult> TestPlaceUpsertAsync()
        {
            // 投入データ作成
            List<Place> places;
            places = CreateUpsertTestPlaces();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = await _administratorUsecase.StorePlacesAsync(places);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"　{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");
            return Ok(result);
        }

        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected List<Place> CreateTestPlaces(int num)
        {
            List<Place> places = new List<Place>();
            for (int i = 1; i < num + 1; i++)
            {
                Place tmp_place = new Place { Code = "Place" + i.ToString("00000"), Name = "会場" + i.ToString("00000"), InputNote = i.ToString() };
                places.Add((Place)tmp_place);
            }
            return places;
        }

        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <returns></returns>
        protected List<Place> CreateUpsertTestPlaces()
        {
            List<Place> places = new List<Place>();
            places.Add(new Place { Code = "Place00010", Name = "会場upsert00010", InputNote = 999.ToString() });
            return places;
        }

    }
}
