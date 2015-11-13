using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlateletActive.Data;
using PlateletActive.Core.Interfaces;
using System.Text.RegularExpressions;
using StructureMap.Diagnostics;
using Scratch.ListPermutation;
using PlateletActive.Core.Models;

namespace PlateletActive.Infrastructure.Getters
{
    public class BionLogFileGetter : ILogFileGetter
    {
        public IEnumerable<string> filesImported { get; set; }

        public bool importing { get; private set; }
        public IEnumerable<PlateletActive.Core.Objects.Error> Errors { get; private set; }

        public BionLogFileGetter()
        {
            filesImported = new List<string>();

            importing = false;

            Errors = new List<PlateletActive.Core.Objects.Error>();
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

            int fileIndex = 0;

            foreach (var filePath in filePaths)
            {
                var valid = true;

                using (System.IO.StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvHelper.CsvReader(reader);

                    int rowIndex = 0;

                    List<string> sampleLocations = new List<string>();

                    while (csv.Read())
                    {
                        var hplcData = new Core.Models.HplcData();

                        DateTime fieldATemp = new DateTime();

                        var fieldA = csv.TryGetField<DateTime>(0, out fieldATemp) ? fieldATemp : (DateTime?)null;

                        if (fieldA == null)
                        {
                            if (rowIndex > 2)
                            {
                                string mightBeSampleNameTemp = string.Empty;

                                var mightBeSampleName = csv.TryGetField<string>(2, out mightBeSampleNameTemp) ? mightBeSampleNameTemp : null;

                                ((List<Core.Objects.Error>)Errors).Add(new Core.Objects.Error() { Description = "Unable to import this record. Sample location could not be found.", Property = mightBeSampleNameTemp });
                            }

                            continue;
                        }

                        hplcData.Timestamp = fieldA;

                        string sampleNameTemp = null;

                        var sampleName = csv.TryGetField<string>(2, out sampleNameTemp) ? sampleNameTemp : null;

                        var hplcDataTemp = new Core.Models.HplcData();

                        if (RegexMatchAgeIsFloat(sampleName, ref hplcData))
                        {
                            valid = true;
                        }
                        else if (RegexMatchAgeIsNumber(sampleName, ref hplcData))
                        {
                            valid = true;
                        }
                        else if (RegexMatchAgeIsMissing(sampleName, ref hplcData))
                        {
                            valid = true;
                        }
                        else if (RegexMatchNumberBeforeDateAndNumberAfterUser(sampleName, ref hplcData))
                        {
                            valid = true;
                        }
                        else if (RegexMatchMissingDateHInsteadOfHrs(sampleName, ref hplcData))
                        {
                            valid = true;
                        }
                        else
                        {
                            hplcData.SampleName = sampleName;

                            valid = true;
                        }

                        if (valid)
                        {
                            hplcDatas.Add(hplcData);
                        }

                        rowIndex++;
                    }

                    sampleLocations = sampleLocations.Distinct().ToList();

                    var validSampleLocations = hplcDatas.Select(i => i.SampleLocation).Distinct().ToList();

                    var errorHplcs = hplcDatas.Where(i => !string.IsNullOrEmpty(i.message) && !i.message.ToLower().Contains("user")).ToList();
                }

                ((List<string>)filesImported).Add(filePath);

                fileIndex++;
            }

            importing = false;

            return hplcDatas;
        }

        /// <summary>
        /// F6B B6085 40H 1740 VP
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private static bool RegexMatchMissingDateHInsteadOfHrs(string sampleName, ref HplcData hplcData)
        {
            string re1 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";    // Alphanum 1
            string re2 = ".*?"; // Non-greedy match on filler
            string re3 = "\\d+";    // Uninteresting: int
            string re4 = ".*?"; // Non-greedy match on filler
            string re5 = "(\\d+)";  // Integer Number 1
            string re6 = ".*?"; // Non-greedy match on filler
            string re7 = "((?:[a-z][a-z]+))";   // Word 1

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(sampleName);
            if (m.Success)
            {
                hplcData.SampleLocation = GetLocation(m.Groups[1].ToString());
                hplcData.SampleAge = m.Groups[2].ToString();
                hplcData.User = m.Groups[3].ToString();

                return true;
            }

            return false;
        }

        private static string GetLocation(string v)
        {
            return v;
        }

        /// <summary>
        /// 17/2/2015 11:08:30 AM C1 @ 900 SR 4.87
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private static bool RegexMatchNumberBeforeDateAndNumberAfterUser(string txt, ref HplcData hplcData)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "((?:[0]?[1-9]|[1][012])[-:\\/.](?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))[-:\\/.](?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])";   // MMDDYYYY 1
            string re3 = ".*?"; // Non-greedy match on filler
            string re4 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";    // Alphanum 1
            string re5 = ".*?"; // Non-greedy match on filler
            string re6 = "((?:[a-z][a-z]+))";   // Word 1

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                var timeStampTemp = new DateTime();

                hplcData.Timestamp = DateTime.TryParse(m.Groups[1].ToString(), out timeStampTemp) ? timeStampTemp : (DateTime?)null;
                hplcData.SampleLocation = GetLocation(m.Groups[2].ToString());
                hplcData.User = m.Groups[3].ToString();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 147/1/2015 3:17:27 PM WS @ 1430 DN
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private static bool RegexMatchAgeIsMissing(string sampleName, ref HplcData hplcData)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "((?:[0]?[1-9]|[1][012])[-:\\/.](?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))[-:\\/.](?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])";   // MMDDYYYY 1
            string re3 = ".*?"; // Non-greedy match on filler
            string re4 = "(?:[a-z][a-z]+)"; // Uninteresting: word
            string re5 = ".*?"; // Non-greedy match on filler
            string re6 = "((?:[a-z][a-z]+))";   // Word 1
            string re7 = ".*?"; // Non-greedy match on filler
            string re8 = "((?:[a-z][a-z]+))";   // Word 2

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(sampleName);
            if (m.Success)
            {
                var timeStampTemp = new DateTime();

                hplcData.Timestamp = DateTime.TryParse(m.Groups[1].ToString(), out timeStampTemp) ? timeStampTemp : (DateTime?)null;
                hplcData.SampleLocation = GetLocation(m.Groups[2].ToString());
                hplcData.User = m.Groups[3].ToString();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 207/1/2015 2:14:16 PM F7B B5918 @30HR @1300 JM
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private static bool RegexMatchAgeIsNumber(string sampleName, ref HplcData hplcData)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "((?:[0]?[1-9]|[1][012])[-:\\/.](?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))[-:\\/.](?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])";   // MMDDYYYY 1
            string re3 = ".*?"; // Non-greedy match on filler
            string re4 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";    // Alphanum 1
            string re5 = ".*?"; // Non-greedy match on filler
            string re6 = "(?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))(?![\\d])";    // Uninteresting: day
            string re7 = ".*?"; // Non-greedy match on filler
            string re8 = "((?:(?:[0-2]?\\d{1})|(?:[3][01]{1})))(?![\\d])";  // Day 1
            string re9 = ".*?"; // Non-greedy match on filler
            string re10 = "(?:[a-z][a-z]+)";    // Uninteresting: word
            string re11 = ".*?";    // Non-greedy match on filler
            string re12 = "((?:[a-z][a-z]+))";  // Word 1

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8 + re9 + re10 + re11 + re12, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(sampleName);
            if (m.Success)
            {
                var timeStampTemp = new DateTime();

                hplcData.Timestamp = DateTime.TryParse(m.Groups[1].ToString(), out timeStampTemp) ? timeStampTemp : (DateTime?)null;
                hplcData.SampleLocation = GetLocation(m.Groups[2].ToString());
                hplcData.SampleAge = m.Groups[3].ToString();
                hplcData.User = m.Groups[4].ToString();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 10/10/2015 2:33:06 PM24 F4 B6167 DROP @ 61.20 HRS @ 1416 SS
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private static bool RegexMatchAgeIsFloat(string sampleName, ref HplcData hplcData)
        {
            string re1 = "((?:[0]?[1-9]|[1][012])[-:\\/.](?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))[-:\\/.](?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])";   // MMDDYYYY 1
            string re2 = ".*?"; // Non-greedy match on filler
            string re3 = "(?:[a-z][a-z]*[0-9]+[a-z0-9]*)";  // Uninteresting: alphanum
            string re4 = ".*?"; // Non-greedy match on filler
            string re5 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";    // Alphanum 1
            string re6 = ".*?"; // Non-greedy match on filler
            string re7 = "([+-]?\\d*\\.\\d+)(?![-+0-9\\.])";    // Float 1
            string re8 = ".*?"; // Non-greedy match on filler
            string re9 = "(?:[a-z][a-z]+)"; // Uninteresting: word
            string re10 = ".*?";    // Non-greedy match on filler
            string re11 = "((?:[a-z][a-z]+))";  // Word 1

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8 + re9 + re10 + re11, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(sampleName);
            if (m.Success)
            {
                DateTime timeStampTemp = new DateTime();

                hplcData.Timestamp = DateTime.TryParse(m.Groups[1].ToString(), out timeStampTemp) ? timeStampTemp : (DateTime?)null;
                hplcData.SampleLocation = GetLocation(m.Groups[2].ToString());
                hplcData.SampleAge = m.Groups[3].ToString();
                hplcData.User = m.Groups[4].ToString();

                return true;
            }

            return false;
        }

        public IEnumerable<PlateletActive.Core.Models.HplcData> GetLogFileData1(string path)
        {
            importing = true;

            var filePaths = System.IO.Directory.GetFiles(path, "*.ESTDConc");

            var hplcDatas = new List<PlateletActive.Core.Models.HplcData>();

            int fileIndex = 0;

            foreach (var filePath in filePaths)
            {
                var valid = true;

                using (System.IO.StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    var csv = new CsvHelper.CsvReader(reader);

                    int rowIndex = 0;

                    List<string> sampleLocations = new List<string>();

                    while (csv.Read())
                    {
                        var hplcData = new Core.Models.HplcData();

                        DateTime fieldATemp = new DateTime();

                        var fieldA = csv.TryGetField<DateTime>(0, out fieldATemp) ? fieldATemp : (DateTime?)null;

                        if (fieldA == null)
                        {
                            if (rowIndex > 2)
                            {
                                string mightBeSampleNameTemp = string.Empty;

                                var mightBeSampleName = csv.TryGetField<string>(2, out mightBeSampleNameTemp) ? mightBeSampleNameTemp : null;

                                ((List<Core.Objects.Error>)Errors).Add(new Core.Objects.Error() { Description = "Unable to import this record. Sample location could not be found.", Property = mightBeSampleNameTemp });
                            }

                            continue;
                        }

                        hplcData.Timestamp = fieldA;

                        string sampleNameTemp = null;

                        var sampleName = csv.TryGetField<string>(2, out sampleNameTemp) ? sampleNameTemp : null;

                        sampleName = sampleName.Replace("<V", string.Empty).Trim();

                        hplcData.SampleName = sampleName;

                        var sampleNameParts = sampleName.Split(new string[] { "AM", "PM" }, StringSplitOptions.RemoveEmptyEntries);

                        if (sampleNameParts.Last().Trim().StartsWith("CAL 1"))
                        {
                            hplcData.SampleLocation = "CAL 1";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 2"))
                        {
                            hplcData.SampleLocation = "CAL 2";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 3"))
                        {
                            hplcData.SampleLocation = "CAL 3";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 4"))
                        {
                            hplcData.SampleLocation = "CAL 4";
                        }
                        else if (sampleNameParts.Last().Trim().StartsWith("CAL 5"))
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
                        else if (sampleNameParts.Last().Trim().StartsWith("F7"))
                        {
                            hplcData.SampleLocation = "YPD F1";
                        }
                        else if (Regex.IsMatch(sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First(), @"^[a-zA-Z0-9.]+$") 
                            && !Regex.IsMatch(sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First(), @"^[0-9][a-zA-Z.]+$"))
                        {
                            var unrefinedLocation = sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First();

                            // If this unrefined location matches a known sample location then add it else try to split characters away from it.
                            var sampleLocationCandidate = Regex.IsMatch(unrefinedLocation, @"^[a-zA-Z0-9.]+$") ? unrefinedLocation : Regex.Split(unrefinedLocation, @"^[a-zA-Z0-9.]+$").First();

                            var finalSampleLocationCandidate = sampleLocationCandidate.Contains("@") ? sampleLocationCandidate.Split('@').First() : sampleLocationCandidate;

                            hplcData.SampleLocation = finalSampleLocationCandidate;
                        }
                        else if (Regex.IsMatch(sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First(), @"^[a-zA-Z0-9@]+$")
                            && !Regex.IsMatch(sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First(), @"^[0-9][a-zA-Z.]+$"))

                        {
                            var intermediateSampleLocation = sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First().Split("@".ToCharArray()).First();

                            hplcData.SampleLocation = intermediateSampleLocation;
                        }
                        else if (Regex.IsMatch(sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First(), @"^[a-zA-Z0-9@]+$"))
                        {
                            var intermediateSampleLocation = sampleNameParts.Last().Trim().Split(" ".ToCharArray()).First().Split("@".ToCharArray()).First();

                            // Take away number from string i.e. 0YPD => YPD.
                            var finalLocation = Regex.Split(intermediateSampleLocation, @"[0-9]+").Where(i => !string.IsNullOrEmpty(i)).First();

                            hplcData.SampleLocation = finalLocation;
                        }
                        else
                        {
                            sampleLocations.Add(sampleNameParts.Last().Trim());

                            //valid = false;

                            var error = new Core.Objects.Error() { Description = sampleName + ": Sample location could not be found.", Property = sampleName };

                            ((List<Core.Objects.Error>)Errors).Add(error);

                            hplcData.message = error.Description;

                            //continue;
                        }

                        var lastSampleNameParts = sampleNameParts.Last().Trim().Split(new string[] { "HRS" }, StringSplitOptions.RemoveEmptyEntries);

                        if (lastSampleNameParts.Count() > 1)
                        {
                            // Get the second to last element and split that by spaces.
                            var secondToLastElement = lastSampleNameParts.ElementAt(lastSampleNameParts.Count() - 1);

                            var secondToLastElementParts = secondToLastElement.Split(" ".ToCharArray());

                            var lastPart = secondToLastElementParts.Last();

                            int sampleAgeTemp = -1;

                            hplcData.SampleAge = Int32.TryParse(lastPart, out sampleAgeTemp) ? sampleAgeTemp.ToString() : null;
                        }
                        else
                        {
                            //((List<Core.Objects.Error>)Errors).Add(new Core.Objects.Error() { Description = "Sample age could not be found.", Property = sampleName });
                        }

                        // Get the user name.
                        // If the last part of this string has 2 or 3 letters then assume it is the users initials.
                        var veryLastPart = lastSampleNameParts.Last().Split(" ".ToCharArray()).Last();

                        var secondToVeryLastPart = lastSampleNameParts.Last().Split(" ".ToCharArray()).Count() < 2 ? lastSampleNameParts.Last().Split(" ".ToCharArray()).First() : lastSampleNameParts.Last().Split(" ".ToCharArray()).ElementAt(lastSampleNameParts.Last().Split(" ".ToCharArray()).Count() - 2);

                        if (veryLastPart.Count() > 1
                            && veryLastPart.Count() < 4
                            && Regex.IsMatch(veryLastPart, @"^[a-zA-Z]+$"))
                        {
                            hplcData.User = veryLastPart;
                        }
                        else if (secondToVeryLastPart.Count() > 1
                            && secondToVeryLastPart.Count() < 4
                            && Regex.IsMatch(secondToVeryLastPart, @"^[a-zA-Z]+$"))
                        {
                            hplcData.User = secondToVeryLastPart;
                        }
                        else if (!Regex.IsMatch(veryLastPart, @"^[0-9]+$") 
                            && !Regex.IsMatch(veryLastPart, @"^[0-9@]+$") 
                            && Regex.IsMatch(veryLastPart, @"^[a-zA-Z0-9@]+$") 
                            && Regex.Split(veryLastPart, @"[0-9]+").Count() > 1 
                            && veryLastPart.Contains("@"))
                        {
                            // If this string is not just numbers and we can split it by numbers and get it alone and it contained a @ 
                            // then the user forgot to put a space between the data and his username.
                            hplcData.User = Regex.Split(veryLastPart, @"[0-9]+").Last();
                        }
                        else
                        {
                            //valid = false;

                            var error = new Core.Objects.Error() { Description = sampleName + ": User could not be found.", Property = sampleName };

                            ((List<Core.Objects.Error>)Errors).Add(error);

                            hplcData.message = error.Description;

                            //continue;
                        }

                        if (valid)
                        {
                            hplcDatas.Add(hplcData);
                        }

                        rowIndex++;
                    }

                    sampleLocations = sampleLocations.Distinct().ToList();

                    var validSampleLocations = hplcDatas.Select(i => i.SampleLocation).Distinct().ToList();

                    var errorHplcs = hplcDatas.Where(i => !string.IsNullOrEmpty(i.message) && !i.message.ToLower().Contains("user")).ToList();
                }

                ((List<string>)filesImported).Add(filePath);

                fileIndex++;
            }

            importing = false;

            return hplcDatas;
        }

        
    }
}
