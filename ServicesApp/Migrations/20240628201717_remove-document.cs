using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class removedocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                values: new object[] { "24df0f6c-e1ec-4a78-9d55-14e1ec5a8257", "AQAAAAIAAYagAAAAEC2u3+i030pRvZtHgycfIYqzDrQ79QZ26GH3tWhESisY5nnrA/Z/A2LgGp2w6hQE3g==", "cca52069-71d1-4809-a271-7b7fae743b8f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
