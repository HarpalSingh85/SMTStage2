using System;
using System.IO;
using System.Text;


namespace StateMigrationBackend.LogEngine
{      
    public enum LogOptions
    {
        Enabled,
        Disabled
    }

    public enum LogState
    {
        Append,
        Overwrite
    }

    enum LogOperation
    {
        BackupOperation,
        RestoreOperation
    }



    static class Logger
    {
        private static readonly object _syncRootObj = new object();

        private static string _logPath = default(string);

        public static void log(string _errorPath, string _exceptionMessage, LogOptions _logOptions)
        {
            lock(_syncRootObj)
            {
                if (_logOptions.Equals(LogOptions.Enabled))
                {
                    try
                    {
                        logbackend(_errorPath, _exceptionMessage);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                    
                }
            }
            

        }

        public static void assignLogPath(string __logpath, LogState _logstate, LogOperation _logoperation)
        {   
            _logPath = (_logoperation.Equals(LogOperation.BackupOperation)) ? $"{__logpath}\\Backup_ErrorLog_{DateTime.Now.ToString("yyyy_MM_dd")}.csv" : $"{__logpath}\\Restore_ErrorLog_{DateTime.Now.ToString("yyyy_MM_dd")}.csv";

            if (_logstate.Equals(LogState.Overwrite))
            {
                if(File.Exists(_logPath))
                {
                    try
                    {
                        File.Delete(_logPath);
                    }
                    catch(Exception)
                    {
                        throw;
                    }                                    
                }
            }
            
        }

        private static void logbackend(string _errorPath, string _exceptionMessage)
        {
            StringBuilder builder = new StringBuilder();

            if (!File.Exists(_logPath))
            {
                var header = string.Format("\"{0}\",\"{1}\",\"{2}\"",
                                            "Source Path",
                                            "Exception",
                                            "Time"
                                            );
                builder.AppendLine(header);

                var contents = string.Format("\"{0}\",\"{1}\",\"{2}\"",
                                                    _errorPath,
                                                    _exceptionMessage,
                                                    DateTime.Now.ToLocalTime()
                                                    );
                builder.AppendLine(contents);

            }
            else
            {
                var contents = string.Format("\"{0}\",\"{1}\",\"{2}\"",
                                                    _errorPath,
                                                    _exceptionMessage,
                                                    DateTime.Now.ToLocalTime()
                                                    );
                builder.AppendLine(contents);
            }

            try
            {
                using (StreamWriter writer = File.AppendText(_logPath))
                {
                    writer.WriteLine(builder.ToString().TrimEnd('\r', '\n'));                    
                                        
                }

            }
            catch (Exception)
            {
                throw;           
            }

        }
    }
                   
}
