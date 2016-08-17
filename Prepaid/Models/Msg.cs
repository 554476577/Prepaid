namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Msg")]
    public partial class Msg
    {
        public Msg()
        {
            Alarms = new HashSet<Alarm>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNo { get; set; }

        [StringLength(250)]
        public string Content { get; set; }

        [StringLength(20)]
        public string PostType { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual ICollection<Alarm> Alarms { get; set; }

        public virtual Room Room { get; set; }
    }
}
