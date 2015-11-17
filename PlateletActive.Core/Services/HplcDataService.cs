using AutoClutch.Auto.Repo.Interfaces;
using AutoClutch.Auto.Service.Services;
using PlateletActive.Core.Interfaces;
using PlateletActive.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Core.Services
{
    public class HplcDataService : Service<HplcData>, IHplcDataService
    {
        private ILogFileGetter _logFileGetter;

        private ILogFileMover _logFileMover;

        private IRepository<HplcData> _hplcDataRepository;

        public bool importing { get; private set; }

        //public bool _disposed;

        public HplcDataService(IRepository<HplcData> hplcDataRepository, 
            ILogFileGetter logFileGetter,
            ILogFileMover logFileMover)
            : base(hplcDataRepository)
        {
            _hplcDataRepository = hplcDataRepository;

            _logFileGetter = logFileGetter;

            _logFileMover = logFileMover;
        }

        // a finalizer is not necessary, as it is inherited from
        // the base class

        //public override void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            // free other managed objects that implement
        //            _hplcDataRepository.Dispose();

        //            _logFileGetter.Dispose();

        //            _logFileMover.Dispose();
        //        }

        //        // release any unmanaged objects
        //        // set object references to null

        //        _disposed = true;
        //    }

        //    base.Dispose(disposing);
        //}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _hplcDataRepository.Dispose();

                    _hplcDataRepository = null;

                    _logFileGetter.Dispose();

                    _logFileGetter = null;

                    _logFileMover.Dispose();

                    _logFileMover = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //~HplcDataService()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public new void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        public bool IsImporting()
        {
            return importing;
        }

        public void ImportHplcData(string inPath, string outPath = null, int? clientId = null)
        {
            // If the getter is not already getting files then go ahead with the import.
            if(!_logFileGetter.IsImporting())
            {
                importing = true;

                var hplcDatas = _logFileGetter.GetLogFileData(inPath);

                // Enter clientId in all of these imported hplcData rows if there is a client id.
                if (clientId != null)
                {
                    hplcDatas = hplcDatas.Select(i =>
                    {
                        i.clientId = clientId;
                        return i;
                    }).ToList();
                }

                // Save data to database.
                var holder = AddRange(hplcDatas, lazyLoadingEnabled: false, proxyCreationEnabled: false, autoDetectChangesEnabled: false);

                // Move files to out folder if an outpath is supplied.
                if (outPath != null)
                {
                    _logFileMover.MoveFiles(_logFileGetter.GetNamesOfFilesImported(), outPath);
                }

                importing = false;
            }

        }
    }
}
