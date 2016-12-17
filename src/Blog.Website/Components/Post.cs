using System.Threading.Tasks;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Components
{
    public class Post : ViewComponent
    {
        private readonly IItemStore _itemStore;

        public Post(IItemStore itemStore)
        {
            _itemStore = itemStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(RenderingContext renderingContext)
        {
            return View();
        }
    }
}