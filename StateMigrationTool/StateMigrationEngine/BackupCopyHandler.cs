using StateMigrationBackend.LogEngine;
using StateMigrationBackend.Models;
using StateMigrationBackend.Validation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StateMigrationBackend.StateMigrationEngine
{    
   class BackupCopyHandler
   {

    protected internal event EventHandler<StringEventArgs> OnAtomicCurrent;        

    #region FullCopy

    private DirectoryInfo CopyRecursive(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        DirectoryInfo newDirectoryInfo = null;

        if(token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();
        }

        try
        {
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(newDirectoryInfo.FullName));
        }
        catch(Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }
            

        foreach (var fileInfo in source.GetFiles())
        {
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(fileInfo.FullName));

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            try
            {    
                if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                {
                    AccessValidator validator = new AccessValidator();
                    if (validator.CanRead(fileInfo.FullName))
                    {    
                        var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);
                        
                        fileInfo.CopyTo(_targetpath, true);                        
                        File.SetAttributes(_targetpath, FileAttributes.Normal);
                    }
                    else
                    {
                        Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                    }
                }
                    
            }
            catch(OperationCanceledException)
            {
                throw;
            }

            catch(Exception Ex)
            {
                try
                {
                    if (!Ex.Message.Contains("The operation was canceled."))
                    {
                        Logger.log(fileInfo.FullName, Ex.Message, _logOptions);

                    }
                    else
                        break;
                    
                    continue;
                }
                catch (Exception)
                {
                    throw;
                }

            } 
                 
        }

        foreach (var childDirectoryInfo in source.GetDirectories())
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }            

            try
            {
                if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    CopyRecursive(childDirectoryInfo, newDirectoryInfo, _logOptions, token);
                }
                else
                {
                    Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link",_logOptions);
                }
            }

            catch(OperationCanceledException)
            {
                throw;
            }

            catch(Exception Ex)
            {
                try
                {
                    if (!Ex.Message.Contains("The operation was canceled."))
                    {
                        Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                    }
                    else
                        break;
                    continue;
                }
                catch(Exception)
                {
                    throw;
                }
                    
            }
                    
        }
        return newDirectoryInfo;

    }
        
    private DirectoryInfo ParallelCopy(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, ParallelOptions _po)
    {
        DirectoryInfo newDirectoryInfo = null; 
                   
        try
        {                
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(newDirectoryInfo.FullName));
        }
        catch (Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }


        try
        {
            Parallel.ForEach(source.GetFiles(), _po, (fileInfo, state) =>
            {
                OnAtomicCurrent?.Invoke(this,new StringEventArgs(fileInfo.FullName));
                try
                {
                    _po.CancellationToken.ThrowIfCancellationRequested();

                    if (_po.CancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                    {
                        AccessValidator validator = new AccessValidator();
                        if (validator.CanRead(fileInfo.FullName))
                        {
                            var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);

                                    
                            fileInfo.CopyTo(_targetpath, true);
                            File.SetAttributes(_targetpath, FileAttributes.Normal);
                        }
                        else
                        {                                    
                            Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    try
                    {
                        if (!Ex.Message.Contains("The operation was canceled."))
                        {                                   
                            Logger.log(fileInfo.FullName, Ex.Message, _logOptions);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

            });

            Parallel.ForEach(source.GetDirectories(), _po, (childDirectoryInfo, state) =>
            {

                try
                {
                    _po.CancellationToken.ThrowIfCancellationRequested();

                    if (_po.CancellationToken.IsCancellationRequested)
                    {
                        state.Break();
                    }

                    if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {                        
                        ParallelCopy(childDirectoryInfo, newDirectoryInfo, _logOptions, _po);
                    }
                    else
                    {
                        Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link", _logOptions);
                    }
                }

                catch (Exception Ex)
                {

                    try
                    {
                        if (!Ex.Message.Contains("The operation was canceled."))
                        {
                            Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                        }

                    }

                    catch (Exception)
                    {
                        throw;
                    }

                }

            });

        }
        catch (OperationCanceledException)
        {
            throw;
        }
         
        return newDirectoryInfo;
    }

    private DirectoryInfo PartialParallelCopy(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, ParallelOptions _po)
    {
        DirectoryInfo newDirectoryInfo = null;
        try
        {
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(newDirectoryInfo.FullName));
        }
        catch (Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }

        try
        {
            Parallel.ForEach(source.GetFiles(), _po, (fileInfo, state) =>
            {
                OnAtomicCurrent?.Invoke(this,new StringEventArgs(fileInfo.FullName));

                _po.CancellationToken.ThrowIfCancellationRequested();

                if (_po.CancellationToken.IsCancellationRequested)
                {
                    state.Break();
                }
                
                try
                {
                    if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                    {
                        AccessValidator validator = new AccessValidator();
                        if (validator.CanRead(fileInfo.FullName))
                        {
                            var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);
                                                        
                            fileInfo.CopyTo(_targetpath, true);
                            File.SetAttributes(_targetpath, FileAttributes.Normal);
                        }
                        else
                        {
                            Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    try
                    {
                        Logger.log(fileInfo.FullName, Ex.Message, _logOptions);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

            });
        }
        catch (OperationCanceledException)
        {
            throw;
        }

        foreach (DirectoryInfo childDirectoryInfo in source.GetDirectories())
        {
            if (_po.CancellationToken.IsCancellationRequested)
            {
                _po.CancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {                    
                    PartialParallelCopy(childDirectoryInfo, newDirectoryInfo, _logOptions, _po);
                }
                else
                {
                    Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link", _logOptions);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }

            catch (Exception Ex)
            {
                try
                {
                    if (!Ex.Message.Contains("The operation was canceled."))
                    {
                        Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                    }
                    else
                        break;

                    continue;
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        return newDirectoryInfo;

    }

    protected internal async Task CopyRecursiveAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {            
        await Task.Factory.StartNew(() => CopyRecursive(source, target, _logOptions, token));
    }

    protected internal async Task CopyPartialParallelAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        ParallelOptions po = new ParallelOptions();
        po.CancellationToken = token;
        po.MaxDegreeOfParallelism = Environment.ProcessorCount;
        await Task.Factory.StartNew(() => PartialParallelCopy(source, target, _logOptions, po));

    }
        
    protected internal async Task ParallelCopyAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        ParallelOptions po = new ParallelOptions();
        po.CancellationToken = token;
        po.MaxDegreeOfParallelism = Environment.ProcessorCount;
        await Task.Factory.StartNew(() => ParallelCopy(source, target, _logOptions, po));
            
    }

    #endregion
        
    #region DifferenceCopy

    private bool IsSourceNewer(string source, string target)
    {
        return (File.GetLastWriteTime(source).CompareTo(File.GetLastWriteTime(target)) > 0) ? true : false;
    }

    private DirectoryInfo DifferenceCopyRecursive(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        DirectoryInfo newDirectoryInfo = null;

        if (token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();
        }

        try
        {
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(newDirectoryInfo.FullName));
        }
        catch (Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }


        foreach (var fileInfo in source.GetFiles())
        {
            OnAtomicCurrent?.Invoke(this, new StringEventArgs(fileInfo.FullName));

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            try
            {
                if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                {
                    AccessValidator validator = new AccessValidator();

                    if (validator.CanRead(fileInfo.FullName))
                    {                           
                        var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);

                        if (File.Exists(_targetpath))
                        {
                            if (IsSourceNewer(fileInfo.FullName, _targetpath))
                            {
                                
                                fileInfo.CopyTo(_targetpath, true);
                                File.SetAttributes(_targetpath, FileAttributes.Normal);
                            }

                        }
                        else
                        {                            
                            fileInfo.CopyTo(_targetpath, true);
                            File.SetAttributes(_targetpath, FileAttributes.Normal);
                        }

                    }
                    else
                    {
                        Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                    }
                }
            }

            catch(OperationCanceledException)
            {
                throw;
            }

            catch (Exception Ex)
            {                    
                try
                {   if (!Ex.Message.Contains("The operation was canceled."))
                        {
                            Logger.log(fileInfo.FullName, Ex.Message, _logOptions);
                        }
                        else
                            break;
                    
                    continue;
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        foreach (var childDirectoryInfo in source.GetDirectories())
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            try
            {
                if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {   
                    DifferenceCopyRecursive(childDirectoryInfo, newDirectoryInfo, _logOptions, token);
                }
                else
                {
                    Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link", _logOptions);
                }
            }

            catch (OperationCanceledException)
            {
                throw;
            }

            catch (Exception Ex)
            {                    
                try
                {
                        if (!Ex.Message.Contains("The operation was canceled."))
                        {
                            Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                        }
                        else
                            break;
                    
                    continue;
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
        return newDirectoryInfo;

    }

    private DirectoryInfo DifferenceParallelCopy(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, ParallelOptions _po)
    {                        

        DirectoryInfo newDirectoryInfo = null;
        try
        {
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this,new StringEventArgs(newDirectoryInfo.FullName));
        }
            
        catch (Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }

        try
        {
            Parallel.ForEach(source.GetFiles(), _po, (fileInfo, state) =>
            {
                OnAtomicCurrent?.Invoke(this,new StringEventArgs(fileInfo.FullName));

                _po.CancellationToken.ThrowIfCancellationRequested();

                if (_po.CancellationToken.IsCancellationRequested)
                {
                    state.Break();
                }
                
                try
                {                        

                    if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                    {
                        AccessValidator validator = new AccessValidator();

                        if (validator.CanRead(fileInfo.FullName))
                        {
                            var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);

                            if (File.Exists(_targetpath))
                            {
                                if (IsSourceNewer(fileInfo.FullName, _targetpath))
                                {
                                    
                                    fileInfo.CopyTo(_targetpath, true);
                                    File.SetAttributes(_targetpath, FileAttributes.Normal);
                                }

                            }
                            else
                            {                                
                                fileInfo.CopyTo(_targetpath, true);
                                File.SetAttributes(_targetpath, FileAttributes.Normal);
                            }

                        }
                        else
                        {
                            Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                        }
                    }
                }

                catch (Exception Ex)
                {
                    try
                    {                            
                        if (!Ex.Message.Contains("The operation was canceled."))
                        {
                            Logger.log(fileInfo.FullName, Ex.Message, _logOptions);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }


            });

            Parallel.ForEach(source.GetDirectories(), _po, (childDirectoryInfo, state) =>
            {
                _po.CancellationToken.ThrowIfCancellationRequested();

                if(_po.CancellationToken.IsCancellationRequested)
                {
                    state.Break();
                }
                    
                try
                {
                    if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {                        
                        DifferenceParallelCopy(childDirectoryInfo, newDirectoryInfo, _logOptions, _po);
                    }
                    else
                    {
                        Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link", _logOptions);
                    }
                }

                catch (Exception Ex)
                {                      

                    try
                    {
                        if(!Ex.Message.Contains("The operation was canceled."))
                        {
                            Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                        }
                                                       
                    }
                        
                    catch (Exception)
                    {
                        throw;
                    }

                }

            });

        }
        catch(OperationCanceledException)
        {
            throw;
        }
            
        return newDirectoryInfo;
    }

    private DirectoryInfo DifferencePartialParallelCopy(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, ParallelOptions _po)
    {
        DirectoryInfo newDirectoryInfo = null;
        try
        {
            newDirectoryInfo = target.CreateSubdirectory(source.Name);
            OnAtomicCurrent?.Invoke(this,new StringEventArgs(newDirectoryInfo.FullName));
        }
        catch (Exception Ex)
        {
            Logger.log(newDirectoryInfo.FullName, Ex.Message, _logOptions);
        }

        try
        {
            Parallel.ForEach(source.GetFiles(), _po, (fileInfo, state) =>
            {
                OnAtomicCurrent?.Invoke(this,new StringEventArgs(fileInfo.FullName));

                _po.CancellationToken.ThrowIfCancellationRequested();

                if (_po.CancellationToken.IsCancellationRequested)
                {
                    state.Break();
                }
                
                try
                {
                    if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && AccessViolationHelper.IsNotLocked(fileInfo.FullName, _logOptions))
                    {
                        AccessValidator validator = new AccessValidator();

                        if (validator.CanRead(fileInfo.FullName))
                        {
                            var _targetpath = Path.Combine(newDirectoryInfo.FullName, fileInfo.Name);

                            if (File.Exists(_targetpath))
                            {
                                if (IsSourceNewer(fileInfo.FullName, _targetpath))
                                {                                    
                                    fileInfo.CopyTo(_targetpath, true);
                                    File.SetAttributes(_targetpath, FileAttributes.Normal);
                                }

                            }
                            else
                            {                                
                                fileInfo.CopyTo(_targetpath, true);
                                File.SetAttributes(_targetpath, FileAttributes.Normal);
                            }
                        }
                        else
                        {
                            Logger.log(fileInfo.FullName, "Access Denied", _logOptions);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    try
                    {
                        Logger.log(fileInfo.FullName, Ex.Message, _logOptions);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

            });
        }
        catch(OperationCanceledException)
        {
            throw;
        }
                   
            
        foreach (DirectoryInfo childDirectoryInfo in source.GetDirectories())
        {
            if (_po.CancellationToken.IsCancellationRequested)
            {
                _po.CancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {                    
                    DifferencePartialParallelCopy(childDirectoryInfo, newDirectoryInfo, _logOptions, _po);
                }
                else
                {
                    Logger.log(childDirectoryInfo.FullName, "Reparse Point / Hard Link", _logOptions);
                }
            }
                               

            catch (Exception Ex)
            {
                try
                {
                    if (!Ex.Message.Contains("The operation was canceled."))
                    {
                        Logger.log(childDirectoryInfo.FullName, Ex.Message, _logOptions);
                    }

                    continue;
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        return newDirectoryInfo;      

    }

    protected internal async Task DifferenceCopyRecursiveAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        await Task.Factory.StartNew(() => DifferenceCopyRecursive(source, target, _logOptions, token));
    }

    protected internal async Task DifferenceCopyPartialParallelAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        ParallelOptions po = new ParallelOptions();
        po.CancellationToken = token;
        po.MaxDegreeOfParallelism = Environment.ProcessorCount;
        await Task.Factory.StartNew(() => DifferencePartialParallelCopy(source, target, _logOptions, po));
    }

    protected internal async Task DifferenceParallelCopyAsync(DirectoryInfo source, DirectoryInfo target, LogOptions _logOptions, CancellationToken token)
    {
        ParallelOptions po = new ParallelOptions();
        po.CancellationToken = token;
        po.MaxDegreeOfParallelism = Environment.ProcessorCount;
        await Task.Factory.StartNew(() => DifferenceParallelCopy(source, target, _logOptions, po));

    }

    #endregion

    }

}
