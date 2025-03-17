using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;
using Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.Default;
using Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases.KitashinagawaClinic;

/// <summary>
/// 北品川クリニック様向け　データ取り込み処理
/// </summary>
public class DataImportUsecase : IDataImportUsecase
{
    private readonly ITeamUsecase _teamUsecase;
    private readonly IPlaceUsecase _placeUsecase;
    private readonly IPlaceScheduleUsecase _placeScheduleUsecase;

    private readonly IStaffUsecase _staffUsecase;

    private readonly IOrganizationUsecase _organizationUsecase;

    private readonly IThresholdUsecase _thresholdUsecase;
    private readonly IExamNormalValueRangeUsecase _examNormalValueRangeUsecase;

    private readonly IExamineeUsecase _examineeUsecase;
    private readonly IConsultUsecase _consultUsecase;
    private readonly ITicketUsecase _ticketUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="teamUsecase">EC2006_班を登録する</param>
    /// <param name="placeUsecase">EC2007_会場を登録する</param>
    /// <param name="placeScheduleUsecase">EC2012_会場日程を登録する</param>
    /// <param name="staffUsecase">EC2011_職員を登録する</param>
    /// <param name="organizationUsecase">EC2008_団体を登録する</param>
    /// <param name="thresholdUsecase">EC2014_基準パターンを登録する</param>
    /// <param name="examNormalValueRangeUsecase">EC2009_基準値(範囲)を登録する</param>
    /// <param name="examineeUsecase">EC2001_受診者を登録する</param>
    /// <param name="consultUsecase">EC2004_受診を更新する</param>
    /// <param name="ticketUsecase">EC2002_受付を更新する</param>
    public DataImportUsecase(ITeamUsecase teamUsecase, IPlaceUsecase placeUsecase, IPlaceScheduleUsecase placeScheduleUsecase, IStaffUsecase staffUsecase,
                             IOrganizationUsecase organizationUsecase, IThresholdUsecase thresholdUsecase, IExamNormalValueRangeUsecase examNormalValueRangeUsecase,
                             IExamineeUsecase examineeUsecase, IConsultUsecase consultUsecase, ITicketUsecase ticketUsecase)
    {
        _teamUsecase = teamUsecase;
        _placeUsecase = placeUsecase;
        _placeScheduleUsecase = placeScheduleUsecase;
        _staffUsecase = staffUsecase;
        _organizationUsecase = organizationUsecase;
        _thresholdUsecase = thresholdUsecase;
        _examNormalValueRangeUsecase = examNormalValueRangeUsecase;
        _examineeUsecase = examineeUsecase;
        _consultUsecase = consultUsecase;
        _ticketUsecase = ticketUsecase;
    }

    /// <summary>
    /// EC1001_ファイル取り込みを実行する_随時(北品川)
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="objectKey">オブジェクト名</param>
    /// <returns></returns>
    public async Task<Boolean> StoreConstantlyDataAsync(string bucketName, string objectKey)
    {
        var s3ZipFileLister = new S3ConnectUtility();

        var errorObjects = new List<ErrorObject>();

        // Dummy 開始時刻
        errorObjects.Add(new ErrorObject
        {
            Code = "00000",
            Message = "開始日時",
            InputNote = DateTime.Now.ToString("yyyyMMdd HHmmss"),
        }
    );

        try
        {
            var getFileData = await s3ZipFileLister.DownloadToMemoryAsync(bucketName, objectKey);

            var ec2001s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2001*.json");
            if (ec2001s.Any())
            {
                foreach (var ec2001 in ec2001s)
                {
                    using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2001))
                    {
                        var examinees = JsonSerializer.DeserializeAsync<List<Examinee>>(fileStream).Result;

                        if (examinees != null && examinees.Any())
                        {
                            errorObjects.AddRange(await _examineeUsecase.StoreExamineesAsync(examinees));
                        }
                    }
                }
            }

            var ec2004s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2004*.json");
            if (ec2004s.Any())
            {
                foreach (var ec2004 in ec2004s)
                {
                    using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2004))
                    {
                        var consults = JsonSerializer.DeserializeAsync<List<Consult>>(fileStream).Result;

                        if (consults != null && consults.Any())
                        {
                            errorObjects.AddRange(await _consultUsecase.StoreConsultAsync(consults));
                        }
                    }
                }
            }

            var ec2002s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2002*.json");
            if (ec2002s.Any())
            {
                foreach (var ec2002 in ec2002s)
                {
                    using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2002))
                    {
                        var tickets = JsonSerializer.DeserializeAsync<List<Ticket>>(fileStream).Result;

                        if (tickets != null && tickets.Any())
                        {
                            errorObjects.AddRange(await _ticketUsecase.StoreTicketsAsync(tickets));
                        }
                    }
                }
            }

            // ファイルを移動
            await s3ZipFileLister.MoveFileAsync(bucketName, objectKey, $"Backup/ConstantlyData/{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}");
        }
        catch (Exception ex)
        {
            // Todo 例外を握りつぶす(Jsonの型エラーとかが飛んでくる可能性があるため)
            errorObjects.Add(new ErrorObject
            {
                Code = "99999",
                Message = ex.Message,
                InputNote = ex.GetType().Name,
            }
                );
        }

        // Dummy 終了時刻
        errorObjects.Add(new ErrorObject
        {
            Code = "00000",
            Message = "終了日時",
            InputNote = DateTime.Now.ToString("yyyyMMdd HHmmss"),
        }
    );

        if (errorObjects.Any())
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            var fileContents = new Dictionary<string, byte[]>
            {
                { $"ErrorRec{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json", JsonSerializer.SerializeToUtf8Bytes(errorObjects,options) }
            };

            var uploadFile = $"FromWELLSHIP/StoreDailyData/{DateTime.Now.ToString("yyyyMMdd")}/ErrorRec{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip";
            await s3ZipFileLister.UploadDataToS3AsZipAsync(bucketName, uploadFile, fileContents);
        }

        return true;
    }

    /// <summary>
    /// EC1002_ファイル取り込みを実行する_日次(北品川)
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <returns></returns>
    public async Task<Boolean> StoreDailyDataAsync(string bucketName)
    {
        var s3ZipFileLister = new S3ConnectUtility();

        var s3SharchFiles = await s3ZipFileLister.ListFilesInFolderAsync(bucketName, "ToWELLSHIP/", "ToWELLSHIP*.Zip");

        var errorObjects = new List<ErrorObject>();

        // Dummy 開始時刻
        errorObjects.Add(new ErrorObject
        {
            Code = "00000",
            Message = "開始日時",
            InputNote = DateTime.Now.ToString("yyyyMMdd HHmmss"),
        }
    );

        try
        {
            foreach (var zipFile in s3SharchFiles)
            {
                var getFileData = await s3ZipFileLister.DownloadToMemoryAsync(bucketName, zipFile);

                var ec2006s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2006*.json");
                if (ec2006s.Any())
                {
                    foreach (var ec2006 in ec2006s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2006))
                        {
                            var teams = JsonSerializer.DeserializeAsync<List<Team>>(fileStream).Result;

                            if (teams != null && teams.Any())
                            {
                                errorObjects.AddRange(await _teamUsecase.StoreTeamsAsync(teams));
                            }
                        }
                    }
                }

                var ec2007s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2007*.json");
                if (ec2007s.Any())
                {
                    foreach (var ec2007 in ec2007s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2007))
                        {
                            var places = JsonSerializer.DeserializeAsync<List<Place>>(fileStream).Result;

                            if (places != null && places.Any())
                            {
                                errorObjects.AddRange(await _placeUsecase.StorePlacesAsync(places));
                            }
                        }
                    }
                }

                var ec2012s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2012*.json");
                if (ec2012s.Any())
                {
                    foreach (var ec2012 in ec2012s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2012))
                        {
                            var placeSchedules = JsonSerializer.DeserializeAsync<List<PlaceSchedule>>(fileStream).Result;

                            if (placeSchedules != null && placeSchedules.Any())
                            {
                                errorObjects.AddRange(await _placeScheduleUsecase.StorePlaceSchedulesAsync(placeSchedules));
                            }
                        }
                    }
                }

                var ec2011s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2011*.json");
                if (ec2011s.Any())
                {
                    foreach (var ec2011 in ec2011s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2011))
                        {
                            var staffs = JsonSerializer.DeserializeAsync<List<Staff>>(fileStream).Result;

                            if (staffs != null && staffs.Any())
                            {
                                errorObjects.AddRange(await _staffUsecase.StoreStaffsAsync(staffs));
                            }
                        }
                    }
                }

                var ec2008s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2008*.json");
                if (ec2008s.Any())
                {
                    foreach (var ec2008 in ec2008s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2008))
                        {
                            var organizations = JsonSerializer.DeserializeAsync<List<Organization>>(fileStream).Result;

                            if (organizations != null && organizations.Any())
                            {
                                errorObjects.AddRange(await _organizationUsecase.StoreOrganizationsAsync(organizations));
                            }
                        }
                    }
                }

                var ec2014s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2014*.json");
                if (ec2014s.Any())
                {
                    foreach (var ec2014 in ec2014s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2014))
                        {
                            var thresholds = JsonSerializer.DeserializeAsync<List<Threshold>>(fileStream).Result;

                            if (thresholds != null && thresholds.Any())
                            {
                                errorObjects.AddRange(await _thresholdUsecase.StoreThresholdsAsync(thresholds));
                            }
                        }
                    }
                }

                var ec2009s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2009*.json");
                if (ec2009s.Any())
                {
                    foreach (var ec2009 in ec2009s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2009))
                        {
                            var examNormalValueRanges = JsonSerializer.DeserializeAsync<List<ExamNormalValueRange>>(fileStream).Result;

                            if (examNormalValueRanges != null && examNormalValueRanges.Any())
                            {
                                errorObjects.AddRange(await _examNormalValueRangeUsecase.StoreExamNormalValueRangeAsync(examNormalValueRanges));
                            }
                        }
                    }
                }

                var ec2001s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2001*.json");
                if (ec2001s.Any())
                {
                    foreach (var ec2001 in ec2001s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2001))
                        {
                            var examinees = JsonSerializer.DeserializeAsync<List<Examinee>>(fileStream).Result;

                            if (examinees != null && examinees.Any())
                            {
                                errorObjects.AddRange(await _examineeUsecase.StoreExamineesAsync(examinees));
                            }
                        }
                    }
                }

                var ec2004s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2004*.json");
                if (ec2004s.Any())
                {
                    foreach (var ec2004 in ec2004s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2004))
                        {
                            var consults = JsonSerializer.DeserializeAsync<List<Consult>>(fileStream).Result;

                            if (consults != null && consults.Any())
                            {
                                errorObjects.AddRange(await _consultUsecase.StoreConsultAsync(consults));
                            }
                        }
                    }
                }

                var ec2002s = ZipFileProcessor.SearchFilesInZipStream(getFileData, "ec2002*.json");
                if (ec2002s.Any())
                {
                    foreach (var ec2002 in ec2002s)
                    {
                        using (var fileStream = ZipFileProcessor.ExtractFileFromZipStream(getFileData, ec2002))
                        {
                            var tickets = JsonSerializer.DeserializeAsync<List<Ticket>>(fileStream).Result;

                            if (tickets != null && tickets.Any())
                            {
                                errorObjects.AddRange(await _ticketUsecase.StoreTicketsAsync(tickets));
                            }
                        }
                    }
                }

                // ファイルを移動
                await s3ZipFileLister.MoveFileAsync(bucketName, zipFile, $"Backup/StoreDailyData/{DateTime.Now.ToString("yyyyMMdd")}");
            }
        }
        catch (Exception ex)
        {
            // Todo 例外を握りつぶす(Jsonの型エラーとかが飛んでくる可能性があるため)
            errorObjects.Add(new ErrorObject
            {
                Code = "99999",
                Message = ex.Message,
                InputNote = ex.GetType().Name,
            }
                );
        }

        // Dummy 終了時刻
        errorObjects.Add(new ErrorObject
        {
            Code = "00000",
            Message = "終了日時",
            InputNote = DateTime.Now.ToString("yyyyMMdd HHmmss"),
        }
    );

        if (errorObjects.Any())
        {

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), 
                WriteIndented = true 
            };

            var fileContents = new Dictionary<string, byte[]>
            {
                { $"ErrorRec{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json", JsonSerializer.SerializeToUtf8Bytes(errorObjects,options) }
            };

            var uploadFile = $"FromWELLSHIP/StoreDailyData/{DateTime.Now.ToString("yyyyMMdd")}/ErrorRec{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip";
            await s3ZipFileLister.UploadDataToS3AsZipAsync(bucketName, uploadFile, fileContents);
        }

        return true;
    }
}
