using Agenda.Dominio.Entidades;
using FluentAssertions;

namespace AgendaDotnet.Testes.Dominio
{
    public class UsuarioTests
    {
        [Fact]
        public void CriarUsuario_Valido_DeveRetornarSucesso()
        {
            // Act
            var resultado = Usuario.Criar("João", "joao@email.com", "123456", null);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Nome.Should().Be("João");
            resultado.Dados.Email.Should().Be("joao@email.com");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CriarUsuario_ComNomeInvalido_DeveFalhar(string nomeInvalido)
        {
            var resultado = Usuario.Criar(nomeInvalido, "teste@email.com", "123456", null);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Nome inválido");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalido")]
        public void CriarUsuario_ComEmailInvalido_DeveFalhar(string emailInvalido)
        {
            var resultado = Usuario.Criar("João", emailInvalido, "123456", null);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Email inválido");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void CriarUsuario_ComSenhaInvalida_DeveFalhar(string senhaInvalida)
        {
            var resultado = Usuario.Criar("João", "joao@email.com", senhaInvalida, null);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Senha inválida");
        }

        public void SetNome_Valido_DeveAtualizar()
        {
            var usuario = Usuario.Criar("Antigo", "teste@email.com", "123456", null).Dados;

            usuario.SetNome("Novo Nome");

            usuario.Nome.Should().Be("Novo Nome");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SetNome_Invalido_DeveLancarExcecao(string nomeInvalido)
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;

            var acao = () => usuario.SetNome(nomeInvalido);

            acao.Should().Throw<ArgumentException>().WithMessage("Nome inválido");
        }

        [Fact]
        public void SetEmail_Valido_DeveAtualizar()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;

            usuario.SetEmail("NOVO@email.com");

            usuario.Email.Should().Be("novo@email.com");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sem-arroba")]
        public void SetEmail_Invalido_DeveLancarExcecao(string emailInvalido)
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;

            var acao = () => usuario.SetEmail(emailInvalido);

            acao.Should().Throw<ArgumentException>().WithMessage("Email inválido");
        }

        [Fact]
        public void AlterarSenha_ComSenhaAtualIncorreta_DeveFalhar()
        {
            var usuario = Usuario.Criar("Maria", "maria@email.com", "senha123", null).Dados;

            var resultado = usuario.AlterarSenha("errada", "novaSenha", "novaSenha");

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Senha atual incorreta");
        }

        [Fact]
        public void AlterarSenha_ComConfirmacaoIncorreta_DeveFalhar()
        {
            var usuario = Usuario.Criar("José", "jose@email.com", "senha123", null).Dados;

            var resultado = usuario.AlterarSenha("senha123", "novaSenha", "confirmacaoErrada");

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Confirmação de senha não confere");
        }

        [Fact]
        public void AlterarSenha_Correta_DeveAlterar()
        {
            var usuario = Usuario.Criar("Ana", "ana@email.com", "senha123", null).Dados;

            var resultado = usuario.AlterarSenha("senha123", "novaSenha", "novaSenha");

            resultado.Sucesso.Should().BeTrue();
        }

        [Fact]
        public void Ativar_DeveAtivarUsuario()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;

            usuario.Ativar();

            usuario.Ativo.Should().BeTrue();
        }

        [Fact]
        public void Desativar_DeveDesativarUsuario()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;
            usuario.Ativar();

            usuario.Desativar();

            usuario.Ativo.Should().BeFalse();
        }

        [Fact]
        public void TornarAdministrador_DeveAtivarFlag()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", null).Dados;

            usuario.TornarAdministrador();

            usuario.Administrador.Should().BeTrue();
        }

        [Fact]
        public void AdicionarEndereco_Valido_DeveAdicionar()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;
            var endereco = Endereco.Criar("Rua X", "123", usuario).Dados;

            usuario.AdicionarEndereco(endereco);

            usuario.Enderecos.Should().Contain(endereco);
        }

        [Fact]
        public void AdicionarEndereco_Nulo_DeveLancarExcecao()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;

            var acao = () => usuario.AdicionarEndereco(null);

            acao.Should().Throw<ArgumentException>().WithMessage("Endereço inválido");
        }

        [Fact]
        public void AdicionarEndereco_DeOutroUsuario_DeveLancarExcecao()
        {
            var usuario1 = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;
            var usuario2 = Usuario.Criar("Outro", "outro@email.com", "123456", 2).Dados;

            var endereco = Endereco.Criar("Rua A", "10", usuario2).Dados;

            var acao = () => usuario1.AdicionarEndereco(endereco);

            acao.Should().Throw<InvalidOperationException>().WithMessage("Endereço pertence a outro usuário");
        }

        [Fact]
        public void AdicionarServico_Valido_DeveAdicionar()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;
            var servico = Servico.Criar("Corte", "Corte de cabelo masculino", 30m, usuario).Dados;

            usuario.AdicionarServico(servico);

            usuario.Servicos.Should().Contain(servico);
        }

        [Fact]
        public void AdicionarServico_Nulo_DeveLancarExcecao()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;

            var acao = () => usuario.AdicionarServico(null);

            acao.Should().Throw<ArgumentException>().WithMessage("Serviço inválido");
        }

        [Fact]
        public void AdicionarServico_DeOutroUsuario_DeveLancarExcecao()
        {
            var usuario1 = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;
            var usuario2 = Usuario.Criar("Outro", "outro@email.com", "123456", 2).Dados;

            var servico = Servico.Criar("Barba", "Aparar barba", 25m, usuario2).Dados;

            var acao = () => usuario1.AdicionarServico(servico);

            acao.Should().Throw<InvalidOperationException>().WithMessage("Serviço pertence a outro usuário");
        }

        [Fact]
        public void AdicionarAgendamento_Valido_DeveAdicionar()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;
            var agendamento = new Agendamento();

            usuario.AdicionarAgendamento(agendamento);

            usuario.Agendamentos.Should().Contain(agendamento);
        }

        [Fact]
        public void AdicionarAgendamento_Nulo_DeveLancarExcecao()
        {
            var usuario = Usuario.Criar("João", "teste@email.com", "123456", 1).Dados;

            var acao = () => usuario.AdicionarAgendamento(null);

            acao.Should().Throw<ArgumentException>().WithMessage("Agendamento inválido");
        }
    }
}
