using System.Threading.Tasks;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Components
{
    public class Posts : ViewComponent
    {
        private readonly IItemStore _itemStore;

        public Posts(IItemStore itemStore)
        {
            _itemStore = itemStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(RenderingContext renderingContext)
        {
            var model = await _itemStore.GetVersionAsync("/home/posts", renderingContext.ItemLanguageName);

            return View(model);
        }
    }
}