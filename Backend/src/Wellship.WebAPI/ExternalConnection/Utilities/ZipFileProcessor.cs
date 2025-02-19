using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

/// <summary>
/// Zipファイル操作ユーティリティ
/// </summary>
public class ZipFileProcessor
{
    /// <summary>
    /// Zipアーカイブ内の指定したファイルを取得する
    /// </summary>
    /// <param name="fileBytes">Zipアーカイブ</param>
    /// <param name="fileName">取得ファイル</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static Stream ExtractFileFromZipStream(byte[] fileBytes, string fileName)
    {
        if (fileBytes == null || fileBytes.Length == 0)
        {
            throw new ArgumentException("The file bytes cannot be null or empty.", nameof(fileBytes));
        }

        // byte[] を MemoryStream に変換
        // Zipアーカイブを開く
        var memoryStream = new MemoryStream(fileBytes);
        // Zipアーカイブを開く
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, leaveOpen: true))
        {
            // アーカイブ内のファイルを検索
            var entry = archive.GetEntry(fileName);
            if (entry != null)
            {
                return entry.Open(); // entry.Open() は閉じられないように注意
            }
            else
            {
                throw new FileNotFoundException($"The file '{fileName}' was not found in the zip archive.");
            }
        }
    }
    /// <summary>
    /// Zipアーカイブ内のファイル一覧を取得する
    /// </summary>
    /// <param name="fileBytes">Zipアーカイブ</param>
    /// <param name="searchPattern">検索オプション</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<string> SearchFilesInZipStream(byte[] fileBytes, string searchPattern = "*")
    {
        if (fileBytes == null || fileBytes.Length == 0)
        {
            throw new ArgumentException("The file bytes cannot be null or empty.", nameof(fileBytes));
        }

        var fileList = new List<string>();
        // ワイルドカードを正規表現に変換
        // '*' を任意の文字列に
        // '?' を任意の1文字に
        // 文字列の終わりまで一致
        string regexPattern = $"^{Regex.Escape(searchPattern).ToLower().Replace(@"\*", ".*").Replace(@"\?", ".")}$";

        var regex = new Regex(regexPattern);

        // byte[] を MemoryStream に変換
        // Zipアーカイブを開く
        using (var memoryStream = new MemoryStream(fileBytes))
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
        {
            // アーカイブ内のすべてのエントリ（ファイル）を取得
            foreach (var entry in archive.Entries)
            {
                // フォルダを無視し、ファイルのみを対象にする
                if (!entry.FullName.EndsWith("/"))  // フォルダを無視
                {
                    // ファイル名にワイルドカードを適用して検索
                    string fileName = Path.GetFileName(entry.FullName).ToLower();
                    if (regex.IsMatch(fileName))  // ファイル名がワイルドカードに一致する場合
                    {
                        fileList.Add(entry.FullName);  // ファイルのパスをリストに追加
                    }
                }
            }
        }

        return fileList;
    }
}
