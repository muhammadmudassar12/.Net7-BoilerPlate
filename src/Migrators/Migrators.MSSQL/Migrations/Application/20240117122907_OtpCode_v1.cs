using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class OtpCode_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpCodes",
                schema: "System",
                table: "OtpCodes");

            migrationBuilder.RenameTable(
                name: "OtpCodes",
                schema: "System",
                newName: "Settings",
                newSchema: "System");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                schema: "System",
                table: "Settings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                schema: "System",
                table: "Settings");

            migrationBuilder.RenameTable(
                name: "Settings",
                schema: "System",
                newName: "OtpCodes",
                newSchema: "System");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpCodes",
                schema: "System",
                table: "OtpCodes",
                column: "Id");
        }
    }
}
