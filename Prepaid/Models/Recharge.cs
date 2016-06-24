namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Recharge")]
    public partial class Recharge
    {
        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        [StringLength(32)]
        public string UserID { get; set; }

        public int Money { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual User User { get; set; }
    }
}
