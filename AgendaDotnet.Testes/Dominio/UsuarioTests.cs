using Agenda.Dominio.Entidades;
using FluentAssertions;

namespace AgendaDotnet.Testes.Dominio
{
    public class UsuarioTests
    {
        [Fact]
        public void Criar_ComDadosValidos_DeveRetornarUsuarioValidoESucesso()
        {
            // Arrange
            var nome = "João Silva";
            var email = "joao.silva@example.com";
            var senha = "Senha123";
            long? id = null;

            // Act
            var resultado = Usuario.Criar(nome, email, senha, id);            

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(nome, resultado.Dados.Nome);
            Assert.Equal(email.ToLower(), resultado.Dados.Email);
            Assert.Equal(senha, resultado.Dados.Senha);
            Assert.False(resultado.Dados.Ativo);
            Assert.False(resultado.Dados.Administrador);
            Assert.True(resultado.Dados.Valido);
            Assert.Empty(resultado.Dados.Notificacoes);
        }

        [Fact]
        public void Criar_ComIdEspecifico_DeveAtribuirIdCorretamente()
        {
            // Arrange
            var nome = "Maria Souza";
            var email = "maria.souza@example.com";
            var senha = "Senha1234";
            long idEsperado = 10;

            // Act
            var resultado = Usuario.Criar(nome, email, senha, idEsperado);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(idEsperado, resultado.Dados.Id);
        }

        [Theory]
        [InlineData("", "email@test.com", "Senha123", "Nome inválido")]
        [InlineData("Nome Teste", "", "Senha123", "Email inválido")]
        [InlineData("Nome Teste", "emailinvalido", "Senha123", "Email inválido")]
        [InlineData("Nome Teste", "email@test.com", "abc", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")]
        [InlineData("Nome Teste", "email@test.com", "abcde", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")]
        [InlineData("Nome Teste", "email@test.com", "abcdef", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")] 
        [InlineData("Nome Teste", "email@test.com", "123456", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")] 
        public void Criar_ComDadosInvalidos_DeveRetornarFalhaENotificacoes(string nome, string email, string senha, string mensagemEsperada)
        {
            // Arrange & Act
            var resultado = Usuario.Criar(nome, email, senha, null);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Contains(mensagemEsperada, resultado.Mensagem);
        }

        [Fact]
        public void AlterarSenha_ComSenhaAtualCorretaENovaSenhaValida_DeveRetornarSucessoEAlterarSenha()
        {
            // Arrange
            var senhaAtual = "Senha123";
            var novaSenha = "NovaSenha456";
            var confirmacaoSenha = "NovaSenha456";
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", senhaAtual, null).Dados;

            // Act
            var resultado = usuario.AlterarSenha(senhaAtual, novaSenha, confirmacaoSenha);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Senha alterada com sucesso", resultado.Mensagem);
            Assert.Equal(novaSenha, usuario.Senha);
        }

        [Fact]
        public void AlterarSenha_ComSenhaAtualIncorreta_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var senhaAtual = "Senha123";
            var senhaIncorreta = "SenhaErrada";
            var novaSenha = "NovaSenha456";
            var confirmacaoSenha = "NovaSenha456";
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", senhaAtual, null).Dados;

            // Act
            var resultado = usuario.AlterarSenha(senhaIncorreta, novaSenha, confirmacaoSenha);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Senha atual incorreta", resultado.Mensagem);
            Assert.Equal(senhaAtual, usuario.Senha); // Senha não deve ser alterada
        }

          [Fact]
        public void AlterarSenha_ComConfirmacaoDeSenhaDiferente_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var senhaAtual = "Senha123";
            var novaSenha = "NovaSenha456";
            var confirmacaoSenha = "ConfirmacaoErrada";
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", senhaAtual, null).Dados;

            // Act
            var resultado = usuario.AlterarSenha(senhaAtual, novaSenha, confirmacaoSenha);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("Confirmação de senha não confere", resultado.Mensagem);
            Assert.Equal(senhaAtual, usuario.Senha); // Senha não deve ser alterada
        }

        [Theory]
        [InlineData("abc", "abc", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")] // Nova senha inválida
        [InlineData("123456", "123456", "Senha deve ter no mínimo 6 caracteres e conter ao menos um número")] // Nova senha inválida
        public void AlterarSenha_ComNovaSenhaInvalida_DeveAdicionarNotificacaoEAlterarSenha(string novaSenha, string confirmacaoSenha, string mensagemEsperada)
        {
            // Arrange
            var senhaAtual = "Senha123";
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", senhaAtual, null).Dados;

            // Act
            var resultado = usuario.AlterarSenha(senhaAtual, novaSenha, confirmacaoSenha);

            // Assert
            Assert.True(resultado.Sucesso); // O método de AlterarSenha em si retorna sucesso se as senhas conferem
            Assert.True(resultado.Dados);
            Assert.Equal(novaSenha, usuario.Senha); // A senha é atribuída mesmo sendo inválida
            Assert.Contains(mensagemEsperada, usuario.Notificacoes);
            Assert.False(usuario.Valido); // O usuário se torna inválido devido à senha inválida
        }

        [Fact]
        public void Ativar_DeveDefinirAtivoComoVerdadeiro()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;
            Assert.False(usuario.Ativo); // Assegura que inicia como false

            // Act
            usuario.Ativar();

            // Assert
            Assert.True(usuario.Ativo);
        }

        [Fact]
        public void Desativar_DeveDefinirAtivoComoFalso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;
            usuario.Ativar(); // Assegura que inicia como true
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
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;
            Assert.False(usuario.Administrador); // Assegura que inicia como false

            // Act
            usuario.TornarAdministrador();

            // Assert
            Assert.True(usuario.Administrador);
        }

        [Fact]
        public void AdicionarEndereco_ComDadosValidos_DeveAdicionarEnderecoARequisicaoERetornarSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;
            var logradouro = "Rua Teste";
            var numero = "123";

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

        [Theory]
        [InlineData("", "123", "Logradouro inválido")]
        [InlineData("Rua Teste", "", "Número inválido")]
        public void AdicionarEndereco_ComDadosInvalidos_DeveRetornarFalhaEMensagemDeErro(string logradouro, string numero, string mensagemEsperada)
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;

            // Act
            var resultado = usuario.AdicionarEndereco(logradouro, numero);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Contains(mensagemEsperada, resultado.Mensagem);
            Assert.Empty(usuario.Enderecos);
        }

        [Fact]
        public void AdicionarServico_ComDadosValidos_DeveAdicionarServicoARequisicaoERetornarSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;
            var nomeServico = "Corte de Cabelo";
            var descricaoServico = "Corte masculino";
            var valorServico = 30.00m;

            // Act
            var resultado = usuario.AdicionarServico(nomeServico, descricaoServico, valorServico);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Serviço adicionado com sucesso", resultado.Mensagem);
            Assert.Single(usuario.Servicos);
            Assert.Equal(nomeServico, usuario.Servicos.First().Nome);
            Assert.Equal(descricaoServico, usuario.Servicos.First().Descricao);
            Assert.Equal(valorServico, usuario.Servicos.First().Valor);
        }

        [Theory]
        [InlineData("", "Desc", 10.00, "Nome do serviço inválido")]
        [InlineData("Nome", "", 10.00, "Descrição do serviço inválida")]
        [InlineData("Nome", "Desc", 0, "Valor do serviço deve ser maior que zero")]
        [InlineData("Nome", "Desc", -5, "Valor do serviço deve ser maior que zero")]
        public void AdicionarServico_ComDadosInvalidos_DeveRetornarFalhaEMensagemDeErro(string nome, string descricao, decimal valor, string mensagemEsperada)
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", null).Dados;

            // Act
            var resultado = usuario.AdicionarServico(nome, descricao, valor);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Contains(mensagemEsperada, resultado.Mensagem);
            Assert.Empty(usuario.Servicos);
        }

        [Fact]
        public void DeterminarDisponibilidade_ComServicoPertencenteAoUsuarioEValido_DeveRetornarSucesso()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", 1).Dados;
            usuario.AdicionarServico("Corte", "Descrição", 50.00m);
            var servico = usuario.Servicos.First(); // Assume que o serviço foi adicionado com sucesso e possui IdUsuario igual ao do usuário.

            var data = DateTime.Today.AddDays(1);
            var horaInicio = new TimeSpan(9, 0, 0);
            var horaFim = new TimeSpan(10, 0, 0);

            // Act
            var resultado = usuario.DeterminarDisponibilidade(data, horaInicio, horaFim, servico);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(resultado.Dados);
            Assert.Equal("Disponibilidade determinada com sucesso", resultado.Mensagem);
        }

        [Fact]
        public void DeterminarDisponibilidade_ComServicoNaoPertencenteAoUsuario_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", 1).Dados;
            var outroUsuario = Usuario.Criar("Outro Usuário", "outro@test.com", "Senha456", 2).Dados;
            outroUsuario.AdicionarServico("Massagem", "Desc", 100.00m);
            var servicoDeOutroUsuario = outroUsuario.Servicos.First(); // Serviço que não pertence ao 'usuario'

            var data = DateTime.Today.AddDays(1);
            var horaInicio = new TimeSpan(9, 0, 0);
            var horaFim = new TimeSpan(10, 0, 0);

            // Act
            var resultado = usuario.DeterminarDisponibilidade(data, horaInicio, horaFim, servicoDeOutroUsuario);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.True(resultado.Dados); // O true aqui é estranho, mas é o que o código original retorna.
            Assert.Equal("O serviço nao pertence ao usuario", resultado.Mensagem);
        }

        // [Theory] - Seria necessário se a classe Disponibilidade.Criar tivesse validações que pudessem falhar.
        // O teste abaixo pressupõe que Disponibilidade.Criar pode falhar e propagar a mensagem.
        [Fact]
        public void DeterminarDisponibilidade_ComDadosDeDisponibilidadeInvalidos_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var usuario = Usuario.Criar("Nome Teste", "email@test.com", "Senha123", 1).Dados;
            usuario.AdicionarServico("Corte", "Descrição", 50.00m);
            var servico = usuario.Servicos.First();

            var data = DateTime.Today.AddDays(-1); // Data passada, tornando a disponibilidade inválida
            var horaInicio = new TimeSpan(9, 0, 0);
            var horaFim = new TimeSpan(8, 0, 0); // Hora fim antes da hora início

            // Nota: Para este teste funcionar de forma isolada, precisaríamos que o método
            // Disponibilidade.Criar realmente gerasse a mensagem de erro esperada com base
            // nesses dados inválidos. Estamos assumindo esse comportamento.
            // No mundo real, você mockaria Disponibilidade.Criar ou garantiria que ele falhasse.

            // Act
            var resultado = usuario.DeterminarDisponibilidade(data, horaInicio, horaFim, servico);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            // Verifica se a mensagem de erro é propagada da Disponibilidade.Criar
            Assert.Contains("Data da disponibilidade não pode ser retroativa", resultado.Mensagem);
        }

        [Fact]
        public void AgendarServico_ComDisponibilidadeValidaEOuNaoSendoProprietarioDoServico_DeveRetornarSucesso()
        {
            // Arrange
            var usuarioContratante = Usuario.Criar("Contratante", "contratante@test.com", "Senha123", 1).Dados;
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@test.com", "Senha456", 2).Dados;
            usuarioPrestador.AdicionarServico("Servico X", "Desc X", 100m);
            var servicoDoPrestador = usuarioPrestador.Servicos.First();

            // Criar uma disponibilidade válida (assumindo que Disponibilidade.Criar funciona)
            var disponibilidadeResult = Disponibilidade.Criar(DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), servicoDoPrestador);
            var disponibilidade = disponibilidadeResult.Dados;

            // Act
            var resultado = usuarioContratante.AgendarServico(disponibilidade);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal("Disponibilidade determinada com sucesso", resultado.Mensagem); // Mensagem um pouco genérica, mas segue o código
            Assert.Single(usuarioContratante.Agendamentos);
            Assert.Equal(disponibilidade, usuarioContratante.Agendamentos.First().Disponibilidade);
            Assert.Equal(usuarioContratante, usuarioContratante.Agendamentos.First().UsuarioCliente);
        }

        [Fact]
        public void AgendarServico_ComUsuarioTentandoAgendarServicoProprio_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@test.com", "Senha456", 1).Dados;
            usuarioPrestador.AdicionarServico("Servico Y", "Desc Y", 50m);
            var servicoDoPrestador = usuarioPrestador.Servicos.First();

            // Criar uma disponibilidade para o serviço do próprio prestador
            var disponibilidadeResult = Disponibilidade.Criar(DateTime.Today.AddDays(1), new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), servicoDoPrestador);
            var disponibilidade = disponibilidadeResult.Dados;

            // Act
            var resultado = usuarioPrestador.AgendarServico(disponibilidade);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            Assert.Equal("O Usuario nao pode agendar seus proprios servicos", resultado.Mensagem);
            Assert.Empty(usuarioPrestador.Agendamentos);
        }

        [Fact]
        public void AgendarServico_ComDisponibilidadeInvalida_DeveRetornarFalhaEMensagemDeErro()
        {
            // Arrange
            var usuarioContratante = Usuario.Criar("Contratante", "contratante@test.com", "Senha123", 1).Dados;
            var usuarioPrestador = Usuario.Criar("Prestador", "prestador@test.com", "Senha456", 2).Dados;
            usuarioPrestador.AdicionarServico("Servico Z", "Desc Z", 75m);
            var servicoDoPrestador = usuarioPrestador.Servicos.First();

            // Criar uma disponibilidade que seria inválida para Agendamento.Criar
            // Por exemplo, uma disponibilidade com data passada ou que já esteja ocupada (assumindo lógica em Agendamento.Criar)
            // Para este teste, vamos simular que Disponibilidade.Criar retorna um resultado inválido para Agendamento.Criar.
            // Para um teste unitário puro de Usuario, idealmente mockaríamos Agendamento.Criar para retornar falha.
            // Dado que não estamos usando mocks aqui, vamos simular uma situação que levaria Agendamento.Criar a falhar.
            // Exemplo: uma disponibilidade que, para a lógica de Agendamento, seria inválida (por exemplo, data no passado)
            // No entanto, a classe de Agendamento não é fornecida, então vamos simular um cenário simples onde
            // `Agendamento.Criar` retornaria falha por algum motivo interno (ex: validações da própria Agendamento).
            // A melhor prática aqui seria mockar Agendamento.Criar. Sem mocks, dependemos da implementação de Agendamento.

            // Para simular a falha, vamos criar uma disponibilidade "válida" sinteticamente, mas a ideia é que
            // a validação falhe *dentro* de Agendamento.Criar.
            // Aqui, apenas faremos um caso onde a disponibilidade por si só já não seria utilizável,
            // mas o ideal é que Agendamento.Criar tenha as validações.

            // Vamos forçar uma falha no Agendamento.Criar. Isso exigiria mockar Agendamento.Criar
            // ou ter um construtor de teste em Agendamento que pudesse ser instanciado com erros.
            // Sem Mocks, é difícil testar a linha `if(!agendamentoResult.Sucesso)`.

            // Para este teste, assumiremos que se a disponibilidade.Servico for null, o Agendamento.Criar falharia.
            // Isso não é real para o seu código, então a forma mais robusta é com mocks.

            // --- REFAZENDO ESTE TESTE COM MELHOR INTENÇÃO DADA AS LIMITAÇÕES ---
            // A linha `if(!agendamentoResult.Sucesso)` significa que precisamos de uma forma de
            // fazer `Agendamento.Criar` retornar `Sucesso = false`.
            // Sem mocks, isso exigiria que `Agendamento.Criar` falhe com base em algum input inválido
            // na `Disponibilidade`. Para este exemplo, farei uma Disponibilidade que,
            // se tivesse validação para `Servico` ser `null`, falharia.

            // Crio uma Disponibilidade "inválida" para o Agendamento (ex: sem serviço associado, se isso fosse uma validação)
            // Note: O código de Agendamento.Criar não foi fornecido, então esta é uma suposição.
            var disponibilidadeSemServico = (Disponibilidade)Activator.CreateInstance(typeof(Disponibilidade), true); // Cria instância sem invocar construtor público
            typeof(Disponibilidade).GetProperty("Servico").SetValue(disponibilidadeSemServico, null); // Força um serviço nulo para simular falha interna no Agendamento.Criar

            // Act
            var resultado = usuarioContratante.AgendarServico(disponibilidadeSemServico);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.False(resultado.Dados);
            // A mensagem de erro esperada viria do Agendamento.Criar
            // Aqui estou colocando uma mensagem genérica, pois não sei qual seria a de Agendamento.
            // Num teste real, você testaria a mensagem específica retornada por Agendamento.Criar.
            Assert.Contains("Erro: Serviço da disponibilidade não pode ser nulo", resultado.Mensagem); // Exemplo de mensagem que viria do Agendamento.Criar
            Assert.Empty(usuarioContratante.Agendamentos);
        }
    }
}
