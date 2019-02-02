using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Models;
using StateMigrationBackend.LogEngine;
using StateMigrationBackend.Validation;
using StateMigrationBackend.StateRegions;
using StateMigrationBackend.StateMigrationEngine.Interfaces;

namespace StateMigrationBackend.StateMigrationEngine
{
    class DataBackup : IDataBackup
    {
        public event EventHandler<StringEventArgs> OnBackupStart;
        public event EventHandler<StringEventArgs> OnBackupComplete;
        public event EventHandler<StringEventArgs> OnAtomicCurrentCopyStatus;
        public event EventHandler<int> OnAtomicTotalCounts;
        public event EventHandler<bool> OnCalculationStart;

        #region FullBackups

        public async Task BackupAsync(Regions regions, string TargetPath, string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {
                     
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
                        
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));

            if(!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    //CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    int counts = await countModel.TotalCountsAsync(source);
                    //OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    OnAtomicTotalCounts?.Invoke(this, counts);
                    await objbackuphandler.CopyRecursiveAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {

                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        //CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        // OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        int counts = await countModel.TotalCountsAsync(source);
                        OnAtomicTotalCounts?.Invoke(this, counts);
                        await objbackuphandler.CopyRecursiveAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }

            }            
                        
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));

        }
        
        public async Task BackupPartialParallelAsync(Regions regions, string TargetPath, string source, string _user,LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {
            
            Logger.assignLogPath(TargetPath, _logstate, _logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
                       
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));

            if (!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    await objbackuphandler.CopyPartialParallelAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {
                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        await objbackuphandler.CopyPartialParallelAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }
            }
            
                        
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));

        }              

        public async Task BackupParallelAsync(Regions regions, string TargetPath,string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
                        
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));
            if (!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    await objbackuphandler.ParallelCopyAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {
                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        await objbackuphandler.ParallelCopyAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }
            }
                                    
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));

        }

        

        #endregion

        #region DifferenceBackups

        public async Task DifferenceBackupAsync(Regions regions, string TargetPath,string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {
            
            Logger.assignLogPath(TargetPath, _logstate,_logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
            
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));
            if (!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    await objbackuphandler.DifferenceCopyRecursiveAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {                
                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        await objbackuphandler.DifferenceCopyRecursiveAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }
            }           
                        
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));

        }

        public async Task DifferenceBackupPartialParallelAsync(Regions regions, string TargetPath,string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {
            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
                        
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));
            if (!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    await objbackuphandler.DifferenceCopyPartialParallelAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {
                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        await objbackuphandler.DifferenceCopyPartialParallelAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }
            }          
                        
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));

        }

        public async Task DifferenceBackupParallelAsync(Regions regions, string TargetPath,string source, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {
            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            BackupCopyHandler objbackuphandler = new BackupCopyHandler();
            objbackuphandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;
                        
            OnBackupStart?.Invoke(this, new StringEventArgs("Data Backup Started"));
            if (!string.IsNullOrEmpty(source))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(source))
                {
                    OnCalculationStart?.Invoke(this, true);
                    EnumData countModel = new EnumData();
                    CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(source));
                    OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                    await objbackuphandler.DifferenceParallelCopyAsync(new DirectoryInfo(source), new DirectoryInfo(TargetPath), _logOptions, token);
                }
                else
                    Logger.log(source, "path not found", _logOptions);
            }
            else
            {
                foreach (var path in regions.GetPaths(_user))
                {
                    PathValidator validator = new PathValidator();
                    if (validator.Validate(path))
                    {
                        OnCalculationStart?.Invoke(this, true);
                        EnumData countModel = new EnumData();
                        CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(path));
                        OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                        await objbackuphandler.DifferenceParallelCopyAsync(new DirectoryInfo(path), new DirectoryInfo(TargetPath), _logOptions, token);
                    }
                    else
                        Logger.log(path, "path not found", _logOptions);
                }

            }
            OnBackupComplete?.Invoke(this, new StringEventArgs("Data Backup Completed"));
            

        }

        #endregion

        #region EventDelegation

        private void DataBackup_OnAtomicCurrent(object sender, StringEventArgs e)
        {            
            OnAtomicCurrentCopyStatus?.Invoke(this, e);
        }

        #endregion 

    }
}
