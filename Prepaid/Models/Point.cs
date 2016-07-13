namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Point")]
    public partial class Point
    {
        public Point()
        {
            DeviceLinks = new HashSet<DeviceLink>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string PointID { get; set; }

        [StringLength(32)]
        public string ModuleID { get; set; }

        [Required]
        [StringLength(50)]
        public string Protocol { get; set; }

        public int? AreaID { get; set; }

        [StringLength(20)]
        public string Floor { get; set; }

        public double? TopPos { get; set; }

        public double? LeftPos { get; set; }

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

        [StringLength(50)]
        public string ValueOrigin { get; set; }

        [StringLength(150)]
        public string ValueFunc { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        [StringLength(50)]
        public string MinValue { get; set; }

        [StringLength(50)]
        public string MaxValue { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(20)]
        public string Unit { get; set; }

        public int? Price { get; set; }

        public DateTime? DateTime { get; set; }

        public bool? IsArchive { get; set; }

        public int? ArchiveInterval { get; set; }

        public DateTime? ArchiveTime { get; set; }

        public bool? ArchiveTag { get; set; }

        public int? ParentID { get; set; }

        [StringLength(150)]
        public string Remark1 { get; set; }

        [StringLength(150)]
        public string Remark2 { get; set; }

        [StringLength(150)]
        public string Remark3 { get; set; }

        public virtual ICollection<DeviceLink> DeviceLinks { get; set; }
    }
}
