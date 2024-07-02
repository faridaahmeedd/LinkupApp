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
            migrationBuilder.AddColumn<string>(
                name: "OfficialDocument",
                table: "Provider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e1c0701f-3d59-43dc-9700-b3338cfc8636", "AQAAAAIAAYagAAAAEKPlxKBEjVh6b0Ggvu4T2S/1tnSUKZOmfA61eVcevdea6/VFCDvCnyWfphNF0Upr5A==", "c85fb936-c0c9-4524-ba83-331f5cc90a67" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficialDocument",
                table: "Provider");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9dbcff18-f698-42af-bf23-9e887f7cdcf4", "AQAAAAIAAYagAAAAEJF7yy2f40jyl1zWrjTRp2CHcqfmKdrND3hpCKrIMc72S4VI4Wv/XD7jZ1NNfsftSw==", "e3cc94d6-9c39-4d57-8e6c-54a16aee9127" });
        }
    }
}
