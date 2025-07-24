using Agenda.API.Extensoes;
using Agenda.Aplicacao.Comunicacao.Usuario.Commands;
using Agenda.Aplicacao.Comunicacao.Usuario.Querys;
using Agenda.Aplicacao.Factory;
using Agenda.Dominio.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.API.Controllers
{
    [ApiController]  
    [Route("api/[controller]/")] 
    public class UsuarioController(IMediator mediator) : AgendaAPIControllerBase  
    {   
        private IMediator _mediator = mediator;

        [HttpPost("AutoCadastro")]  
        public async Task<IActionResult> AutoCadastro([FromBody] AutoCadastroCommand request)
        {
            
            var resultado = await _mediator.Send(CommandFactory.Create<AutoCadastroCommand, ResultadoGenerico<long>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPut("Atualizar")]
        public async Task<IActionResult> Atualizar([FromBody] AtualizarUsuarioCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<AtualizarUsuarioCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPut("AlterarSenha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaUsuarioCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<AlterarSenhaUsuarioCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("AtivarUsuario")]
        public async Task<IActionResult> AtivarUsuario([FromBody] AtivarUsuarioCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<AtivarUsuarioCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("DesativarUsuario")]
        public async Task<IActionResult> DesativarUsuario([FromBody] DesativarUsuarioCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<DesativarUsuarioCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("TornarAdministrador")]
        public async Task<IActionResult> TornarAdministrador([FromBody] TornarUsuarioAdministradorCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<TornarUsuarioAdministradorCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("VincularEndereco")]
        public async Task<IActionResult> VincularEndereco([FromBody] VincularEnderecoAoUsuarioCommand request)
        {
            var resultado = await _mediator.Send(
                CommandFactory.Create<VincularEnderecoAoUsuarioCommand, ResultadoGenerico<bool>>(request)
            );

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("VincularServico")]
        public async Task<IActionResult> VincularServico([FromBody] VincularServicoAoUsuarioCommand request)
        {
            var resultado = await _mediator.Send(CommandFactory.Create<VincularServicoAoUsuarioCommand, ResultadoGenerico<bool>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> ObterPorId(long id)
        {
            var resultado = await _mediator.Send(new ObterUsuarioPorIdQuery(id));

            if (!resultado.Sucesso)
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            var resultado = await _mediator.Send(new ObterTodosUsuariosQuery());

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("ObterTodosPaginado")]
        public async Task<IActionResult> ObterUsuariosPaginados([FromQuery] int pagina = 0, [FromQuery] int linhasPorPagina = 10)
        {
            var resultado = await _mediator.Send(
                new ObterUsuariosPaginadosQuery(pagina, linhasPorPagina)
            );

            return Ok(resultado);
        }

        [HttpGet("ObterUsuariosComFiltro")]
        public async Task<IActionResult> ObterUsuariosComFiltro(
            [FromQuery] string? nome,
            [FromQuery] string? email,
            [FromQuery] bool? ativo,
            [FromQuery] bool? administrador,
            [FromQuery] int pagina = 0,
            [FromQuery] int linhasPorPagina = 10)
        {
            var resultado = await _mediator.Send(new ObterUsuariosComFiltroQuery
            {
                Nome = nome,
                Email = email,
                Ativo = ativo,
                Administrador = administrador,
                Pagina = pagina,
                LinhasPorPagina = linhasPorPagina
            });

            return Ok(resultado);
        }
    }
}
