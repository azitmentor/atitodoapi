using Atitodo.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
	public class TodoController : MyControllerBase
	{
		private readonly MainDbContext _mainDbContext;
		private readonly IConfiguration _configuration;

		public TodoController(MainDbContext mainDbContext, IConfiguration configuration)
		{
			_mainDbContext = mainDbContext;
			_configuration = configuration;
		}

		[Authorize]
		[HttpPost("list")]
		public ActionResult<List<t_todo>> GetList([FromBody] SearchModel searcModel)
		{
			if (!UserId.HasValue)
			{
				return Unauthorized();
			}

			var query = _mainDbContext.t_todo.Where(p => p.userid == UserId);
			if (searcModel != null && !string.IsNullOrWhiteSpace(searcModel.text))
			{
				query = query.Where(p => p.todotext.Contains(searcModel.text));
			}
			return query.ToList();
		}

		[Authorize]
		[HttpPost("save")]
		public ActionResult Save(t_todo item)
		{
			if (!UserId.HasValue || item.userid != UserId)
			{
				return Unauthorized();
			}

			_mainDbContext.t_todo.Update(item);
			_mainDbContext.SaveChanges();
			return Ok();
		}

		[Authorize]
		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			if (!UserId.HasValue)
			{
				return Unauthorized();
			}

			var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id && p.userid == UserId);
			if (item != null)
			{
				_mainDbContext.t_todo.Remove(item);
				_mainDbContext.SaveChanges();
				return Ok();
			}
			return NotFound();
		}

		[HttpGet("jwt")]
		public string jwt()
		{
			//create claims details based on the user information
			var claims = new[] {
						new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
						new Claim("UserId", "1234"),
						new Claim("DisplayName", "disp"),
						new Claim("UserName", "username"),
						new Claim("Email", "email")
					};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: DateTime.UtcNow.AddMinutes(120),
				signingCredentials: signIn);

			return new JwtSecurityTokenHandler().WriteToken(token);
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
	}
}
