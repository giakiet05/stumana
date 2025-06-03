using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stumana.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolYears",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    StartYear = table.Column<int>(type: "INTEGER", nullable: false),
                    EndYear = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoreToPass = table.Column<double>(type: "REAL", nullable: false),
                    YearId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_SchoolYears_YearId",
                        column: x => x.YearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    GradeId = table.Column<string>(type: "TEXT", nullable: false),
                    YearId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classrooms_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classrooms_SchoolYears_YearId",
                        column: x => x.YearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Coefficient = table.Column<double>(type: "REAL", nullable: false),
                    YearId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreTypes_SchoolYears_YearId",
                        column: x => x.YearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ScoreToPass = table.Column<double>(type: "REAL", nullable: false),
                    YearId = table.Column<string>(type: "TEXT", nullable: false),
                    GradeId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_SchoolYears_YearId",
                        column: x => x.YearId,
                        principalTable: "SchoolYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Semester = table.Column<int>(type: "INTEGER", nullable: false),
                    Conduct = table.Column<string>(type: "TEXT", nullable: false),
                    Absence = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    ClassroomId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssignments_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectScoreTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    ScoreTypeId = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectScoreTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectScoreTypes_ScoreTypes_ScoreTypeId",
                        column: x => x.ScoreTypeId,
                        principalTable: "ScoreTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectScoreTypes_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Attempt = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentAssignmentId = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectScoreTypeId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_StudentAssignments_StudentAssignmentId",
                        column: x => x.StudentAssignmentId,
                        principalTable: "StudentAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_SubjectScoreTypes_SubjectScoreTypeId",
                        column: x => x.SubjectScoreTypeId,
                        principalTable: "SubjectScoreTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_YearId",
                table: "Assets",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_GradeId",
                table: "Classrooms",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_YearId",
                table: "Classrooms",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_StudentAssignmentId",
                table: "Scores",
                column: "StudentAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubjectScoreTypeId",
                table: "Scores",
                column: "SubjectScoreTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreTypes_YearId",
                table: "ScoreTypes",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_ClassroomId",
                table: "StudentAssignments",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_StudentId",
                table: "StudentAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_GradeId",
                table: "Subjects",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_YearId",
                table: "Subjects",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectScoreTypes_ScoreTypeId",
                table: "SubjectScoreTypes",
                column: "ScoreTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectScoreTypes_SubjectId",
                table: "SubjectScoreTypes",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "StudentAssignments");

            migrationBuilder.DropTable(
                name: "SubjectScoreTypes");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "ScoreTypes");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "SchoolYears");
        }
    }
}
