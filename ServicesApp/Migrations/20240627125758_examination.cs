using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class examination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExaminationComment",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ebd7219a-966b-4555-a505-a8e8a3d07885", "AQAAAAIAAYagAAAAELYSFwZzhhnrUV8uOWT6pys5KBqmo9wq27kNXIwDM2x1d1jn4JlFXr2sTamb9Q0cxA==", "4244dcc9-ff8d-4642-ab43-adf41535613d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationComment",
                table: "Requests");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ca9080ad-bf39-4283-bd18-589a65d09df4", "AQAAAAIAAYagAAAAEA+i4vBFZFSYhlSR+yL6Arr//LQqXufQKgZ7nZ5kHZUVR8WtBvFwdNNDc75XbMQubg==", "aae82e29-a47d-4433-8232-61145c1e9d24" });
        }
    }
}
