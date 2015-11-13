using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlateletActive.Core.Interfaces;
using PlateletActive.Infrastructure.Getters;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Infrastructure.Getters.Tests
{
    [TestClass()]
    public class LogFileGetter_GetLogFileData_Should
    {
        [TestMethod()]
        public void GetLogFileData()
        {
            // Arrange.
            var container = new Container(c => c.AddRegistry<PlateletActive.CompositionRoot.DefaultRegistry>());

            var logFileGetter = container.GetInstance<ILogFileGetter>();

            var path = "C:\\dev\\PlateletActive\\PlateletActive.Infrastructure\\LogFiles\\In";

            // Act.
            var result = logFileGetter.GetLogFileData(path);

            // Assert.
            Assert.IsTrue(result.Any());
        }

        [TestMethod()]
        public void RememberFileNames()
        {
            // Arrange.
            var container = new Container(c => c.AddRegistry<PlateletActive.CompositionRoot.DefaultRegistry>());

            var logFileGetter = container.GetInstance<ILogFileGetter>();

            var path = "C:\\dev\\PlateletActive\\PlateletActive.Infrastructure\\LogFiles\\In";

            // Act.
            var result = logFileGetter.GetLogFileData(path);

            // Assert.
            Assert.IsTrue(logFileGetter.GetNamesOfFilesImported().Any());
        }

        //[TestMethod()]
        //public void Should_only_match_alphabetical_once_ignoring_whitespace()
        //{
        //    //var target = new[] { "abc", "abcdefghijk", "abdfkmnpstvxz", "cxy", "cdklstxy",
        //    //    "bfrtw", "a b c", "acg jko pr", "a z", "v  z",
        //    //"a  b cdefg kl", "uv xyz", "ab de gh", "x yz", "abcdefghijklmnopqrstuvwxyz"};
        //    //var dontMatch = new[] { "abbc", "abcb", "a bcdjkrza", "qwerty", "zyxcba",
        //    //    "abcdfe", "ab c dfe", "a  z  a", "asdfg", "asd  f g", "poqwoieruytjhfg" };

        //    var target = new[] { "hey1", "day1" };

        //    var dontMatch = new[] { "1" };

        //    GenerateRegex(target, dontMatch, 30);
        //}

        private static void GenerateRegex(IEnumerable<string> target, IEnumerable<string> dontMatch, int expectedLength)
        {
            string distinctSymbols = new String(target.SelectMany(x => x).Distinct().ToArray());
            string genes = distinctSymbols + "?*()+";

            Func<string, Scratch.GeneticAlgorithm.FitnessResult> calcFitness = str =>
            {
                if (!IsValidRegex(str))
                {
                    return new Scratch.GeneticAlgorithm.FitnessResult
                    {
                        Value = Int32.MaxValue
                    };
                }
                var regex = new System.Text.RegularExpressions.Regex("^" + str + "$");
                uint fitness = target.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 0U : 1));
                uint nonFitness = dontMatch.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 10U : 0));
                return new Scratch.GeneticAlgorithm.FitnessResult
                {
                    Value = fitness + nonFitness
                };
            };

            int targetGeneLength = 1;
            for (;;)
            {
                var best = new Scratch.GeneticAlgorithm.GeneticSolver(50 + 10 * targetGeneLength).GetBestGenetically(targetGeneLength, genes, calcFitness);
                if (calcFitness(best.GetStringGenes()).Value != 0)
                {
                    Console.WriteLine("-- not solved with regex of length " + targetGeneLength);
                    targetGeneLength++;
                    if (targetGeneLength > expectedLength)
                    {
                        Assert.Fail("failed to find a solution within the expected length");
                    }
                    continue;
                }
                Console.WriteLine("solved with: " + best);
                break;
            }
        }

        private static bool HasBalancedParentheses(string str)
        {
            int depth = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ')')
                {
                    depth--;
                    if (depth < 0)
                    {
                        return false;
                    }
                }
                if (str[i] == '(')
                {
                    depth++;
                }
            }
            return (depth == 0);
        }

        private static bool IsValidRegex(string str)
        {
            if (!HasBalancedParentheses(str))
            {
                return false;
            }
            if (str.All(x => "?*+()".Contains(x)))
            {
                return false;
            }
            if (")?*+".Any(x => str.First() == x))
            {
                return false;
            }
            if (str.Last() == ')')
            {
                return false;
            }
            if (str.Contains("()"))
            {
                return false;
            }
            if (str.Contains("(*") || str.Contains("(+") || str.Contains("(?"))
            {
                return false;
            }
            if (str.Contains("?*") || str.Contains("?+") || str.Contains("??") || str.Contains("*?"))
            {
                return false;
            }
            if (str.Contains("++") || str.Contains("**") || str.Contains("+*") || str.Contains("*+"))
            {
                return false;
            }
            try
            {
                new System.Text.RegularExpressions.Regex("^" + str + "$");
            }
            catch (Exception)
            {
                Console.WriteLine("-- bad: " + str);
                return false;
            }
            return true;
        }
    }
}