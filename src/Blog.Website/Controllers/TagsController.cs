using Blog.Website.Data;
using Blog.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Blog.Website.Controllers
{
    public class TagsController : Controller
    {
        private readonly IPostRepository _repository;

        public TagsController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [Route("tags/{*name}")]
        public IActionResult Index(string name)
        {
            var model = new HomeModel(_repository.Get().Where(p => p.Tags.Contains(name)));

            if (model.Posts.Any())
            {
                ViewBag.Title = $"Posts tagged with '{name}':";

                return View(model);
            }

            return NotFound();
        }
    }
}