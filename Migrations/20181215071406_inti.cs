using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StaticFilesTest.Migrations
{
    public partial class inti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcceptedStudents",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Sname = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: false),
                    Grades = table.Column<string>(nullable: true),
                    TotalGrade = table.Column<float>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    GraduateSchool = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedStudents", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionsOffices",
                columns: table => new
                {
                    Aname = table.Column<string>(nullable: false),
                    Apassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionsOffices", x => x.Aname);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Bname = table.Column<string>(nullable: false),
                    ApplicationBeginTime = table.Column<DateTime>(nullable: false),
                    ApplicationEndTime = table.Column<DateTime>(nullable: false),
                    EnrollmentBeginTime = table.Column<DateTime>(nullable: false),
                    EnrollmentEndTime = table.Column<DateTime>(nullable: false),
                    GradeLine = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Bname);
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Mid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Mname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.Mid);
                });

            migrationBuilder.CreateTable(
                name: "StudentsAccounts",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Spassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsAccounts", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "UnacceptedStudents",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Sname = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: false),
                    Grades = table.Column<string>(nullable: true),
                    TotalGrade = table.Column<float>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    GraduateSchool = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnacceptedStudents", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Uname = table.Column<string>(nullable: false),
                    Upassword = table.Column<string>(nullable: true),
                    Enrollment = table.Column<int>(nullable: false),
                    ExpandRate = table.Column<float>(nullable: false),
                    ApprovalStatus = table.Column<bool>(nullable: false),
                    Aname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Uname);
                    table.ForeignKey(
                        name: "FK_Universities_AdmissionsOffices_Aname",
                        column: x => x.Aname,
                        principalTable: "AdmissionsOffices",
                        principalColumn: "Aname",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Uname = table.Column<string>(nullable: false),
                    Mid = table.Column<int>(nullable: false),
                    AdmissionMethod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => new { x.Sid, x.Uname, x.Mid });
                    table.ForeignKey(
                        name: "FK_Admissions_Majors_Mid",
                        column: x => x.Mid,
                        principalTable: "Majors",
                        principalColumn: "Mid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admissions_AcceptedStudents_Sid",
                        column: x => x.Sid,
                        principalTable: "AcceptedStudents",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Admissions_Universities_Uname",
                        column: x => x.Uname,
                        principalTable: "Universities",
                        principalColumn: "Uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Uname = table.Column<string>(nullable: false),
                    Mid = table.Column<int>(nullable: false),
                    No = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => new { x.Sid, x.Uname, x.Mid });
                    table.ForeignKey(
                        name: "FK_Applications_Majors_Mid",
                        column: x => x.Mid,
                        principalTable: "Majors",
                        principalColumn: "Mid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_UnacceptedStudents_Sid",
                        column: x => x.Sid,
                        principalTable: "UnacceptedStudents",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Universities_Uname",
                        column: x => x.Uname,
                        principalTable: "Universities",
                        principalColumn: "Uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollegeEnrollments",
                columns: table => new
                {
                    Uname = table.Column<string>(nullable: false),
                    Mid = table.Column<int>(nullable: false),
                    Bname = table.Column<string>(nullable: false),
                    Menrollment = table.Column<int>(nullable: false),
                    EnrollmentRemaning = table.Column<int>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollegeEnrollments", x => new { x.Uname, x.Mid, x.Bname });
                    table.UniqueConstraint("AK_CollegeEnrollments_Bname_Mid_Uname", x => new { x.Bname, x.Mid, x.Uname });
                    table.ForeignKey(
                        name: "FK_CollegeEnrollments_Batches_Bname",
                        column: x => x.Bname,
                        principalTable: "Batches",
                        principalColumn: "Bname",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollegeEnrollments_Majors_Mid",
                        column: x => x.Mid,
                        principalTable: "Majors",
                        principalColumn: "Mid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollegeEnrollments_Universities_Uname",
                        column: x => x.Uname,
                        principalTable: "Universities",
                        principalColumn: "Uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliverFiles",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Uname = table.Column<string>(nullable: false),
                    Mid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverFiles", x => new { x.Sid, x.Uname, x.Mid });
                    table.UniqueConstraint("AK_DeliverFiles_Mid_Sid_Uname", x => new { x.Mid, x.Sid, x.Uname });
                    table.ForeignKey(
                        name: "FK_DeliverFiles_Majors_Mid",
                        column: x => x.Mid,
                        principalTable: "Majors",
                        principalColumn: "Mid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliverFiles_UnacceptedStudents_Sid",
                        column: x => x.Sid,
                        principalTable: "UnacceptedStudents",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliverFiles_Universities_Uname",
                        column: x => x.Uname,
                        principalTable: "Universities",
                        principalColumn: "Uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentUniversityAdjustments",
                columns: table => new
                {
                    Sid = table.Column<string>(nullable: false),
                    Uname = table.Column<string>(nullable: false),
                    Adjustment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentUniversityAdjustments", x => new { x.Sid, x.Uname });
                    table.ForeignKey(
                        name: "FK_StudentUniversityAdjustments_UnacceptedStudents_Sid",
                        column: x => x.Sid,
                        principalTable: "UnacceptedStudents",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentUniversityAdjustments_Universities_Uname",
                        column: x => x.Uname,
                        principalTable: "Universities",
                        principalColumn: "Uname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_Mid",
                table: "Admissions",
                column: "Mid");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_Sid",
                table: "Admissions",
                column: "Sid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_Uname",
                table: "Admissions",
                column: "Uname");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Mid",
                table: "Applications",
                column: "Mid");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Uname",
                table: "Applications",
                column: "Uname");

            migrationBuilder.CreateIndex(
                name: "IX_CollegeEnrollments_Mid",
                table: "CollegeEnrollments",
                column: "Mid");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverFiles_Uname",
                table: "DeliverFiles",
                column: "Uname");

            migrationBuilder.CreateIndex(
                name: "IX_StudentUniversityAdjustments_Uname",
                table: "StudentUniversityAdjustments",
                column: "Uname");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Aname",
                table: "Universities",
                column: "Aname");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "CollegeEnrollments");

            migrationBuilder.DropTable(
                name: "DeliverFiles");

            migrationBuilder.DropTable(
                name: "StudentsAccounts");

            migrationBuilder.DropTable(
                name: "StudentUniversityAdjustments");

            migrationBuilder.DropTable(
                name: "AcceptedStudents");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropTable(
                name: "UnacceptedStudents");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "AdmissionsOffices");
        }
    }
}
