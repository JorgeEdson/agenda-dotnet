using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Entidades;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class AdicionarServicoHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<Command<AdicionarServicoCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<AdicionarServicoCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var requisicao = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(requisicao.IdUsuario);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                var resultadoAdicaoServico = usuario.AdicionarServico(requisicao.Nome, requisicao.Descricao, requisicao.Valor);

                if(!resultadoAdicaoServico.Sucesso)
                    return new ResultadoGenerico<bool>(false, resultadoAdicaoServico.Mensagem, false);

                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Serviço vinculado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, "Erro ao vincular serviço: " + ex.Message, false);
            }
        }
    }


    public class AdicionarServicoCommand
    {
        public long IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}
