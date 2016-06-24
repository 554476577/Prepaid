namespace Prepaid.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PrepaidContext : DbContext
    {
        public PrepaidContext()
            : base("name=PrepaidContext")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<CreditLevel> CreditLevels { get; set; }
        public virtual DbSet<DeviceLink> DeviceLinks { get; set; }
        public virtual DbSet<EnergyBill> EnergyBills { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<Recharge> Recharges { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<CreditLevel>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<DeviceLink>()
                .Property(e => e.UserID)
                .IsFixedLength();

            modelBuilder.Entity<DeviceLink>()
                .HasMany(e => e.EnergyBills)
                .WithRequired(e => e.DeviceLink)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Point>()
                .Property(e => e.ModuleID)
                .IsFixedLength();

            modelBuilder.Entity<Recharge>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Recharge>()
                .Property(e => e.UserID)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .HasMany(e => e.DeviceLinks)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Recharges)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);
        }
    }
}
