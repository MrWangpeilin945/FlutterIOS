using System.Data;

using Dapper;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.TypeHandler;

/// <summary>
/// DateOnly型のカスタムタイプハンドラー。
/// PostgreSQLのdate型とC#のDateOnly型をマッピングします。
/// </summary>
public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <summary>
    /// DateOnly型の値をデータベースパラメータに設定します。
    /// 値はDateTimeに変換されます。
    /// </summary>
    /// /// <param name="parameter">データベースパラメータ。</param>
    /// <param name="date">設定するDateOnly型の値。</param>
    public override void SetValue(IDbDataParameter parameter, DateOnly date)
    {
        parameter.Value = date.ToDateTime(new TimeOnly(0, 0));
    }

    /// <summary>
    /// データベースから取得した値をDateOnly型に変換します。
    /// </summary>
    /// <param name="value">データベースから取得した値。</param>
    /// <returns>変換されたDateOnly型の値。</returns>
    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime((DateTime)value);
    }
}
