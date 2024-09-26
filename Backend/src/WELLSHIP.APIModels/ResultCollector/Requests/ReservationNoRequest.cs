using System.Text.Json.Serialization;

namespace Ryobi.Wellship.APIModels.Requests;

/// <summary>
/// 予約No検証用リクエストモデル
/// </summary>
public class ReservationNoRequest
{
    /// <summary>
    /// 予約No
    /// </summary>
    [JsonPropertyName("reservationNo")]
    public string ReservationNo { get; set; } = "";
}
