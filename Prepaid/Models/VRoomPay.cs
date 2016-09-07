namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VRoomPay")]
    public partial class VRoomPay
    {
        [Key]
        [StringLength(20)]
        public string RoomNo { get; set; }

        public double? Area { get; set; }

        public double? PreValue { get; set; }

        public double? Value { get; set; }

        public int? Price1 { get; set; }

        public int? Price2 { get; set; }

        public int? Price3 { get; set; }

        public int? Price4 { get; set; }

        public int? Price5 { get; set; }

        public double? TotolArea { get; set; }
    }
}
