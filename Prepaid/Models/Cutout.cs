namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cutout")]
    public partial class Cutout
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [StringLength(250)]
        public string Reason { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Device Device { get; set; }
    }
}
