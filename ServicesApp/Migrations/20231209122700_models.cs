using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_TimeSlots_TimeSlotid",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ServiceRequestId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_TimeSlotid",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ServiceRequestId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "TimeSlotid",
                table: "Offers",
                newName: "TimeSlotId");

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_RequestId",
                table: "Offers",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Requests_RequestId",
                table: "Offers",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Requests_RequestId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_RequestId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "TimeSlotId",
                table: "Offers",
                newName: "TimeSlotid");

            migrationBuilder.AddColumn<int>(
                name: "ServiceRequestId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ServiceRequestId",
                table: "Offers",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TimeSlotid",
                table: "Offers",
                column: "TimeSlotid");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_TimeSlots_TimeSlotid",
                table: "Offers",
                column: "TimeSlotid",
                principalTable: "TimeSlots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
