using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class AgregarUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NoHaPagado",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoHaPagado",
                table: "AspNetUsers");
        }
    }
}
