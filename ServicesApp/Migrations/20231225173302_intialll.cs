using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class intialll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "144cfe0a-d056-4844-99b3-0cd5de43ec7e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fe9bbcd-eb74-448e-9580-1c4bd31f7958");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "831f1147-aa54-4038-ac59-aafc37bbb7f2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7e3d7e17-106e-40ae-8b9d-b2e6f3fa7d02", "3", "Admin", "Admin" },
                    { "956cd045-b346-4359-98e8-0ff4a65590df", "1", "Customer", "Customer" },
                    { "e9860e5d-74ac-4a3a-b383-4198b94ae385", "2", "Provider", "Provider" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e3d7e17-106e-40ae-8b9d-b2e6f3fa7d02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "956cd045-b346-4359-98e8-0ff4a65590df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e9860e5d-74ac-4a3a-b383-4198b94ae385");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "144cfe0a-d056-4844-99b3-0cd5de43ec7e", "1", "Customer", "Customer" },
                    { "5fe9bbcd-eb74-448e-9580-1c4bd31f7958", "2", "Provider", "Provider" },
                    { "831f1147-aa54-4038-ac59-aafc37bbb7f2", "3", "Admin", "Admin" }
                });
        }
    }
}
