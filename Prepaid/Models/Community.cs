namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Community")]
    public partial class Community
    {
        public Community()
        {
            Buildings = new HashSet<Building>();
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        public double? Area { get; set; }

        public int? Capacity { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
    }
}
