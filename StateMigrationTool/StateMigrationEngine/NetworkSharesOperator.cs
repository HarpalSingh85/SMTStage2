using System;
using Microsoft.Win32;
using System.Threading;
using System.Management;
using System.Collections.Generic;
using StateMigrationBackend.Models;
using BackupBackend.StateMigrationEngine;
using StateMigrationBackend.StateMigrationEngine.Interfaces;

namespace StateMigrationBackend.StateMigrationEngine
{
   public class NetworkSharesOperator : RegistryInterpolator, INetworkSharesOperator
    {
        public event EventHandler<StringEventArgs> OnStateChange;
        public event EventHandler<StringEventArgs> OnAtomicStateChange;

        public Dictionary<string, string> GetNetworkSharesWMI(CancellationToken token)
        {
            Dictionary<string, string> _mappeddrives = new Dictionary<string, string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MappedLogicalDisk");

            foreach (ManagementObject queryObj in searcher.Get())
            {                

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                if(queryObj["Name"] != null && queryObj["ProviderName"] !=null)
                {
                    OnAtomicStateChange?.Invoke(this, new StringEventArgs($"Capturing : {queryObj["Name"].ToString()} {queryObj["ProviderName"].ToString()}"));

                    _mappeddrives.Add(queryObj["Name"].ToString(), queryObj["ProviderName"].ToString());
                }

                
            }
            return _mappeddrives;
        }
        
        
        public List<Key> GetNetworkShares(string _regpath, CancellationToken token)
        {
            List<Key> NetworkShares = new List<Key>();

            NetworkShares = GetSubkeysValue(_regpath, RegistryHive.Users, token);

            foreach (var share in NetworkShares)
            {
                string _driveletter = share.KeyName;
                foreach (var value in share.Values)
                {
                    if (value.Key.Equals("RemotePath"))
                    {
                        OnAtomicStateChange?.Invoke(this,new StringEventArgs($"Capturing : {_driveletter}  {value.Value.ToString()}"));                        
                    }
                }

            }

            return NetworkShares;
        }
        
        public void SetNetworkShares(OperationModel _opModel,CancellationToken token)
        {
            Dictionary<string,string> networkShares = new Dictionary<string, string>();

            if(_opModel.NetworkSharesWMI != null)
            {
                if(_opModel.NetworkSharesWMI.Count > 0)
                {
                    foreach (var share in _opModel.NetworkSharesWMI)
                    {
                        networkShares.Add(share.Key, share.Value);
                    }
                }
                
            }

            if (_opModel.NetworkShares != null)
            {
                if (_opModel.NetworkShares.Count > 0)
                {
                    foreach (var share in _opModel.NetworkShares)
                    { 
                        string _driveletter = share.KeyName;
                        foreach(var value in share.Values)
                        {                           
                           if(value.Key.Equals("RemotePath"))
                            {
                                networkShares.Add(_driveletter, value.Value.ToString());
                            }
                        }
                       
                    }
                }

            }

            if (networkShares.Count > 0)
            {
                INetworkDriveAPI oNetworkDrive = new NetworkDriveAPI();                
                
                oNetworkDrive.OnAtomicStateChange += ONetworkDrive_OnAtomicStateChange;
                foreach (var networkShare in networkShares)
                {
                    //ADD MAP DRIVE LOGIC
                    try
                    {   
                        oNetworkDrive.Map(networkShare.Key, networkShare.Value, true, token);
                    }

                    catch(Exception)
                    {
                        throw;
                    }
                    
                }
            }

        }                

        private void ONetworkDrive_OnAtomicStateChange(object sender, StringEventArgs e)
        {
            OnStateChange?.Invoke(this,e);
            //await Task.Factory.FromAsync((asyncCallback, @object) => OnStateChange?.BeginInvoke(null, e, asyncCallback, @object), OnStateChange.EndInvoke, null);
        }
    }
}
