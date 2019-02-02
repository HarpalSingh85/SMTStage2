using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Models;
using StateMigrationBackend.LogEngine;
using StateMigrationBackend.StateRegions;
using StateMigrationBackend.Validation;
using StateMigrationBackend.StateMigrationEngine.Interfaces;
using System.Linq;

namespace StateMigrationBackend.StateMigrationEngine
{
    class DataRestore : IDataRestore
    {
        public event EventHandler<StringEventArgs> OnDataRestoreStart;
        public event EventHandler<StringEventArgs> OnDataRestoreComplete;
        public event EventHandler<StringEventArgs> OnAtomicCurrentCopyStatus;        
        public event EventHandler<int> OnAtomicTotalCounts;
        public event EventHandler<bool> OnCalculationStart;

        #region FullRestores

        public async Task RestoreAsync(Regions regions, string TargetPath,string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            if (model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));
            
            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);

                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {
                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.CopyRecursiveAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.CopyRecursiveAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }

                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }

            
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }
        
        public async Task RestorePartialParallelAsync(Regions regions, string TargetPath, string _user,LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath, _logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            if (model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));
            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);

                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {
                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.CopyPartialParallelAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.CopyPartialParallelAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }

                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }

            
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }              

        public async Task RestoreParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            if (model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));
            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);

                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {
                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.ParallelCopyAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.ParallelCopyAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }

                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }

            
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }

        #endregion

        #region DifferenceRestores

        public async Task DifferenceRestoreAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath, _logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            if(model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }
            

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));

            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();            
            
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);
                    
                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {                        

                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.DifferenceCopyRecursiveAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.DifferenceCopyRecursiveAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }

                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }
            
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }

        public async Task DifferenceRestorePartialParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            
            if (model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));
            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);

                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {
                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.DifferenceCopyPartialParallelAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.DifferenceCopyPartialParallelAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }
                        
                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }

            
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }

        public async Task DifferenceRestoreParallelAsync(Regions regions, string TargetPath, string _user, LogOptions _logOptions, LogState _logstate, LogOperation _logoperation, CancellationToken token)
        {            
            Logger.assignLogPath(TargetPath,_logstate,_logoperation);
            RestoreCopyHandler objrestorehandler = new RestoreCopyHandler();
            objrestorehandler.OnAtomicCurrent += DataBackup_OnAtomicCurrent;

            var model = FileOperations.Read(TargetPath);
            if (model.CustomPaths != null)
            {
                regions.SetPaths(model.CustomPaths);
            }

            OnDataRestoreStart?.Invoke(this, new StringEventArgs("Data Restore Started"));
            var BackupDirectoriesInfo = (new DirectoryInfo(TargetPath)).GetDirectories();
            foreach (var path in regions.GetPaths(_user))
            {
                PathValidator validator = new PathValidator();
                if (validator.Validate(path))
                {
                    var _path = new DirectoryInfo(path);

                    foreach (var item in BackupDirectoriesInfo.Where(x => x.Name.Equals(_path.Name)))
                    {
                        if (!(new DirectoryInfo(path)).Name.Equals(item.Name))
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            new DirectoryInfo($@"{path}\{item.Name}").Create();
                            await objrestorehandler.DifferenceParallelCopyAsync(item, new DirectoryInfo($@"{path}\{item.Name}"), _logOptions, token);

                        }
                        else
                        {
                            OnCalculationStart?.Invoke(this, true);
                            EnumData countModel = new EnumData();
                            CountModel counts = await countModel.GetCountAsync(new DirectoryInfo(item.FullName));
                            OnAtomicTotalCounts?.Invoke(this, (counts.FileCount + counts.DirCount));
                            await objrestorehandler.DifferenceParallelCopyAsync(item, new DirectoryInfo(path), _logOptions, token);
                        }

                    }

                }
                else
                    Logger.log(path, "path not found", _logOptions);
            }
                        
            OnDataRestoreComplete?.Invoke(this, new StringEventArgs("Data Restore Completed"));

        }

        #endregion

        #region EventDelegation

        private void DataBackup_OnAtomicCurrent(object sender, StringEventArgs e)
        {
            OnAtomicCurrentCopyStatus?.Invoke(this,e); 
        }

        #endregion 

    }
}
