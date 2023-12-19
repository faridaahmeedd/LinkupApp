using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class serviceimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Requests",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "441dd1a2-79ee-4c6b-8cff-6e951fb529e0", "3", "Admin", "Admin" },
                    { "5c27e805-c587-445a-b35b-a04f7f1eb7ce", "1", "Customer", "Customer" },
                    { "f2449a0f-2110-446f-9703-2871851f67bb", "2", "Provider", "Provider" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "441dd1a2-79ee-4c6b-8cff-6e951fb529e0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c27e805-c587-445a-b35b-a04f7f1eb7ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2449a0f-2110-446f-9703-2871851f67bb");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Requests");

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
    }
}
