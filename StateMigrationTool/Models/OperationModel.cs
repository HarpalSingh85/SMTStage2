using StateMigrationBackend.LogEngine;
using System;
using System.Collections.Generic;

namespace StateMigrationBackend.Models
{
    public class OperationModel
    {       
        public string SID { get; set; }
        public Dictionary<string, string> NetworkSharesWMI { get; set; }
        public List<Printers> NetworkPrintersWMI { get; set; }
        public List<Key> NetworkShares { get; set; }
        public List<Printers> NetworkPrinters { get; set; }
        public List<string> CustomPaths { get; set; }

    }

    public class Key
    {
        public string KeyName { get; set; }
        public List<KeyValuePair<string, object>> Values { get; set; }

        public Key()
        {
            Values = new List<KeyValuePair<string, object>>();
        }

    }

    public class StringEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public StringEventArgs(string message)
        {
            this.Message = message;
        }

    }

    public class SettingsModel
    {
        public LogOptions LogOptions { get; set; }
        public LogState LogType { get; set; }
        public int ResourceUtilization { get; set; }
        public bool SharedDevices { get; set; } = false;
        public string BROperationType { get; set; }

    }
}
