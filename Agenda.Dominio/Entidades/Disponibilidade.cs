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
                throw new ArgumentException("Data de agendamento inválida");
            }

            Data = data;
        }
        public void SetHoraInicio(TimeSpan horaInicio)
        {
            if (horaInicio < TimeSpan.Zero || horaInicio > TimeSpan.FromHours(24))
            {
                throw new ArgumentException("Hora de início inválida");
            }
            HoraInicio = horaInicio;
        }

        public void SetHoraFim(TimeSpan horaFim)
        {
            if (horaFim < TimeSpan.Zero || horaFim > TimeSpan.FromHours(24))
            {
                throw new ArgumentException("Hora de fim inválida");
            }
            HoraFim = horaFim;
        }

        public void VincularServico(Servico servico)
        {
            if (servico.Id <= 0 || servico is null)
            {
                throw new ArgumentException("ID de serviço inválido");
            }
            IdServico = servico.Id;
        }

        public static ResultadoGenerico<Disponibilidade>  Criar(DateTime data, TimeSpan horaInicio, TimeSpan horaFim, Servico servico)
        {
            try
            {
                return new ResultadoGenerico<Disponibilidade>(
                true,
                "Agenda criada com sucesso!",
                new Disponibilidade(data, horaInicio, horaFim, servico));
            }
            catch (Exception ex) 
            {
                return new ResultadoGenerico<Disponibilidade>(
                    false,
                    "Falha ao criar Disponibilidade: "+ex.Message,
                    null
                );
            }
            
        }
        #endregion
    }
}
