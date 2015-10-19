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

            // Run maintenance every n number of minutes.
            maintenanceTimer.Interval = (1000 * 60 * minutes);

            maintenanceTimer.Enabled = true;

            eventLog1.WriteEntry("PlateletActive Service has initiated.");
        }

        /// <summary>
        /// This method runs every time the timer elapses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void maintenanceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Get values from the .config file.  This may be necessary to get these
                // values in case they have changed since the last time the service ran.
                var inPath = ConfigurationManager.AppSettings["inPath"];

                var outPath = ConfigurationManager.AppSettings["outPath"];

                int clientId = -1;

                if (!Int32.TryParse(ConfigurationManager.AppSettings["clientId"], out clientId))
                {
                    eventLog1.WriteEntry("No client Id has been found. Therefore a client Id will not be used for this import.");
                }

                eventLog1.WriteEntry("PlateletActive import has begun.");

                _hplcDataService.ImportHplcData(inPath, outPath, clientId == -1 ? null : (int?)clientId);

                eventLog1.WriteEntry("PlateletActive import has ended.");

                // After this import check to see if the timer has changed in the .config file.
                int minutesTemp = 0;

                if(Int32.TryParse(ConfigurationManager.AppSettings["minutes"], out minutesTemp) 
                    && (minutesTemp * 1000 * 60) != maintenanceTimer.Interval)
                {
                    maintenanceTimer.Interval = (1000 * 60 * minutesTemp);

                    maintenanceTimer.Enabled = true;
                }
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

        /// <summary>
        /// This method gets the stack trace for reporting purposes.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
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
