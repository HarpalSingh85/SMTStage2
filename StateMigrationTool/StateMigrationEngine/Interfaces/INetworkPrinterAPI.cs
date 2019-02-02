using StateMigrationBackend.Models;
using System;
using System.Threading;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    interface INetworkPrinterAPI
    {
        event EventHandler<StringEventArgs> OnAtomicStateChange;
        event EventHandler<StringEventArgs> OnAtomicStateError;
        bool AddPrinter(string sPrinterName, CancellationToken token);
        bool DisconnectPrinter(string sPrinterName, CancellationToken token);        
        void RenamePrinter(string sPrinterName, string newName, CancellationToken token);
        void SetDefaultPrinter(string sPrinterName, CancellationToken token);
    }
}