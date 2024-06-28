using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class admin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emergency",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Provider",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "419625f5-a958-4122-bd88-8cb01ba9c4c7", "AQAAAAIAAYagAAAAEJkRHP0uYBoos4C3CIobZJGYfpE+sq1y03cjW7ThQYn2AwiQmGaGF0gi1xXHWgBu3w==", "9f034bfd-1b23-410f-8198-ff7aad337096" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emergency",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Provider");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ebd7219a-966b-4555-a505-a8e8a3d07885", "AQAAAAIAAYagAAAAELYSFwZzhhnrUV8uOWT6pys5KBqmo9wq27kNXIwDM2x1d1jn4JlFXr2sTamb9Q0cxA==", "4244dcc9-ff8d-4642-ab43-adf41535613d" });
        }
    }
}
