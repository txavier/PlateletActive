using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlateletActive.Data;
using PlateletActive.Core.Interfaces;

namespace PlateletActive.Infrastructure.Getters
{
    public class BionLogFileGetter : ILogFileGetter
    {
        public IEnumerable<string> filesImported { get; set; }
        public bool importing { get; private set; }

        public BionLogFileGetter()
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

            var filePaths = System.IO.Directory.GetFiles(path, "*.ESTDConc");

            var hplcDatas = new List<PlateletActive.Core.Models.HplcData>();

            int concentrationColumn = -1;

            foreach (var filePath in filePaths)
            {
                var valid = true;

                using (System.IO.StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvHelper.CsvReader(reader);

                    int row = 2;

                    var hplcData = new Core.Models.HplcData();

                    while (csv.Read())
                    {
                        if (!valid)
                        {
                            continue;
                        }

                        DateTime fieldATemp = new DateTime();

                        var fieldA = csv.TryGetField<DateTime>(0, out fieldATemp) ? fieldATemp : (DateTime?)null;

                        if (fieldA == null)
                        {
                            continue;
                        }

                        hplcData.Timestamp = fieldA;

                        string sampleNameTemp = null;

                        var sampleName = csv.TryGetField<string>(2, out sampleNameTemp) ? sampleNameTemp : null;

                        hplcData.SampleName = sampleName;

                        var sampleNameParts = sampleName.Split(new string[] { "AM", "PM" }, StringSplitOptions.RemoveEmptyEntries);

                        if(sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 1";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 2";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 3";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 4";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 5";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CHECK V"))
                        {
                            hplcData.SampleLocation = "CHECK V";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("F1"))
                        {
                            hplcData.SampleLocation = "F1";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("YP"))
                        {
                            hplcData.SampleLocation = "YP";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("SLURRY2"))
                        {
                            hplcData.SampleLocation = "SLURRY2";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("LIQ"))
                        {
                            hplcData.SampleLocation = "LIQ";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("YPD F1"))
                        {
                            hplcData.SampleLocation = "YPD F1";
                        }

                        var lastSampleNameParts = sampleNameParts.Last().Trim().Split(new string[] { "HRS" }, StringSplitOptions.RemoveEmptyEntries);

                        if(lastSampleNameParts.Count() > 1)
                        {
                            // Get the second to last element and split that by spaces.
                            var secondToLastElement = lastSampleNameParts.ElementAt(lastSampleNameParts.Count() - 1);

                            var secondToLastElementParts = secondToLastElement.Split(" ".ToCharArray());

                            var lastPart = secondToLastElementParts.Last();

                            int sampleAgeTemp = -1;

                            hplcData.SampleAge = Int32.TryParse(lastPart, out sampleAgeTemp) ? sampleAgeTemp.ToString() : null;
                        }

                        // Get the user name


                        //    if (fieldA == "Sample Name")
                        //    {
                        //        hplcData.SampleName = fieldB;

                        //        var sampleNameParts = fieldB.Split('-');

                        //        int batchId = 0;

                        //        if (sampleNameParts.Count() == 4 && Int32.TryParse(sampleNameParts.ElementAt(1), out batchId))
                        //        {
                        //            hplcData.BatchId = batchId;

                        //            if (hplcData.BatchId == null)
                        //            {
                        //                valid = false;

                        //                continue;
                        //            }

                        //            hplcData.SampleLocation = sampleNameParts.First();

                        //            hplcData.SampleAge = sampleNameParts.ElementAt(2);

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 4)
                        //        {
                        //            hplcData.SampleLocation = sampleNameParts.First();

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 3 && Int32.TryParse(sampleNameParts.ElementAt(0), out batchId))
                        //        {
                        //            hplcData.BatchId = batchId;

                        //            if (hplcData.BatchId == null)
                        //            {
                        //                valid = false;

                        //                continue;
                        //            }

                        //            hplcData.SampleLocation = sampleNameParts.ElementAt(1);

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 3 && Int32.TryParse(sampleNameParts.ElementAt(1), out batchId))
                        //        {
                        //            hplcData.BatchId = batchId;

                        //            if (hplcData.BatchId == null)
                        //            {
                        //                valid = false;

                        //                continue;
                        //            }

                        //            hplcData.SampleLocation = sampleNameParts.First();

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 3)
                        //        {
                        //            hplcData.SampleLocation = sampleNameParts.ElementAt(1);

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 2)
                        //        {
                        //            hplcData.SampleLocation = sampleNameParts.First();

                        //            hplcData.User = sampleNameParts.Last();
                        //        }
                        //        else if (sampleNameParts.Count() == 1)
                        //        {
                        //            hplcData.User = sampleNameParts.First();
                        //        }
                        //    }

                        //    // Find concentration column.
                        //    if (fieldA.Trim() == "ID#")
                        //    {
                        //        string concentrationColumnCheck = string.Empty;

                        //        for (int i = 0; i < 10; i++)
                        //        {
                        //            // If this is the concenctration column then break out of the loop and use this column to get
                        //            // the concentrations.
                        //            if (csv.TryGetField<string>(i, out concentrationColumnCheck) && concentrationColumnCheck.Trim() == "Conc.")
                        //            {
                        //                concentrationColumn = i;

                        //                break;
                        //            }
                        //        }

                        //        // If the concentration column was not found then mark this file as invalid.
                        //        if (concentrationColumn == -1)
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }
                        //    }

                        //    // Get concentrations.
                        //    if (fieldB == "DP4")
                        //    {
                        //        double hplcDataDp4Temp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataDp4Temp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Dp4 = hplcDataDp4Temp;
                        //    }

                        //    if (fieldB == "DP3")
                        //    {
                        //        double hplcDataDp3Temp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataDp3Temp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Dp3 = hplcDataDp3Temp;
                        //    }

                        //    if (fieldB == "DP2 MALTOSE")
                        //    {
                        //        double hplcDataDp2MaltoseTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataDp2MaltoseTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Dp2Maltose = hplcDataDp2MaltoseTemp;
                        //    }

                        //    if (fieldB == "DP1 GLUCOSE")
                        //    {
                        //        double hplcDataDp1GlucoseTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataDp1GlucoseTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Dp1Glucose = hplcDataDp1GlucoseTemp;
                        //    }

                        //    if (fieldB == "LACTIC ACID")
                        //    {
                        //        double hplcDataLacticAcidTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataLacticAcidTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.LacticAcid = hplcDataLacticAcidTemp;
                        //    }

                        //    if (fieldB == "GLYCEROL")
                        //    {
                        //        double hplcDataGlycerolTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataGlycerolTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Glycerol = hplcDataGlycerolTemp;
                        //    }

                        //    if (fieldB == "ACETIC ACID")
                        //    {
                        //        double hplcDataAceticAcidTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataAceticAcidTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.AceticAcid = hplcDataAceticAcidTemp;
                        //    }

                        //    if (fieldB == "ETHANOL")
                        //    {
                        //        double hplcDataEthanolTemp = -1;

                        //        if (!csv.TryGetField<double>(concentrationColumn, out hplcDataEthanolTemp))
                        //        {
                        //            valid = false;

                        //            continue;
                        //        }

                        //        hplcData.Ethanol = hplcDataEthanolTemp;
                        //    }

                        //    row++;
                        //}

                        if (valid)
                        {
                            hplcDatas.Add(hplcData);

                            ((List<string>)filesImported).Add(filePath);
                        }
                    }
                }
            }

            importing = false;

            return hplcDatas;
        }
    }
}
