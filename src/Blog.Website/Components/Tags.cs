using System.Collections.Generic;
using Lightcore.Kernel.Data;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Website.Components
{
    public class Tags : ViewComponent
    {
        public IViewComponentResult Invoke(Item item)
        {
            var model = new Dictionary<string, string>();

            foreach (var tag in item["tags"].Split(','))
            {
                var name = tag.Trim();
                var url = "/tags/" + name.ToLowerInvariant();

                model.Add(name, url);
            }

            return View(model);
        }
    }
}