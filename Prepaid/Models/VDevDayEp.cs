namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VDevDayEp")]
    public partial class VDevDayEp
    {
        [Key]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [StringLength(10)]
        public string DateTime { get; set; }

        public double? MinValue { get; set; }

        public double? MaxValue { get; set; }

        public double? Value { get; set; }
    }
}
