using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServicesApp.Migrations
{
    /// <inheritdoc />
    public partial class offer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b01c681-6549-4b27-b72b-b05cac51e5af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1640ad47-cffa-45e9-8093-3a312702fd6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bed3bd23-9e39-400b-8206-7b91f226d3a3");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TimeSlots",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ServiceRequestId",
                table: "Offers",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_ServiceRequestId",
                table: "Offers",
                newName: "IX_Offers_RequestId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6d04e218-c600-489c-b5e0-e216a744f018", "1", "Customer", "Customer" },
                    { "737e2eca-6ca5-4310-aae3-5982ffa83889", "3", "Admin", "Admin" },
                    { "c7c0e226-c7f3-44ba-9898-5f33b09196ac", "2", "Provider", "Provider" }
                });

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

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d04e218-c600-489c-b5e0-e216a744f018");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "737e2eca-6ca5-4310-aae3-5982ffa83889");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7c0e226-c7f3-44ba-9898-5f33b09196ac");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TimeSlots",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Offers",
                newName: "ServiceRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Offers_RequestId",
                table: "Offers",
                newName: "IX_Offers_ServiceRequestId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b01c681-6549-4b27-b72b-b05cac51e5af", "2", "Provider", "Provider" },
                    { "1640ad47-cffa-45e9-8093-3a312702fd6e", "1", "Customer", "Customer" },
                    { "bed3bd23-9e39-400b-8206-7b91f226d3a3", "3", "Admin", "Admin" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Requests_ServiceRequestId",
                table: "Offers",
                column: "ServiceRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
