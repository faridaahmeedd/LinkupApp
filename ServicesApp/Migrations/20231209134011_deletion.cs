using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class deletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Requests_ServiceRequestId",
                table: "TimeSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Requests_ServiceRequestId",
                table: "TimeSlots",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Requests_ServiceRequestId",
                table: "TimeSlots");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Requests_ServiceRequestId",
                table: "TimeSlots",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
