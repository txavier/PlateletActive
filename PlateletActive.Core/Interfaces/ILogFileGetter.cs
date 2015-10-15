using System.Collections.Generic;
using PlateletActive.Core.Models;

namespace PlateletActive.Core.Interfaces
{
    public interface ILogFileGetter
    {
        IEnumerable<HplcData> GetLogFileData(string path);
        bool IsImporting();
        IEnumerable<string> GetNamesOfFilesImported();
    }
}