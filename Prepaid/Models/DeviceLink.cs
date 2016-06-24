namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceLink")]
    public partial class DeviceLink
    {
        public DeviceLink()
        {
            EnergyBills = new HashSet<EnergyBill>();
        }

        public int ID { get; set; }

        [StringLength(32)]
        public string UserID { get; set; }

        public int? PointID { get; set; }

        public int? Status { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Point Point { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<EnergyBill> EnergyBills { get; set; }
    }
}
