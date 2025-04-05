using AD.Auth;
using AD.Model;
using AD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AWMSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _tokenService.ValidateUserAndGenerateToken(request);

            if (response == null)
            {
                _logger.LogWarning("Autenticación fallida para el usuario: {Username}", request.Username);
                return Unauthorized("Credenciales de usuario inválidas o perfil expirado");
            }

            _logger.LogInformation("Token generado correctamente para el usuario: {Username}", request.Username);
            return Ok(response);
        }
    }
}
