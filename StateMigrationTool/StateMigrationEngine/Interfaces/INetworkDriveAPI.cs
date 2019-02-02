using StateMigrationBackend.Models;
using System;
using System.Threading;

namespace StateMigrationBackend.StateMigrationEngine.Interfaces
{
    interface INetworkDriveAPI
    {
        event EventHandler<StringEventArgs> OnAtomicStateChange;       
        int Disconnect(string sDriveLetter, bool bForceDisconnect, CancellationToken token);        
        void Map(string sDriveLetter, string sNetworkPath, bool persistant, CancellationToken token);
    }
}