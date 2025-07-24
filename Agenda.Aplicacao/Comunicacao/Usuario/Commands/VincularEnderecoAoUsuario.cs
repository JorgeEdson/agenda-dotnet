using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Entidades;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using static Agenda.Aplicacao.Factory.CommandFactory;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Commands
{
    public class VincularEnderecoAoUsuarioHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<Command<VincularEnderecoAoUsuarioCommand, ResultadoGenerico<bool>>, ResultadoGenerico<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoGenerico<bool>> Handle(Command<VincularEnderecoAoUsuarioCommand, ResultadoGenerico<bool>> request, CancellationToken cancellationToken)
        {
            try
            {
                var data = request.Data;

                // 1. Buscar o usuário
                var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(data.UsuarioId);
                if (usuario == null)
                    return new ResultadoGenerico<bool>(false, "Usuário não encontrado", false);

                // 2. Criar o endereço com lógica de domínio
                var resultadoEndereco = Endereco.Criar(data.Logradouro, data.Numero, usuario);
                if (!resultadoEndereco.Sucesso)
                    return new ResultadoGenerico<bool>(false, resultadoEndereco.Mensagem, false);

                var endereco = resultadoEndereco.Dados;

                // 3. Adicionar à coleção de endereços do usuário
                usuario.AdicionarEndereco(endereco); // ← já existe no domínio

                // 4. Persistir
                await _unitOfWork.SaveChangesAsync();

                return new ResultadoGenerico<bool>(true, "Endereço vinculado com sucesso", true);
            }
            catch (Exception ex)
            {
                return new ResultadoGenerico<bool>(false, $"Erro ao vincular endereço: {ex.Message}", false);
            }
        }
    }

    public class VincularEnderecoAoUsuarioCommand
    {
        public long UsuarioId { get; set; }
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }
}
