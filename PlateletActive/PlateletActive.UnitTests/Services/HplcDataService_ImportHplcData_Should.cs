using AutoClutch.Auto.Repo.Interfaces;
using AutoClutch.Auto.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlateletActive.Core.Interfaces;
using PlateletActive.Core.Models;
using PlateletActive.Core.Services;
using StructureMap.AutoMocking;
using StructureMap.AutoMocking.Moq;
using System;
using System.Collections.Generic;
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
            var autoMocker = new MoqAutoMocker<HplcDataService>();

            var hplcDataService = autoMocker.ClassUnderTest;

            var hplcData = new HplcData { SampleNumber = "" };

            var hplcDatas = new List<HplcData>() { hplcData };

            var mockHplcDataRepository = Mock.Get(autoMocker.Get<IRepository<HplcData>>())
                .Setup(i => i.Add(It.IsAny<HplcData>(), It.IsAny<bool>()))
                .Returns(hplcData);

            var mockLogFileGetter = Mock.Get(autoMocker.Get<ILogFileGetter>())
                .Setup(i => i.GetLogFileData(It.IsAny<string>()))
                .Returns(hplcDatas);

            // Act.
            hplcDataService.ImportHplcData("C:\\dev\\PlateletActive\\PlateletActive\\PlateletActive.Infrastructure\\LogFiles\\In",
                "C:\\dev\\PlateletActive\\PlateletActive\\PlateletActive.Infrastructure\\LogFiles\\Out");

            // Assert.
            Assert.IsTrue(true);
        }
    }
}