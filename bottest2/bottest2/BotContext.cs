using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace bottest2
{
    public partial class BotContext : DbContext
    {
        public BotContext()
        {
        }

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Req> Reqs { get; set; }
        public virtual DbSet<Tarif> Tarifs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserTarif> UserTarifs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=C:\\test2\\Bot.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Req>(entity =>
            {
                entity.HasKey(e => e.RId);

                entity.ToTable("Req");

                entity.HasIndex(e => e.RId, "IX_Req_R_ID")
                    .IsUnique();

                entity.Property(e => e.RId).HasColumnName("R_ID");

                entity.Property(e => e.Res).HasColumnName("RES");

                entity.Property(e => e.UId).HasColumnName("U_ID");
            });

            modelBuilder.Entity<Tarif>(entity =>
            {
                entity.ToTable("Tarif");

                entity.HasIndex(e => e.Id, "IX_Tarif_ID")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "IX_Tarif_Name")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Id, "IX_User_ID")
                    .IsUnique();

                entity.HasIndex(e => e.Login, "IX_User_Login")
                    .IsUnique();

                entity.HasIndex(e => e.Password, "IX_User_Password")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChatId).HasColumnName("ChatID");

                entity.Property(e => e.Ip).HasColumnName("IP");

                entity.Property(e => e.Login).IsRequired();

                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<UserTarif>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.ToTable("User_Tarif");

                entity.HasIndex(e => e.UId, "IX_User_Tarif_U_ID")
                    .IsUnique();

                entity.Property(e => e.UId)
                    .ValueGeneratedNever()
                    .HasColumnName("U_ID");

                entity.Property(e => e.TId).HasColumnName("T_ID");

                entity.HasOne(d => d.TIdNavigation)
                    .WithMany(p => p.UserTarifs)
                    .HasForeignKey(d => d.TId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.UserTarif)
                    .HasForeignKey<UserTarif>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
