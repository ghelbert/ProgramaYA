using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgramaYA.Migrations
{
    /// <inheritdoc />
    public partial class suscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Cursos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Cursos");
        }
    }
}
