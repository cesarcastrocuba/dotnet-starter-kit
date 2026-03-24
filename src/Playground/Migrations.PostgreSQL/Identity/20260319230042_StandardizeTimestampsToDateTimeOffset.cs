using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Identity
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevokedOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "LastActivityOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "LastActivityAt");

            migrationBuilder.RenameColumn(
                name: "ExpiresOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "UserSessions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresOnUtc",
                schema: "identity",
                table: "Users",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "LastPasswordChangeOnUtc",
                schema: "identity",
                table: "Users",
                newName: "LastPasswordChangeDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordHistory_UserId_CreatedOnUtc",
                schema: "identity",
                table: "PasswordHistory",
                newName: "IX_PasswordHistory_UserId_CreatedAt");

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

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "Groups",
                newName: "CreatedAt");
        }
    }
}
