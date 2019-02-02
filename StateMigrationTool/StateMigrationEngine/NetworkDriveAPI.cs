using System;
using System.Security;
using System.Threading;
using StateMigrationBackend.Models;
using System.Runtime.InteropServices;
using StateMigrationBackend.StateMigrationEngine.Interfaces;

namespace BackupBackend.StateMigrationEngine
{  

    class NetworkDriveAPI : INetworkDriveAPI
    {

        public event EventHandler<StringEventArgs> OnAtomicStateChange;

        
        private enum Flags
        {
            CONNECT_INTERACTIVE = 0x00000008,
            CONNECT_PROMPT = 0x00000010,
            CONNECT_UPDATE_PROFILE = 0x00000001,
            CONNECT_REDIRECT = 0x00000080,
            CONNECT_COMMANDLINE = 0x00000800,
            CONNECT_CMD_SAVECRED = 0x00001000
        }
        
        public void Map(string sDriveLetter, string sNetworkPath, bool persistant, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            int iFlags = 0;

            if (sNetworkPath.Substring(sNetworkPath.Length - 1, 1) == @"\")
            {
                sNetworkPath = sNetworkPath.Substring(0, sNetworkPath.Length - 1);
            }
            SafeNativeMethods.NETRESOURCE oNetworkResource = new SafeNativeMethods.NETRESOURCE()
            {
                oResourceType = SafeNativeMethods.ResourceType.RESOURCETYPE_DISK,
                sLocalName = sDriveLetter + ":",
                sRemoteName = sNetworkPath
            };


            if (persistant) { iFlags += (int)Flags.CONNECT_UPDATE_PROFILE; }

            if (IsDriveMapped(sDriveLetter, token))
            {
                Disconnect(sDriveLetter, true, token);
            }
            SafeNativeMethods.WNetAddConnection2(ref oNetworkResource, null, null, iFlags);

            OnAtomicStateChange?.Invoke(this,new StringEventArgs($"{sDriveLetter} drive mapped to {sNetworkPath}"));
        }

        public int Disconnect(string sDriveLetter, bool bForceDisconnect, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (bForceDisconnect)
            {
                OnAtomicStateChange?.Invoke(this,new StringEventArgs($"{sDriveLetter} drive disconnected"));
                return SafeNativeMethods.WNetCancelConnection2(sDriveLetter + ":", 0, 1);
            }
            else
            {
                OnAtomicStateChange?.Invoke(this,new StringEventArgs($"{sDriveLetter} drive disconnected"));
                return SafeNativeMethods.WNetCancelConnection2(sDriveLetter + ":", 0, 0);
            }
        }

        private bool IsDriveMapped(string sDriveLetter, CancellationToken token)
        {
            string[] DriveList = Environment.GetLogicalDrives();
            for (int i = 0; i < DriveList.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (sDriveLetter + ":\\" == DriveList[i].ToString())
                {
                    return true;
                }
            }
            return false;
        }

    }


    [SuppressUnmanagedCodeSecurity]
     static class SafeNativeMethods
    {
        internal enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }
        internal enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }
        internal enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010
        }
        internal enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NETRESOURCE
        {
            public ResourceScope oResourceScope;
            public ResourceType oResourceType;
            public ResourceDisplayType oDisplayType;
            public ResourceUsage oResourceUsage;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sLocalName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sRemoteName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sComments;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sProvider;

        }

        [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int WNetAddConnection2(ref NETRESOURCE oNetworkResource, string sPassword, string sUserName, int iFlags);

        [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int WNetCancelConnection2(string sLocalName, uint iFlags, int iForce);
    }
    
}


