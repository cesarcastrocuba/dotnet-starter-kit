using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Identity
{
    /// <inheritdoc />
    public partial class Identity_TenancyStandardization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "UserSessions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "UserGroups",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "PasswordHistory",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "InboxMessages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "Groups",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "GroupRoles",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages",
                columns: new[] { "Id", "HandlerName", "TenantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "identity",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "identity",
                table: "PasswordHistory");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "UserGroups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "InboxMessages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "Groups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "GroupRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InboxMessages",
                schema: "identity",
                table: "InboxMessages",
                columns: new[] { "Id", "HandlerName" });
        }
    }
}
