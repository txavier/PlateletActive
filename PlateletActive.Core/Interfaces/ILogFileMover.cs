using System.Collections.Generic;

namespace PlateletActive.Core.Interfaces
{
    public interface ILogFileMover
    {
        void MoveFiles(IEnumerable<string> namesOfFilesToMove, string outPath);
    }
}