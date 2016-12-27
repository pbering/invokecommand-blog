using Blog.Website.Data;
using Blog.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _repository;

        public HomeController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            ViewBag.Title = "Home";

            var model = new HomeModel(_repository.Get());

            return View(model);
        }
    }
}