using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PlateletActive.Core.Interfaces;

namespace PlateletActive
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(c => c.AddRegistry<PlateletActive.CompositionRoot.DefaultRegistry>());

            if (args.Contains("--Start"))
            {
                var hplcDataService = container.GetInstance<IHplcDataService>();

                System.Console.WriteLine("Time now: " + DateTime.Now.ToString());

                System.Console.WriteLine();

                var inPath = ConfigurationManager.AppSettings["inPath"];

                var outPath = ConfigurationManager.AppSettings["outPath"];

                hplcDataService.ImportHplcData(inPath, outPath);

                System.Console.WriteLine("Time now: " + DateTime.Now.ToString());
            }

            if (!args.Any())
            {
                ShowHelpMenu();
            }
            else
            {
                foreach (var arg in args)
                {
                    System.Console.WriteLine(arg);

                    if (arg.ToLower().Contains("--help")
                        || arg.ToLower().Contains("\\h"))
                    {
                        ShowHelpMenu();
                    }
                }
            }

            System.Console.WriteLine();

            System.Console.WriteLine("Press any key to continue...");

            System.Console.Read();
        }

        private static void ShowHelpMenu()
        {
            System.Console.WriteLine("--Start       This option starts the import process.");
            System.Console.WriteLine("--Help                            This option shows all help information.");
        }

    }
}
