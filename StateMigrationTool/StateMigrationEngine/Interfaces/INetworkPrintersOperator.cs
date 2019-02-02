using System;
using System.Collections.Generic;
using System.Threading;
using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    public interface INetworkPrintersOperator
    {
        event EventHandler<StringEventArgs> OnStateChange;
        event EventHandler<StringEventArgs> OnAtomicStateChange;
        event EventHandler<StringEventArgs> OnStateError;

        List<Printers> GetMappedPrinters(string _regpath, CancellationToken token);
        List<Printers> GetMappedPrintersWMI(CancellationToken token);
        void SetMappedPrinters(OperationModel _opModel, CancellationToken token);
    }
}