using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.MSSQL.MultiTenancy
{
    /// <inheritdoc />
    public partial class MultitenancyValidUptoStandardization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidUpto",
                schema: "tenant",
                table: "Tenants",
                newName: "ValidUptoOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidUptoOnUtc",
                schema: "tenant",
                table: "Tenants",
                newName: "ValidUpto");
        }
    }
}
