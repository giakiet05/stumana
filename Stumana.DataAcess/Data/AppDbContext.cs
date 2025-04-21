using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stumana.DataAcess.Models;
using Microsoft.EntityFrameworkCore;




namespace Stumana.DataAcess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
       
        public DbSet<Score> Scores { get; set; }
        public DbSet<ScoreType> ScoreTypes { get; set; }
      
        public DbSet<SubjectScoreType> SubjectScoreTypes { get; set; }
        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Unique Constraints
         
            // Configure max key length issue
            modelBuilder.Entity<Grade>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<User>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Classroom>().Property(g => g.Id).HasMaxLength(191);          
            modelBuilder.Entity<SchoolYear>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Student>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Subject>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<ScoreType>().Property(g => g.Id).HasMaxLength(191);
            modelBuilder.Entity<Score>().Property(g => g.Id).HasMaxLength(191);         
            modelBuilder.Entity<StudentAssignment>().Property(g => g.Id).HasMaxLength(191);         
            modelBuilder.Entity<SubjectScoreType>().Property(g => g.Id).HasMaxLength(191);


            // Configure date fields
            modelBuilder.Entity<Student>().Property(s => s.Birthday).HasColumnType("datetime");

            base.OnModelCreating(modelBuilder);
        }
    }
}
