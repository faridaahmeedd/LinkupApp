using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class ServicesApp : Migration
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
                    { "6954697e-edae-4ed4-8166-ada00cbde579", "2", "Provider", "Provider" },
                    { "9a036b88-c347-4ffb-841f-a1f6279dc6e5", "1", "Customer", "Customer" },
                    { "ef6fe85b-e9a2-404e-8a86-bb196935684b", "3", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6954697e-edae-4ed4-8166-ada00cbde579");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a036b88-c347-4ffb-841f-a1f6279dc6e5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef6fe85b-e9a2-404e-8a86-bb196935684b");

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
