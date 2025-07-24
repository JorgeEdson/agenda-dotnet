using Agenda.Dominio.Entidades;
using FluentAssertions;

namespace AgendaDotnet.Testes.Dominio
{
    public class UsuarioTests
    {
        [Fact]
        public void Usuario_Criar_DeveRetornarUsuarioValido_QuandoDadosSaoValidos()
        {
            // Arrange
            string nome = "Teste Usuário";
            string email = "teste@email.com";
            string senha = "senhaSegura123";

            // Act
            var resultado = Usuario.Criar(nome, email, senha, null);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(nome, resultado.Dados.Nome);
            Assert.Equal(email, resultado.Dados.Email);
            Assert.Equal(senha, resultado.Dados.Senha);
            Assert.False(resultado.Dados.Ativo);
            Assert.False(resultado.Dados.Administrador);
            Assert.True(resultado.Dados.Valido);
        }

        [Fact]
        public void Usuario_Criar_DeveRetornarUsuarioValidoComId_QuandoIdEhFornecido()
        {
            // Arrange
            string nome = "Teste Usuário";
            string email = "teste@email.com";
            string senha = "senhaSegura123";
            long id = 100;

            // Act
            var resultado = Usuario.Criar(nome, email, senha, id);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(id, resultado.Dados.Id);
            Assert.True(resultado.Dados.Valido);
        }

        [Theory]
        [InlineData("", "teste@email.com", "senhaValida", "Nome inválido")]
        [InlineData("Nome Valido", "", "senhaValida", "Email inválido")]
        [InlineData("Nome Valido", "emailInvalido", "senhaValida", "Email inválido")]
        [InlineData("Nome Valido", "teste@email.com", "curta", "Senha inválida (mínimo 6 caracteres)")]
        public void Usuario_Criar_DeveRetornarErro_QuandoDadosSaoInvalidos(string nome, string email, string senha, string mensagemEsperada)
        {
            // Act
            var resultado = Usuario.Criar(nome, email, senha, null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Contains(mensagemEsperada, resultado.Mensagem);
        }

        [Fact]
        public void AlterarSenha_DeveAlterarSenhaComSucesso_QuandoDadosValidos()
        {
            // Arrange
            string senhaOriginal = "senhaAntiga123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaOriginal, null).Dados;
            string novaSenha = "novaSenhaSegura";
            string confirmacaoNovaSenha = "novaSenhaSegura";

            // Act
            var resultado = usuario.AlterarSenha(senhaOriginal, novaSenha, confirmacaoNovaSenha);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Senha alterada com sucesso", resultado.Mensagem);
            Assert.Equal(novaSenha, usuario.Senha);
            Assert.True(usuario.Valido); // Não deve adicionar notificações de erro
        }

        [Fact]
        public void AlterarSenha_DeveFalhar_QuandoSenhaAtualIncorreta()
        {
            // Arrange
            string senhaOriginal = "senhaAntiga123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaOriginal, null).Dados;
            string novaSenha = "novaSenhaSegura";
            string confirmacaoNovaSenha = "novaSenhaSegura";

            // Act
            var resultado = usuario.AlterarSenha("senhaIncorreta", novaSenha, confirmacaoNovaSenha);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Senha atual incorreta", resultado.Mensagem);
            Assert.Equal(senhaOriginal, usuario.Senha); // Senha não deve ser alterada
        }

        [Fact]
        public void AlterarSenha_DeveFalhar_QuandoConfirmacaoSenhaNaoConfere()
        {
            // Arrange
            string senhaOriginal = "senhaAntiga123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaOriginal, null).Dados;
            string novaSenha = "novaSenhaSegura";
            string confirmacaoNovaSenha = "confirmacaoDiferente";

            // Act
            var resultado = usuario.AlterarSenha(senhaOriginal, novaSenha, confirmacaoNovaSenha);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Confirmação de senha não confere", resultado.Mensagem);
            Assert.Equal(senhaOriginal, usuario.Senha); // Senha não deve ser alterada
        }

        [Fact]
        public void Ativar_DeveDefinirAtivoComoVerdadeiro()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", null).Dados;
            Assert.False(usuario.Ativo); // Inicia como falso

            // Act
            usuario.Ativar();

            // Assert
            Assert.True(usuario.Ativo);
        }

        [Fact]
        public void Desativar_DeveDefinirAtivoComoFalso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", null).Dados;
            usuario.Ativar(); // Ativa para depois desativar
            Assert.True(usuario.Ativo);

            // Act
            usuario.Desativar();

            // Assert
            Assert.False(usuario.Ativo);
        }

        [Fact]
        public void TornarAdministrador_DeveDefinirAdministradorComoVerdadeiro()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", null).Dados;
            Assert.False(usuario.Administrador); // Inicia como falso

            // Act
            usuario.TornarAdministrador();

            // Assert
            Assert.True(usuario.Administrador);
        }

        [Fact]
        public void AdicionarEndereco_DeveAdicionarEndereco_QuandoPertenceAoUsuario()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", 1).Dados;
            var endereco = new Endereco(1);

            // Act
            usuario.AdicionarEndereco(endereco);

            // Assert
            Assert.Single(usuario.Enderecos);
            Assert.Contains(endereco, usuario.Enderecos);
            Assert.True(usuario.Valido);
        }

        [Fact]
        public void AdicionarEndereco_DeveAdicionarNotificacao_QuandoEnderecoNaoPertenceAoUsuario()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", 1).Dados;
            var endereco = new Endereco(2); // Endereço com ID de usuário diferente

            // Act
            usuario.AdicionarEndereco(endereco);

            // Assert
            Assert.Empty(usuario.Enderecos); // Não deve adicionar o endereço
            Assert.Contains("Endereço pertence a outro usuário", usuario.Notificacoes);
            Assert.False(usuario.Valido);
        }

        [Fact]
        public void AdicionarServico_DeveAdicionarServico_QuandoPertenceAoUsuario()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", 1).Dados;
            var servico = new Servico(1);

            // Act
            usuario.AdicionarServico(servico);

            // Assert
            Assert.Single(usuario.Servicos);
            Assert.Contains(servico, usuario.Servicos);
            Assert.True(usuario.Valido);
        }

        [Fact]
        public void AdicionarServico_DeveAdicionarNotificacao_QuandoServicoNaoPertenceAoUsuario()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", 1).Dados;
            var servico = new Servico(2); // Serviço com ID de usuário diferente

            // Act
            usuario.AdicionarServico(servico);

            // Assert
            Assert.Empty(usuario.Servicos); // Não deve adicionar o serviço
            Assert.Contains("Serviço pertence a outro usuário", usuario.Notificacoes);
            Assert.False(usuario.Valido);
        }

        [Fact]
        public void AdicionarAgendamento_DeveAdicionarAgendamento()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "senhaValida", 1).Dados;
            var agendamento = new Agendamento();

            // Act
            usuario.AdicionarAgendamento(agendamento);

            // Assert
            Assert.Single(usuario.Agendamentos);
            Assert.Contains(agendamento, usuario.Agendamentos);
            Assert.True(usuario.Valido); // Adicionar agendamento não adiciona notificação por si só
        }


    }
}
