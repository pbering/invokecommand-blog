using System;
using System.Threading.Tasks;
using Lightcore.Kernel.Pipelines;
using Lightcore.Kernel.Pipelines.Response;
using Microsoft.Extensions.Primitives;

namespace Blog.Data.Lightcore
{
    public class SetPublicHttpCacheHeadersProcessor : Processor<ResponseArgs>
    {
        public override Task ProcessAsync(ResponseArgs args)
        {
            var acceptedStatusCode = args.HttpContext.Response.StatusCode >= 200 && args.HttpContext.Response.StatusCode < 400;

            if (acceptedStatusCode)
            {
                args.HttpContext.Response.Headers.Add("Cache-Control", new StringValues("public, max-age=3600"));
            }

            return Task.CompletedTask;
        }
    }
}