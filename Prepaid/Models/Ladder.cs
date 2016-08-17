namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ladder")]
    public partial class Ladder
    {
        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(32)]
        public string TypeID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int MinEnergy { get; set; }

        public int MaxEnergy { get; set; }

        public int Price { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual DeviceType DeviceType { get; set; }
    }
}
