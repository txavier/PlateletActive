namespace PlateletActive.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HplcData")]
    public partial class HplcData
    {
        [Key]
        public int HplcDatale { get; set; }

        public int? SiteID { get; set; }

        public int? BatchID { get; set; }

        public string SampleAge { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Timestamp { get; set; }

        public string SampleName { get; set; }

        public double? DP4 { get; set; }

        public double? DP3 { get; set; }

        public double? DP2Maltose { get; set; }

        public double? DP1Glucose { get; set; }

        public double? LacticAcid { get; set; }

        public double? Glycerol { get; set; }

        public double? AceticAcid { get; set; }

        public double? Ethanol { get; set; }

        public string User { get; set; }

        public string SampleLocation { get; set; }

        public virtual tSite tSite { get; set; }
    }
}
