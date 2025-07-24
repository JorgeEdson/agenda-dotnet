using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Agenda.Infraestrutura.Mapeamentos
{
    public class ServicoMapping : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.HasKey(Servico => Servico.Id);  // Cria chave primaria
            builder.Property(Servico => Servico.Id).ValueGeneratedNever();  // Não gerar Id Automático
            builder.Property(Servico => Servico.Descricao).IsRequired(); // Requerido
            builder.Property(Servico => Servico.Valor).IsRequired(); // Requerido

            builder.HasOne(servico => servico.Usuario)
               .WithMany(usuario => usuario.Servicos)
               .HasForeignKey(servico => servico.Id)
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
