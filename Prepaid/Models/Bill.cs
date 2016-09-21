namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bill")]
    public partial class Bill
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [Required]
        [StringLength(32)]
        public string LotNo { get; set; }

        public double PreValue { get; set; }

        public double CurValue { get; set; }

        public int? AccountBalance { get; set; }

        public int Money { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Device Device { get; set; }
    }
}
