using Agenda.Dominio.Entidades;
using Agenda.Infraestrutura.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Infraestrutura.Contextos
{
    public class Contexto(DbContextOptions<Contexto>options): DbContext(options)
    {
        public DbSet<Disponibilidade> Agendas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());  // Executa os mapeamentos definidos em "Mapeamentos" sem necessidade de apontar.
            UsuarioSeed.Seed(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder) 
        {
            if (!optionsbuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json").Build();

                var stringConexao = configuration.GetConnectionString("conexaoPadrao");

                optionsbuilder.UseSqlServer(stringConexao);
            }
        }
    }
}
