using UsuarioEntidade = Agenda.Dominio.Entidades.Usuario;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Utils;
using MediatR;

namespace Agenda.Aplicacao.Comunicacao.Usuario.Querys
{
    public class ObterUsuarioPorIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<ObterUsuarioPorIdQuery, ResultadoGenerico<UsuarioResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ResultadoGenerico<UsuarioResponse>> Handle(ObterUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.ObterPorIdAsync<UsuarioEntidade>(request.Id);

            if (usuario == null)
                return new ResultadoGenerico<UsuarioResponse>(false, "Usuário não encontrado", null);

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ativo = usuario.Ativo,
                Administrador = usuario.Administrador
            };

            return new ResultadoGenerico<UsuarioResponse>(true, "Usuário obtido com sucesso", response);
        }
    }

    public record ObterUsuarioPorIdQuery(long Id) : IRequest<ResultadoGenerico<UsuarioResponse>>;

    public class UsuarioResponse
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Administrador { get; set; }
    }
}
