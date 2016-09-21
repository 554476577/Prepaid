namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceArchive")]
    public partial class DeviceArchive
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        public double? Value { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }

        public virtual Device Device { get; set; }
    }
}
