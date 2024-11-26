using System;
using ExtService.GateWay.DBContext.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExtService.GateWay.DBContext.Migrations
{
    /// <inheritdoc />
    public partial class FunctionsAndTriggersExtensionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderValue",
                table: "MethodHeaders");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<string>(
                name: "HttpMethodName",
                table: "SubMethodInfo",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PluginId",
                table: "MethodHeaders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PluginParameters",
                columns: table => new
                {
                    ParameterId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    ParameterName = table.Column<string>(type: "text", nullable: false),
                    ParameterType = table.Column<string>(type: "text", nullable: false),
                    ParameterValue = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginParameters", x => x.ParameterId);
                });

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    PluginId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PluginName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.PluginId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                columns: table => new
                {
                    TransactionLogId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    TableName = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: false),
                    NewValue = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DBUser = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLog", x => x.TransactionLogId);
                });

            migrationBuilder.CreateTable(
                name: "PluginLinks",
                columns: table => new
                {
                    LinkId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    MethodHeaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PluginId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginLinks", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK_PluginLinks_MethodHeaders_MethodHeaderId",
                        column: x => x.MethodHeaderId,
                        principalTable: "MethodHeaders",
                        principalColumn: "MethodHeaderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PluginLinks_PluginParameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "PluginParameters",
                        principalColumn: "ParameterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PluginLinks_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "PluginId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MethodHeaders_PluginId",
                table: "MethodHeaders",
                column: "PluginId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginLinks_MethodHeaderId",
                table: "PluginLinks",
                column: "MethodHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginLinks_ParameterId",
                table: "PluginLinks",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginLinks_PluginId",
                table: "PluginLinks",
                column: "PluginId");

            migrationBuilder.AddForeignKey(
                name: "FK_MethodHeaders_Plugins_PluginId",
                table: "MethodHeaders",
                column: "PluginId",
                principalTable: "Plugins",
                principalColumn: "PluginId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(PGSQLFunction.LogTransactionsTriggerFunctionCreateCommand);
            migrationBuilder.Sql(PGSQLFunction.EncryptPluginParameterValueTriggerFunctionCreateCommand);
            migrationBuilder.Sql(PGSQLFunction.DecryptPluginParameterValueFunctionCreateCommand);
            migrationBuilder.Sql(PGSQLTriggers.LogTransactionsTriggerForIdentificationTableCreateCommand);
            migrationBuilder.Sql(PGSQLTriggers.EncryptPluginParametersTriggerForPluginParameterTableCreateCommand);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MethodHeaders_Plugins_PluginId",
                table: "MethodHeaders");

            migrationBuilder.DropTable(
                name: "PluginLinks");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropTable(
                name: "PluginParameters");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropIndex(
                name: "IX_MethodHeaders_PluginId",
                table: "MethodHeaders");

            migrationBuilder.DropColumn(
                name: "HttpMethodName",
                table: "SubMethodInfo");

            migrationBuilder.DropColumn(
                name: "PluginId",
                table: "MethodHeaders");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<string>(
                name: "HeaderValue",
                table: "MethodHeaders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(PGSQLTriggers.LogTransactionsTriggerForIdentificationTableDropCommand);
            migrationBuilder.Sql(PGSQLTriggers.EncryptPluginParametersTriggerForPluginParameterTableDropCommand);
            migrationBuilder.Sql(PGSQLFunction.LogTransactionsTriggerFunctionDropCommand);
            migrationBuilder.Sql(PGSQLFunction.EncryptPluginParameterValueTriggerFunctionDropCommand);
            migrationBuilder.Sql(PGSQLFunction.DecryptPluginParameterValueFunctionDropCommand);
        }
    }
}
