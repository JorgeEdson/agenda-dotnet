using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Entidades
{
    public class Servico : EntidadeBase
    {
        #region Propriedades
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public decimal Valor { get; private set; } = 0;
        public long IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }
        public List<Disponibilidade> Disponibilidades { get; private set; }
        #endregion

        #region Metodos Privados
        private void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0)
                AdicionarNotificacao("Usuário inválido para vincular ao serviço");

            IdUsuario = usuario.Id;
        }
        #endregion

        #region Construtores
        public Servico() { }

        protected Servico(string nome, string descricao, decimal valor, Usuario usuario)
        {
            SetNome(nome);
            SetDescricao(descricao);
            SetValor(valor);
            VincularUsuario(usuario);
            Disponibilidades = new List<Disponibilidade>();
        }
        #endregion

        #region Métodos Públicos
        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                AdicionarNotificacao("Nome do serviço inválido");

            Nome = nome;
        }

        public void SetDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                AdicionarNotificacao("Descrição do serviço inválida");

            Descricao = descricao.Trim();
        }

        public void SetValor(decimal valor)
        {
            if (valor < 0)
                AdicionarNotificacao("Valor do serviço não pode ser negativo");

            Valor = valor;
        }

        public static ResultadoGenerico<Servico> Criar(string nome, string descricao, decimal valor, Usuario usuario)
        {
            var servico = new Servico(nome, descricao, valor, usuario);

            if (!servico.Valido)
                return new ResultadoGenerico<Servico>(false, "Erro ao criar serviço: " + servico.ObterMensagemDeErros(), null);

            return new ResultadoGenerico<Servico>(true, "Serviço criado com sucesso!", servico);
        }
        #endregion
    }
}
