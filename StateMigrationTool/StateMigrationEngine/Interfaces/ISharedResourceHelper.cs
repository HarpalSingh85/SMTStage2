using System;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Models;
using System.Collections.Generic;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    public interface ISharedResourceHelper
    {
        event EventHandler<StringEventArgs> OnNetworkAtomicChange;
        event EventHandler<StringEventArgs> OnNetworkPrintersBackupCompleted;
        event EventHandler<StringEventArgs> OnNetworkPrintersBackupStart;
        event EventHandler<StringEventArgs> OnNetworkPrintersRestoreCompleted;
        event EventHandler<StringEventArgs> OnNetworkPrintersRestoreStart;
        event EventHandler<StringEventArgs> OnNetworkSharesBackupCompleted;
        event EventHandler<StringEventArgs> OnNetworkSharesBackupStart;
        event EventHandler<StringEventArgs> OnNetworkSharesRestoreCompleted;
        event EventHandler<StringEventArgs> OnNetworkSharesRestoreStart;
        event EventHandler<StringEventArgs> OnPrinterAtomicChange;
        event EventHandler<StringEventArgs> OnRestoreStateChange;
        event EventHandler<StringEventArgs> OnRestoreStateError;

        Task<OperationModel> GetResourcesBackupAsync(string _userid,List<string> custompaths, string _backupPath, CancellationToken token);
        Task SetResourcesAsync(string _userid, string _backupPath, CancellationToken token);
    }
}