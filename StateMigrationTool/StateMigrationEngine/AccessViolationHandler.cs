using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using StateMigrationBackend.LogEngine;
using System.Security;
using System.Threading.Tasks;
using StateMigrationBackend.Validation;

namespace StateMigrationBackend.StateMigrationEngine
{       
    internal static class AccessViolationHelper
    {
        private static readonly object _syncRoot = new object();        

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;


        private static Predicate<string> _IsNotLocked = (fileName) => {
            bool _notlocked = true;

            lock (_syncRoot)
            {
                SafeFileHandle fileHandle = SafeNativeMethods.CreateFile(fileName, FileSystemRights.Modify, FileShare.Read, IntPtr.Zero, FileMode.OpenOrCreate, FileOptions.None, IntPtr.Zero);

                if (fileHandle.IsInvalid)
                {
                    var _Error = Marshal.GetLastWin32Error();

                    if (_Error == ERROR_SHARING_VIOLATION || _Error == ERROR_LOCK_VIOLATION)
                    {
                        _notlocked = false;                       
                    }
                    
                }
                fileHandle.Close();                
            }
            
            return _notlocked;
        };
             
        internal static bool IsNotLocked(string _filename, LogOptions _logOptions)
        {
            bool _result = _IsNotLocked(_filename);
            if (_result == false)
            {
                Logger.log(_filename, "File is open or is in use", _logOptions);
            }
            return _result;

        }

        internal static bool IsNotReparse(string d, bool directory)
        {
            if(directory)
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                if (dir.Exists && (dir.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                FileInfo file = new FileInfo(d);
                if (file.Exists && (file.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    return true;
                }
                else
                    return 
false;
            }
            
        }

        internal static int GetTotalCount(DirectoryInfo rootdirectory,int count = 0)
        {

                Parallel.ForEach(rootdirectory.EnumerateFiles("*.*",SearchOption.TopDirectoryOnly), (fileInfo) =>
                {                   
                   
                    if (fileInfo.Exists && ((File.GetAttributes(fileInfo.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint))
                    {
                        count += 1;
                    }
                  
                });

                Parallel.ForEach(rootdirectory.EnumerateDirectories("*.*",SearchOption.AllDirectories), (childDirectoryInfo) =>
                {

                        if (childDirectoryInfo.Exists && (childDirectoryInfo.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                        {
                        count += 1;
                        GetTotalCount(childDirectoryInfo, count);
                        }
                    
                });

            return count;
           
        }


        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern SafeFileHandle CreateFile(string lpFileName,FileSystemRights dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, FileOptions dwFlagsAndAttributes, IntPtr hTemplateFile);
            
        }
    }

}
