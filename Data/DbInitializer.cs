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

            // Obtener primera sede
            var sedePrincipal = context.Sedes.First();

            // Crear profesor administrador
            context.Profesores.Add(
                new Profesor
                {
                    Nombre = "Admin",
                    Passwd = "1234",
                    SedeId = sedePrincipal.Id
                }
            );

            context.SaveChanges();
        }
    }
}
