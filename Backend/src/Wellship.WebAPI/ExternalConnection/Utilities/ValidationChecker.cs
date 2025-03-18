using System.Text.RegularExpressions;
using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Utilities;

/// <summary>
/// 妥当性チェッカー
///     ・必須項目の空値のチェック
///     ・キー重複チェック
///     ・文字数のチェック
///     ・文字形式のチェック
/// </summary>
public static class ValidationChecker
{
    /// <summary>
    /// 必須項目の空値のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> SpaceCheckProperties(IEnumerable<object> requestCollection, string[] properties, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            // プロパティ単位にチェックする
            foreach (var propertyName in properties)
            {
                var propertyValue = request.GetType().GetProperty(propertyName)?.GetValue(request)?.ToString();
                if (string.IsNullOrWhiteSpace(propertyValue))
                {
                    var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                    warningList.Add(request);
                    errorObjects.Add(new ErrorObject
                    {
                        Code = "10004",
                        Message = $"必須項目が不足しています。{propertyName}",
                        InputNote = inputNote ?? ""
                    });
                }
            }
        }
        return warningList;
    }

    /// <summary>
    /// 子プロパティの必須項目の空値のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧（親プロパティ名, 子プロパティ名）</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> SpaceCheckChildrenProperties(IEnumerable<object> requestCollection, List<(string ParentProperty, string ChildrenProperty)> properties, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            // プロパティ単位にチェックする
            foreach (var (parentProperty, childrenProperty) in properties)
            {
                var parentObj = request.GetType().GetProperty(parentProperty)?.GetValue(request);
                if (parentObj != null)
                {
                    if (parentObj is IEnumerable<object> parentEnumerable)
                    {
                        foreach (var item in parentEnumerable)
                        {
                            var propertyValue = item.GetType().GetProperty(childrenProperty)?.GetValue(item)?.ToString();
                            if (string.IsNullOrWhiteSpace(propertyValue))
                            {
                                var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                                warningList.Add(request);
                                errorObjects.Add(new ErrorObject
                                {
                                    Code = "10004",
                                    Message = $"必須項目が不足しています。{parentProperty}.{childrenProperty}",
                                    InputNote = inputNote ?? ""
                                });
                            }
                        }
                    }
                }
            }
        }
        return warningList;
    }

    /// <summary>
    /// キー重複チェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> DuplicateCheckProperties(IEnumerable<object> requestCollection, string[] properties, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var propertyName in properties)
        {
            // 重複している値を取得する
            var groupedWarnings =
                    requestCollection
                            .Select(request => new
                            {
                                PropertyValue = request.GetType().GetProperty(propertyName)?.GetValue(request)?.ToString(),
                                Warning = request
                            })
                            .GroupBy(x => x.PropertyValue)
                            .Where(g => g.Count() > 1);
            // 値が重複しているオブジェクトを抽出する
            foreach (var group in groupedWarnings)
            {
                foreach (var item in group)
                {
                    var warning = item.Warning;
                    var inputNote = warning.GetType().GetProperty("InputNote")?.GetValue(warning)?.ToString();
                    warningList.Add(warning);
                    errorObjects.Add(new ErrorObject
                    {
                        Code = "10003",
                        Message = $"キー項目が重複しています。{propertyName}: {item.PropertyValue}",
                        InputNote = inputNote ?? ""
                    });
                }
            }
        }
        return warningList;
    }

    /// <summary>
    /// 子プロパティのキー重複チェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧（親プロパティ名, 子プロパティ名）</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> DuplicateCheckChildrenProperties(IEnumerable<object> requestCollection, List<(string ParentProperty, string ChildrenProperty)> properties, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            foreach (var (parentProperty, childrenProperty) in properties)
            {
                var parentObj = request.GetType().GetProperty(parentProperty)?.GetValue(request);
                if (parentObj != null)
                {
                    if (parentObj is IEnumerable<object> parentEnumerable)
                    {
                        var values = parentEnumerable.Cast<object>()
                            .Select(item => item.GetType().GetProperty(childrenProperty)?.GetValue(item)?.ToString())
                            .ToList();
                        // 重複があるかチェック
                        var duplicates = values.GroupBy(v => v)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => g.Key)
                                            .ToList();

                        if (duplicates.Any())
                        {
                            warningList.Add(request);
                            foreach (var duplicate in duplicates)
                            {
                                var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                                errorObjects.Add(new ErrorObject
                                {
                                    Code = "10003",
                                    Message = $"キー項目が重複しています。{parentProperty}.{childrenProperty}: {duplicate}",
                                    InputNote = inputNote ?? ""
                                });
                            }
                        }
                    }
                }
            }
        }
        return warningList;
    }

    /// <summary>
    /// 文字数のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧</param>
    /// <param name="maxLengths">最大文字長一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> StringLengthCheckProperties(IEnumerable<object> requestCollection, string[] properties, int[] maxLengths, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            int index = 0; 
            // プロパティ単位にチェックする
            foreach (var propertyName in properties)
            {
                var propertyValue = request.GetType().GetProperty(propertyName)?.GetValue(request)?.ToString();
                if (propertyValue?.Length > maxLengths[index])
                {
                    var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                    warningList.Add(request);
                    errorObjects.Add(new ErrorObject
                    {
                        Code = "10005",
                        Message = $"制限数を超えています。{propertyName}:{propertyValue}",
                        InputNote = inputNote ?? ""
                    });
                }
                index++;
            }
        }
        return warningList;
    }

    /// <summary>
    /// 子プロパティの文字数のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧（親プロパティ名, 子プロパティ名）</param>
    /// <param name="maxLengths">最大文字長一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> StringLengthChildrenProperties(IEnumerable<object> requestCollection, List<(string ParentProperty, string ChildrenProperty)> properties, int[] maxLengths, List<ErrorObject> errorObjects)
    {

        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            int index = 0; 
            // プロパティ単位にチェックする
            foreach (var (parentProperty, childrenProperty) in properties)
            {
                var parentObj = request.GetType().GetProperty(parentProperty)?.GetValue(request);
                if (parentObj != null)
                {
                    if (parentObj is IEnumerable<object> parentEnumerable)
                    {
                        foreach (var item in parentEnumerable)
                        {
                            var propertyValue = item.GetType().GetProperty(childrenProperty)?.GetValue(item)?.ToString();
                            if (propertyValue?.Length > maxLengths[index])
                            {
                                var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                                warningList.Add(request);
                                errorObjects.Add(new ErrorObject
                                {
                                    Code = "10005",
                                    Message = $"制限数を超えています。{parentProperty}.{childrenProperty}:{propertyValue}",
                                    InputNote = inputNote ?? ""
                                });
                            }
                        }
                    }
                }
                index++;
            }
        }
        return warningList;
    }

    /// <summary>
    /// 文字形式のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧</param>
    /// <param name="patterns">文字パターン一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> StringPatternCheckProperties(IEnumerable<object> requestCollection, string[] properties, string[] patterns, List<ErrorObject> errorObjects)
    {
        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            int index = 0; 
            // プロパティ単位にチェックする
            foreach (var propertyName in properties)
            {
                var propertyValue = request.GetType().GetProperty(propertyName)?.GetValue(request)?.ToString();
                if (!string.IsNullOrEmpty(propertyValue) && !Regex.IsMatch(propertyValue, patterns[index]))
                {
                    var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                    warningList.Add(request);
                    errorObjects.Add(new ErrorObject
                    {
                        Code = "10006",
                        Message = $"値の形式が無効です。{propertyName}:{propertyValue}",
                        InputNote = inputNote ?? ""
                    });
                }
                index++;
            }
        }
        return warningList;
    }

    /// <summary>
    /// 子プロパティの文字形式のチェック
    /// </summary>
    /// <param name="requestCollection">リクエストクラスのコレクション</param>
    /// <param name="properties">チェック対象のプロパティ名一覧（親プロパティ名, 子プロパティ名）</param>
    /// <param name="patterns">文字パターン一覧</param>
    /// <param name="errorObjects">エラーオブジェクト</param>
    public static List<object> StringPatternChildrenProperties(IEnumerable<object> requestCollection, List<(string ParentProperty, string ChildrenProperty)> properties, string[] patterns, List<ErrorObject> errorObjects)
    {

        var warningList = new List<object>();

        foreach (var request in requestCollection)
        {
            int index = 0; 
            // プロパティ単位にチェックする
            foreach (var (parentProperty, childrenProperty) in properties)
            {
                var parentObj = request.GetType().GetProperty(parentProperty)?.GetValue(request);
                if (parentObj != null)
                {
                    if (parentObj is IEnumerable<object> parentEnumerable)
                    {
                        foreach (var item in parentEnumerable)
                        {
                            var propertyValue = item.GetType().GetProperty(childrenProperty)?.GetValue(item)?.ToString();
                            if (!string.IsNullOrEmpty(propertyValue) && !Regex.IsMatch(propertyValue, patterns[index]))
                            {
                                var inputNote = request.GetType().GetProperty("InputNote")?.GetValue(request)?.ToString();
                                warningList.Add(request);
                                errorObjects.Add(new ErrorObject
                                {
                                    Code = "10006",
                                    Message = $"値の形式が無効です。{parentProperty}.{childrenProperty}:{propertyValue}",
                                    InputNote = inputNote ?? ""
                                });
                            }
                        }
                    }
                }
                index++;
            }
        }
        return warningList;
    }
}