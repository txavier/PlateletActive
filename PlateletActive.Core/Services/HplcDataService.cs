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
    public class HplcDataService : Service<HplcData>
    {
        private readonly ILogFileGetter _logFileGetter;

        private readonly ILogFileMover _logFileMover;

        public bool importing { get; private set; }

        public HplcDataService(IRepository<HplcData> hplcDataRepository, 
            ILogFileGetter logFileGetter,
            ILogFileMover logFileMover)
            : base(hplcDataRepository)
        {
            _logFileGetter = logFileGetter;

            _logFileMover = logFileMover;
        }

        public bool IsImporting()
        {
            return importing;
        }

        public void ImportHplcData(string inPath, string outPath = null)
        {
            // If the getter is not already getting files then go ahead with the import.
            if(!_logFileGetter.IsImporting())
            {
                importing = true;

                var hplcDatas = _logFileGetter.GetLogFileData(inPath);

                // Save data to database.
                AddRange(hplcDatas);

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
