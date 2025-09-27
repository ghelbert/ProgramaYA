using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgramaYA.Migrations
{
    /// <inheritdoc />
    public partial class suscripcion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Meses = table.Column<string>(type: "TEXT", nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    FechaTermino = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suscripciones_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Suscripciones_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_ApplicationUserId",
                table: "Suscripciones",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_CursoId",
                table: "Suscripciones",
                column: "CursoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suscripciones");
        }
    }
}
