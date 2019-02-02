using Microsoft.Win32;
using System;
using System.Collections.Generic;
using StateMigrationBackend.Models;
using System.Threading;

namespace StateMigrationBackend.StateMigrationEngine
{
    public class RegistryInterpolator
    {
        protected internal List<Key> GetSubkeysValue(string path, RegistryHive hive, CancellationToken token)
        {
            List<Key> result = new List<Key>();
            using (var hiveKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
            using (var key = hiveKey.OpenSubKey(path))
            {
                var subkeys = key?.GetSubKeyNames();
                if (subkeys != null)
                {
                    foreach (var subkey in subkeys)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        var values = GetKeyValue(hiveKey, path, subkey);
                        result.Add(values);
                    }
                }

            }
            return result;
        }

        protected internal Key GetKeyValue(RegistryKey hive, string path, string keyName)
        {
            var result = new Key() { KeyName = keyName };
            var subkey = $@"{path}\{keyName}";
            using (RegistryKey key = Registry.Users.OpenSubKey(subkey))
            {
                if (key == null) return null;

                foreach (var valueName in key.GetValueNames())
                {
                    var val = key.GetValue(valueName);
                    var pair = new KeyValuePair<string, object>(valueName, val);
                    result.Values.Add(pair);
                }
            }

            return result;
        }
    }
}
