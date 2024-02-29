using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0be3083b-9854-4623-895c-a1f671aaf458", "AQAAAAIAAYagAAAAEFCDTyiCrL4M9C4SUwIqX0JiI9bSbuMDmHcNtY6JjIUJVD2d49yWGvXBD3oezSlCPg==", "b7af208b-2dca-4542-aa70-fe5319ce5507" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5d9c4dc1-9ea5-4f41-b1d6-857f9d2b420a", "AQAAAAIAAYagAAAAEH2WVw40/l6V8VlKWz1nIq5h3OXhBXj/fivYWTi1Srjqdb2sW0qUf/pDl9jKw0v+WQ==", "ef1ec0f1-d2a1-47df-a8f1-d7561f346db7" });
        }
    }
}
