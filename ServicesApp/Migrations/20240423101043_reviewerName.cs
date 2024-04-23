using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class reviewerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Requests_requestId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Requests_requestId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "requestId",
                table: "Reviews",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_requestId",
                table: "Reviews",
                newName: "IX_Reviews_RequestId");

            migrationBuilder.RenameColumn(
                name: "requestId",
                table: "Reports",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_requestId",
                table: "Reports",
                newName: "IX_Reports_RequestId");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3774a28-e38b-4631-8011-93d1b4928081", "AQAAAAIAAYagAAAAEGSPlr1P9ptfMpU+071a6smq1R9RSceYHR6Ss91btB/XBPZj2xlpLlqbF/n+l4tQZw==", "f2349ee0-840a-4c4f-b574-4bed83b36f64" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Requests_RequestId",
                table: "Reports",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Requests_RequestId",
                table: "Reviews",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Requests_RequestId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Requests_RequestId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewerName",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Reviews",
                newName: "requestId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RequestId",
                table: "Reviews",
                newName: "IX_Reviews_requestId");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Reports",
                newName: "requestId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_RequestId",
                table: "Reports",
                newName: "IX_Reports_requestId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cec205bd-e853-4dd9-a4d0-f2e850af0187", "AQAAAAIAAYagAAAAEF4bk5VR+SlTddwiQDrNFUD27MbCX3aoK88CJOoawGeq5Qhml0NYEGFN7uoEf37U8g==", "24638ae5-0ca6-4e47-981a-ec9df3291679" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Requests_requestId",
                table: "Reports",
                column: "requestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Requests_requestId",
                table: "Reviews",
                column: "requestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
