using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.MSSQL.Identity
{
    /// <inheritdoc />
    public partial class IdentityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CorrelationId",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                schema: "identity",
                table: "InboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedOnUtc_ProcessedOnUtc_IsDead",
                schema: "identity",
                table: "OutboxMessages",
                columns: new[] { "CreatedOnUtc", "ProcessedOnUtc", "IsDead" },
                filter: "[ProcessedOnUtc] IS NULL AND [IsDead] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_Id_HandlerName_TenantId",
                schema: "identity",
                table: "InboxMessages",
                columns: new[] { "Id", "HandlerName", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_CreatedOnUtc_ProcessedOnUtc_IsDead",
                schema: "identity",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboxMessages_Id_HandlerName_TenantId",
                schema: "identity",
                table: "InboxMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "CorrelationId",
                schema: "identity",
                table: "OutboxMessages",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                schema: "identity",
                table: "InboxMessages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages",
                columns: new[] { "Id", "HandlerName", "TenantId" });
        }
    }
}
