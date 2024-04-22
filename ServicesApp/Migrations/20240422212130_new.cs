using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "Reports");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cec205bd-e853-4dd9-a4d0-f2e850af0187", "AQAAAAIAAYagAAAAEF4bk5VR+SlTddwiQDrNFUD27MbCX3aoK88CJOoawGeq5Qhml0NYEGFN7uoEf37U8g==", "24638ae5-0ca6-4e47-981a-ec9df3291679" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d4db68af-6c88-4cc4-944d-0abed4f65d57", "AQAAAAIAAYagAAAAEPuN6V0JNtJlBFcE1Mp24zv7Luu9F634c+wWWCw/lMOTXhWolYv7eFdHt/fnvxbNqg==", "22e9cd4c-5470-4753-a0a2-e9895d9aa221" });
        }
    }
}
