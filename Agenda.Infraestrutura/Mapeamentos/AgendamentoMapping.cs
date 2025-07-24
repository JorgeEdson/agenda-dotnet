using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infraestrutura.Mapeamentos
{
    public class AgendamentoMapping : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder)
        {
            builder.HasKey(Agendamento => Agendamento.Id);  // Cria chave primaria
            builder.Property(Agendamento => Agendamento.Id).ValueGeneratedNever();  // Não gerar Id Automático

            builder.HasOne(agendamento => agendamento.UsuarioCliente)
               .WithMany(usuario => usuario.Agendamentos)
               .HasForeignKey(Agendamento => Agendamento.IdUsuarioCliente)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(agendamento => agendamento.Disponibilidade)
                   .WithMany(disponibilidade => disponibilidade.Agendamentos)
                   .HasForeignKey(agendamento => agendamento.IdDisponibilidade)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
