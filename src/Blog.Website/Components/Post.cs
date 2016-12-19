using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Components
{
    public class Post : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}