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

    }
}