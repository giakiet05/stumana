using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAcess.Models;

namespace Stumana.DataAcess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<ScoreType> ScoreTypes { get; set; }
        public DbSet<ClassroomOffering> ClassroomOfferings { get; set; }
        public DbSet<SubjectOffering> SubjectOfferings { get; set; }
        public DbSet<SubjectScoreType> SubjectScoreTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Unique Constraints
            modelBuilder.Entity<ClassroomOffering>().HasIndex(e => new { e.ClassroomId, e.SchoolYearId }).IsUnique();
            modelBuilder.Entity<SubjectOffering>().HasIndex(e => new { e.SchoolYearId, e.SubjectId }).IsUnique();
            modelBuilder.Entity<SubjectScoreType>().HasIndex(e => new { e.ScoreTypeId, e.SubjectOfferingId }).IsUnique();
            modelBuilder.Entity<StudentAssignment>().HasIndex(e => new { e.StudentId, e.ClassroomOfferingId }).IsUnique();
            modelBuilder.Entity<TeacherAssignment>().HasIndex(e => new { e.TeacherId, e.ClassroomOfferingId, e.SubjectOfferingId, e.Weekday, e.Period }).IsUnique();

            // Configure max key length issue
            modelBuilder.Entity<Grade>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Major>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<User>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Classroom>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Teacher>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<SchoolYear>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Province>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<District>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<School>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Student>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Subject>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<ScoreType>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Score>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<ClassroomOffering>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<SubjectOffering>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<StudentAssignment>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<TeacherAssignment>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<SubjectScoreType>().Property(g => g.Id).HasMaxLength(191);
            

            // Configure date fields
            modelBuilder.Entity<Student>().Property(s => s.Birthday).HasColumnType("datetime");

            base.OnModelCreating(modelBuilder);
        }
    }
}
