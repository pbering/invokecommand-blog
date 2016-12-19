using System;
using System.Linq;
using System.Threading.Tasks;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Components
{
    public class Tag : ViewComponent
    {
        private readonly IItemStore _itemStore;

        public Tag(IItemStore itemStore)
        {
            _itemStore = itemStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(RenderingContext renderingContext)
        {
            var tag = await _itemStore.GetVersionAsync(renderingContext.ItemId, renderingContext.ItemLanguageName);
            var posts = await _itemStore.GetVersionAsync("/home/posts", renderingContext.ItemLanguageName);
            var model = posts.Children
                             .Where(p => p.Fields["tags"] != null)
                             .Where(p => p["tags"].IndexOf(tag.Name, StringComparison.OrdinalIgnoreCase) > -1);

            ViewBag.TagName = tag.Name;

            return View(model);
        }
    }
}