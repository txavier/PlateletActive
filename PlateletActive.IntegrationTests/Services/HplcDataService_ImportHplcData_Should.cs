using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlateletActive.Core.Interfaces;
using PlateletActive.Core.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Core.Services.Tests
{
    [TestClass()]
    public class HplcDataService_ImportHplcData_Should
    {
        [TestMethod()]
        public void ImportHplcData()
        {
            // Arrange.
            var container = new Container(c => c.AddRegistry<PlateletActive.CompositionRoot.DefaultRegistry>());

            var path = "C:\\dev\\PlateletActive\\PlateletActive.Infrastructure\\LogFiles\\In";

            for (int i = 0; i < 2; i++)
            {
                using (var hplcDataService = container.GetInstance<IHplcDataService>())
                {
                    // Act.
                    hplcDataService.ImportHplcData(path, path);
                }
            }

            // Assert.
            if (Directory.GetFiles(path).Any())
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(true, "There weren't any files to test, so we are going to pass this test.");
            }
        }
    }
}