using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class RoutesSitesGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkerIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Lon = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxCheckinVicinity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QrCode = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    QrCodeStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOperational = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupsSites",
                schema: "Core",
                columns: table => new
                {
                    GroupsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SitesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsSites", x => new { x.GroupsId, x.SitesId });
                    table.ForeignKey(
                        name: "FK_GroupsSites_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalSchema: "Core",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsSites_Sites_SitesId",
                        column: x => x.SitesId,
                        principalSchema: "Core",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutesSites",
                schema: "Core",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SitesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutesSites", x => new { x.RouteId, x.SitesId });
                    table.ForeignKey(
                        name: "FK_RoutesSites_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "Core",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutesSites_Sites_SitesId",
                        column: x => x.SitesId,
                        principalSchema: "Core",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupsSites_SitesId",
                schema: "Core",
                table: "GroupsSites",
                column: "SitesId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutesSites_SitesId",
                schema: "Core",
                table: "RoutesSites",
                column: "SitesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupsSites",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "RoutesSites",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "Core");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpCode",
                schema: "System",
                table: "OtpCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_File",
                schema: "System",
                table: "File");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "System",
                table: "OtpCode");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "System",
                table: "File");

            migrationBuilder.RenameTable(
                name: "OtpCode",
                schema: "System",
                newName: "OtpCodes",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "File",
                schema: "System",
                newName: "Files",
                newSchema: "Catalog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpCodes",
                schema: "Catalog",
                table: "OtpCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                schema: "Catalog",
                table: "Files",
                column: "Id");
        }
    }
}
