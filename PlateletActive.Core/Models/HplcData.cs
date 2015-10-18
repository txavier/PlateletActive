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
        public int HplcDataId { get; set; }

        public int? BatchId { get; set; }

        public string SampleAge { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Timestamp { get; set; }

        public string SampleName { get; set; }

        public double? Dp4 { get; set; }

        public double? Dp3 { get; set; }

        public double? Dp2Maltose { get; set; }

        public double? Dp1Glucose { get; set; }

        public double? LacticAcid { get; set; }

        public double? Glycerol { get; set; }

        public double? AceticAcid { get; set; }

        public double? Ethanol { get; set; }

        public string SampleLocation { get; set; }

        public string User { get; set; }
    }
}
