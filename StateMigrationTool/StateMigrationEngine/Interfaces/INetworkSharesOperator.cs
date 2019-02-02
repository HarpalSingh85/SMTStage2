using System;
using System.Collections.Generic;
using System.Threading;
using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    public interface INetworkSharesOperator
    {
        event EventHandler<StringEventArgs> OnStateChange;
        event EventHandler<StringEventArgs> OnAtomicStateChange;

        List<Key> GetNetworkShares(string _regpath, CancellationToken token);
        Dictionary<string, string> GetNetworkSharesWMI(CancellationToken token);
        void SetNetworkShares(OperationModel _opModel, CancellationToken token);
    }
}