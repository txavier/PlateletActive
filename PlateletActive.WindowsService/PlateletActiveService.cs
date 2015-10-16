using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using System.Configuration;
using PlateletActive.Core.Interfaces;

namespace PlateletActive.WindowsService
{
    public partial class PlateletActiveService : ServiceBase
    {
        private static System.Timers.Timer maintenanceTimer;

        private readonly IHplcDataService _hplcDataService;

        public PlateletActiveService()
        {
            InitializeComponent();

            // Initialize the log.
            if (!System.Diagnostics.EventLog.SourceExists("PlateletActiveService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "PlateletActiveService", "");
            }
            eventLog1.Source = "PlateletActiveService";

            eventLog1.Log = "";

            eventLog1.WriteEntry("PlateletActive Service initiating...");

            var container = new StructureMap.Container(c => c.AddRegistry<PlateletActive.CompositionRoot.DefaultRegistry>());

            _hplcDataService = container.GetInstance<IHplcDataService>();

            // Initialize the maintenance timer.
            int minutesTemp = 10;

            var minutes = Int32.TryParse(ConfigurationManager.AppSettings["minutes"], out minutesTemp) ? minutesTemp : 60;

            maintenanceTimer = new System.Timers.Timer(1000 * 60 * minutes);

            maintenanceTimer.Elapsed += new System.Timers.ElapsedEventHandler(maintenanceTimer_Elapsed);

            // Run maintenance every day.
            maintenanceTimer.Interval = (1000 * 60 * minutes);

            maintenanceTimer.Enabled = true;

            eventLog1.WriteEntry("PlateletActive Service has initiated.");
        }

        void maintenanceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var inPath = ConfigurationManager.AppSettings["inPath"];

                var outPath = ConfigurationManager.AppSettings["outPath"];

                eventLog1.WriteEntry("PlateletActive import has begun.");

                //// Get DEP Access Key
                //string DepAccessKey = ConfigurationManager.AppSettings.Get("DepAccessKey");

                //if (DepAccessKey == null)
                //{
                //    throw new Exception("Unable to continue the DEP Access key cannot be found.");
                //}

                _hplcDataService.ImportHplcData(inPath, outPath);

                eventLog1.WriteEntry("PlateletActive import has ended.");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("PlateletActiveService system maintenance has thrown an exception. " + GetStackTrace(ex));
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                int minutesTemp = 0;

                var minutes = Int32.TryParse(ConfigurationManager.AppSettings["minutes"], out minutesTemp) ? minutesTemp : 60;

                maintenanceTimer.Interval = (1000 * 60 * minutes);

                maintenanceTimer.Enabled = true;

                eventLog1.WriteEntry("Platelet Active Service started");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(GetStackTrace(ex));
            }
        }

        public string GetStackTrace(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return ex.ToString() + "--------------------------" +
                    GetStackTrace(ex.InnerException);
            }

            return ex.ToString();
        }

        protected override void OnStop()
        {
            maintenanceTimer.Enabled = false;

            eventLog1.WriteEntry("Platelet Active Service stopped");
        }
    }
}
