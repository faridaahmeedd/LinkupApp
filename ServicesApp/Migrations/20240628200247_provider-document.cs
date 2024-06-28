using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class providerdocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Document",
                table: "Provider",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminOfferStatus",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "225fc93c-60c3-4e7f-a5a8-f115057cb644", "AQAAAAIAAYagAAAAEPJlz4JkRkPgX+gk1azf0E1BpfiRNbsxZmaYdSB7mQyxv+Goirz0QEkmZNoCyEuKHg==", "6cf447b1-d66d-4e60-bd1e-0b7ccdbe2689" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "Provider");

            migrationBuilder.AlterColumn<string>(
                name: "AdminOfferStatus",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d1085982-54c3-457f-962a-6896e9c893f5", "AQAAAAIAAYagAAAAEKe6S6e5v6Wi28wme3y9B7ibFexya2JqLsVhB0L3sIlS1tQoVa1u0hgSI05iWuOscQ==", "bc37b3ad-8064-49f1-b9f7-e031268750a2" });
        }
    }
}
