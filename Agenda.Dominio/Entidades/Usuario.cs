using Agenda.Dominio.Utils;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6 || !senha.Any(char.IsDigit))
                AdicionarNotificacao("Senha deve ter no mínimo 6 caracteres e conter ao menos um número");

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
            Usuario usuario = id is not null
                ? new Usuario(nome, email, senha, id)
                : new Usuario(nome, email, senha, null);

            if (!usuario.Valido)            
                return new ResultadoGenerico<Usuario>(false, "Erro: " + usuario.ObterMensagemDeErros(), null);            

            return new ResultadoGenerico<Usuario>(true,"Usuário criado com sucesso", usuario);
        }

        public ResultadoGenerico<bool> AdicionarEndereco(string logradouro, string numero)
        {
            var enderecoResult = Endereco.Criar(logradouro,numero,this);

            if (!enderecoResult.Sucesso) 
                return new ResultadoGenerico<bool>(false, enderecoResult.Mensagem, false);           

            var enderecoObj = enderecoResult.Dados;

            _enderecos.Add(enderecoObj);

            return new ResultadoGenerico<bool>(true, "Endereco adicionado com sucesso", true);
        }

        public ResultadoGenerico<bool> AdicionarServico(string nome, string descricao, decimal valor)
        {
            var servicoResult = Servico.Criar(nome,descricao,valor,this);

            if (!servicoResult.Sucesso)
                return new ResultadoGenerico<bool>(false, servicoResult.Mensagem, false);

            var servicoObj = servicoResult.Dados;

            _servicos.Add(servicoObj);

            return new ResultadoGenerico<bool>(true, "Serviço adicionado com sucesso", true);
        }

        public ResultadoGenerico<bool> DeterminarDisponibilidade(DateTime data, TimeSpan horaInicio, TimeSpan horaFim, Servico servico)
        {
            if (servico.IdUsuario != Id)
                return new ResultadoGenerico<bool>(false, "O serviço nao pertence ao usuario", true);

            var disponibilidadeResult = Disponibilidade.Criar(data, horaInicio, horaFim, servico);

            if(!disponibilidadeResult.Sucesso)
                return new ResultadoGenerico<bool>(false, disponibilidadeResult.Mensagem, false);

            return new ResultadoGenerico<bool>(true, "Disponibilidade determinada com sucesso", true);
        }

        public ResultadoGenerico<bool> AgendarServico(Disponibilidade disponibilidade) 
        {
            if(_servicos.Contains(disponibilidade.Servico))
                return new ResultadoGenerico<bool>(false, "O Usuario nao pode agendar seus proprios servicos", false);

            var agendamentoResult = Agendamento.Criar(disponibilidade, this);

            if(!agendamentoResult.Sucesso)
                return new ResultadoGenerico<bool>(false, agendamentoResult.Mensagem, false);

            return new ResultadoGenerico<bool>(true, "Disponibilidade determinada com sucesso", true);
        }

        #endregion
    }
}
