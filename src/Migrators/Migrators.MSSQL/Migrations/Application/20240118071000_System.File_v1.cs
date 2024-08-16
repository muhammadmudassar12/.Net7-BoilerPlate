using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class SystemFile_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "System",
                newName: "Products",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "OtpCodes",
                schema: "System",
                newName: "OtpCodes",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "System",
                newName: "Files",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Brands",
                schema: "System",
                newName: "Brands",
                newSchema: "Catalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Catalog",
                newName: "Products",
                newSchema: "System");

            migrationBuilder.RenameTable(
                name: "OtpCodes",
                schema: "Catalog",
                newName: "OtpCodes",
                newSchema: "System");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "Catalog",
                newName: "Files",
                newSchema: "System");

            migrationBuilder.RenameTable(
                name: "Brands",
                schema: "Catalog",
                newName: "Brands",
                newSchema: "System");
        }
    }
}
