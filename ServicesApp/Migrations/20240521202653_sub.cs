using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class sub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Subcategories",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "MinFees",
                table: "Subcategories",
                newName: "MinFeesEn");

            migrationBuilder.RenameColumn(
                name: "MaxFees",
                table: "Subcategories",
                newName: "MinFeesAr");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Subcategories",
                newName: "NameAr");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Subcategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Subcategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxFeesAr",
                table: "Subcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxFeesEn",
                table: "Subcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1d199eb6-147e-4636-875b-c30f4bbc242c", "AQAAAAIAAYagAAAAEE/e57I2293gMkzekmOfIXPnweq8d6e79fn88vVaI2zQ02lS5NqjqgV0arWi4N3naA==", "183810ca-bfb4-4ed3-bf50-841fdac07397" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Subcategories");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Subcategories");

            migrationBuilder.DropColumn(
                name: "MaxFeesAr",
                table: "Subcategories");

            migrationBuilder.DropColumn(
                name: "MaxFeesEn",
                table: "Subcategories");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Subcategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "Subcategories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "MinFeesEn",
                table: "Subcategories",
                newName: "MinFees");

            migrationBuilder.RenameColumn(
                name: "MinFeesAr",
                table: "Subcategories",
                newName: "MaxFees");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "510c74e7-2ab7-4586-9828-a620bae56376", "AQAAAAIAAYagAAAAED34g/GKs6dFEpIuqhOscfVeggS++41JPCD4m//vKlIFeVLXbglu4ePux7QbTrmHow==", "73273db0-b4ce-46de-b2b5-fe6e7d43c89e" });
        }
    }
}
