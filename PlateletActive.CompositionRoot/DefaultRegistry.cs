using AutoClutch.Auto.Repo.Interfaces;
using AutoClutch.Auto.Repo.Objects;
using AutoClutch.Auto.Service.Interfaces;
using AutoClutch.Auto.Service.Services;
using PlateletActive.Core.Interfaces;
using PlateletActive.Core.Services;
using PlateletActive.Infrastructure.Getters;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.CompositionRoot
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssembliesFromApplicationBaseDirectory();
                    scan.WithDefaultConventions();
                });

            For<DbContext>().Use<PlateletActive.Data.PlateletActiveContext>();

            For(typeof(IService<>)).Use(typeof(Service<>));

            For(typeof(IRepository<>)).Use(typeof(Repository<>));

            For<IHplcDataService>().Use<HplcDataService>();

            For<ILogFileGetter>().Use<LogFileGetter>();
        }
    }
}
