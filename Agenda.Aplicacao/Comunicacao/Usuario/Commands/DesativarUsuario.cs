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
                var data = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.Id);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                if (!usuario.Ativo)
                    return new ResultadoGenerico<bool>(false, "Usuário já está desativado", false);

                usuario.Desativar();

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
        public long Id { get; set; }
    }
}
