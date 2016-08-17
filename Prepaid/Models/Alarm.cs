namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Alarm")]
    public partial class Alarm
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string DeviceNo { get; set; }

        [Required]
        [StringLength(250)]
        public string Content { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        public int? Level { get; set; }

        public int? MsgID { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Device Device { get; set; }

        public virtual Msg Msg { get; set; }
    }
}
