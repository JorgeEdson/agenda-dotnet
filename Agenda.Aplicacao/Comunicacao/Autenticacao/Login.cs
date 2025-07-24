using Agenda.Aplicacao.Factory;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using Microsoft.Extensions.Configuration;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Autenticacao
{

    public class LoginHandler
        : IRequestHandler<Command<LoginCommand, ResultadoGenerico<GerarTokenJwtResponse>>, ResultadoGenerico<GerarTokenJwtResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public IConfiguration Configuration;

        public LoginHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
        }

        public async Task<ResultadoGenerico<GerarTokenJwtResponse>> Handle(Command<LoginCommand, ResultadoGenerico<GerarTokenJwtResponse>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                var usuario = await _unitOfWork.ObterUsuarioPorEmailAsync(data.Email);

                if (usuario == null)
                    return new ResultadoGenerico<GerarTokenJwtResponse>(false, "Usuário não encontrado.", null);

                if (!usuario.Ativo)
                    return new ResultadoGenerico<GerarTokenJwtResponse>(false, "Usuário inativo.", null);

                var senhaHash = EncriptadorUtil.Criptografar(data.Senha);

                var autenticado = usuario.Senha.Equals(senhaHash);

                if (!autenticado)
                    return new ResultadoGenerico<GerarTokenJwtResponse>(false, "Senha inválida.", null);

                var token = TokenJwtFactory.Criar(
                    issuer: Configuration["JWT:ISSUER"],
                    audience: Configuration["JWT:AUDIENCE"],
                    secretKey: Configuration["JWT:KEY"],
                    usuario.Id,
                    usuario.Email,
                    administrador: usuario.Administrador);

                if (!token.Sucesso)
                    return new ResultadoGenerico<GerarTokenJwtResponse>(false, token.Mensagem, null);

                return new ResultadoGenerico<GerarTokenJwtResponse>(true, "Token gerado com sucesso.", new GerarTokenJwtResponse { Token = token.Dados.JWT_TOKEN });
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<GerarTokenJwtResponse>(false, $"Falha ao gerar token de acesso: {ex.Message}.", null);
            }
        }
    }

    public class LoginCommand
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
