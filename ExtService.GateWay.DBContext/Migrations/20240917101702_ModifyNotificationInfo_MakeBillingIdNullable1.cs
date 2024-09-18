using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNotificationInfo_MakeBillingIdNullable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo",
                column: "BillingId",
                principalTable: "Billing",
                principalColumn: "BillingId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo",
                column: "BillingId",
                principalTable: "Billing",
                principalColumn: "BillingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
