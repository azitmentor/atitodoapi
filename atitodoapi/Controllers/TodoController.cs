using Atitodo.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
			if (!UserId.HasValue)
			{
				return Unauthorized();
			}

			item.userid = UserId.Value;
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
	}
}
