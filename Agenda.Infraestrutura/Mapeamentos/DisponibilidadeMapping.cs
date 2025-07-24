using Agenda.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Agenda.Infraestrutura.Mapeamentos
{
    public class DisponibilidadeMapping : IEntityTypeConfiguration<Disponibilidade>
    {
        public void Configure(EntityTypeBuilder<Disponibilidade> builder)
        {
            builder.HasKey(agendaHorario => agendaHorario.Id);  // Cria chave primaria
            builder.Property(agendaHorario => agendaHorario.Id).ValueGeneratedNever();  // Não gerar Id Automático
            builder.Property(agendaHorario => agendaHorario.Data).IsRequired(); // Requerido
            builder.Property(agendaHorario => agendaHorario.HoraInicio).IsRequired(); // Requerido
            builder.Property(agendaHorario => agendaHorario.HoraFim).IsRequired(); // Requerido

            builder.HasOne(disponibilidade => disponibilidade.Servico)
               .WithMany(servico => servico.Disponibilidades)
               .HasForeignKey(disponibilidade => disponibilidade.IdServico)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

