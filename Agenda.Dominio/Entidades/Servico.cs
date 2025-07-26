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
        public long IdEndereco { get; private set; }
        public Endereco Endereco { get; private set; }
        
        private readonly List<Disponibilidade> _disponibilidades = new();
        public IReadOnlyCollection<Disponibilidade> Disponibilidades => _disponibilidades.AsReadOnly();
        #endregion

        #region Metodos Privados
        private void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0) 
            {
                AdicionarNotificacao("Usuário inválido para vincular ao serviço");
                return;
            }            

            Usuario = usuario;
            IdUsuario = usuario.Id;
        }
        
        #endregion

        #region Construtores
        public Servico() { }

        protected Servico(string nome, string descricao, decimal valor, Usuario usuario, Endereco endereco)
        {
            SetNome(nome);
            SetDescricao(descricao);
            SetValor(valor);
            VincularUsuario(usuario);
            VincularEndereco(endereco);
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
            if (valor <= 0)
                AdicionarNotificacao("Valor do serviço não pode ser negativo ou zero");

            Valor = valor;
        }

        public void VincularEndereco(Endereco endereco)
        {
            if (endereco is null || endereco.Id <= 0)
            {
                AdicionarNotificacao("Endereço inválido para vincular ao serviço");
                return;
            }

            Endereco = endereco;
            IdEndereco = endereco.Id;
        }

        public ResultadoGenerico<bool> DeterminarDisponibilidade(DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            var disponibilidadeResult = Disponibilidade.Criar(data, horaInicio, horaFim, this);

            if (!disponibilidadeResult.Sucesso)
                return new ResultadoGenerico<bool>(false, disponibilidadeResult.Mensagem, false);

            _disponibilidades.Add(disponibilidadeResult.Dados);

            return new ResultadoGenerico<bool>(true, "Disponibilidade determinada com sucesso", true);
        }

        public static ResultadoGenerico<Servico> Criar(string nome, string descricao, decimal valor, Usuario usuario, Endereco endereco)
        {
            var servico = new Servico(nome, descricao, valor, usuario, endereco);

            if (!servico.Valido)
                return new ResultadoGenerico<Servico>(false, "Erro ao criar serviço: " + servico.ObterMensagemDeErros(), null);

            return new ResultadoGenerico<Servico>(true, "Serviço criado com sucesso!", servico);
        }
        #endregion
    }
}
