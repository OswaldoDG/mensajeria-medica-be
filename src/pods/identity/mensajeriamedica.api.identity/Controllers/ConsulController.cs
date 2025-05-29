using comunes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace mensajeriamedica.api.identity.Controllers;

[Route("consul")]
[ApiController]
public class ConsulController(ILogger<ConsulController> logger, IOptions<ConfiguracionAPI> options) : ControllerBase
{
    private readonly ConfiguracionAPI configuracionAPI = options.Value;

    [HttpGet("configuracion/api")]
    [AllowAnonymous]
    [SwaggerOperation("Obtiene la configuración de los enpoints interservicio para la aplicación.")]
    [SwaggerResponse(statusCode: 200, type: typeof(ConfiguracionAPI), description: "Configuracion de API válida.")]
    public ActionResult<ConfiguracionAPI> ObtieneCOnfiguracionAPIInterna()
    {
        return Ok(configuracionAPI);
    }

}
