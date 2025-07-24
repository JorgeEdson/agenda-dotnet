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
        #endregion

        #region Construtores
        public Endereco() { }

        protected Endereco(string logradouro, string numero, Usuario usuario)
        {
            SetLogradouro(logradouro);
            SetNumero(numero);
            VincularUsuario(usuario);
            Ativo = false;
        }
        #endregion

        #region Métodos Públicos
        public void SetLogradouro(string logradouro)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
                throw new ArgumentException("Logradouro inválido");

            Logradouro = logradouro.Trim();
        }

        public void SetNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("Número inválido");

            Numero = numero.Trim();
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;

        public void VincularUsuario(Usuario usuario)
        {
            if (usuario is null || usuario.Id <= 0)
                throw new ArgumentException("Usuário inválido");

            Usuario = usuario;
            IdUsuario = usuario.Id;
        }

        public static ResultadoGenerico<Endereco> Criar(string logradouro, string numero, Usuario usuario)
        {

            try 
            {
                return new ResultadoGenerico<Endereco>(
                true,
                "Endereço criado com sucesso!",
                new Endereco(logradouro, numero, usuario)
                );
            }
            catch (Exception ex) 
            {
                return new ResultadoGenerico<Endereco>(
                    false,
                    "Falha ao criar Endereco: "+ex.Message,
                    null
                );
            }
            
        }
        #endregion
    }
}
