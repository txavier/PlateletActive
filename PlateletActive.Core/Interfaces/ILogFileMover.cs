using System;
using System.Collections.Generic;

namespace PlateletActive.Core.Interfaces
{
    public interface ILogFileMover : IDisposable
    {
        void MoveFiles(IEnumerable<string> namesOfFilesToMove, string outPath);
    }
}