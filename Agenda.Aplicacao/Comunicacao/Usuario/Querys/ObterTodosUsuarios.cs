using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;
using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Querys
{
    public class ObterTodosUsuariosQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ObterTodosUsuariosQuery, ResultadoGenerico<List<UsuarioResponse>>>
    {
        public async Task<ResultadoGenerico<List<UsuarioResponse>>> Handle(ObterTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            var usuarios = await unitOfWork.ObterTodosAsync<UsuarioEntidade>();

            var resposta = usuarios.Select(usuario => new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ativo = usuario.Ativo,
                Administrador = usuario.Administrador
            }).ToList();

            return new ResultadoGenerico<List<UsuarioResponse>>(true, "Usuários encontrados com sucesso!", resposta);
        }
    }

    public class ObterTodosUsuariosQuery : IRequest<ResultadoGenerico<List<UsuarioResponse>>> { }
}
