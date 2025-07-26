using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Entidades
{
    public class Agendamento : EntidadeBase
    {
        #region Propriedades
        public long IdDisponibilidade { get; private set; }
        public Disponibilidade Disponibilidade { get; private set; }
        public long IdUsuarioCliente { get; private set; }
        public Usuario UsuarioCliente { get; private set; }
        public bool Aceito { get; private set; }
        public DateTime DataCadastro { get; set; }
        #endregion

        #region Metodos Privados
        private void VincularDisponibilidade(Disponibilidade disponibilidade)
        {
            if (disponibilidade is null || disponibilidade.Id <= 0) 
            {
                AdicionarNotificacao("Agenda inválida");
                return;
            
            }

            Disponibilidade = disponibilidade;
            IdDisponibilidade = disponibilidade.Id;
        }

        private void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0) 
            {
                AdicionarNotificacao("Usuário inválido");
                return;
            }

            UsuarioCliente = usuario;
            IdUsuarioCliente = usuario.Id;
        }
        #endregion

        #region Construtores
        public Agendamento() { }

        private Agendamento(Disponibilidade disponibilidade, Usuario usuarioCliente)
        {
            VincularDisponibilidade(disponibilidade);
            VincularUsuario(usuarioCliente);
            Aceito = false;
            DataCadastro = DateTime.Now;
        }
        #endregion

        #region Métodos Públicos

        public void AceitarAgendamento() => Aceito = true;
        

        public static ResultadoGenerico<Agendamento> Criar(Disponibilidade disponibilidade, Usuario usuarioCliente)
        {
            Agendamento agendamento = new Agendamento(disponibilidade,usuarioCliente);

            if(!agendamento.Valido)
                return new ResultadoGenerico<Agendamento>(
                    false,
                    "Falha ao criar o Agendamento: " + agendamento.ObterMensagemDeErros(),
                    null
                );

            return new ResultadoGenerico<Agendamento>(
                true,
                "Agendamento criado com sucesso!",
                new Agendamento(disponibilidade, usuarioCliente)
                );
            
        }
        #endregion
    }
}
