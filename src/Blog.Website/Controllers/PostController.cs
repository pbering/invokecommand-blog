using System;
using System.Linq;
using Blog.Website.Data;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Controllers;

public class PostController : Controller
{
    private readonly IPostRepository _repository;

    public PostController(IPostRepository repository)
    {
        _repository = repository;
    }

    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    [Route("posts/{*name}")]
    public IActionResult Index(string name)
    {
        var post = _repository.Get().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (post != null)
        {
            return View(post);
        }

        return NotFound();
    }
}