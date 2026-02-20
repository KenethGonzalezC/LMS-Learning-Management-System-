using LMS.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
        public DbSet<Profesor> Profesores => Set<Profesor>();
        public DbSet<Modulo> Modulos => Set<Modulo>();
        public DbSet<Pregunta> Preguntas => Set<Pregunta>();
        public DbSet<EstudianteCurso> EstudiantesCursos => Set<EstudianteCurso>();
        public DbSet<EstudianteModulo> EstudiantesModulos => Set<EstudianteModulo>();
        public DbSet<ResultadoModulo> ResultadosModulos => Set<ResultadoModulo>();
        public DbSet<Sede> Sedes => Set<Sede>();
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Coordinador> Coordinadores { get; set; }
        public ICollection<ResultadoModulo>? Resultados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // CURSO - PROFESOR
            // ===============================
            modelBuilder.Entity<Curso>()
                .HasOne(c => c.Profesor)
                .WithMany(p => p.Cursos)
                .HasForeignKey(c => c.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CURSO - SEDE
            // ===============================
            modelBuilder.Entity<Curso>()
                .HasOne(c => c.Sede)
                .WithMany(s => s.Cursos)
                .HasForeignKey(c => c.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // ESTUDIANTE - SEDE
            // ===============================
            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Sede)
                .WithMany(s => s.Estudiantes)
                .HasForeignKey(e => e.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // PROFESOR - SEDE
            // ===============================
            modelBuilder.Entity<Profesor>()
                .HasOne(p => p.Sede)
                .WithMany(s => s.Profesores)
                .HasForeignKey(p => p.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // MODULO - CURSO
            // ===============================
            modelBuilder.Entity<Modulo>()
                .HasOne(m => m.Curso)
                .WithMany(c => c.Modulos)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // PREGUNTA - MODULO
            // ===============================
            modelBuilder.Entity<Pregunta>()
                .HasOne(p => p.Modulo)
                .WithMany(m => m.Preguntas)
                .HasForeignKey(p => p.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // ESTUDIANTE CURSO (tabla puente)
            // ===============================
            modelBuilder.Entity<EstudianteCurso>()
                .HasOne(ec => ec.Estudiante)
                .WithMany(e => e.EstudiantesCursos)
                .HasForeignKey(ec => ec.EstudianteCedula)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstudianteCurso>()
                .HasOne(ec => ec.Curso)
                .WithMany(c => c.EstudiantesCursos)
                .HasForeignKey(ec => ec.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // ESTUDIANTE MODULO (tabla puente)
            // ===============================
            modelBuilder.Entity<EstudianteModulo>()
                .HasKey(em => new { em.EstudianteCedula, em.ModuloId });

            modelBuilder.Entity<EstudianteModulo>()
                .HasOne<Estudiante>()
                .WithMany()
                .HasForeignKey(em => em.EstudianteCedula)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstudianteModulo>()
                .HasOne<Modulo>()
                .WithMany()
                .HasForeignKey(em => em.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // RESULTADO MODULO
            // ===============================
            modelBuilder.Entity<ResultadoModulo>()
                .HasOne<Estudiante>()
                .WithMany()
                .HasForeignKey(r => r.EstudianteCedula)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResultadoModulo>()
                .HasOne<Modulo>()
                .WithMany()
                .HasForeignKey(r => r.ModuloId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // PRECISIÓN DECIMAL
            // ===============================
            modelBuilder.Entity<Modulo>()
                .Property(m => m.NotaMinima)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ResultadoModulo>()
                .Property(r => r.NotaObtenida)
                .HasPrecision(5, 2);

            // ===============================
            // ENTIDAD PARA COORDINADOR
            // ===============================
            modelBuilder.Entity<Coordinador>().HasData(
                new Coordinador
                    {
                        CoordinadorId = 1,
                        Usuario = "admin",
                        Password = "1234"
                    }
            );

            // ===============================
            // RESULTADOS EN MODULOS PARA CORD
            // ===============================
            modelBuilder.Entity<ResultadoModulo>()
                .HasOne(r => r.Modulo)
                .WithMany(m => m.Resultados)
                .HasForeignKey(r => r.ModuloId);

            modelBuilder.Entity<ResultadoModulo>()
                .HasOne(r => r.Estudiante)
                .WithMany()
                .HasForeignKey(r => r.EstudianteCedula);
        }
    }
}
