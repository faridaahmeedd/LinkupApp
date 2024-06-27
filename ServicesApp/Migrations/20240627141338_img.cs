using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class img : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Requests");

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Img = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Requests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "26d325a4-06b1-47ec-8816-4780f987ee4e", "AQAAAAIAAYagAAAAEKVEMmLB1xF38FCuRwD/pPXCbB+oVoyCBpJsM0wLtxoUtkjXdYcmbUyaPFsuYK/wew==", "2f210216-ffbd-459e-aeda-c0230cdb1950" });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ServiceRequestId",
                table: "Image",
                column: "ServiceRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Requests",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ebd7219a-966b-4555-a505-a8e8a3d07885", "AQAAAAIAAYagAAAAELYSFwZzhhnrUV8uOWT6pys5KBqmo9wq27kNXIwDM2x1d1jn4JlFXr2sTamb9Q0cxA==", "4244dcc9-ff8d-4642-ab43-adf41535613d" });
        }
    }
}
