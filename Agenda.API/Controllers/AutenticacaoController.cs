using Agenda.API.Extensoes;
using Agenda.API.Utils;
using Agenda.Aplicacao.Comunicacao.Autenticacao;
using Agenda.Aplicacao.Factory;
using Agenda.Dominio.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.API.Controllers
{
    public class AutenticacaoController : AgendaAPIControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAcessorService _service;
        private readonly IMediator _mediator;

        public AutenticacaoController(IConfiguration configuration, IMediator mediator, IHttpContextAcessorService service)
        {
            _configuration = configuration;
            _mediator = mediator;
            _service = service;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            ResultadoGenerico<GerarTokenJwtResponse> resultado = await _mediator.Send(
                CommandFactory.Create<LoginCommand, ResultadoGenerico<GerarTokenJwtResponse>>(request));

            if (!resultado.Sucesso)
                return BadRequest(resultado.Mensagem);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTime.UtcNow.AddHours(8),
                SameSite = SameSiteMode.Strict
            };

            HttpContext.Response.Cookies.Append("jwt_token", resultado.Dados.Token, cookieOptions);

            return Ok();
        }
    }
}
