namespace PlateletActive.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Core.Models;

    public partial class PlateletActiveContext : DbContext
    {
        public PlateletActiveContext()
            : base("name=PlateletActiveContext")
        {
        }

        public virtual DbSet<HplcData> HplcDatas { get; set; }
        public virtual DbSet<tSite> tSites { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HplcData>()
                .Property(e => e.SampleAge)
                .IsUnicode(false);

            modelBuilder.Entity<HplcData>()
                .Property(e => e.SampleName)
                .IsUnicode(false);

            modelBuilder.Entity<HplcData>()
                .Property(e => e.SampleLocation)
                .IsUnicode(false);
        }
    }
}
