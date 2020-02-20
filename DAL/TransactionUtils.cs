using System;
using System.Transactions;

namespace CUSTOMRP.DAL
{
    /// <summary>
    /// Utility class based on this blog article
    /// http://blogs.msdn.com/b/dbrowne/archive/2010/06/03/using-new-transactionscope-considered-harmful.aspx
    /// </summary>
    public class TransactionUtils
    {
        public static TransactionScope CreateTransactionScope()
        {
            return CreateTransactionScope(TransactionScopeOption.Required, TransactionManager.MaximumTimeout);
        }

        public static TransactionScope CreateTransactionScope(TransactionScopeOption option)
        {
            return CreateTransactionScope(option, new TimeSpan(0, 15, 0));
        }

        public static TransactionScope CreateTransactionScope(TransactionScopeOption option, TimeSpan timeout)
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = option == TransactionScopeOption.Suppress ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = timeout;
            return new TransactionScope(option, transactionOptions);
        }
    }
}