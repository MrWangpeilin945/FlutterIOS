using System.Globalization;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure;

/// <summary>
/// テナントに関する情報を提供するProviderです。
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// テナントのキー文字列が取得できない環境でデフォルトとして使用されるテナントのキー文字列
    /// </summary>
    public const string DefaultTenant = "wellship";
    /// <summary>
    /// テナントを識別するキー文字列
    /// </summary>
    public string TenantKey { get; }
}

/// <summary>
/// テナントに関する情報を提供するProviderです。
/// </summary>
public class TenantProvider : ITenantProvider
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpContextAccessor">HttpContextからアクセスされた情報を取得します</param>
    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        var request = httpContextAccessor.HttpContext?.Request;
        ArgumentNullException.ThrowIfNull(request);
        var uri = new UriBuilder(request.Scheme, request.Host.Host).Uri;
        TenantKey = uri.HostNameType switch
        {
            // IPアクセス時はサブドメインが付与できないためデフォルトテナントを参照
            UriHostNameType.IPv4 or UriHostNameType.IPv6 => ITenantProvider.DefaultTenant,
            // ローカルホストアクセス時もデフォルトテナントを参照
            UriHostNameType.Dns when uri.Host.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) => ITenantProvider.DefaultTenant,
            UriHostNameType.Dns => GetSubDomainFromDnsHost(uri.Host),
            _ => throw new NotSupportedException($"{uri.HostNameType} is not supported.")
        };
    }

    /// <inheritdoc/>
    public string TenantKey { get; private init; }

    /// <summary>
    /// DNSHost名（tenant-01.wellship.jpなど）からサブドメイン部を取得します。
    /// </summary>
    /// <param name="dnsHost">DNSHost名</param>
    /// <returns></returns>
    private static string GetSubDomainFromDnsHost(string dnsHost)
    {
        // NOTE: 厳密にサブドメイン部を取得しようとするとpublic suffix listの参照が必要になります。
        //       public suffix list: https://publicsuffix.org/list/public_suffix_list.dat
        //
        //       WELLSHIPではwellship.jpまたはwellship-stg.jpのみ使用し、ユーザーに応じて異なるドメインでの提供を想定していないため
        //       単純に末尾から.xxxxxxx.xxを取り除くのみの実装とします。

        // 完全修飾FQDNのケースを除外します。
        var trimmedHost = dnsHost[^1] == '.' ? dnsHost.AsSpan(0, dnsHost.Length - 1) : dnsHost.AsSpan();
        // NOTE: 最初に出現する.と最後に出現する.のindexが同じ場合
        //       →xxxxxxx.xxのようなドメインにサブドメインがない場合として判断できます
        var firstPeriodIndex = trimmedHost.IndexOf('.');
        var lastPeriodIndex = trimmedHost.LastIndexOf('.');
        if (firstPeriodIndex == lastPeriodIndex)
        {
            return ITenantProvider.DefaultTenant;
        }
        var secondLastPeriodIndex = trimmedHost[..lastPeriodIndex].LastIndexOf('.');
        Span<char> buff = stackalloc char[secondLastPeriodIndex];
        trimmedHost[..secondLastPeriodIndex].ToLower(buff, CultureInfo.InvariantCulture);
        return buff.ToString();
    }
}