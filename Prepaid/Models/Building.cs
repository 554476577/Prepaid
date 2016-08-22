namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Building")]
    public partial class  Building
    {
        public Building()
        {
            Rooms = new HashSet<Room>();
        }

        [Key]
        [StringLength(20)]
        public string BuildingNo { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string CommunityID { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int Floors { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Community Community { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
