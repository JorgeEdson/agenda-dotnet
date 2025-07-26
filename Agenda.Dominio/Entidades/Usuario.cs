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

        #region metodos privados
        private bool SetSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) ||
                senha.Trim().Length < 6 ||
                !senha.Any(char.IsDigit) ||
                !senha.Any(char.IsLetter))
            {
                AdicionarNotificacao("Senha deve ter no mínimo 6 caracteres e conter ao menos um número");
                return false;
            }

            Senha = senha;
            return true;
        }

        private void Ativar() => Ativo = true;

        private void Desativar() => Ativo = false;

        private void AdicionarAgendamentoInterno(Agendamento agendamento)
        {
            if (agendamento is null)
                return;

            _agendamentos.Add(agendamento);
        }
        #endregion



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
            {
                AdicionarNotificacao("Nome inválido");
                return;
            }

            Nome = nome.Trim();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                AdicionarNotificacao("Email inválido");
                return;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email.Trim())
                {
                    AdicionarNotificacao("Email inválido");
                    return;
                }
            }
            catch
            {
                AdicionarNotificacao("Email inválido");
                return;
            }

            Email = email.Trim().ToLower();
        }

        public ResultadoGenerico<bool> AlterarSenha(string senhaAtual, string novaSenha, string confirmacaoSenha)
        {
            if (Senha != senhaAtual)
                return new ResultadoGenerico<bool>(false, "Senha atual incorreta", false);

            if (novaSenha != confirmacaoSenha)
                return new ResultadoGenerico<bool>(false, "Confirmação de senha não confere", false);

            if(!SetSenha(novaSenha))
                return new ResultadoGenerico<bool>(false, "Erro: "+ObterMensagemDeErros(), false);

            return new ResultadoGenerico<bool>(true, "Senha alterada com sucesso", true);
        }

        public void TornarAdministrador() => Administrador = true;

        public ResultadoGenerico<bool> DesativarUsuario(Usuario usuario)
        {
            if (!Administrador)
                return new ResultadoGenerico<bool>(false, "Somente administradores podem desativar usuários", false);

            if (usuario is null)
                return new ResultadoGenerico<bool>(false, "Usuário inválido", false);

            if (!usuario.Ativo)
                return new ResultadoGenerico<bool>(false, "Usuário já está desativado", false);

            usuario.Desativar();

            return new ResultadoGenerico<bool>(true, "Usuário desativado com sucesso", true);
        }

        public ResultadoGenerico<bool> AtivarUsuario(Usuario usuario)
        {
            if (!Administrador)
                return new ResultadoGenerico<bool>(false, "Somente administradores podem Ativar usuários", false);

            if (usuario is null)
                return new ResultadoGenerico<bool>(false, "Usuário inválido", false);

            if (usuario.Ativo)
                return new ResultadoGenerico<bool>(false, "Usuário já está ativo", false);

            usuario.Ativar();

            return new ResultadoGenerico<bool>(true, "Usuário ativado com sucesso", true);
        }

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

        public ResultadoGenerico<bool> AdicionarServico(string nome, string descricao, decimal valor, Endereco endereco)
        {
            if (!endereco.Ativo)
                return new ResultadoGenerico<bool>(false, "O endereço para este serviço esta desativado", false);

            var servicoResult = Servico.Criar(nome,descricao,valor,this, endereco);

            if (!servicoResult.Sucesso)
                return new ResultadoGenerico<bool>(false, servicoResult.Mensagem, false);

            var servicoObj = servicoResult.Dados;

            _servicos.Add(servicoObj);

            return new ResultadoGenerico<bool>(true, "Serviço adicionado com sucesso", true);
        }        

        public ResultadoGenerico<bool> RealizarAgendamento(Disponibilidade disponibilidade) 
        {
            if(_servicos.Contains(disponibilidade.Servico))
                return new ResultadoGenerico<bool>(false, "O Usuario nao pode agendar seus proprios servicos", false);

            var dataHoraInicio = disponibilidade.Data.Date + disponibilidade.HoraInicio;
            if (dataHoraInicio <= DateTime.Now)
                return new ResultadoGenerico<bool>(false, "Essa disponibilidade já expirou", false);

            bool jaAgendado = _agendamentos.Any(a => a.Disponibilidade.Id == disponibilidade.Id);
            if (jaAgendado)
                return new ResultadoGenerico<bool>(false, "Você já agendou esse horário", false);

            var agendamentoResult = Agendamento.Criar(disponibilidade, this);

            if(!agendamentoResult.Sucesso)
                return new ResultadoGenerico<bool>(false, agendamentoResult.Mensagem, false);

            var agendamento = agendamentoResult.Dados;

            disponibilidade.Servico.Usuario.AdicionarAgendamentoInterno(agendamento);

            return new ResultadoGenerico<bool>(true, "Agendamento realizado sucesso", true);
        }

        public ResultadoGenerico<bool> AtivarEndereco(long idEndereco)
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == idEndereco);

            if (endereco is null)
                return new ResultadoGenerico<bool>(false, "Endereço não encontrado", false);

            if (endereco.Ativo)
                return new ResultadoGenerico<bool>(false, "Endereço já está ativo", false);

            endereco.Ativar();

            return new ResultadoGenerico<bool>(true, "Endereço ativado com sucesso", true);
        }

        public ResultadoGenerico<bool> DesativarEndereco(long idEndereco)
        {
            var endereco = _enderecos.FirstOrDefault(e => e.Id == idEndereco);

            if (endereco is null)
                return new ResultadoGenerico<bool>(false, "Endereço não encontrado", false);

            if (!endereco.Ativo)
                return new ResultadoGenerico<bool>(false, "Endereço já está desativado", false);

            
            bool enderecoVinculadoAServico = _servicos.Any(s => s.IdEndereco == idEndereco);

            if (enderecoVinculadoAServico)
                return new ResultadoGenerico<bool>(false, "Não é possível desativar o endereço pois está vinculado a um ou mais serviços", false);

            endereco.Desativar();

            return new ResultadoGenerico<bool>(true, "Endereço desativado com sucesso", true);
        }

        public ResultadoGenerico<bool> AceitarAgendamento(long idAgendamento) 
        {
            var agendamento = _agendamentos.FirstOrDefault(a => a.Id == idAgendamento);

            if (agendamento is null)
                return new ResultadoGenerico<bool>(false, "Agendamento não encontrado", false);
            
            //if (agendamento.Disponibilidade?.Servico?.IdUsuario != Id)
            //    return new ResultadoGenerico<bool>(false, "Usuario não tem permissão para aceitar este agendamento", false);

            if (agendamento.Aceito)
                return new ResultadoGenerico<bool>(false, "Este agendamento já foi aceito anteriormente", false);

            agendamento.AceitarAgendamento();

            return new ResultadoGenerico<bool>(true, "Agendamento aceito com sucesso", true);

        }
        #endregion
    }
}
