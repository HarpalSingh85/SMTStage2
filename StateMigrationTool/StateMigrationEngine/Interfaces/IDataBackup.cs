using System;
using StateMigrationBackend.LogEngine;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    interface IDataBackup
    {
        event EventHandler<StringEventArgs> OnAtomicCurrentCopyStatus;
        event EventHandler<StringEventArgs> OnBackupComplete;
        event EventHandler<StringEventArgs> OnBackupStart;
        event EventHandler<int> OnAtomicTotalCounts;
        event EventHandler<bool> OnCalculationStart;

        Task BackupAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task BackupParallelAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task BackupPartialParallelAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceBackupAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceBackupParallelAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceBackupPartialParallelAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
    }
}