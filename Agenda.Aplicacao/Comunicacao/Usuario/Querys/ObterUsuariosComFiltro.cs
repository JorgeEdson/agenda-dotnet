using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Querys
{

    public class ObterUsuariosComFiltroHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<ObterUsuariosComFiltroQuery, ResultadoPaginado<UsuarioResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultadoPaginado<UsuarioResponse>> Handle(ObterUsuariosComFiltroQuery request, CancellationToken cancellationToken)
        {
            var usuarios = await _unitOfWork.ObterUsuariosComFiltroAsync(
            request.Nome,
            request.Email,
            request.Ativo,
            request.Administrador,
            request.Pagina,
            request.LinhasPorPagina
        );

            // Total real (não apenas página atual)
            var total = await _unitOfWork.CountAsync<UsuarioEntidade>();

            var resposta = usuarios
                .Select(u => new UsuarioResponse
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Ativo = u.Ativo,
                    Administrador = u.Administrador
                })
                .ToArray();

            return new ResultadoPaginado<UsuarioResponse>(resposta, total);
        }
    }

    public class ObterUsuariosComFiltroQuery : IRequest<ResultadoPaginado<UsuarioResponse>>
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public bool? Ativo { get; set; }
        public bool? Administrador { get; set; }
        public int Pagina { get; set; } = 0;
        public int LinhasPorPagina { get; set; } = 10;
    }
}
