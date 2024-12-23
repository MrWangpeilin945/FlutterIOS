using System.Transactions;

namespace Ryobi.Wellship.WebAPI.ResultCollector.Infrastructure.Transaction
{
    public static class TransactionScopeHelper
    {
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
}
