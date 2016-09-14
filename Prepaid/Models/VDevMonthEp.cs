namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VDevMonthEp")]
    public partial class VDevMonthEp
    {
        [Key]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [StringLength(7)]
        public string DateTime { get; set; }

        public double? MinValue { get; set; }

        public double? MaxValue { get; set; }

        public double? Value { get; set; }
    }
}
