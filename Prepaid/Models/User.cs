namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public User()
        {
            DeviceLinks = new HashSet<DeviceLink>();
            Recharges = new HashSet<Recharge>();
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [StringLength(50)]
        public string RealName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(50)]
        public string BuildingNo { get; set; }

        [StringLength(50)]
        public string BuildingName { get; set; }

        public int? FloorCount { get; set; }

        public int? Floor { get; set; }

        [StringLength(50)]
        public string RoomNo { get; set; }

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

        public virtual ICollection<DeviceLink> DeviceLinks { get; set; }

        public virtual ICollection<Recharge> Recharges { get; set; }
    }
}
