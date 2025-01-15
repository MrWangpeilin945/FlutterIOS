using System.Data;

using Dapper;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.PostgreSQL.TypeHandler;

/// <summary>
/// DateTimeOffset型のカスタムタイプハンドラー。
/// /// PostgreSQLのtimestamp with timezone型とC#のDateTimeOffset型をマッピングします。
/// </summary>
public class SqlDateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    /// <summary>
    /// DateTimeOffset型の値をデータベースパラメータに設定します。
    /// 値はUTC時間に変換されます。
    /// </summary>
    /// <param name="parameter">データベースパラメータ。</param>
    /// <param name="dateTimeOffset">設定するDateTimeOffset型の値。</param>
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset dateTimeOffset)
    {
        parameter.Value = dateTimeOffset.UtcDateTime;
    }

    /// <summary>
    /// データベースから取得した値をDateTimeOffset型に変換します。
    /// 値はUTCとして解釈され、ローカル時間に変換されます。
    /// </summary>
    /// <param name="value">データベースから取得した値。</param>
    /// <returns>変換されたDateTimeOffset型の値。</returns>
    public override DateTimeOffset Parse(object value)
    {
        var dateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        return new DateTimeOffset(dateTime).ToLocalTime();
    }
}
