using Agenda.Dominio.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.API.Extensoes
{
    public class AgendaAPIControllerBase: ControllerBase
    {
        public OkObjectResult Ok<T>(ResultadoGenerico<T> value)
        {
            return base.Ok(new { message = value.Mensagem, dados = value.Dados });
        }

        public BadRequestObjectResult BadRequest<T>(ResultadoGenerico<T> value)
        {
            return base.BadRequest(new { message = value.Mensagem });
        }
    }
}
