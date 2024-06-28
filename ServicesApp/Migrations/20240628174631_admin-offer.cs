using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class adminoffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminOffer",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdminOfferStatus",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d1085982-54c3-457f-962a-6896e9c893f5", "AQAAAAIAAYagAAAAEKe6S6e5v6Wi28wme3y9B7ibFexya2JqLsVhB0L3sIlS1tQoVa1u0hgSI05iWuOscQ==", "bc37b3ad-8064-49f1-b9f7-e031268750a2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOffer",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "AdminOfferStatus",
                table: "Offers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "419625f5-a958-4122-bd88-8cb01ba9c4c7", "AQAAAAIAAYagAAAAEJkRHP0uYBoos4C3CIobZJGYfpE+sq1y03cjW7ThQYn2AwiQmGaGF0gi1xXHWgBu3w==", "9f034bfd-1b23-410f-8198-ff7aad337096" });
        }
    }
}
