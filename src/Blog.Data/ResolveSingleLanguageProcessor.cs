using System.Threading.Tasks;
using Lightcore.Kernel;
using Lightcore.Kernel.Data.Globalization;
using Lightcore.Kernel.Pipelines;
using Lightcore.Kernel.Pipelines.Request;
using Microsoft.AspNetCore.Http;

namespace Blog.Data
{
    public class ResolveSingleLanguageProcessor : Processor<RequestArgs>
    {
        public override Task ProcessAsync(RequestArgs args)
        {
            var context = args.HttpContext.LightcoreContext();

            context.Language = Language.Default;

            args.HttpContext.Request.Path = new PathString(args.HttpContext.Request.Path.Value.Replace("/" + context.Language.Name, ""));

            return Task.CompletedTask;
        }
    }
}