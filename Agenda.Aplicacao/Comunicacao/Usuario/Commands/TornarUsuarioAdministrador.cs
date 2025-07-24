using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{

    public class TornarUsuarioAdministradorHandler(IUnitOfWork unitOfWork) :
       IRequestHandler<Command<TornarUsuarioAdministradorCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<TornarUsuarioAdministradorCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.Id);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                if (usuario.Administrador)
                    return new ResultadoGenerico<bool>(false, "Usuário já é administrador", false);

                usuario.TornarAdministrador();

                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Usuário promovido a administrador com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, $"Erro ao promover usuário. Desc.: {ex.Message}", false);
            }
        }
    }

    public class TornarUsuarioAdministradorCommand
    {
        public long Id { get; set; }
    }
}
