using System;
using System.Threading;
using System.Management;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateMigrationEngine.Interfaces;

namespace BackupBackend.StateMigrationEngine
{
    class NetworkPrinterAPI : INetworkPrinterAPI
    {
        private static ManagementScope objManagementScope = null;

        public event EventHandler<StringEventArgs> OnAtomicStateChange;
        public event EventHandler<StringEventArgs> OnAtomicStateError;

        public bool AddPrinter(string sPrinterName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            try
            {
                objManagementScope = new ManagementScope(ManagementPath.DefaultPath);
                objManagementScope.Connect();

                ManagementClass objPrinterClass = new ManagementClass(new ManagementPath("Win32_Printer"), null);
                ManagementBaseObject objInputParameters = objPrinterClass.GetMethodParameters("AddPrinterConnection");
                objInputParameters.SetPropertyValue("Name", sPrinterName);
                if(IsPrinterInstalled(sPrinterName, token))
                {
                    DisconnectPrinter(sPrinterName, token);
                }
                objPrinterClass?.InvokeMethod("AddPrinterConnection", objInputParameters, null);
                OnAtomicStateChange?.Invoke(this,new StringEventArgs($"Completed adding printer : {sPrinterName}"));
                return true;
            }

            catch (OperationCanceledException)
            {
                throw;
            }

            catch (Exception)
            {
                OnAtomicStateError?.Invoke(this,new StringEventArgs($"Failed adding printer : {sPrinterName}"));
                return false;
            }
        }
        
        public bool DisconnectPrinter(string sPrinterName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            objManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            objManagementScope.Connect();

            SelectQuery objSelectQuery = new SelectQuery();
            objSelectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + sPrinterName.Replace("\\", "\\\\") + "'";
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(objManagementScope, objSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    oItem.Delete();
                    OnAtomicStateChange?.Invoke(this,new StringEventArgs($"Disconnected printer : {sPrinterName}"));
                    return true;
                }
            }
            return false;
        }        

        public void RenamePrinter(string sPrinterName, string newName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            objManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            objManagementScope.Connect();

            SelectQuery objSelectQuery = new SelectQuery();
            objSelectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + sPrinterName.Replace("\\", "\\\\") + "'";
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(objManagementScope, objSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    oItem?.InvokeMethod("RenamePrinter", new object[] { newName });
                    OnAtomicStateChange?.Invoke(this, new StringEventArgs($"Renamed printer : {sPrinterName} to {newName}"));
                    return;
                }
            }
        }
                
        public void SetDefaultPrinter(string sPrinterName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            objManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            objManagementScope.Connect();

            SelectQuery objSelectQuery = new SelectQuery();
            objSelectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + sPrinterName.Replace("\\", "\\\\") + "'";
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(objManagementScope, objSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    oItem?.InvokeMethod("SetDefaultPrinter", new object[] { sPrinterName });
                    OnAtomicStateChange?.Invoke(this, new StringEventArgs($"Completed setting default printer : {sPrinterName}"));
                    return;
                }
            }
        }
        
        private bool IsPrinterInstalled(string sPrinterName, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            objManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            objManagementScope.Connect();

            SelectQuery objSelectQuery = new SelectQuery();
            objSelectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" + sPrinterName.Replace("\\", "\\\\") + "'";
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(objManagementScope, objSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            return oObjectCollection.Count > 0;
        }
    }

}


