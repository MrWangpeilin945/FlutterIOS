using System.Transactions;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction;

/// <summary>
/// トランザクションスコープに関連するヘルパークラスです
/// </summary>
public static class TransactionScopeHelper
{
    /// <summary>
    /// トランザクションスコープを払い出します。
    /// トランザクション分離レベルはReadCommittedとし、30秒でタイムアウトします。
    /// トランザクションスコープがまだ張られていない場合は新規にトランザクションスコープを作りますが、既に張られている場合はそのスコープを使う設定にしています。
    /// </summary>
    /// <returns></returns>
    public static TransactionScope GetTransactionScope()
    {
        var op = new TransactionOptions()
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = new TimeSpan(0, 0, 30),
        };
        return new TransactionScope(TransactionScopeOption.Required, op, TransactionScopeAsyncFlowOption.Enabled);
    }
}
