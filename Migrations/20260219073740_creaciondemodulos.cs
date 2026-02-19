using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class creaciondemodulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfesorId",
                table: "Preguntas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_ProfesorId",
                table: "Preguntas",
                column: "ProfesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Preguntas_Profesores_ProfesorId",
                table: "Preguntas",
                column: "ProfesorId",
                principalTable: "Profesores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preguntas_Profesores_ProfesorId",
                table: "Preguntas");

            migrationBuilder.DropIndex(
                name: "IX_Preguntas_ProfesorId",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "Preguntas");
        }
    }
}
