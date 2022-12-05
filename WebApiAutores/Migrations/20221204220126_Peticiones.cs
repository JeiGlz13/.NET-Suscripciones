using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class Peticiones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Peticiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    FechaPeticion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LlaveAPIId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peticiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Peticiones_LlaveAPIs_LlaveAPIId",
                        column: x => x.LlaveAPIId,
                        principalTable: "LlaveAPIs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peticiones_LlaveAPIId",
                table: "Peticiones",
                column: "LlaveAPIId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peticiones");
        }
    }
}
