using Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard;

namespace Ryobi.Wellship.WebAPI.ExternalConnection.Usecases;
/// <summary>
/// EC2002_受付を更新する
/// </summary>
public interface ITicketUsecase
{
    /// <summary>
    /// EC2002_受付を更新する
    /// </summary>
    /// <param name="tickets">更新する受付のリスト</param>
    /// <returns>エラーリスト</returns>
    public Task<List<ErrorObject>> StoreTicketsAsync(List<Ticket> tickets);
}
