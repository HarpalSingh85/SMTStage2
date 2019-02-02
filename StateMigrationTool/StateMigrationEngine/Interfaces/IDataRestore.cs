using System;
using StateMigrationBackend.LogEngine;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateRegions;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    interface IDataRestore
    {
        event EventHandler<StringEventArgs> OnAtomicCurrentCopyStatus;
        event EventHandler<StringEventArgs> OnDataRestoreComplete;
        event EventHandler<StringEventArgs> OnDataRestoreStart;       
        event EventHandler<int> OnAtomicTotalCounts;
        event EventHandler<bool> OnCalculationStart;

        Task RestoreAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task RestoreParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task RestorePartialParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceRestoreAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceRestoreParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
        Task DifferenceRestorePartialParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token);
    }
}