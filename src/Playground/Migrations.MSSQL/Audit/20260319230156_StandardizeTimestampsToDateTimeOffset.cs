using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.MSSQL.Audit
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

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReceivedOnUtc",
                schema: "audit",
                table: "AuditRecords",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OccurredOnUtc",
                schema: "audit",
                table: "AuditRecords",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredOnUtc",
                schema: "audit",
                table: "AuditRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedOnUtc",
                schema: "audit",
                table: "AuditRecords",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

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
