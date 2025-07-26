using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Entidades
{
    public class Disponibilidade : EntidadeBase
    {
        #region Propriedades
        public DateTime Data { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFim { get; private set; }
        public long IdServico { get; private set; }
        public Servico Servico { get; private set; }
        public List<Agendamento> Agendamentos { get; private set; }
        #endregion

        #region Metodos Privados
        private void VincularServico(Servico servico)
        {
            if (servico.Id <= 0 || servico is null)
            {
                AdicionarNotificacao("Serviço invalido");
                return;
            }
            Servico = servico;
            IdServico = servico.Id;
        }
        #endregion

        #region Construtores
        public Disponibilidade()
        {}
        protected Disponibilidade(DateTime data, TimeSpan horaInicio, TimeSpan horaFim, Servico servico)
        {
            SetData(data);
            SetHoraInicio(horaInicio);
            SetHoraFim(horaFim);
            VincularServico(servico);
        }
        #endregion

        #region Metodos Públicos
        public void SetData(DateTime data)
        {
            if (data <= DateTime.Now)
            {
                AdicionarNotificacao("Data de agendamento inválida");
                return;
            }

            Data = data;
        }
        public void SetHoraInicio(TimeSpan horaInicio)
        {
            if (horaInicio < TimeSpan.Zero || horaInicio > TimeSpan.FromHours(24))
            {
                AdicionarNotificacao("Hora de início inválida");
                return;
            }
            HoraInicio = horaInicio;
        }

        public void SetHoraFim(TimeSpan horaFim)
        {
            if (horaFim < TimeSpan.Zero || horaFim > TimeSpan.FromHours(24))
            {
                AdicionarNotificacao("Hora de fim inválida");
                return;
            }

            if (HoraInicio != default && horaFim <= HoraInicio)
            {
                AdicionarNotificacao("Hora de fim deve ser posterior à hora de início");
                return;
            }

            HoraFim = horaFim;
        }

        public static ResultadoGenerico<Disponibilidade>  Criar(DateTime data, TimeSpan horaInicio, TimeSpan horaFim, Servico servico)
        {
            Disponibilidade disponibilidade = new Disponibilidade(data, horaInicio, horaFim, servico);

            if(!disponibilidade.Valido)
                return new ResultadoGenerico<Disponibilidade>(
                    false,
                    "Erro: " + disponibilidade.ObterMensagemDeErros(),
                    null
                );

            return new ResultadoGenerico<Disponibilidade>(
                true,
                "Agenda criada com sucesso!",
                new Disponibilidade(data, horaInicio, horaFim, servico));

        }
        #endregion
    }
}
