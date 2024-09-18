using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNotificationInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationInfo_SystemInfo_SystemId",
                table: "NotificationInfo");

            migrationBuilder.RenameColumn(
                name: "SystemId",
                table: "NotificationInfo",
                newName: "BillingId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationInfo_SystemId",
                table: "NotificationInfo",
                newName: "IX_NotificationInfo_BillingId");

            migrationBuilder.AddColumn<string>(
                name: "RecipientList",
                table: "NotificationInfo",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "NotificationInfo",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo",
                column: "BillingId",
                principalTable: "Billing",
                principalColumn: "BillingId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationInfo_Billing_BillingId",
                table: "NotificationInfo");

            migrationBuilder.DropColumn(
                name: "RecipientList",
                table: "NotificationInfo");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "NotificationInfo");

            migrationBuilder.RenameColumn(
                name: "BillingId",
                table: "NotificationInfo",
                newName: "SystemId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationInfo_BillingId",
                table: "NotificationInfo",
                newName: "IX_NotificationInfo_SystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationInfo_SystemInfo_SystemId",
                table: "NotificationInfo",
                column: "SystemId",
                principalTable: "SystemInfo",
                principalColumn: "SystemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
