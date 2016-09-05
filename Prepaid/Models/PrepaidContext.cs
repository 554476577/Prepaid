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
        public virtual DbSet<Alarm> Alarms { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Community> Communities { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<Cutout> Cutouts { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DevicePayLink> DevicePayLinks { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Ladder> Ladders { get; set; }
        public virtual DbSet<Msg> Msgs { get; set; }
        public virtual DbSet<Recharge> Recharges { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<VDevicePay> VDevicePays { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Bill>()
                .Property(e => e.LotNo)
                .IsFixedLength();

            modelBuilder.Entity<Building>()
                .Property(e => e.CommunityID)
                .IsFixedLength();

            modelBuilder.Entity<Community>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Community>()
                .HasMany(e => e.Buildings)
                .WithRequired(e => e.Community)
                .HasForeignKey(e => e.CommunityID);

            modelBuilder.Entity<Credit>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Device>()
                .Property(e => e.TypeID)
                .IsFixedLength();

            modelBuilder.Entity<DeviceType>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<DeviceType>()
                .HasMany(e => e.Devices)
                .WithRequired(e => e.DeviceType)
                .HasForeignKey(e => e.TypeID);

            modelBuilder.Entity<DeviceType>()
                .HasMany(e => e.Ladders)
                .WithRequired(e => e.DeviceType)
                .HasForeignKey(e => e.TypeID);

            modelBuilder.Entity<Ladder>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Ladder>()
                .Property(e => e.TypeID)
                .IsFixedLength();

            modelBuilder.Entity<Msg>()
                .HasMany(e => e.Alarms)
                .WithOptional(e => e.Msg)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Recharge>()
                .Property(e => e.UUID)
                .IsFixedLength();
        }
    }
}
