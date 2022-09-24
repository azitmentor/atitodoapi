using Atitodo.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Atitodoapi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : MyControllerBase
	{
		private readonly MainDbContext _mainDbContext;
		private readonly IConfiguration _configuration;
		private readonly SignInManager<IdentityUser<int>> _signInManager;
		private readonly UserManager<IdentityUser<int>> _userManager;

		public LoginController(MainDbContext mainDbContext, IConfiguration configuration, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
		{
			_mainDbContext = mainDbContext;
			_configuration = configuration;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[Authorize]
		[HttpGet("jwt2")]
		public ActionResult jwt2()
		{
			if (!UserId.HasValue)
			{
				return Unauthorized();
			}
			return Ok(UserId);
		}

		//[Authorize]
		[HttpPost("register")]
		public ActionResult register(RegisterModel registerModel)
		{
			var usr = new IdentityUser<int>();
			usr.Email = registerModel.email;
			usr.UserName = registerModel.username;
			var res = _userManager.CreateAsync(usr, registerModel.password).Result;
			if (res.Succeeded)
			{
				return Ok();
			}
			if (res.Errors != null)
			{
				Console.Error.WriteLine("Error");
				foreach (var err in res.Errors)
				{
					Console.WriteLine(err);
				}
			}

			return BadRequest();
		}

		[HttpPost("login")]
		public ActionResult login(LoginModel loginModel)
		{
			var res = _signInManager.PasswordSignInAsync(loginModel.username, loginModel.password, true, false).Result;
			if (res.Succeeded)
			{
				var usr = _signInManager.UserManager.FindByNameAsync(loginModel.username).Result;
				//create claims details based on the user information
				var claims = new[] {
						new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
						new Claim("UserId", usr.Id.ToString()),
						new Claim("DisplayName", "disp"),
						new Claim("UserName", "username"),
						new Claim("Email", usr.Email)
					};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
				var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(
					_configuration["Jwt:Issuer"],
					_configuration["Jwt:Audience"],
					claims,
					expires: DateTime.UtcNow.AddDays(120),
					signingCredentials: signIn);

				return Ok(new JwtSecurityTokenHandler().WriteToken(token));

			}
			return BadRequest();
		}
	}

}
