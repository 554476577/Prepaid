namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log")]
    public partial class Log
    {
        public int ID { get; set; }

        [Required]
        [StringLength(32)]
        public string UserID { get; set; }

        public int? Type { get; set; }

        [StringLength(50)]
        public string ClientAddr { get; set; }

        [StringLength(250)]
        public string Content { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Admin Admin { get; set; }
    }
}
