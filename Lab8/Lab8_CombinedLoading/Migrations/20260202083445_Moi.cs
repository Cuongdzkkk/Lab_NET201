using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lab8_CombinedLoading.Migrations
{
    /// <inheritdoc />
    public partial class Moi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Instructor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Grade = table.Column<decimal>(type: "decimal(3,1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Credits", "Instructor", "Title" },
                values: new object[,]
                {
                    { 1, 4, "ThS. Nguyễn Văn A", "Lập trình C#" },
                    { 2, 3, "TS. Trần Thị B", "Cơ sở dữ liệu" },
                    { 3, 4, "ThS. Lê Văn C", "ASP.NET Core" },
                    { 4, 3, "ThS. Lê Văn C", "Entity Framework" },
                    { 5, 3, "ThS. Phạm Văn D", "Phát triển Web" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Class", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "CNTT-K65", "an.nguyen@email.com", "Nguyễn Văn An" },
                    { 2, "CNTT-K65", "binh.tran@email.com", "Trần Thị Bình" },
                    { 3, "CNTT-K66", "cuong.le@email.com", "Lê Hoàng Cường" },
                    { 4, "CNTT-K66", "dung.pham@email.com", "Phạm Thu Dung" },
                    { 5, "CNTT-K67", "duc.hoang@email.com", "Hoàng Minh Đức" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "EnrollmentId", "CourseId", "EnrollmentDate", "Grade", "StudentId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.5m, 1 },
                    { 2, 2, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7.0m, 1 },
                    { 3, 3, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1 },
                    { 4, 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 9.0m, 2 },
                    { 5, 2, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.0m, 2 },
                    { 6, 4, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 9.5m, 2 },
                    { 7, 5, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2 },
                    { 8, 3, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 7.5m, 3 },
                    { 9, 4, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.0m, 3 },
                    { 10, 1, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 6.5m, 4 },
                    { 11, 3, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4 },
                    { 12, 5, new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4 },
                    { 13, 2, new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.5m, 5 },
                    { 14, 5, new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
