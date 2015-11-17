using System;

namespace PlateletActive.Core.Interfaces
{
    public interface IHplcDataService : AutoClutch.Auto.Service.Interfaces.IService<Models.HplcData>, IDisposable
    {
        bool importing { get; }
        void ImportHplcData(string inPath, string outPath = null, int? clientId = null);
        bool IsImporting();
    }
}