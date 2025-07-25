using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Entidades;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class VincularEnderecoAoUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<AdicionarEnderecoCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<AdicionarEnderecoCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                // 1. Buscar o usuário
                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.UsuarioId);
                if (usuario == null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);


                var resultadoAdicaoEndereco = usuario.AdicionarEndereco(data.Logradouro, data.Numero);
                if(!resultadoAdicaoEndereco.Sucesso)
                    return new ResultadoGenerico<bool>(false, resultadoAdicaoEndereco.Mensagem, false);
                
                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Endereço vinculado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, $"Erro ao vincular endereço: {ex.Message}", false);
            }
        }
    }

    public class AdicionarEnderecoCommand
    {
        public long UsuarioId { get; set; }
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }
}
