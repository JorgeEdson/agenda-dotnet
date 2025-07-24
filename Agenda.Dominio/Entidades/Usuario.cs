using Agenda.Dominio.Utils;

namespace Agenda.Dominio.Entidades
{
    public class Usuario : EntidadeBase
    {
        #region Propriedades
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public bool Ativo { get; private set; } = false;
        public bool Administrador { get; private set; } = false;
        private readonly List<Endereco> _enderecos = new();
        public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();
        private readonly List<Servico> _servicos = new();
        public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();
        private readonly List<Agendamento> _agendamentos = new();
        public IReadOnlyCollection<Agendamento> Agendamentos => _agendamentos.AsReadOnly();
        #endregion

        private void SetSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
                AdicionarNotificacao("Senha inválida (mínimo 6 caracteres)");

            Senha = senha;
        }

        #region Construtores
        public Usuario() { }

        private Usuario(string nome, string email, string senha, long? id)
        {
            if (id is not null)
                SetId(id.Value);

            SetNome(nome);
            SetEmail(email);
            SetSenha(senha);
            Ativo = false;
            Administrador = false;
        }
        #endregion

        #region Métodos Públicos
        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                AdicionarNotificacao("Nome inválido");

            Nome = nome.Trim();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                AdicionarNotificacao("Email inválido");

            Email = email.Trim().ToLower();
        }

        public ResultadoGenerico<bool> AlterarSenha(string senhaAtual, string novaSenha, string confirmacaoSenha)
        {
            if (Senha != senhaAtual)
                return new ResultadoGenerico<bool>(false, "Senha atual incorreta", false);

            if (novaSenha != confirmacaoSenha)
                return new ResultadoGenerico<bool>(false, "Confirmação de senha não confere", false);

            SetSenha(novaSenha);

            return new ResultadoGenerico<bool>(true, "Senha alterada com sucesso", true);
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;

        public void TornarAdministrador() => Administrador = true;

        public static ResultadoGenerico<Usuario> Criar(string nome, string email, string senha, long? id)
        {
            Usuario usuario;

            if (id is not null)
                usuario = new Usuario(nome, email, senha, id);

            usuario = new Usuario(nome, email, senha, null);

            if (!usuario.Valido)            
                return new ResultadoGenerico<Usuario>(false, "Erro: " + usuario.ObterMensagemDeErros(), null);            

            return new ResultadoGenerico<Usuario>(true,"Usuário criado com sucesso", usuario);
        }

        public void AdicionarEndereco(Endereco endereco)
        {   

            if (endereco.IdUsuario != Id)
                AdicionarNotificacao("Endereço pertence a outro usuário");

            _enderecos.Add(endereco);
        }

        public void AdicionarServico(Servico servico)
        {
            if (servico.IdUsuario != Id)
                AdicionarNotificacao("Serviço pertence a outro usuário");

            _servicos.Add(servico);
        }

        public void AdicionarAgendamento(Agendamento agendamento)
        {
            _agendamentos.Add(agendamento);
        }

        #endregion
    }
}
