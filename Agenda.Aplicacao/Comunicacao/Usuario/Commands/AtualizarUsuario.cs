using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;
using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class AtualizarUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<AtualizarUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<AtualizarUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                // Buscar o usuário existente no banco
                var usuarioExistente = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.Id);

                if (usuarioExistente is null)
                {
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);
                }

                // Atualizar os dados
                usuarioExistente.SetNome(data.Nome);
                usuarioExistente.SetEmail(data.Email);

                // Salvar alterações
                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Usuário atualizado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, "Erro ao atualizar usuário. Desc.: " + ex.Message, false);
            }
        }
    }

    public class AtualizarUsuarioCommand
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
