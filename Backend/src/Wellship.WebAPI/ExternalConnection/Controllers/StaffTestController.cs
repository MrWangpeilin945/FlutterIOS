using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

using Ryobi.Wellship.Core.Enums;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Controllers
{
    /// <summary>
    /// 職員テスト用コントローラー
    /// </summary>
    [OpenApiIgnore]
    public class StaffTestController : ControllerBase
    {
        private readonly Usecases.IStaffUsecase _staffUsecase;

        /// <summary>
        /// コントローラの生成
        /// </summary>
        /// <param name="staffUsecase"></param>
        public StaffTestController(Usecases.IStaffUsecase staffUsecase)
        {
            _staffUsecase = staffUsecase;
        }

        /// <summary>
        /// 職員登録テスト（Upsertテスト）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/staff/test1")]
        public async Task<IActionResult> StaffTestAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            //投入データ作成
            List<Staff> staffs;
            staffs = CreateTestStaffs(10000);
            stopwatch.Stop();
            var creationTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            var result = await _staffUsecase.StoreStaffsAsync(staffs);
            var processingTime = stopwatch.Elapsed.TotalSeconds;
            return Ok(new
            {
                DataCreationTime = creationTime.ToString() + "秒",
                DataProcessingTime = processingTime.ToString() + "秒",
                ErrorObject = result
            });
        }

        /// <summary>
        /// 投入データ作成
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected List<Staff> CreateTestStaffs(int num)
        {
            List<Staff> staffs = new List<Staff>();
            for (int i = 1; i < num + 1; i++)
            {
                Staff tmp_staff = new Staff { StaffCode = "Staff" + i.ToString("00000"), LoginId = "LoginId_Hoge" + i.ToString("00000"), Name = "職員 太郎" + i.ToString("00000"), Password = "P@ssw0rd" + i.ToString("00000"), Enabled = true, RoleId = (Role)10, InputNote = i.ToString() };
                staffs.Add((Staff)tmp_staff);
            }
            return staffs;
        }

    }
}
