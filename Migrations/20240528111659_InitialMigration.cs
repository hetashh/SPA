using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SPA.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Procedures",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "A relaxing full-body massage", "Swedish Massage", 75m },
                    { 2, "A therapeutic massage with hot stones", "Hot Stone Massage", 100m },
                    { 3, "A soothing massage with essential oils", "Aromatherapy", 80m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Balance", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, 1000m, "password123", "john_doe" },
                    { 2, 1000m, "password456", "jane_smith" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingTime", "ProcedureId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2024, 6, 2, 16, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 3, new DateTime(2024, 6, 3, 11, 0, 0, 0, DateTimeKind.Unspecified), 3, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ProcedureId",
                table: "Bookings",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
