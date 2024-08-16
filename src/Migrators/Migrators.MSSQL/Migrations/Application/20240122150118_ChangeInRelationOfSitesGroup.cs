using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class ChangeInRelationOfSitesGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var isGroupsSitesTableExists = migrationBuilder.Sql("IF OBJECT_ID('Core.GroupsSites', 'U') IS NOT NULL SELECT 1 ELSE SELECT 0;").ToString();

            // Check if RoutesSites table exists
            var isRoutesSitesTableExists = migrationBuilder.Sql("IF OBJECT_ID('Core.RoutesSites', 'U') IS NOT NULL SELECT 1 ELSE SELECT 0;").ToString();


            if (isGroupsSitesTableExists == "1")
            {
                migrationBuilder.DropTable(
                    name: "GroupsSites",
                    schema: "Core");
            }

            // Drop RoutesSites table if it exists
            if (isRoutesSitesTableExists == "1")
            {
                migrationBuilder.DropTable(
                    name: "RoutesSites",
                    schema: "Core");
            }

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                schema: "Core",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RouteId",
                schema: "Core",
                table: "Sites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sites_GroupId",
                schema: "Core",
                table: "Sites",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_RouteId",
                schema: "Core",
                table: "Sites",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Groups_GroupId",
                schema: "Core",
                table: "Sites",
                column: "GroupId",
                principalSchema: "Core",
                principalTable: "Groups",
                principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Sites_Routes_RouteId",
            //    schema: "Core",
            //    table: "Sites",
            //    column: "RouteId",
            //    principalSchema: "Core",
            //    principalTable: "Routes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Groups_GroupId",
                schema: "Core",
                table: "Sites");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Sites_Routes_RouteId",
            //    schema: "Core",
            //    table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_GroupId",
                schema: "Core",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_RouteId",
                schema: "Core",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "GroupId",
                schema: "Core",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RouteId",
                schema: "Core",
                table: "Sites");

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
    }
}
