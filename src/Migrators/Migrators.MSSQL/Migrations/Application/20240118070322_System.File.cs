using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class SystemFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "Brands",
                schema: "Catalog",
                newName: "Brands",
                newSchema: "System");

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "System");

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
                name: "Brands",
                schema: "System",
                newName: "Brands",
                newSchema: "Catalog");
        }
    }
}
