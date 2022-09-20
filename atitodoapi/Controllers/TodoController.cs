using Atitodo.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atitodoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly MainDbContext _mainDbContext;

        public TodoController(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        [HttpPost("list")]
        public List<t_todo> GetList([FromBody] SearchModel searcModel)
        {
            var query = _mainDbContext.t_todo.AsQueryable();
            if (searcModel!=null && !string.IsNullOrWhiteSpace(searcModel.text))
            {
                query = query.Where(p => p.todotext.Contains(searcModel.text));
            }
            return query.ToList();
        }

        [HttpPost("save")]
        public void Save(t_todo item)
        {
            _mainDbContext.t_todo.Update(item);
            _mainDbContext.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var item = _mainDbContext.t_todo.FirstOrDefault(p => p.id == id);
            if (item != null)
            {
                _mainDbContext.t_todo.Remove(item);
                _mainDbContext.SaveChanges();
            }
        }
    }
}
