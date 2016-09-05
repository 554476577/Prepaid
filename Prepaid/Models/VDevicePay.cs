namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VDevicePay")]
    public partial class VDevicePay
    {
        [Key]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        public double? TotolArea { get; set; }
    }
}
