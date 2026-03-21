using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Identity
{
    /// <inheritdoc />
    public partial class IdentityTemporalStandardizationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedAt",
                schema: "identity",
                table: "UserGroups",
                newName: "AddedAtOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                schema: "identity",
                table: "RoleClaims",
                newName: "CreatedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedAtOnUtc",
                schema: "identity",
                table: "UserGroups",
                newName: "AddedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "identity",
                table: "RoleClaims",
                newName: "CreatedOn");
        }
    }
}
