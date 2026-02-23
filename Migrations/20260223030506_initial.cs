using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coordinadores",
                columns: table => new
                {
                    CoordinadorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinadores", x => x.CoordinadorId);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Cedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Cedula);
                    table.ForeignKey(
                        name: "FK_Estudiantes_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profesores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Passwd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profesores_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    CursoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.CursoId);
                    table.ForeignKey(
                        name: "FK_Cursos_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cursos_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    AsistenciaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.AsistenciaId);
                    table.ForeignKey(
                        name: "FK_Asistencias_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "CursoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asistencias_Estudiantes_EstudianteCedula",
                        column: x => x.EstudianteCedula,
                        principalTable: "Estudiantes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstudiantesCursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudiantesCursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstudiantesCursos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "CursoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstudiantesCursos_Estudiantes_EstudianteCedula",
                        column: x => x.EstudianteCedula,
                        principalTable: "Estudiantes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    ModuloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotaMinima = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    CantidadPreguntas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.ModuloId);
                    table.ForeignKey(
                        name: "FK_Modulos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "CursoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstudiantesModulos",
                columns: table => new
                {
                    EstudianteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    Puntaje = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudiantesModulos", x => new { x.EstudianteCedula, x.ModuloId });
                    table.ForeignKey(
                        name: "FK_EstudiantesModulos_Estudiantes_EstudianteCedula",
                        column: x => x.EstudianteCedula,
                        principalTable: "Estudiantes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstudiantesModulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    PreguntaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enunciado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpcionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpcionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpcionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpcionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespuestaCorrecta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    ModuloId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.PreguntaId);
                    table.ForeignKey(
                        name: "FK_Preguntas_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Preguntas_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosModulos",
                columns: table => new
                {
                    ResultadoModuloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    NotaObtenida = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosModulos", x => x.ResultadoModuloId);
                    table.ForeignKey(
                        name: "FK_ResultadosModulos_Estudiantes_EstudianteCedula",
                        column: x => x.EstudianteCedula,
                        principalTable: "Estudiantes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultadosModulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Coordinadores",
                columns: new[] { "CoordinadorId", "Password", "Usuario" },
                values: new object[] { 1, "1234", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_CursoId",
                table: "Asistencias",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_EstudianteCedula",
                table: "Asistencias",
                column: "EstudianteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_ProfesorId",
                table: "Cursos",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_SedeId",
                table: "Cursos",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_SedeId",
                table: "Estudiantes",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesCursos_CursoId",
                table: "EstudiantesCursos",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesCursos_EstudianteCedula",
                table: "EstudiantesCursos",
                column: "EstudianteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_EstudiantesModulos_ModuloId",
                table: "EstudiantesModulos",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_CursoId",
                table: "Modulos",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_ModuloId",
                table: "Preguntas",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_ProfesorId",
                table: "Preguntas",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_SedeId",
                table: "Profesores",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosModulos_EstudianteCedula",
                table: "ResultadosModulos",
                column: "EstudianteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosModulos_ModuloId",
                table: "ResultadosModulos",
                column: "ModuloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Coordinadores");

            migrationBuilder.DropTable(
                name: "EstudiantesCursos");

            migrationBuilder.DropTable(
                name: "EstudiantesModulos");

            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "ResultadosModulos");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Profesores");

            migrationBuilder.DropTable(
                name: "Sedes");
        }
    }
}
