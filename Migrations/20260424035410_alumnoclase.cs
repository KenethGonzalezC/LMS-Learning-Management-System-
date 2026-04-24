using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class alumnoclase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntregasClases_Estudiantes_EstudianteCedula",
                table: "EntregasClases");

            migrationBuilder.AddForeignKey(
                name: "FK_EntregasClases_Estudiantes_EstudianteCedula",
                table: "EntregasClases",
                column: "EstudianteCedula",
                principalTable: "Estudiantes",
                principalColumn: "Cedula",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntregasClases_Estudiantes_EstudianteCedula",
                table: "EntregasClases");

            migrationBuilder.AddForeignKey(
                name: "FK_EntregasClases_Estudiantes_EstudianteCedula",
                table: "EntregasClases",
                column: "EstudianteCedula",
                principalTable: "Estudiantes",
                principalColumn: "Cedula",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
