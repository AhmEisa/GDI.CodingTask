using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiBooking.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CarColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CarModel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistrationExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CarId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Username" },
                values: new object[] { 1L, "$2a$11$2fuGVTwW3JdiCuR/NInAK.BUuPdepLHTGJxyyv6x7Rt3dSWNi3d9e", "user@gdi.com" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CarId",
                table: "Attachments",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarNumber",
                table: "Cars",
                column: "CarNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}
