using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Entidades
{
    public class Endereco : EntidadeBase
    {
        #region Propriedades
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public bool Ativo { get; private set; } = false;
        public long IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }
        #endregion

        #region Metodos Privados
        private void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0) 
            {
                AdicionarNotificacao("Usuário inválido");
                return;
            }

            Usuario = usuario;
            IdUsuario = usuario.Id;
        }
        #endregion

        #region Construtores
        public Endereco() { }

        protected Endereco(string logradouro, string numero, Usuario usuario)
        {
            SetLogradouro(logradouro);
            SetNumero(numero);
            VincularUsuario(usuario);
            Ativo = true;
        }
        #endregion

        #region Métodos Públicos
        public void SetLogradouro(string logradouro)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
                AdicionarNotificacao("Logradouro inválido");

            Logradouro = logradouro.Trim();
        }

        public void SetNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                AdicionarNotificacao("Número inválido");

            Numero = numero.Trim();
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;        

        public static ResultadoGenerico<Endereco> Criar(string logradouro, string numero, Usuario usuario)
        {
            Endereco endereco = new Endereco(logradouro,numero,usuario);

            if (!endereco.Valido)
                return new ResultadoGenerico<Endereco>(false, "Erro: " + endereco.ObterMensagemDeErros(), null);

            return new ResultadoGenerico<Endereco>(true, "Endereço criado com sucesso!", endereco);
        }
        #endregion
    }
}
