namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Device")]
    public partial class Device
    {
        public Device()
        {
            Alarms = new HashSet<Alarm>();
            Bills = new HashSet<Bill>();
            Cutouts = new HashSet<Cutout>();
        }

        [Key]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNo { get; set; }

        [Required]
        [StringLength(32)]
        public string TypeID { get; set; }

        [StringLength(50)]
        public string Protocol { get; set; }

        [StringLength(50)]
        public string Scope { get; set; }

        [StringLength(50)]
        public string DeviceName { get; set; }

        [StringLength(150)]
        public string PhyAddr { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemID { get; set; }

        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(150)]
        public string ItemDescription { get; set; }

        public int? Status { get; set; }

        public double? PreValue { get; set; }

        public double? Value { get; set; }

        public double? Rate { get; set; }

        public DateTime? DateTime { get; set; }

        public bool? IsArchive { get; set; }

        public int? ArchiveInterval { get; set; }

        public DateTime? ArchiveTime { get; set; }

        public bool? ArchiveTag { get; set; }

        [StringLength(150)]
        public string Remark1 { get; set; }

        [StringLength(150)]
        public string Remark2 { get; set; }

        [StringLength(150)]
        public string Remark3 { get; set; }

        public virtual ICollection<Alarm> Alarms { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }

        public virtual ICollection<Cutout> Cutouts { get; set; }

        public virtual DeviceType DeviceType { get; set; }

        public virtual Room Room { get; set; }
    }
}
