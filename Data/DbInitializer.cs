using LMS.Models;
using LMS.Models.Entidades;

namespace LMS.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Si ya hay datos, no hace nada
            if (context.Profesores.Any())
                return;

            // Crear sedes si no existen
            if (!context.Sedes.Any())
            {
                context.Sedes.AddRange(
                    new Sede { Nombre = "Hatillo" },
                    new Sede { Nombre = "Linda Vista" },
                    new Sede { Nombre = "Alajuelita" }
                );

                context.SaveChanges();
            }

            // Obtener todas las sedes
            var sedes = context.Sedes.ToList();

            // Crear un profesor por cada sede
            foreach (var sede in sedes)
            {
                context.Profesores.Add(new Profesor
                {
                    Nombre = $"Profesor {sede.Nombre}",
                    Passwd = "1234",
                    SedeId = sede.Id
                });
            }

            context.SaveChanges();
        }
    }
}
