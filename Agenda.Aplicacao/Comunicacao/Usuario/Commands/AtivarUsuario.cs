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
                var data = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.Id);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                if (usuario.Ativo)
                    return new ResultadoGenerico<bool>(false, "Usuário já está ativo", false);

                usuario.Ativar();

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
        public long Id { get; set; }
    }
}
