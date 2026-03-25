using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.MSSQL.Identity
{
    /// <inheritdoc />
    public partial class StandardizeTimestampsToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                schema: "identity",
                table: "UserSessions",
                newName: "RevokedOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastActivityAt",
                schema: "identity",
                table: "UserSessions",
                newName: "LastActivityOnUtc");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                schema: "identity",
                table: "UserSessions",
                newName: "ExpiresOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "identity",
                table: "UserSessions",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                schema: "identity",
                table: "Users",
                newName: "RefreshTokenExpiresOnUtc");

            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeDate",
                schema: "identity",
                table: "Users",
                newName: "LastPasswordChangeOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "identity",
                table: "PasswordHistory",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordHistory_UserId_CreatedAt",
                schema: "identity",
                table: "PasswordHistory",
                newName: "IX_PasswordHistory_UserId_CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                schema: "identity",
                table: "Groups",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "identity",
                table: "Groups",
                newName: "LastModifiedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "identity",
                table: "Groups",
                newName: "CreatedOnUtc");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RevokedOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastActivityOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpiresOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RefreshTokenExpiresOnUtc",
                schema: "identity",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastPasswordChangeOnUtc",
                schema: "identity",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ProcessedOnUtc",
                schema: "identity",
                table: "InboxMessages",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ProcessedOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedOnUtc",
                schema: "identity",
                table: "InboxMessages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastPasswordChangeOnUtc",
                schema: "identity",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiresOnUtc",
                schema: "identity",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastActivityOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedOnUtc",
                schema: "identity",
                table: "UserSessions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "Groups",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedOnUtc",
                schema: "identity",
                table: "Groups",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                schema: "identity",
                table: "Groups",
                newName: "ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordHistory_UserId_CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                newName: "IX_PasswordHistory_UserId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeOnUtc",
                schema: "identity",
                table: "Users",
                newName: "LastPasswordChangeDate");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresOnUtc",
                schema: "identity",
                table: "Users",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExpiresOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "LastActivityOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "LastActivityAt");

            migrationBuilder.RenameColumn(
                name: "RevokedOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "RevokedAt");
        }
    }
}
