using System.IO.Compression;
using System.Text.RegularExpressions;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

/// <summary>
/// AWS S3接続ユーティリティ
/// </summary>
public class S3ConnectUtility
{
    private static AmazonS3Client? _s3Client;

    private static readonly RegionEndpoint region = RegionEndpoint.GetBySystemName("ap-northeast-1");

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public S3ConnectUtility()
    {
        _s3Client = CreateS3Client();
    }

    /// <summary>
    /// デストラクタ
    /// </summary>
    ~S3ConnectUtility()
    {
        if (_s3Client != null)
        {
            _s3Client.Dispose();
        }
    }

    /// <summary>
    ///  S3クライアントの作成
    /// </summary>
    /// <returns></returns>
    private static AmazonS3Client CreateS3Client()
    {
        return new AmazonS3Client(region);
    }

    /// <summary>
    /// S3からファイルを取得する
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="fileKey">オブジェクト名</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<byte[]> DownloadToMemoryAsync(string bucketName, string fileKey)
    {
        try
        {
            if (_s3Client == null)
            {
                _s3Client = CreateS3Client();
            }

            // ファイルのダウンロード
            var response = await _s3Client.GetObjectAsync(bucketName, fileKey);

            // ZipファイルのストリームをMemoryStreamにコピー
            using (var memoryStream = new MemoryStream())
            {
                await response.ResponseStream.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }
        catch (AmazonS3Exception s3Ex)
        {
            // S3関連のエラーが発生した場合
            throw new Exception("An error occurred while communicating with S3: " + s3Ex.Message, s3Ex);
        }
        catch (Exception ex)
        {
            // その他のエラーが発生した場合
            throw new Exception("An unexpected error occurred: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// S3バケット内のファイルリストを取得する関数 
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="folderPrefix">検索フォルダ</param>
    /// <param name="searchPattern">検索オプション</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<string>> ListFilesInFolderAsync(string bucketName, string folderPrefix = "", string searchPattern = "*")
    {
        var fileList = new List<string>();

        try
        {
            if (_s3Client == null)
            {
                _s3Client = CreateS3Client();
            }

            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            // フォルダプレフィックスが指定されていれば、Prefixを設定
            if (!string.IsNullOrWhiteSpace(folderPrefix))
            {
                // フォルダのプレフィックスを指定
                if (folderPrefix.EndsWith("/"))
                {
                    request.Prefix = folderPrefix;
                }
                else
                {
                    request.Prefix = folderPrefix + "/";
                }
                request.Delimiter = "/";       // 区切り文字（フォルダ内のみを取得する）
            }

            ListObjectsV2Response response;

            // ワイルドカードを正規表現に変換
            // '*' を任意の文字列に
            // '?' を任意の1文字に
            // 文字列の終わりまで一致
            string regexPattern = $"^{Regex.Escape(searchPattern).ToLower().Replace(@"\*", ".*").Replace(@"\?", ".")}$";

            var regex = new Regex(regexPattern);

            // S3バケットのファイルリストを取得
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                // アーカイブ内のすべてのエントリ（ファイル）を取得
                foreach (S3Object obj in response.S3Objects)
                {
                    // フォルダを無視し、ファイルのみを対象にする
                    if (!obj.Key.EndsWith("/"))  // フォルダを無視
                    {
                        // ファイル名にワイルドカードを適用して検索
                        string fileName = Path.GetFileName(obj.Key).ToLower();
                        if (regex.IsMatch(fileName))  // ファイル名がワイルドカードに一致する場合
                        {
                            fileList.Add(obj.Key);  // ファイルのパスをリストに追加
                        }
                    }
                }
                // 次のページがある場合
                request.ContinuationToken = response.NextContinuationToken;

            } while (response.IsTruncated); // ファイルリストが続く場合、次ページを取得

            return fileList;

        }
        catch (AmazonS3Exception s3Ex)
        {
            // S3関連のエラーが発生した場合
            throw new Exception("An error occurred while communicating with S3: " + s3Ex.Message, s3Ex);
        }
        catch (Exception ex)
        {
            // その他のエラーが発生した場合
            throw new Exception("An unexpected error occurred: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// メモリ上のデータをファイルにし、ZipにしてS3へアップロードする関数
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="s3zipFilePath">登録ファイルパス</param>
    /// <param name="fileContents">登録データ</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task UploadDataToS3AsZipAsync(string bucketName, string s3zipFilePath, Dictionary<string, byte[]> fileContents)
    {
        try
        {
            if (_s3Client == null)
            {
                _s3Client = CreateS3Client();
            }

            // メモリストリームにZipファイルを作成
            using (var zipMemoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var file in fileContents)
                    {
                        // サブディレクトリ付きのファイルパスを指定してエントリを作成
                        var entry = archive.CreateEntry(file.Key);
                        using (var entryStream = entry.Open())
                        using (var writer = new BinaryWriter(entryStream))
                        {
                            writer.Write(file.Value); // メモリ上のデータをバイト配列として書き込む
                        }
                    }
                }

                // Zipファイルのポインタを先頭に戻す
                zipMemoryStream.Position = 0;

                // S3にアップロード
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = $"{s3zipFilePath}", // フォルダ内にアップロードする場合
                    InputStream = zipMemoryStream,
                    ContentType = "application/zip" // Zipファイルなので
                };

                var uploadResponse = await _s3Client.PutObjectAsync(uploadRequest);

                // アップロード後のレスポンスをログに記録するなど、必要に応じて確認可能
                Console.WriteLine($"Upload successful. Status code: {uploadResponse.HttpStatusCode}");
            }
        }
        catch (AmazonS3Exception s3Ex)
        {
            // S3関連のエラーが発生した場合
            throw new Exception("An error occurred while communicating with S3: " + s3Ex.Message, s3Ex);
        }
        catch (Exception ex)
        {
            // その他のエラーが発生した場合
            throw new Exception("An unexpected error occurred: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// S3上でファイルを別のフォルダに移動するメソッド
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="sourceKey">対象ファイル</param>
    /// <param name="destinationFolder">移動先フォルダ</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task MoveFileAsync(string bucketName, string sourceKey, string destinationFolder)
    {
        try
        {
            if (_s3Client == null)
            {
                _s3Client = CreateS3Client();
            }

            // 移動先のフォルダが存在しない場合、空のオブジェクトを作成してフォルダを作成
            var destinationKey = $"{destinationFolder}/{sourceKey.Split('/')[^1]}"; // 移動先ファイル名を作成
            var folderKey = $"{destinationFolder.TrimEnd('/')}/"; // フォルダを表すキー

            // フォルダ（空のオブジェクト）を作成
            await CreateEmptyFolderIfNotExistsAsync(bucketName, folderKey);

            // ファイルを新しい場所にコピー
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                SourceKey = sourceKey,
                DestinationBucket = bucketName,
                DestinationKey = destinationKey
            };

            var copyResponse = await _s3Client.CopyObjectAsync(copyRequest);

            // 元のファイルを削除
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = sourceKey
            };

            var deleteResponse = await _s3Client.DeleteObjectAsync(deleteRequest);
        }
        catch (AmazonS3Exception s3Ex)
        {
            // S3関連のエラーが発生した場合
            throw new Exception("An error occurred while communicating with S3: " + s3Ex.Message, s3Ex);
        }
        catch (Exception ex)
        {
            // その他のエラーが発生した場合
            throw new Exception("An unexpected error occurred: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// フォルダが存在しない場合、空のオブジェクトを作成してフォルダを作成するメソッド
    /// </summary>
    /// <param name="bucketName">バケット名</param>
    /// <param name="folderKey">フォルダ名</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task CreateEmptyFolderIfNotExistsAsync(string bucketName, string folderKey)
    {
        try
        {
            if (_s3Client == null)
            {
                _s3Client = CreateS3Client();
            }

            // オブジェクトが存在しない場合、空のオブジェクトを作成（フォルダを作成）
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = folderKey,
                Delimiter = "/"
            };

            var listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest);

            if (listObjectsResponse.S3Objects.Count == 0)
            {
                // フォルダが存在しない場合、空のファイルを作成してフォルダを作成
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = folderKey, // 空のオブジェクトでフォルダを作成
                    ContentBody = string.Empty
                };
                await _s3Client.PutObjectAsync(putRequest);
            }
        }
        catch (AmazonS3Exception s3Ex)
        {
            // S3関連のエラーが発生した場合
            throw new Exception("An error occurred while communicating with S3: " + s3Ex.Message, s3Ex);
        }
        catch (Exception ex)
        {
            // その他のエラーが発生した場合
            throw new Exception("An unexpected error occurred: " + ex.Message, ex);
        }
    }
}