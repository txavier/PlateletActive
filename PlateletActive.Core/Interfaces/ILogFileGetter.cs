using System.Collections.Generic;
using PlateletActive.Core.Models;
using System;

namespace PlateletActive.Core.Interfaces
{
    public interface ILogFileGetter : IDisposable
    {
        IEnumerable<HplcData> GetLogFileData(string path);
        bool IsImporting();
        IEnumerable<string> GetNamesOfFilesImported();
    }
}