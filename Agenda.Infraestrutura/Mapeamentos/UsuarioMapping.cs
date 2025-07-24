using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infraestrutura.Mapeamentos
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(Usuario => Usuario.Id);  // Cria chave primaria
            builder.Property(Usuario => Usuario.Id).ValueGeneratedNever();  // Não gerar Id Automático
            builder.Property(Usuario => Usuario.Nome).IsRequired(); // Requerido
            builder.Property(Usuario => Usuario.Email).IsRequired(); // Requerido
            builder.Property(Usuario => Usuario.Senha).IsRequired(); // Requerido
        }
    }
}
