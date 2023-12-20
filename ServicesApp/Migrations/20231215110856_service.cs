using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class service : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f206e5f-5373-4c7a-861b-fce1ea1a85f4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55aafdc7-1bb5-42ef-b961-1f63ba299118");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da80f1c8-b3d5-471f-a256-6005273e87fb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "70cd399a-1028-460c-ae13-72bbc5f62773", "2", "Provider", "Provider" },
                    { "a95a6bfd-40bb-4cab-898e-56e233f70615", "3", "Admin", "Admin" },
                    { "d85c45fc-ab18-48bc-9ab9-ba4215899e8d", "1", "Customer", "Customer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70cd399a-1028-460c-ae13-72bbc5f62773");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a95a6bfd-40bb-4cab-898e-56e233f70615");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d85c45fc-ab18-48bc-9ab9-ba4215899e8d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f206e5f-5373-4c7a-861b-fce1ea1a85f4", "2", "Provider", "Provider" },
                    { "55aafdc7-1bb5-42ef-b961-1f63ba299118", "1", "Customer", "Customer" },
                    { "da80f1c8-b3d5-471f-a256-6005273e87fb", "3", "Admin", "Admin" }
                });
        }
    }
}
