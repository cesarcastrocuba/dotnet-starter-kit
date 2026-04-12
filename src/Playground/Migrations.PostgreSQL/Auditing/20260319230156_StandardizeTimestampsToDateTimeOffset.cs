using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Auditing
{
    /// <inheritdoc />
    public partial class StandardizeTimestampsToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedAtUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "ReceivedOnUtc");

            migrationBuilder.RenameColumn(
                name: "OccurredAtUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "OccurredOnUtc");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_OccurredAtUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "IX_AuditRecords_OccurredOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedOnUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "ReceivedAtUtc");

            migrationBuilder.RenameColumn(
                name: "OccurredOnUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "OccurredAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_OccurredOnUtc",
                schema: "audit",
                table: "AuditRecords",
                newName: "IX_AuditRecords_OccurredAtUtc");
        }
    }
}
