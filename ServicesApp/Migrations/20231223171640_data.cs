using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06385b62-d855-4781-ad16-298ad1627a4f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bfadf93-00b7-4ab6-a8fc-3cdb4d332ff5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "360c94f6-8059-48e4-8a5e-840a2a62e227");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "06385b62-d855-4781-ad16-298ad1627a4f", "3", "Admin", "Admin" },
                    { "1bfadf93-00b7-4ab6-a8fc-3cdb4d332ff5", "1", "Customer", "Customer" },
                    { "360c94f6-8059-48e4-8a5e-840a2a62e227", "2", "Provider", "Provider" }
                });
        }
    }
}
