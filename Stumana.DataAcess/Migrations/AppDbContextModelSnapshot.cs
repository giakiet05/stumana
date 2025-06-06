﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stumana.DataAcess.Data;

#nullable disable

namespace Stumana.DataAcess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("Stumana.DataAcess.Models.Asset", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxAge")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxCapacity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinAge")
                        .HasColumnType("INTEGER");

                    b.Property<double>("ScoreToPass")
                        .HasColumnType("REAL");

                    b.Property<string>("YearId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("YearId");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Classroom", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<string>("GradeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("YearId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.HasIndex("YearId");

                    b.ToTable("Classrooms");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Grade", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.SchoolYear", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<int>("EndYear")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartYear")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("SchoolYears");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Score", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<int>("Attempt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StudentAssignmentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectScoreTypeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("StudentAssignmentId");

                    b.HasIndex("SubjectScoreTypeId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.ScoreType", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<double>("Coefficient")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("YearId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("YearId");

                    b.ToTable("ScoreTypes");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Student", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.StudentAssignment", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<int>("Absence")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClassroomId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Conduct")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Semester")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("StudentId");

                    b.ToTable("StudentAssignments");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Subject", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<string>("GradeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("ScoreToPass")
                        .HasColumnType("REAL");

                    b.Property<string>("YearId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.HasIndex("YearId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.SubjectScoreType", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ScoreTypeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ScoreTypeId");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectScoreTypes");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(191)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Asset", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.SchoolYear", "Year")
                        .WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Year");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Classroom", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.Grade", "Grade")
                        .WithMany()
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stumana.DataAcess.Models.SchoolYear", "Year")
                        .WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Score", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.StudentAssignment", "StudentAssignment")
                        .WithMany()
                        .HasForeignKey("StudentAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stumana.DataAcess.Models.SubjectScoreType", "SubjectScoreType")
                        .WithMany()
                        .HasForeignKey("SubjectScoreTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StudentAssignment");

                    b.Navigation("SubjectScoreType");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.ScoreType", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.SchoolYear", "Year")
                        .WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Year");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.StudentAssignment", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.Classroom", "Classroom")
                        .WithMany()
                        .HasForeignKey("ClassroomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stumana.DataAcess.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Classroom");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.Subject", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.Grade", "Grade")
                        .WithMany()
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stumana.DataAcess.Models.SchoolYear", "Year")
                        .WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("Stumana.DataAcess.Models.SubjectScoreType", b =>
                {
                    b.HasOne("Stumana.DataAcess.Models.ScoreType", "ScoreType")
                        .WithMany()
                        .HasForeignKey("ScoreTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stumana.DataAcess.Models.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScoreType");

                    b.Navigation("Subject");
                });
#pragma warning restore 612, 618
        }
    }
}
