using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using StateMigrationBackend.Validation;
using StateMigrationBackend.Models;
using StateMigrationBackend.StateMigrationEngine.Interfaces;
using System.Collections.Generic;

namespace StateMigrationBackend.StateMigrationEngine
{
   public class SharedResourceHelper : ISharedResourceHelper
    {
        public event EventHandler<StringEventArgs> OnNetworkSharesBackupCompleted;
        public event EventHandler<StringEventArgs> OnNetworkSharesBackupStart;

        public event EventHandler<StringEventArgs> OnNetworkPrintersBackupStart;
        public event EventHandler<StringEventArgs> OnNetworkPrintersBackupCompleted;

        public event EventHandler<StringEventArgs> OnNetworkSharesRestoreCompleted;
        public event EventHandler<StringEventArgs> OnNetworkSharesRestoreStart;

        public event EventHandler<StringEventArgs> OnNetworkPrintersRestoreStart;
        public event EventHandler<StringEventArgs> OnNetworkPrintersRestoreCompleted;

        public event EventHandler<StringEventArgs> OnRestoreStateChange;
        public event EventHandler<StringEventArgs> OnRestoreStateError;

        public event EventHandler<StringEventArgs> OnPrinterAtomicChange;
        public event EventHandler<StringEventArgs> OnNetworkAtomicChange;
        

        private OperationModel GetResourcesBackup(string _userid,List<string> custompaths,string _backupPath, CancellationToken token)
        {
            OperationModel valResults = new OperationModel();
            OperationModel _valResults = new OperationModel();
            INetworkPrintersOperator objPrintOperator = new NetworkPrintersOperator();
            INetworkSharesOperator objNetworkOperator = new NetworkSharesOperator();
            objNetworkOperator.OnAtomicStateChange += ObjNetworkOperator_OnAtomicStateChange;
            objPrintOperator.OnAtomicStateChange += ObjPrintOperator_OnAtomicStateChange;

            if(custompaths.Count > 0)
            {
                valResults.CustomPaths = new List<string>();
                valResults.CustomPaths.AddRange(custompaths);
            }
                        
            try
            {
                RegistryAccessValidation validate = new RegistryAccessValidation();             
                valResults.SID = validate.GetSID(_userid);
                OnNetworkSharesBackupStart?.Invoke(this, new StringEventArgs("Network Backup Started"));
                if (Environment.UserName.Equals(_userid))
                {                    
                    valResults.NetworkSharesWMI = objNetworkOperator.GetNetworkSharesWMI(token);
                }
                else
                {
                    valResults.NetworkShares = objNetworkOperator.GetNetworkShares($@"{valResults.SID}\Network", token);
                }
                OnNetworkSharesBackupCompleted?.Invoke(this, new StringEventArgs("Network Backup Completed"));


                OnNetworkPrintersBackupStart?.Invoke(this,new StringEventArgs("Printers Backup Started"));
                if (Environment.UserName.Equals(_userid))
                {
                    valResults.NetworkPrintersWMI = objPrintOperator.GetMappedPrintersWMI(token);
                }
                else
                {
                    valResults.NetworkPrinters = objPrintOperator.GetMappedPrinters($@"{valResults.SID}\Printers\Connections", token);
                }

                OnNetworkPrintersBackupCompleted?.Invoke(this, new StringEventArgs("Printers Backup Completed"));

                string _backupjson = JsonConvert.SerializeObject(valResults);
                FileOperations.Write(_backupPath, valResults);
                //_valResults = FileOperations.Read(_backupPath);
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
            }                        
            
            return _valResults;
        }
        
        public async Task<OperationModel> GetResourcesBackupAsync(string _userid, List<string> custompaths, string _backupPath, CancellationToken token)
        {
           return await Task.Factory.StartNew(() => GetResourcesBackup(_userid, custompaths, _backupPath, token));         
            
        }

        private void SetResources(string _userid, string _filePath, CancellationToken token)
        {
            OperationModel model = new OperationModel();
            INetworkPrintersOperator objPrintOperator = new NetworkPrintersOperator();
            INetworkSharesOperator objNetworkOperator = new NetworkSharesOperator();
            try
            {                
                model = FileOperations.Read(_filePath);

                OnNetworkPrintersRestoreStart?.Invoke(this, new StringEventArgs("Printers restore initiated"));
                objPrintOperator.OnStateChange += NetworkPrintersOperator_OnStateChange;
                objPrintOperator.OnStateError += NetworkPrintersOperator_OnStateError;
                objPrintOperator.SetMappedPrinters(model,token);
                OnNetworkPrintersRestoreCompleted?.Invoke(this, new StringEventArgs("Printers restore completed"));

                OnNetworkSharesRestoreStart?.Invoke(this, new StringEventArgs("Network shares restore initiated"));
                objNetworkOperator.OnStateChange += NetworkSharesOperator_OnStateChange;
                objNetworkOperator.SetNetworkShares(model,token);
                OnNetworkSharesRestoreCompleted?.Invoke(this, new StringEventArgs("Network shares restore completed"));
            }
            catch(Exception)
            {
                throw;
            }

        }
        
        public async Task SetResourcesAsync(string _userid, string _backupPath, CancellationToken token)
        {
            await Task.Factory.StartNew(() => SetResources(_userid, _backupPath, token));
        }


        #region EventHandlers

        private void NetworkSharesOperator_OnStateChange(object sender, StringEventArgs e)
        {
            OnRestoreStateChange?.Invoke(this,e);            
        }        

        private void NetworkPrintersOperator_OnStateError(object sender, StringEventArgs e)
        {
            OnRestoreStateError?.Invoke(this,e);            
        }

        private void NetworkPrintersOperator_OnStateChange(object sender, StringEventArgs e)
        {
             OnRestoreStateChange?.Invoke(this, e);             
        }

        private void ObjPrintOperator_OnAtomicStateChange(object sender, StringEventArgs e)
        {
            OnPrinterAtomicChange?.Invoke(this,e);
        }

        private void ObjNetworkOperator_OnAtomicStateChange(object sender, StringEventArgs e)
        {
            OnNetworkAtomicChange?.Invoke(this,e);
        }

        #endregion  
    }

}

