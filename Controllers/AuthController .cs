using final.helper;
using final.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IConfiguration _config;

    public AuthController(IEmployeeRepository employeeRepository, IConfiguration config)
    {
        _employeeRepository = employeeRepository;
        _config = config;
    }

    public class LoginRequest { public string Email { get; set; } public string Password { get; set; } }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _employeeRepository.GetByEmail(req.Email);
        if (user == null) return Unauthorized("Invalid credentials.");

        var decrypted = final.helper.EncryptionHelper.Decrypt(user.Password);
        if (decrypted != req.Password) return Unauthorized("Invalid credentials.");

        var token = JwtHelper.GenerateToken(user.Id.ToString(), user.Role, _config);
        return Ok(new { token });
    }
}
