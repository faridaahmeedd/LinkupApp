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
            migrationBuilder.AddColumn<bool>(
                name: "Examination",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ca9080ad-bf39-4283-bd18-589a65d09df4", "AQAAAAIAAYagAAAAEA+i4vBFZFSYhlSR+yL6Arr//LQqXufQKgZ7nZ5kHZUVR8WtBvFwdNNDc75XbMQubg==", "aae82e29-a47d-4433-8232-61145c1e9d24" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Examination",
                table: "Offers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24b04480-6c72-4ff9-9c81-bff34a3d385e", "AQAAAAIAAYagAAAAEOXygtoj07BV5+uhY/8eY1FpxJi8uRUy6/8mzbtM0bLGtWTY5M67zqSiY4xR+QOyFw==", "a5612dc6-c8e0-476f-a078-b913e097b9df" });
        }
    }
}
