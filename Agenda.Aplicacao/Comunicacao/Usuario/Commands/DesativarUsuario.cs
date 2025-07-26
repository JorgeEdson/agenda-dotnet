using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class DesativarUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<DesativarUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<DesativarUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var requisicao = request.Data;

                var usuarioAdministrador = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(requisicao.IdAdministrador);

                if (usuarioAdministrador is null)
                    return new ResultadoGenerico<bool>(false, "Usuário administrador não encontrado", false);

                var usuarioDesativacao = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(requisicao.IdUsuarioDesativacao);

                if (usuarioDesativacao is null)
                    return new ResultadoGenerico<bool>(false, "Usuário de desativação não encontrado", false);

                if (!usuarioDesativacao.Ativo)
                    return new ResultadoGenerico<bool>(false, "Usuário de desativação já está desativado", false);

                usuarioAdministrador.DesativarUsuario(usuarioDesativacao);

                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Usuário desativado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, $"Erro ao desativar usuário. Desc.: {ex.Message}", false);
            }
        }
    }


    public class DesativarUsuarioCommand
    {
        public long IdAdministrador { get; set; }

        public long IdUsuarioDesativacao { get; set; }
    }
}
