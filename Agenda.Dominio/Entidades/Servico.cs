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
                throw new ArgumentException("Nome do serviço inválido");

            Nome = nome.Trim();
        }

        public void SetDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição do serviço inválida");

            Descricao = descricao.Trim();
        }

        public void SetValor(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("Valor do serviço não pode ser negativo");

            Valor = valor;
        }

        public void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0)
                throw new ArgumentException("Usuário inválido");

            Usuario = usuario;
            IdUsuario = usuario.Id;
        }

        public static ResultadoGenerico<Servico> Criar(string nome, string descricao, decimal valor, Usuario usuario)
        {
            try
            {
                return new ResultadoGenerico<Servico>(
                true,
                "Serviço criado com sucesso!",
                new Servico(nome, descricao, valor, usuario)
            );
            }
            catch (Exception ex) 
            {
                return new ResultadoGenerico<Servico>(false, "Falha ao criar endereço: " + ex.Message, null);
            }
        }
        #endregion
    }
}
