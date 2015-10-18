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
                var valid = true;

                using (System.IO.StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvHelper.CsvReader(reader);

                    int row = 2;

                    var hplcData = new Core.Models.HplcData();

                    while (csv.Read())
                    {
                        if(!valid)
                        {
                            continue;
                        }

                        string fieldATemp = null;

                        var fieldA = csv.TryGetField<string>(0, out fieldATemp) ? fieldATemp : null;

                        string fieldBTemp = null;

                        var fieldB = csv.TryGetField<string>(1, out fieldBTemp) ? fieldBTemp : null;

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

                            if (sampleNameParts.Count() == 4)
                            {
                                hplcData.BatchId = Int32.TryParse(sampleNameParts.ElementAt(1), out batchId) ? batchId : (int?)null;

                                if (hplcData.BatchId == null)
                                {
                                    valid = false;

                                    continue;
                                }

                                hplcData.SampleLocation = sampleNameParts.First();

                                hplcData.SampleAge = sampleNameParts.ElementAt(2);

                                hplcData.User = sampleNameParts.Last();
                            }
                            else if (sampleNameParts.Count() == 3 && Int32.TryParse(sampleNameParts.ElementAt(1), out batchId))
                            {
                                hplcData.BatchId = batchId;

                                if (hplcData.BatchId == null)
                                {
                                    valid = false;

                                    continue;
                                }

                                hplcData.SampleLocation = sampleNameParts.First();

                                hplcData.User = sampleNameParts.Last();
                            }
                            else if (sampleNameParts.Count() == 3)
                            {
                                hplcData.SampleLocation = sampleNameParts.First();

                                hplcData.SampleAge = sampleNameParts.ElementAt(1);

                                hplcData.User = sampleNameParts.Last();
                            }
                            else if (sampleNameParts.Count() == 2)
                            {
                                hplcData.SampleLocation = sampleNameParts.First();

                                hplcData.User = sampleNameParts.Last();
                            }
                            else if (sampleNameParts.Count() == 1)
                            {
                                hplcData.User = sampleNameParts.First();
                            }
                            else
                            {
                                valid = false;

                                continue;
                            }
                        }

                        if (fieldB == "DP4")
                        {
                            double hplcDataDp4Temp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataDp4Temp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Dp4 = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP3")
                        {
                            double hplcDataDp3Temp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataDp3Temp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Dp3 = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP2 MALTOSE")
                        {
                            double hplcDataDp2MaltoseTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataDp2MaltoseTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Dp2Maltose = csv.GetField<double>(5);
                        }

                        if (fieldB == "DP1 GLUCOSE")
                        {
                            double hplcDataDp1GlucoseTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataDp1GlucoseTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Dp1Glucose = csv.GetField<double>(5);
                        }

                        if (fieldB == "LACTIC ACID")
                        {
                            double hplcDataLacticAcidTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataLacticAcidTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.LacticAcid = csv.GetField<double>(5);
                        }

                        if (fieldB == "GLYCEROL")
                        {
                            double hplcDataGlycerolTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataGlycerolTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Glycerol = csv.GetField<double>(5);
                        }

                        if (fieldB == "ACETIC ACID")
                        {
                            double hplcDataAceticAcidTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataAceticAcidTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.AceticAcid = csv.GetField<double>(5);
                        }

                        if (fieldB == "ETHANOL")
                        {
                            double hplcDataEthanolTemp = -1;

                            if (!csv.TryGetField<double>(5, out hplcDataEthanolTemp))
                            {
                                valid = false;

                                continue;
                            }

                            hplcData.Ethanol = csv.GetField<double>(5);
                        }

                        row++;
                    }

                    if (valid)
                    {
                        hplcDatas.Add(hplcData);

                        ((List<string>)filesImported).Add(filePath);
                    }
                }
            }

            importing = false;

            return hplcDatas;
        }
    }
}
