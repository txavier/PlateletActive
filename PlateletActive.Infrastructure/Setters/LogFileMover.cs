using PlateletActive.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Infrastructure.Setters
{
    public class LogFileMover : ILogFileMover, IDisposable
    {
        public LogFileMover()
        {

        }

        public void MoveFiles(IEnumerable<string> namesOfFilesToMove, string outPath)
        {
            foreach(var oldFilePath in namesOfFilesToMove)
            {
                var fileName = oldFilePath.Split("\\".ToCharArray()).LastOrDefault();

                var newFilePath = outPath + "\\" + fileName;

                if (System.IO.File.Exists(oldFilePath) && !System.IO.File.Exists(newFilePath))
                {
                    System.IO.File.Move(oldFilePath, newFilePath);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LogFileMover() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
