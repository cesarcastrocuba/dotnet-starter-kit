using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Multitenancy
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
