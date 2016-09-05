namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DevicePayLink")]
    public partial class DevicePayLink
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNo { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }

        public virtual Device Device { get; set; }

        public virtual Room Room { get; set; }
    }
}
