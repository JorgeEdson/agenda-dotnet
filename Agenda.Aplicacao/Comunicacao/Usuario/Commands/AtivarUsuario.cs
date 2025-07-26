using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;
using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class AtivarUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<AtivarUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<AtivarUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var requisicao = request.Data;

                var usuarioAdministrador = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(requisicao.IdUsuarioAdministrador);

                if (usuarioAdministrador is null)
                    return new ResultadoGenerico<bool>(false, "Usuário Administrador não encontrado", false);

                var usuarioParaAtivar = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(requisicao.IdUsuarioParaAtivar);

                if (usuarioParaAtivar is null)
                    return new ResultadoGenerico<bool>(false, "Usuário para Ativação não encontrado", false);

                var resultadoAtivacao = usuarioAdministrador.AtivarUsuario(usuarioParaAtivar);

                if(!resultadoAtivacao.Sucesso)
                    return new ResultadoGenerico<bool>(false, resultadoAtivacao.Mensagem, false);

                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Usuário ativado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, $"Erro ao ativar usuário. Desc.: {ex.Message}", false);
            }
        }
    }


    public class AtivarUsuarioCommand
    {
        public long IdUsuarioAdministrador { get; set; }
        public long IdUsuarioParaAtivar { get; set; }
    }
}
