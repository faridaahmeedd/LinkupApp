using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class new1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "047a7b45-89d6-4b9f-9519-e5428abd0195", "AQAAAAIAAYagAAAAEE+qCzFtpsHKq0Z+UJ1HrMDjMU/PsXZYA7AWPZH99Iw72Boa6QwJTii8BgEaruOc8Q==", "2d6a6fb3-f8d2-4489-a7b6-684766ff5c57" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce6b90b6-a94f-4789-a1f4-33de02040167", "AQAAAAIAAYagAAAAEAfhYiLNL0yeq6FXsH5y/Mo2VAvtcfzpUeHY5btZ/uiugqo+1N5EMp7xPSMT5hDvJQ==", "712fcccd-a66c-4413-8d86-04465dce6557" });
        }
    }
}
