using System.Linq;
using Blog.Website.Data;
using Blog.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Controllers
{
    public class TagController : Controller
    {
        private readonly IPostRepository _repository;

        public TagController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [Route("tag/{*name}")]
        public IActionResult Index(string name)
        {
            var model =
                new HomeModel(_repository.Get()
                                         .Where(p => p.Tags.Select(t => t.ToLowerInvariant()).Contains(name))
                                         .OrderByDescending(p => p.Published));

            if (model.Posts.Any())
            {
                ViewBag.Title = $"Posts tagged with {name}:";

                return View(model);
            }

            return NotFound();
        }
    }
}