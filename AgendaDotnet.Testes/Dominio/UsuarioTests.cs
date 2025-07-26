using Agenda.Dominio.Entidades;
using FluentAssertions;
using System.Reflection;

namespace AgendaDotnet.Testes.Dominio
{
    public class UsuarioTests
    {
        // Testes para o método Criar
        [Fact]
        public void Criar_ComDadosValidos_DeveRetornarSucessoEUsuarioValido()
        {
            // Arrange
            string nome = "João Silva";
            string email = "joao.silva@email.com";
            string senha = "Senha123";
            long? id = 1;

            // Act
            var resultado = Usuario.Criar(nome, email, senha, id);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(nome, resultado.Dados.Nome);
            Assert.Equal(email.ToLower(), resultado.Dados.Email);
            Assert.Equal(senha, resultado.Dados.Senha);
            Assert.Equal(id, resultado.Dados.Id);
            Assert.False(resultado.Dados.Ativo);
            Assert.False(resultado.Dados.Administrador);
            Assert.True(resultado.Dados.Valido);
        }

        [Fact]
        public void Criar_ComNomeInvalido_DeveRetornarFalha()
        {
            // Arrange
            string nome = "";
            string email = "joao.silva@email.com";
            string senha = "Senha123";

            // Act
            var resultado = Usuario.Criar(nome, email, senha, null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("Nome inválido", resultado.Mensagem);
            Assert.Null(resultado.Dados);
        }

        [Theory]
        [InlineData("emailinvalido")]
        [InlineData("email@")]
        [InlineData("@dominio.com")]
        [InlineData(" ")]
        public void Criar_ComEmailInvalido_DeveRetornarFalha(string emailInvalido)
        {
            // Arrange
            string nome = "João Silva";
            string senha = "Senha123";

            // Act
            var resultado = Usuario.Criar(nome, emailInvalido, senha, null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("Email inválido", resultado.Mensagem);
            Assert.Null(resultado.Dados);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("senhaseletras")]
        [InlineData("123456")] 
        [InlineData("letrass")]
        [InlineData(" ")]
        public void Criar_ComSenhaInvalida_DeveRetornarFalha(string senhaInvalida)
        {
            // Arrange
            string nome = "João Silva";
            string email = "joao.silva@email.com";

            // Act
            var resultado = Usuario.Criar(nome, email, senhaInvalida, null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains("Senha deve ter no mínimo 6 caracteres e conter ao menos um número", resultado.Mensagem);
            Assert.Null(resultado.Dados);
        }

        // Testes para SetNome
        [Fact]
        public void SetNome_ComNomeValido_DeveAtualizarNome()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Antigo", "teste@teste.com", "Senha123", null).Dados;
            string novoNome = "Novo Nome Teste";

            // Act
            usuario.SetNome(novoNome);

            // Assert
            Assert.Equal(novoNome, usuario.Nome);
            Assert.True(usuario.Valido);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void SetNome_ComNomeInvalido_DeveAdicionarNotificacao(string nomeInvalido)
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Antigo", "teste@teste.com", "Senha123", null).Dados;

            // Act
            usuario.SetNome(nomeInvalido);

            // Assert
            Assert.False(usuario.Valido);
            Assert.Contains("Nome inválido", usuario.Notificacoes);
        }

        // Testes para SetEmail
        [Fact]
        public void SetEmail_ComEmailValido_DeveAtualizarEmail()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "antigo@email.com", "Senha123", null).Dados;
            string novoEmail = "novo.email@teste.com";

            // Act
            usuario.SetEmail(novoEmail);

            // Assert
            Assert.Equal(novoEmail.ToLower(), usuario.Email);
            Assert.True(usuario.Valido);
        }

        [Theory]
        [InlineData("emailinvalido")]
        [InlineData("email@")]
        [InlineData("@dominio.com")]
        [InlineData(" ")]
        public void SetEmail_ComEmailInvalido_DeveAdicionarNotificacao(string emailInvalido)
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "valido@email.com", "Senha123", null).Dados;

            // Act
            usuario.SetEmail(emailInvalido);

            // Assert
            Assert.False(usuario.Valido);
            Assert.Contains("Email inválido", usuario.Notificacoes);
        }

        // Testes para AlterarSenha
        [Fact]
        public void AlterarSenha_ComSenhaAtualCorretaENovasSenhasConferem_DeveAlterarSenhaComSucesso()
        {
            // Arrange
            string senhaInicial = "Senha123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaInicial, null).Dados;
            string novaSenha = "NovaSenha456";
            string confirmacaoSenha = "NovaSenha456";

            // Act
            var resultado = usuario.AlterarSenha(senhaInicial, novaSenha, confirmacaoSenha);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Senha alterada com sucesso", resultado.Mensagem);
            Assert.Equal(novaSenha, usuario.Senha);
            Assert.True(usuario.Valido); // Verifica se a SetSenha não adicionou notificações de erro
        }

        [Fact]
        public void AlterarSenha_ComSenhaAtualIncorreta_DeveRetornarFalha()
        {
            // Arrange
            string senhaInicial = "Senha123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaInicial, null).Dados;
            string senhaAtualIncorreta = "SenhaErrada";
            string novaSenha = "NovaSenha456";
            string confirmacaoSenha = "NovaSenha456";

            // Act
            var resultado = usuario.AlterarSenha(senhaAtualIncorreta, novaSenha, confirmacaoSenha);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Senha atual incorreta", resultado.Mensagem);
            Assert.Equal(senhaInicial, usuario.Senha); // Senha não deve ser alterada
        }

        [Fact]
        public void AlterarSenha_ComNovasSenhasNaoConferem_DeveRetornarFalha()
        {
            // Arrange
            string senhaInicial = "Senha123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaInicial, null).Dados;
            string novaSenha = "NovaSenha456";
            string confirmacaoSenhaDiferente = "NovaSenha789";

            // Act
            var resultado = usuario.AlterarSenha(senhaInicial, novaSenha, confirmacaoSenhaDiferente);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Confirmação de senha não confere", resultado.Mensagem);
            Assert.Equal(senhaInicial, usuario.Senha); // Senha não deve ser alterada
        }

        [Fact]
        public void AlterarSenha_ComNovaSenhaInvalida_DeveRetornarSucessoNaOperacaoMasAdicionarNotificacaoNoUsuario()
        {
            // Arrange
            string senhaInicial = "Senha123";
            var usuario = Usuario.Criar("Nome", "email@email.com", senhaInicial, null).Dados;
            string novaSenhaInvalida = "abc"; // Senha muito curta e sem número
            string confirmacaoSenha = "abc";

            // Act
            var resultado = usuario.AlterarSenha(senhaInicial, novaSenhaInvalida, confirmacaoSenha);

            // Assert
            Assert.True(resultado.Sucesso); // A operação de AlterarSenha em si teve sucesso
            Assert.True(resultado.Dados);
            Assert.Equal("Senha alterada com sucesso", resultado.Mensagem);
            Assert.Equal(novaSenhaInvalida, usuario.Senha);
            Assert.False(usuario.Valido); // Mas a senha definida no usuário é inválida
            Assert.Contains("Senha deve ter no mínimo 6 caracteres e conter ao menos um número", usuario.Notificacoes);
        }

        // Testes para TornarAdministrador
        [Fact]
        public void TornarAdministrador_DeveDefinirAdministradorComoTrue()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", null).Dados;
            Assert.False(usuario.Administrador);

            // Act
            usuario.TornarAdministrador();

            // Assert
            Assert.True(usuario.Administrador);
        }

        // Testes para DesativarUsuario
        [Fact]
        public void DesativarUsuario_ComoAdministradorEDesativarUsuarioAtivo_DeveDesativarComSucesso()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();
            var usuarioADesativar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;

            // Simular que o usuário está ativo (acesso via Reflection para método privado)
            var ativarMethod = typeof(Usuario).GetMethod("Ativar", BindingFlags.NonPublic | BindingFlags.Instance);
            ativarMethod.Invoke(usuarioADesativar, null);
            Assert.True(usuarioADesativar.Ativo);

            // Act
            var resultado = admin.DesativarUsuario(usuarioADesativar);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Usuário desativado com sucesso", resultado.Mensagem);
            Assert.False(usuarioADesativar.Ativo);
        }

        [Fact]
        public void DesativarUsuario_ComoNaoAdministrador_DeveRetornarFalha()
        {
            // Arrange
            var naoAdmin = Usuario.Criar("NaoAdmin", "naoadmin@email.com", "Senha123", null).Dados;
            var usuarioADesativar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;
            var ativarMethod = typeof(Usuario).GetMethod("Ativar", BindingFlags.NonPublic | BindingFlags.Instance);
            ativarMethod.Invoke(usuarioADesativar, null);


            // Act
            var resultado = naoAdmin.DesativarUsuario(usuarioADesativar);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Somente administradores podem desativar usuários", resultado.Mensagem);
            Assert.True(usuarioADesativar.Ativo); // Usuário deve permanecer ativo
        }

        [Fact]
        public void DesativarUsuario_ComUsuarioInvalido_DeveRetornarFalha()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();

            // Act
            var resultado = admin.DesativarUsuario(null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Usuário inválido", resultado.Mensagem);
        }

        [Fact]
        public void DesativarUsuario_QuandoUsuarioJaDesativado_DeveRetornarFalha()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();
            var usuarioADesativar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;
            Assert.False(usuarioADesativar.Ativo); // Começa desativado por padrão

            // Act
            var resultado = admin.DesativarUsuario(usuarioADesativar);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Usuário já está desativado", resultado.Mensagem);
            Assert.False(usuarioADesativar.Ativo);
        }

        // Testes para AtivarUsuario
        [Fact]
        public void AtivarUsuario_ComoAdministradorEAtivarUsuarioInativo_DeveAtivarComSucesso()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();
            var usuarioAAtivar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;
            Assert.False(usuarioAAtivar.Ativo);

            // Act
            var resultado = admin.AtivarUsuario(usuarioAAtivar);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Usuário ativado com sucesso", resultado.Mensagem);
            Assert.True(usuarioAAtivar.Ativo);
        }

        [Fact]
        public void AtivarUsuario_ComoNaoAdministrador_DeveRetornarFalha()
        {
            // Arrange
            var naoAdmin = Usuario.Criar("NaoAdmin", "naoadmin@email.com", "Senha123", null).Dados;
            var usuarioAAtivar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;
            Assert.False(usuarioAAtivar.Ativo);

            // Act
            var resultado = naoAdmin.AtivarUsuario(usuarioAAtivar);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Somente administradores podem Ativar usuários", resultado.Mensagem);
            Assert.False(usuarioAAtivar.Ativo); // Usuário deve permanecer inativo
        }

        [Fact]
        public void AtivarUsuario_ComUsuarioInvalido_DeveRetornarFalha()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();

            // Act
            var resultado = admin.AtivarUsuario(null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Usuário inválido", resultado.Mensagem);
        }

        [Fact]
        public void AtivarUsuario_QuandoUsuarioJaAtivo_DeveRetornarFalha()
        {
            // Arrange
            var admin = Usuario.Criar("Admin", "admin@email.com", "Admin123", null).Dados;
            admin.TornarAdministrador();
            var usuarioAAtivar = Usuario.Criar("Usuario", "usuario@email.com", "Usuario123", null).Dados;
            var ativarMethod = typeof(Usuario).GetMethod("Ativar", BindingFlags.NonPublic | BindingFlags.Instance);
            ativarMethod.Invoke(usuarioAAtivar, null);
            Assert.True(usuarioAAtivar.Ativo);

            // Act
            var resultado = admin.AtivarUsuario(usuarioAAtivar);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Usuário já está ativo", resultado.Mensagem);
            Assert.True(usuarioAAtivar.Ativo);
        }

        // Testes para AdicionarEndereco
        [Fact]
        public void AdicionarEndereco_ComDadosValidos_DeveAdicionarEnderecoComSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            string logradouro = "Rua Teste";
            string numero = "123";

            // Act
            var resultado = usuario.AdicionarEndereco(logradouro, numero);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Endereco adicionado com sucesso", resultado.Mensagem);
            Assert.Single(usuario.Enderecos);
            Assert.Equal(logradouro, usuario.Enderecos.First().Logradouro);
            Assert.Equal(numero, usuario.Enderecos.First().Numero);
        }

        [Fact]
        public void AdicionarEndereco_ComDadosInvalidos_NaoDeveAdicionarEnderecoERetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            string logradouro = "";
            string numero = "";

            // Act
            var resultado = usuario.AdicionarEndereco(logradouro, numero);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Contains("Logradouro inválido", resultado.Mensagem);
            Assert.Contains("Número inválido", resultado.Mensagem);
            Assert.Empty(usuario.Enderecos);
        }

        // Testes para AdicionarServico
        [Fact]
        public void AdicionarServico_ComDadosValidosEEnderecoAtivo_DeveAdicionarServicoComSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua do Serviço", "456"); // Adiciona um endereço para o serviço
            var enderecoParaServico = usuario.Enderecos.First();

            string nomeServico = "Corte de Cabelo";
            string descricaoServico = "Corte masculino";
            decimal valorServico = 30.00m;

            // Act
            var resultado = usuario.AdicionarServico(nomeServico, descricaoServico, valorServico, enderecoParaServico);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Serviço adicionado com sucesso", resultado.Mensagem);
            Assert.Single(usuario.Servicos);
            Assert.Equal(nomeServico, usuario.Servicos.First().Nome);
            Assert.Equal(enderecoParaServico.Id, usuario.Servicos.First().IdEndereco);
        }

        [Fact]
        public void AdicionarServico_ComEnderecoDesativado_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua do Serviço", "456"); // Adiciona um endereço
            var enderecoDesativado = usuario.Enderecos.First();
            enderecoDesativado.Desativar(); // Desativa o endereço

            string nomeServico = "Corte de Cabelo";
            string descricaoServico = "Corte masculino";
            decimal valorServico = 30.00m;

            // Act
            var resultado = usuario.AdicionarServico(nomeServico, descricaoServico, valorServico, enderecoDesativado);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("O endereço para este serviço esta desativado", resultado.Mensagem);
            Assert.Empty(usuario.Servicos);
        }

        [Fact]
        public void AdicionarServico_ComDadosInvalidos_NaoDeveAdicionarServicoERetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua do Serviço", "456");
            var enderecoAtivo = usuario.Enderecos.First();

            string nomeServico = ""; 
            string descricaoServico = "Descrição";
            decimal valorServico = 0m;

            // Act
            var resultado = usuario.AdicionarServico(nomeServico, descricaoServico, valorServico, enderecoAtivo);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Contains("Nome do serviço inválido", resultado.Mensagem);
            Assert.Contains("Valor do serviço não pode ser negativo ou zero", resultado.Mensagem);
            Assert.Empty(usuario.Servicos);
        }

        // Testes para AgendarServico
        [Fact]
        public void RealizarAgendamento_ComServicoDeOutroUsuario_DeveAgendarComSucesso()
        {
            // Arrange
            var usuarioCliente = Usuario.Criar("Cliente", "cliente@email.com", "Senha123", 1L).Dados;
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@email.com", "Senha123", 2L).Dados;
            usuarioPrestador.AdicionarEndereco("Rua do Prestador", "100");
            var enderecoPrestador = usuarioPrestador.Enderecos.First();
            var servico = Servico.Criar("Servico do Prestador", "Desc", 50m, usuarioPrestador, enderecoPrestador).Dados;

            
            var dataDisponibilidade = DateTime.Today.AddDays(1); // Exemplo: amanhã
            var horaInicio = new TimeSpan(9, 0, 0); // 09:00
            var horaFim = new TimeSpan(10, 0, 0);   // 10:00

            var disponibilidade = Disponibilidade.Criar(dataDisponibilidade, horaInicio, horaFim, servico).Dados;

            // Act
            var resultado = usuarioCliente.RealizarAgendamento(disponibilidade);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Disponibilidade determinada com sucesso", resultado.Mensagem);
            
        }

        [Fact]
        public void RealizarAgendamento_ComServicoDoProprioUsuario_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Usuario", "usuario@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua Propria", "10");
            var endereco = usuario.Enderecos.First();
            var servicoDoProprioUsuario = Servico.Criar("Meu Serviço", "Desc", 50m, usuario, endereco).Dados;

            // --- MUDANÇA AQUI: Criando TimeSpan para horaInicio e horaFim ---
            var dataDisponibilidade = DateTime.Today.AddDays(1);
            var horaInicio = new TimeSpan(14, 0, 0); // 14:00
            var horaFim = new TimeSpan(15, 0, 0);   // 15:00

            var disponibilidade = Disponibilidade.Criar(dataDisponibilidade, horaInicio, horaFim, servicoDoProprioUsuario).Dados;

            // Act
            var resultado = usuario.RealizarAgendamento(disponibilidade);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("O Usuario nao pode agendar seus proprios servicos", resultado.Mensagem);
        }

        [Fact]
        public void CriarDisponibilidade_ComHoraFimMenorQueInicio_DeveRetornarFalha()
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);
            var horaInicio = new TimeSpan(10, 0, 0);
            var horaFim = new TimeSpan(9, 0, 0); // inválido
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@email.com", "Senha123", 1L).Dados;
            var usuarioCliente = Usuario.Criar("Cliente", "cliente@email.com", "Cliente123", 2L).Dados;

            usuarioPrestador.AdicionarEndereco("Rua do Prestador", "100");
            var enderecoPrestador = usuarioPrestador.Enderecos.First();

            usuarioPrestador.AdicionarServico("Servico do Prestador", "descrição teste", 50m, enderecoPrestador);
            var servico = usuarioPrestador.Servicos.First();

            //Act
            var resultadoDeterminar = servico.DeterminarDisponibilidade(data, horaInicio, horaFim);
            // Assert
            Assert.False(resultadoDeterminar.Sucesso);
            Assert.False(resultadoDeterminar.Dados);
            Assert.Contains("Hora de fim deve ser posterior à hora de início", resultadoDeterminar.Mensagem);
        }

        // Testes para AtivarEndereco
        [Fact]
        public void AtivarEndereco_ComIdExistenteEDesativado_DeveAtivarComSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua A", "1");
            var endereco = usuario.Enderecos.First();
            endereco.Desativar();
            Assert.False(endereco.Ativo);

            // Act
            var resultado = usuario.AtivarEndereco(endereco.Id);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Endereço ativado com sucesso", resultado.Mensagem);
            Assert.True(endereco.Ativo);
        }

        [Fact]
        public void AtivarEndereco_ComIdNaoExistente_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            long idNaoExistente = 999;

            // Act
            var resultado = usuario.AtivarEndereco(idNaoExistente);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Endereço não encontrado", resultado.Mensagem);
        }

        [Fact]
        public void AtivarEndereco_QuandoEnderecoJaAtivo_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua A", "1");
            var endereco = usuario.Enderecos.First();
            Assert.True(endereco.Ativo); // Por padrão, endereços são criados ativos

            // Act
            var resultado = usuario.AtivarEndereco(endereco.Id);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Endereço já está ativo", resultado.Mensagem);
        }

        // Testes para DesativarEndereco
        [Fact]
        public void DesativarEndereco_ComIdExistenteEAtivoESemServicoVinculado_DeveDesativarComSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua B", "2");
            var endereco = usuario.Enderecos.First();
            Assert.True(endereco.Ativo);

            // Act
            var resultado = usuario.DesativarEndereco(endereco.Id);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Endereço desativado com sucesso", resultado.Mensagem);
            Assert.False(endereco.Ativo);
        }

        [Fact]
        public void DesativarEndereco_ComIdNaoExistente_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            long idNaoExistente = 999;

            // Act
            var resultado = usuario.DesativarEndereco(idNaoExistente);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Endereço não encontrado", resultado.Mensagem);
        }

        [Fact]
        public void DesativarEndereco_QuandoEnderecoJaDesativado_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua B", "2");
            var endereco = usuario.Enderecos.First();
            endereco.Desativar();
            Assert.False(endereco.Ativo);

            // Act
            var resultado = usuario.DesativarEndereco(endereco.Id);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Endereço já está desativado", resultado.Mensagem);
        }

        [Fact]
        public void DesativarEndereco_QuandoEnderecoVinculadoAServico_DeveRetornarFalha()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome", "email@email.com", "Senha123", 1L).Dados;
            usuario.AdicionarEndereco("Rua do Servico", "789");
            var enderecoVinculado = usuario.Enderecos.First();
            usuario.AdicionarServico("Servico", "Desc", 100m, enderecoVinculado);

            // Act
            var resultado = usuario.DesativarEndereco(enderecoVinculado.Id);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Não é possível desativar o endereço pois está vinculado a um ou mais serviços", resultado.Mensagem);
            Assert.True(enderecoVinculado.Ativo); // Deve permanecer ativo
        }

        
        [Fact]
        public void AceitarAgendamento_ComIdExistenteEAptoParaAceite_DeveAceitarComSucesso()
        {
            // Arrange
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@email.com", "Senha123", 1L).Dados;
            var usuarioCliente = Usuario.Criar("Cliente", "cliente@email.com", "Cliente123", 2L).Dados;

            usuarioPrestador.AdicionarEndereco("Rua do Prestador", "100");
            var enderecoPrestador = usuarioPrestador.Enderecos.First();

            usuarioPrestador.AdicionarServico("Servico do Prestador", "descrição teste", 50m,enderecoPrestador);
            var servico = usuarioPrestador.Servicos.First();

            servico.DeterminarDisponibilidade(DateTime.Today.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));
            var disponibilidadeServico = servico.Disponibilidades.First();

            //Act
            var resultadoAgendamento = usuarioCliente.RealizarAgendamento(disponibilidadeServico);
            var agendamento = usuarioPrestador.Agendamentos.First(); 
            
            var resultadoAceite = usuarioPrestador.AceitarAgendamento(agendamento.Id);

            // Assert
            Assert.True(resultadoAceite.Sucesso);
            Assert.True(resultadoAceite.Dados);
            Assert.Equal("Agendamento aceito com sucesso", resultadoAceite.Mensagem);
            Assert.True(agendamento.Aceito);
        }

        [Fact]
        public void AceitarAgendamento_ComIdNaoExistente_DeveRetornarFalha()
        {
            // Arrange
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@email.com", "Senha123", 1L).Dados;
            long idNaoExistente = 999;

            // Act
            var resultado = usuarioPrestador.AceitarAgendamento(idNaoExistente);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Agendamento não encontrado", resultado.Mensagem);
        }

        [Fact]
        public void AceitarAgendamento_QuandoOutroUsuarioAceitaAgendamento_DeveRetornarFalha()
        {
            // Arrange
            var usuarioQueNaoEPrestador = Usuario.Criar("NaoPrestador", "nao.prestador@email.com", "Senha123", 1L).Dados;
            var usuarioPrestadorReal = Usuario.Criar("PrestadorReal", "prestador.real@email.com", "Senha123", 2L).Dados;
            var usuarioCliente = Usuario.Criar("Cliente", "cliente@email.com", "Cliente123", 3L).Dados;

            usuarioPrestadorReal.AdicionarEndereco("Rua do Prestador", "100");
            var enderecoPrestadorReal = usuarioPrestadorReal.Enderecos.First();

            usuarioPrestadorReal.AdicionarServico("Servico do Prestador", "descrição teste", 50m, enderecoPrestadorReal);
            var servicoPrestadorReal = usuarioPrestadorReal.Servicos.First();

            servicoPrestadorReal.DeterminarDisponibilidade(DateTime.Today.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));
            var disponibilidadeServico = servicoPrestadorReal.Disponibilidades.First();

            //Act
            var resultadoAgendamento = usuarioCliente.RealizarAgendamento(disponibilidadeServico);
            var agendamento = usuarioPrestadorReal.Agendamentos.First();

            var resultadoAceite = usuarioQueNaoEPrestador.AceitarAgendamento(agendamento.Id);

            // Assert
            Assert.False(resultadoAceite.Sucesso);
            Assert.False(resultadoAceite.Dados);
            Assert.Equal("Agendamento não encontrado", resultadoAceite.Mensagem);
            Assert.False(agendamento.Aceito); 
        }


        [Fact]
        public void AceitarAgendamento_QuandoJaAceitoAnteriormente_DeveRetornarFalha()
        {
            // Arrange
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@email.com", "Senha123", 1L).Dados;
            var usuarioCliente = Usuario.Criar("Cliente", "cliente@email.com", "Cliente123", 2L).Dados;

            usuarioPrestador.AdicionarEndereco("Rua do Prestador", "100");
            var enderecoPrestador = usuarioPrestador.Enderecos.First();
            var servico = Servico.Criar("Servico do Prestador", "Desc", 50m, usuarioPrestador, enderecoPrestador).Dados;

            // --- CORRECTION HERE: Creating DateTime and TimeSpans for availability ---
            var dataDisponibilidade = DateTime.Today.AddDays(1); // Date for the availability
            var horaInicio = new TimeSpan(13, 0, 0); // 13:00 (1 PM)
            var horaFim = new TimeSpan(14, 0, 0);   // 14:00 (2 PM)

            var disponibilidade = Disponibilidade.Criar(dataDisponibilidade, horaInicio, horaFim, servico).Dados;

            var agendamentoResult = Agendamento.Criar(disponibilidade, usuarioCliente);
            var agendamento = agendamentoResult.Dados;
            agendamento.AceitarAgendamento(); // Accept the appointment beforehand

            // Use Reflection to add the appointment to the _agendamentos list of the provider
            // as there's no public method for it in the Usuario class
            var agendamentosField = typeof(Usuario).GetField("_agendamentos", BindingFlags.NonPublic | BindingFlags.Instance);
            var agendamentosList = agendamentosField.GetValue(usuarioPrestador) as List<Agendamento>;
            agendamentosList.Add(agendamento);

            Assert.True(agendamento.Aceito); // Assert that it was indeed accepted initially

            // Act
            var resultado = usuarioPrestador.AceitarAgendamento(agendamento.Id);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Este agendamento já foi aceito anteriormente", resultado.Mensagem);
            Assert.True(agendamento.Aceito); // It should still be accepted
        }
    }
}
