using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.MSSQL.MultiTenancy
{
    /// <inheritdoc />
    public partial class StandardizeTimestampsToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                newName: "StartedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CompletedUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                newName: "CompletedOnUtc");

            migrationBuilder.RenameColumn(
                name: "StartedUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "StartedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CompletedUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "CompletedOnUtc");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ValidUpto",
                schema: "tenant",
                table: "Tenants",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUpto",
                schema: "tenant",
                table: "Tenants",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisioningSteps",
                newName: "CompletedUtc");

            migrationBuilder.RenameColumn(
                name: "StartedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "StartedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "CompletedOnUtc",
                schema: "tenant",
                table: "TenantProvisionings",
                newName: "CompletedUtc");
        }
    }
}
