using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Querys
{
    public class ObterUsuariosPaginadosQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<ObterUsuariosPaginadosQuery, ResultadoPaginado<UsuarioResponse>>
    {
        public async Task<ResultadoPaginado<UsuarioResponse>> Handle(ObterUsuariosPaginadosQuery request, CancellationToken cancellationToken)
        {
            var total = await unitOfWork.CountAsync<UsuarioEntidade>();

            var usuarios = await unitOfWork.ObterTodosPaginadoAsync<UsuarioEntidade>(request.Pagina, request.LinhasPorPagina);

            var resposta = usuarios.Select(usuario => new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ativo = usuario.Ativo,
                Administrador = usuario.Administrador
            }).ToArray();

            return new ResultadoPaginado<UsuarioResponse>(resposta, total);
        }
    }

    public class ObterUsuariosPaginadosQuery : IRequest<ResultadoPaginado<UsuarioResponse>>
    {
        public int Pagina { get; set; }
        public int LinhasPorPagina { get; set; }

        public ObterUsuariosPaginadosQuery(int pagina, int linhasPorPagina)
        {
            Pagina = pagina;
            LinhasPorPagina = linhasPorPagina;
        }
    }
}
