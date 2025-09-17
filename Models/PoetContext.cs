using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace POET.Models;

public partial class PoetContext : DbContext
{
    public PoetContext()
    {
    }

    public PoetContext(DbContextOptions<PoetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestHistory> TestHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ClassMaterial> ClassMaterials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A06B5109E0");

            entity.HasIndex(e => e.ClassCode, "UQ__Classes__2ECD4A5514B2E587").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassCode).HasMaxLength(10);
            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__Teacher__29572725");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Levels__09F03C06688A2775");

            entity.HasIndex(e => e.LevelName, "UQ__Levels__9EF3BE7B34DE7E1B").IsUnique();

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(50);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Profiles__1788CC4C125ED555");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Profile)
                .HasForeignKey<Profile>(d => d.UserId)
                .HasConstraintName("FK__Profiles__UserId__4AB81AF0");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8C51CC2D3F");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OptionA).HasMaxLength(255);
            entity.Property(e => e.OptionB).HasMaxLength(255);
            entity.Property(e => e.OptionC).HasMaxLength(255);
            entity.Property(e => e.OptionD).HasMaxLength(255);
            entity.Property(e => e.QuestionText).HasMaxLength(500);
            entity.Property(e => e.TestId).HasColumnName("TestID");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Questions__TestI__38996AB5");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__Tests__8CC33100FDCA2DB3");

            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.TestName).HasMaxLength(100);

            entity.HasOne(d => d.Class).WithMany(p => p.Tests)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Tests__ClassID__33D4B598");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tests__CreatedBy__34C8D9D1");

            entity.HasOne(d => d.Level).WithMany(p => p.Tests)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tests__LevelID__32E0915F");
        });

        modelBuilder.Entity<TestHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__TestHist__4D7B4ABD7990A458");

            entity.HasIndex(e => e.UserId, "IX_TestHistory_UserId");

            entity.Property(e => e.DateTaken).HasColumnType("datetime");
            entity.Property(e => e.TestName).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.TestHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TestHistory_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC79DCB251");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E43A06D264").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(10);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasMany(d => d.ClassesNavigation).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentClass",
                    r => r.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCl__Class__2D27B809"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCl__Stude__2C3393D0"),
                    j =>
                    {
                        j.HasKey("StudentId", "ClassId").HasName("PK__StudentC__2E74B8036F53B0F6");
                        j.ToTable("StudentClasses");
                        j.IndexerProperty<int>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<int>("ClassId").HasColumnName("ClassID");
                    });
        });

        modelBuilder.Entity<Question>()
        .Ignore(q => q.UserAnswer);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
