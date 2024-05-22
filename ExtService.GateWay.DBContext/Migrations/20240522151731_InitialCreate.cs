using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MethodInfo",
                columns: table => new
                {
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodInfo", x => x.MethodId);
                });

            migrationBuilder.CreateTable(
                name: "SystemInfo",
                columns: table => new
                {
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemInfo", x => x.SystemId);
                });

            migrationBuilder.CreateTable(
                name: "Identification",
                columns: table => new
                {
                    IdentificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    EnvName = table.Column<string>(type: "text", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identification", x => x.IdentificationId);
                    table.ForeignKey(
                        name: "FK_Identification_SystemInfo_SystemId",
                        column: x => x.SystemId,
                        principalTable: "SystemInfo",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DomainName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInfo_SystemInfo_SystemId",
                        column: x => x.SystemId,
                        principalTable: "SystemInfo",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Billing",
                columns: table => new
                {
                    BillingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestLimit = table.Column<int>(type: "integer", nullable: false),
                    RequestCount = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdentificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billing", x => x.BillingId);
                    table.ForeignKey(
                        name: "FK_Billing_Identification_IdentificationId",
                        column: x => x.IdentificationId,
                        principalTable: "Identification",
                        principalColumn: "IdentificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Billing_MethodInfo_MethodId",
                        column: x => x.MethodId,
                        principalTable: "MethodInfo",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationInfo",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationLimitPercentage = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationInfo", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_NotificationInfo_Billing_BillingId",
                        column: x => x.BillingId,
                        principalTable: "Billing",
                        principalColumn: "BillingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationInfo_SystemInfo_SystemId",
                        column: x => x.SystemId,
                        principalTable: "SystemInfo",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billing_IdentificationId",
                table: "Billing",
                column: "IdentificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Billing_MethodId",
                table: "Billing",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Identification_SystemId",
                table: "Identification",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationInfo_BillingId",
                table: "NotificationInfo",
                column: "BillingId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationInfo_SystemId",
                table: "NotificationInfo",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_SystemId",
                table: "UserInfo",
                column: "SystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationInfo");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "Billing");

            migrationBuilder.DropTable(
                name: "Identification");

            migrationBuilder.DropTable(
                name: "MethodInfo");

            migrationBuilder.DropTable(
                name: "SystemInfo");
        }
    }
}
