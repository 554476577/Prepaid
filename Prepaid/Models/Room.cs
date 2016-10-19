namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Room")]
    public partial class Room
    {
        public Room()
        {
            Devices = new HashSet<Device>();
            Msgs = new HashSet<Msg>();
            Recharges = new HashSet<Recharge>();
        }

        [Key]
        [StringLength(20)]
        public string RoomNo { get; set; }

        [Required]
        [StringLength(20)]
        public string BuildingNo { get; set; }

        public int Floor { get; set; }

        public double? Area { get; set; }

        public int? Price { get; set; }

        [Required]
        [StringLength(50)]
        public string RealName { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public int? AccountBalance { get; set; }

        public int? AccountWarnLimit { get; set; }

        public int? CreditScore { get; set; }

        [StringLength(50)]
        public string AlipayAccount { get; set; }

        [StringLength(50)]
        public string WechatAccount { get; set; }

        [StringLength(50)]
        public string BankAccount { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Building Building { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<Msg> Msgs { get; set; }

        public virtual ICollection<Recharge> Recharges { get; set; }
    }
}
