using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Entidades;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class VincularServicoAoUsuarioHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<Command<VincularServicoAoUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<VincularServicoAoUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.IdUsuario);

                if (usuario is null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                var resultadoServico = Servico.Criar(data.Nome, data.Descricao, data.Valor, usuario);

                if (!resultadoServico.Sucesso)
                    return new ResultadoGenerico<bool>(false, resultadoServico.Mensagem, false);

                usuario.AdicionarServico(resultadoServico.Dados);

                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Serviço vinculado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, "Erro ao vincular serviço: " + ex.Message, false);
            }
        }
    }


    public class VincularServicoAoUsuarioCommand
    {
        public long IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}
