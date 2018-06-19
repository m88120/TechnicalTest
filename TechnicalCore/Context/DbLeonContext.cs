using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TechnicalCore.Context
{
    public partial class DbLeonContext : DbContext
    {
        public virtual DbSet<Buckets> Buckets { get; set; }
        public virtual DbSet<ExamDetails> ExamDetails { get; set; }
        public virtual DbSet<ExamQuestionAnswer> ExamQuestionAnswer { get; set; }
        public virtual DbSet<ExamQuestions> ExamQuestions { get; set; }
        public virtual DbSet<Exams> Exams { get; set; }
        public virtual DbSet<ExamSessions> ExamSessions { get; set; }
        public virtual DbSet<ExamStatuses> ExamStatuses { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Sources> Sources { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        ////#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer(@"Server=10.91.0.56;Database=DbLeonCoreTest;user id=sa;password=Impinge250;");
        //            }
        //        }
        public DbLeonContext(DbContextOptions<DbLeonContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buckets>(entity =>
            {
                entity.HasKey(e => e.BucketId);

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ExamDetails>(entity =>
            {
                entity.Property(e => e.Createdate).HasColumnType("date");

                entity.Property(e => e.Enddate).HasColumnType("date");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Startdate).HasColumnType("date");

                entity.HasOne(d => d.ExamSesson)
                    .WithMany(p => p.ExamDetails)
                    .HasForeignKey(d => d.ExamSessonId)
                    .HasConstraintName("FK_tb_ExamDetails_tb_ExamSession");

                entity.HasOne(d => d.ExamStatus)
                    .WithMany(p => p.ExamDetails)
                    .HasForeignKey(d => d.ExamStatusId)
                    .HasConstraintName("FK_ExamStatusId_ExamStatuses");
            });

            modelBuilder.Entity<ExamQuestionAnswer>(entity =>
            {
                entity.HasKey(e => e.AnswerId);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.ExamDetail)
                    .WithMany(p => p.ExamQuestionAnswer)
                    .HasForeignKey(d => d.ExamDetailId)
                    .HasConstraintName("FK_ExamQuestionAnswer_ExamQuestionAnswer");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamQuestionAnswer)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_ExamQuestionAnswer_ExamQuestions");
            });

            modelBuilder.Entity<ExamQuestions>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamQuestions)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("FK_ExamQuestions_Exams");
            });

            modelBuilder.Entity<Exams>(entity =>
            {
                entity.HasKey(e => e.TestId);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.TestTitle).HasMaxLength(200);
            });

            modelBuilder.Entity<ExamSessions>(entity =>
            {
                entity.Property(e => e.Createdate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.Firstname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Modifydate).HasColumnType("datetime");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.ExamSessions)
                    .HasForeignKey(d => d.SourceId)
                    .HasConstraintName("FK_SourceId_Sources");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.ExamSessions)
                    .HasForeignKey(d => d.TestId)
                    .HasConstraintName("FK_ExamId_Exams");
            });

            modelBuilder.Entity<ExamStatuses>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.Property(e => e.ItemTitle)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Bucket)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.BucketId)
                    .HasConstraintName("FK_Items_Items");
            });

            modelBuilder.Entity<Sources>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
