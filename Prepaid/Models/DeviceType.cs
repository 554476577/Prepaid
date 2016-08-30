namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceType")]
    public partial class DeviceType
    {
        public DeviceType()
        {
            Devices = new HashSet<Device>();
            Ladders = new HashSet<Ladder>();
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(20)]
        public string Unit { get; set; }

        public int? Price1 { get; set; }

        public int? Price2 { get; set; }

        public int? Price3 { get; set; }

        public int? Price4 { get; set; }

        public int? Price5 { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<Ladder> Ladders { get; set; }
    }
}
