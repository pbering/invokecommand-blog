using Blog.Website.Data;
using Blog.Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Controllers;

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
        return View(new HomeModel(_repository.Get())
        {
            Title = "Home",
            Url = "/"
        });
    }
}