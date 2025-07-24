using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infraestrutura.Mapeamentos
{
    public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(Endereco => Endereco.Id);  
            builder.Property(Endereco => Endereco.Id).ValueGeneratedNever();  
            builder.Property(Endereco => Endereco.Logradouro).IsRequired();
            builder.Property(Endereco => Endereco.Numero).IsRequired(); 

            
            builder.HasOne(endereco => endereco.Usuario)
                   .WithMany(usuario => usuario.Enderecos)
                   .HasForeignKey(endereco => endereco.IdUsuario)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
