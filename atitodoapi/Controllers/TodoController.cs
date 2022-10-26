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
        public ActionResult<List<t_todo>> GetList([FromBody] SearchModel srcParam)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var query = _mainDbContext.t_todo.Where(p => p.userid == UserId);

            if (srcParam != null)
            {
                if (!srcParam.showdeleted)
                {
                    query = query.Where(p => p.deleted == null);
                }

                if (!srcParam.showarchived)
                {
                    query = query.Where(p => p.archived == null);
                }

                if (!srcParam.showdone)
                {
                    query = query.Where(p => p.done == null);
                }

                if (srcParam.starredonly)
                {
                    query = query.Where(p => p.starred);
                }

                if (srcParam.todayonly)
                {
                    query = query.Where(p => p.fortoday == DateTime.Today);
                }

                if (srcParam.wastoday)
                {
                    query = query.Where(p => p.fortoday != null);
                }

                if (srcParam.hidebelow > 0)
                {
                    query = query.Where(p => p.priority >= srcParam.hidebelow);
                }

                if (!string.IsNullOrWhiteSpace(srcParam.text))
                {
                    query = query.Where(p => p.todotext.Contains(srcParam.text) || p.tags.Contains(srcParam.text));
                }
            }

            var result = query.ToList();

            if (srcParam != null && srcParam.tags != null && srcParam.tags.Count > 0)
            {
                result = result.Where(t => srcParam.tags.Contains(t.tags) || t.tags.Split(" ").Any(ta => srcParam.tags.Any(s => s == ta))).ToList();
            }

            return result.OrderBy(p => p.realvalue).ToList();
        }

        [Authorize]
        [HttpGet("get/{id}")]
        public ActionResult<t_todo> Get(int id)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.userid == UserId && p.id == id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
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
            if (item.id == 0)
            {
                item.created = DateTime.Now;
            }
            item.modified = DateTime.Now;
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
                item.deleted = DateTime.Now;
                _mainDbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("archive/{id}")]
        public ActionResult Archive(int id)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id && p.userid == UserId);
            if (item != null)
            {
                item.archived = DateTime.Now;
                _mainDbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("done/{id}")]
        public ActionResult Done(int id)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id && p.userid == UserId);
            if (item != null)
            {
                item.done = DateTime.Now;
                _mainDbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("star/{id}")]
        public ActionResult Star(int id)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id && p.userid == UserId);
            if (item != null)
            {
                item.starred = !item.starred;
                _mainDbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("today/{id}")]
        public ActionResult Today(int id)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id && p.userid == UserId);
            if (item != null)
            {
                item.fortoday = DateTime.Today;
                _mainDbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("tags")]
        public ActionResult GetTags([FromBody] SearchModel srcParam)
        {
            if (!UserId.HasValue)
            {
                return Unauthorized();
            }

            var itemQuery = _mainDbContext.t_todo.Where(p => p.userid == UserId && p.tags != null);

            if (!srcParam.showdeleted)
            {
                itemQuery = itemQuery.Where(p => p.deleted == null);
            }

            if (!srcParam.showdone)
            {
                itemQuery = itemQuery.Where(p => p.done == null);
            }

            var item = itemQuery.Select(p => p.tags).Distinct().ToList(); ;
            var result = new List<string>();
            item.ForEach(i =>
            {
                var s = i.Split(", ".ToCharArray()).ToList();
                result.AddRange(s);
            });
            return Ok(result.Distinct());
        }
    }
}
