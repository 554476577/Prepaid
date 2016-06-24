namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EnergyBill")]
    public partial class EnergyBill
    {
        public int ID { get; set; }

        public int DeviceLinkID { get; set; }

        public double Value { get; set; }

        public int? Money { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual DeviceLink DeviceLink { get; set; }
    }
}
