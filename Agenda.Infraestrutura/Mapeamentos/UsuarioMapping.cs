using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infraestrutura.Mapeamentos
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(Usuario => Usuario.Id);  
            builder.Property(Usuario => Usuario.Id).ValueGeneratedNever();  
            builder.Property(Usuario => Usuario.Nome).IsRequired(); 
            builder.Property(Usuario => Usuario.Email).IsRequired(); 
            builder.Property(Usuario => Usuario.Senha).IsRequired(); 
        }
    }
}
