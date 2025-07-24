using Agenda.Dominio.Helpers;

namespace Agenda.Dominio.Entidades
{
    public abstract class EntidadeBase
    {
        public long Id { get; private set; }

        private readonly List<string> _notificacoes = new();
        public IReadOnlyCollection<string> Notificacoes => _notificacoes.AsReadOnly();
        public bool Valido => !_notificacoes.Any();

        protected EntidadeBase()
        {
            Id = GeradorIdHelper.ProximoId();
        }

        public void SetId(long id)
        {
            Id = id;
        }

        protected void AdicionarNotificacao(string mensagem)
        {
            _notificacoes.Add(mensagem);
        }

        protected void LimparNotificacoes()
        {
            _notificacoes.Clear();
        }
        public string ObterMensagemDeErros() => string.Join(" | ", Notificacoes);
    }
}
