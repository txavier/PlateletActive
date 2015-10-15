using PlateletActive.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Infrastructure.Setters
{
    public class LogFileMover : ILogFileMover
    {
        public LogFileMover()
        {

        }

        public void MoveFiles(IEnumerable<string> namesOfFilesToMove, string outPath)
        {
            foreach(var oldFilePath in namesOfFilesToMove)
            {
                var fileName = oldFilePath.Split("/".ToCharArray()).LastOrDefault();

                var newFilePath = fileName + "/" + outPath;

                System.IO.File.Move(oldFilePath, newFilePath);
            }
        }
    }
}
