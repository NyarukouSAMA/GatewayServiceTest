using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "MethodInfo",
                columns: table => new
                {
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    MethodName = table.Column<string>(type: "text", nullable: false),
                    MethodPath = table.Column<string>(type: "text", nullable: false),
                    ApiBaseUri = table.Column<string>(type: "text", nullable: false),
                    ApiPrefix = table.Column<string>(type: "text", nullable: false),
                    ApiTimeout = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodInfo", x => x.MethodId);
                });

            migrationBuilder.CreateTable(
                name: "SystemInfo",
                columns: table => new
                {
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    SystemName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemInfo", x => x.SystemId);
                });

            migrationBuilder.CreateTable(
                name: "MethodHeaders",
                columns: table => new
                {
                    MethodHeaderId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    HeaderName = table.Column<string>(type: "text", nullable: false),
                    HeaderValue = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodHeaders", x => x.MethodHeaderId);
                    table.ForeignKey(
                        name: "FK_MethodHeaders_MethodInfo_MethodId",
                        column: x => x.MethodId,
                        principalTable: "MethodInfo",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubMethodInfo",
                columns: table => new
                {
                    SubMethodId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    SubMethodName = table.Column<string>(type: "text", nullable: false),
                    SubMethodPath = table.Column<string>(type: "text", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMethodInfo", x => x.SubMethodId);
                    table.ForeignKey(
                        name: "FK_SubMethodInfo_MethodInfo_MethodId",
                        column: x => x.MethodId,
                        principalTable: "MethodInfo",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identification",
                columns: table => new
                {
                    IdentificationId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
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
                name: "BillingConfig",
                columns: table => new
                {
                    BillingConfigId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PeriodInDays = table.Column<int>(type: "integer", nullable: false),
                    RequestLimitPerPeriod = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdentificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingConfig", x => x.BillingConfigId);
                    table.CheckConstraint("CK_PeriodInDays_Greater_Than_Zero", "\"PeriodInDays\" > 0");
                    table.CheckConstraint("CK_RequestLimitPerPeriod_Greater_Than_Zero", "\"RequestLimitPerPeriod\" > 0");
                    table.CheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                    table.ForeignKey(
                        name: "FK_BillingConfig_Identification_IdentificationId",
                        column: x => x.IdentificationId,
                        principalTable: "Identification",
                        principalColumn: "IdentificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillingConfig_MethodInfo_MethodId",
                        column: x => x.MethodId,
                        principalTable: "MethodInfo",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Billing",
                columns: table => new
                {
                    BillingId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    RequestLimit = table.Column<int>(type: "integer", nullable: false),
                    RequestCount = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdentificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billing", x => x.BillingId);
                    table.UniqueConstraint("AK_Billing_BillingConfigId_StartDate_EndDate", x => new { x.BillingConfigId, x.StartDate, x.EndDate });
                    table.CheckConstraint("CK_RequestCount_GTE_Zero", "\"RequestCount\" >= 0");
                    table.CheckConstraint("CK_RequestLimit_Greater_Than_Zero", "\"RequestLimit\" > 0");
                    table.CheckConstraint("CK_StartDate_Less_Than_EndDate", "\"StartDate\" < \"EndDate\"");
                    table.ForeignKey(
                        name: "FK_Billing_BillingConfig_BillingConfigId",
                        column: x => x.BillingConfigId,
                        principalTable: "BillingConfig",
                        principalColumn: "BillingConfigId",
                        onDelete: ReferentialAction.Cascade);
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
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    NotificationLimitPercentage = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationInfo", x => x.NotificationId);
                    table.CheckConstraint("CK_NotificationLimitPercentage_Range", "\"NotificationLimitPercentage\" > 0 AND \"NotificationLimitPercentage\" < 100");
                    table.ForeignKey(
                        name: "FK_NotificationInfo_BillingConfig_BillingConfigId",
                        column: x => x.BillingConfigId,
                        principalTable: "BillingConfig",
                        principalColumn: "BillingConfigId",
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
                name: "IX_BillingConfig_IdentificationId",
                table: "BillingConfig",
                column: "IdentificationId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingConfig_MethodId",
                table: "BillingConfig",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Identification_SystemId",
                table: "Identification",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_MethodHeaders_MethodId",
                table: "MethodHeaders",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationInfo_BillingConfigId",
                table: "NotificationInfo",
                column: "BillingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationInfo_SystemId",
                table: "NotificationInfo",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_SubMethodInfo_MethodId",
                table: "SubMethodInfo",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_SystemId",
                table: "UserInfo",
                column: "SystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billing");

            migrationBuilder.DropTable(
                name: "MethodHeaders");

            migrationBuilder.DropTable(
                name: "NotificationInfo");

            migrationBuilder.DropTable(
                name: "SubMethodInfo");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "BillingConfig");

            migrationBuilder.DropTable(
                name: "Identification");

            migrationBuilder.DropTable(
                name: "MethodInfo");

            migrationBuilder.DropTable(
                name: "SystemInfo");
        }
    }
}
