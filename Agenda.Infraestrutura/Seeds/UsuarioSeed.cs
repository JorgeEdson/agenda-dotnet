using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infraestrutura.Seeds
{
    public static class UsuarioSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var objUsuario = Usuario.Criar("Denilson", "denilson.analistasist@gmail.com", "senha01",null).Dados;

            modelBuilder.Entity<Usuario>().HasData(objUsuario);
        }
    }
}
