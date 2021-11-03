namespace Webfarm.Sdk.Common.Transactions
{
    using System;
    using System.Transactions;

    public class AppTransactionScope : IDisposable
    {
        private readonly TransactionScope transactionScope;

        public AppTransactionScope()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            this.transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Complete() => this.transactionScope.Complete();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.transactionScope.Dispose();
            }
        }
    }
}
