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
   public class NetworkPrintersOperator : RegistryInterpolator, INetworkPrintersOperator
    {
        public event EventHandler<StringEventArgs> OnStateChange;
        public event EventHandler<StringEventArgs> OnStateError;
        public event EventHandler<StringEventArgs> OnAtomicStateChange;


        public List<Printers> GetMappedPrintersWMI(CancellationToken token)
        {
            List<Printers> _mappedprinters = new List<Printers>();
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Printer");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                if(token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                
                Printers prn = new Printers();
                prn.Name = queryObj.GetPropertyValue("Name").ToString();
                prn.Default = (bool)queryObj.GetPropertyValue("Default");
                prn.Network = (bool)queryObj.GetPropertyValue("Network");
                if(prn.Network)
                {
                    OnAtomicStateChange?.Invoke(this,new StringEventArgs($"Capturing : {prn.Name}"));
                }                
                _mappedprinters.Add(prn);
            }
            return _mappedprinters;
        }

        public List<Printers> GetMappedPrinters(string _regpath, CancellationToken token)
        {
            List<Printers> MappedPrinters = new List<Printers>();

            var _MappedPrinters = GetSubkeysValue(_regpath, RegistryHive.Users, token);
            foreach(var item in _MappedPrinters)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                OnAtomicStateChange?.Invoke(this, new StringEventArgs($"Capturing : {item.KeyName.Replace(@",", @"\")}"));

                MappedPrinters.Add(new Printers()
                {
                    Name = item.KeyName.Replace(@",", @"\"),
                    Network = true,
                    Default = false
                });
            }
            
            return MappedPrinters;
        }

        public void SetMappedPrinters(OperationModel _opModel, CancellationToken token)
        {
            List<Printers> printers = new List<Printers>();

            if (_opModel.NetworkPrinters != null)
            {
                if(_opModel.NetworkPrinters.Count > 0)
                {
                    printers.AddRange(_opModel.NetworkPrinters);
                }
            }
            if (_opModel.NetworkPrintersWMI != null)
            {
                if(_opModel.NetworkPrintersWMI.Count > 0)
                {
                    printers.AddRange(_opModel.NetworkPrintersWMI);
                }
                
            }

            if (printers.Count > 0)
            {
                INetworkPrinterAPI oNetworkPrinter = new NetworkPrinterAPI();
                oNetworkPrinter.OnAtomicStateChange += ONetworkPrinter_OnAtomicStateChange;
                oNetworkPrinter.OnAtomicStateError += ONetworkPrinter_OnAtomicStateError;

                foreach (var printer in printers)
                {                  
                    try
                    {
                        if (printer.Network)
                        {
                            oNetworkPrinter.AddPrinter(printer.Name, token);
                            if (printer.Default)
                            {
                                oNetworkPrinter.SetDefaultPrinter(printer.Name, token);
                            }
                        }
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                                      

                }
            }

        }

        #region EventHandlers

        private void ONetworkPrinter_OnAtomicStateError(object sender, StringEventArgs e)
        {
             OnStateError?.Invoke(null, e);            
        }

        private void ONetworkPrinter_OnAtomicStateChange(object sender, StringEventArgs e)
        {
            OnStateChange?.Invoke(null, e);           
        }
        
        #endregion
    }

}
