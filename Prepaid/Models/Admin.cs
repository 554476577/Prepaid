namespace Prepaid.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Admin")]
    public partial class Admin
    {
        public Admin()
        {
            Logs = new HashSet<Log>();
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(50)]
        public string RealName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
    }
}
