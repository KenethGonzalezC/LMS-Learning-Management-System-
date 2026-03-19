using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class worksforclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntregasClases",
                columns: table => new
                {
                    EntregaClaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaseId = table.Column<int>(type: "int", nullable: false),
                    EstudianteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagenPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentarioProfesor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasClases", x => x.EntregaClaseId);
                    table.ForeignKey(
                        name: "FK_EntregasClases_Clases_ClaseId",
                        column: x => x.ClaseId,
                        principalTable: "Clases",
                        principalColumn: "ClaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntregasClases_Estudiantes_EstudianteCedula",
                        column: x => x.EstudianteCedula,
                        principalTable: "Estudiantes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntregasClases_ClaseId",
                table: "EntregasClases",
                column: "ClaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasClases_EstudianteCedula",
                table: "EntregasClases",
                column: "EstudianteCedula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntregasClases");
        }
    }
}
