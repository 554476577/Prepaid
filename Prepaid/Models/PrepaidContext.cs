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
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Alarm> Alarms { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Community> Communities { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<Cutout> Cutouts { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceArchive> DeviceArchives { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Ladder> Ladders { get; set; }
        public virtual DbSet<Msg> Msgs { get; set; }
        public virtual DbSet<Recharge> Recharges { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<VDevDayEp> VDevDayEps { get; set; }
        public virtual DbSet<VDevMonthEp> VDevMonthEps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Admin>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.Admin)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<Log>()
                .Property(e => e.UserID)
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

            modelBuilder.Entity<Recharge>()
                .Property(e => e.UUID)
                .IsFixedLength();

            modelBuilder.Entity<Room>()
                .HasMany(e => e.Devices)
                .WithOptional(e => e.Room)
                .WillCascadeOnDelete();

            modelBuilder.Entity<VDevDayEp>()
                .Property(e => e.DateTime)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<VDevMonthEp>()
                .Property(e => e.DateTime)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}