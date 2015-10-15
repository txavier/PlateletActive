using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlateletActive.Data;
using PlateletActive.Core.Interfaces;

namespace PlateletActive.Infrastructure.Getters
{
    public class LogFileGetter : ILogFileGetter
    {
        public IEnumerable<string> filesImported { get; set; }
        public bool importing { get; private set; }

        public LogFileGetter()
        {
            filesImported = new List<string>();

            importing = false;
        }

        public IEnumerable<string> GetNamesOfFilesImported()
        {
            return filesImported;
        }

        public bool IsImporting()
        {
            return importing;
        }

        public IEnumerable<PlateletActive.Core.Models.HplcData> GetLogFileData(string path)
        {
            importing = true;

            var filePaths = System.IO.Directory.GetFiles(path, "*.csv");

            var hplcDatas = new List<PlateletActive.Core.Models.HplcData>();

            foreach(var filePath in filePaths)
            {
                using (System.IO.StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvHelper.CsvReader(reader);

                    int row = 2;

                    var hplcData = new Core.Models.HplcData();

                    while (csv.Read())
                    {
                        var fieldA = csv.GetField<string>(0);

                        var fieldB = csv.GetField<string>(1);

                        var fieldBDateTime = new DateTime();

                        if(fieldA == "Generated" && DateTime.TryParse(fieldB, out fieldBDateTime))
                        {
                            hplcData.Timestamp = fieldBDateTime;
                        }

                        if(fieldA == "Sample Name")
                        {
                            hplcData.SampleName = fieldB;

                            var sampleNameParts = fieldB.Split('-');

                            int batchId = 0;

                            hplcData.BatchId = Int32.TryParse(sampleNameParts.ElementAt(1), out batchId) ? batchId : (int?)null;

                            hplcData.FermNumber = sampleNameParts.First();

                            hplcData.SampleAge = sampleNameParts.ElementAt(2);
                        }

                        if(fieldB == "DP4")
                        {
                            hplcData.Dp4 = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP3")
                        {
                            hplcData.Dp3 = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP2 MALTOSE")
                        {
                            hplcData.Dp2Maltose = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP1 GLUCOSE")
                        {
                            hplcData.Dp1Glucose = csv.GetField<double>(5);
                        }

                        if (fieldB == "LACTIC ACID")
                        {
                            hplcData.LacticAcid = csv.GetField<double>(5);
                        }

                        if (fieldB == "GLYCEROL")
                        {
                            hplcData.Glycerol = csv.GetField<double>(5);
                        }

                        if (fieldB == "ACETIC ACID")
                        {
                            hplcData.AceticAcid = csv.GetField<double>(5);
                        }

                        if (fieldB == "ETHANOL")
                        {
                            hplcData.Ethanol = csv.GetField<double>(5);
                        }

                        row++;
                    }

                    hplcDatas.Add(hplcData);

                    filesImported.Concat(new List<string> { filePath });
                }
            }

            importing = false;

            return hplcDatas;
        }
    }
}
