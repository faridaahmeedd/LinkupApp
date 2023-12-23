using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "22f5c206-bf79-4b86-ab91-1ceaa450726d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71053395-72f1-45f0-a3d7-f34e925ee677");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a17533b1-713f-4831-9edd-385c344194ce");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04cac13a-8e4e-4437-8ad3-9eba0c8b8212", "1", "Customer", "Customer" },
                    { "c7d1f160-f358-4de8-b9ef-52980de488eb", "3", "Admin", "Admin" },
                    { "e3684746-bcc0-44b9-a3b6-7af8b26a5bef", "2", "Provider", "Provider" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04cac13a-8e4e-4437-8ad3-9eba0c8b8212");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7d1f160-f358-4de8-b9ef-52980de488eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e3684746-bcc0-44b9-a3b6-7af8b26a5bef");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "22f5c206-bf79-4b86-ab91-1ceaa450726d", "1", "Customer", "Customer" },
                    { "71053395-72f1-45f0-a3d7-f34e925ee677", "2", "Provider", "Provider" },
                    { "a17533b1-713f-4831-9edd-385c344194ce", "3", "Admin", "Admin" }
                });
        }
    }
}
