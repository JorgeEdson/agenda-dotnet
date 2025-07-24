using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;
using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class AlterarSenhaUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<AlterarSenhaUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ResultadoGenerico<bool>> Handle(Command<AlterarSenhaUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.Id);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                var resultado = usuario.AlterarSenha(data.SenhaAtual, data.NovaSenha, data.ConfirmacaoSenha);

                if (!resultado.Sucesso)
                    return resultado;

                await _unitOfWork.SaveChangesAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, "Erro ao alterar senha. Desc.: " + ex.Message, false);
            }
        }
    }

    public class AlterarSenhaUsuarioCommand
    {
        public long Id { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmacaoSenha { get; set; }
    }
}
