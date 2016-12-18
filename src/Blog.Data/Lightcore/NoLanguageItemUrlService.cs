using Lightcore.Configuration;
using Lightcore.Kernel.Data;
using Lightcore.Kernel.Data.Globalization;
using Lightcore.Kernel.Url;
using Microsoft.Extensions.Options;

namespace Blog.Data.Lightcore
{
    public class NoLanguageItemUrlService : IItemUrlService
    {
        private readonly LightcoreOptions _options;

        public NoLanguageItemUrlService(IOptions<LightcoreOptions> options)
        {
            _options = options.Value;
        }

        public string GetUrl(IItemDefinition item, Language language)
        {
            if (item == null)
            {
                return string.Empty;
            }

            return item.Path.ToLowerInvariant().Replace(_options.StartItem.ToLowerInvariant(), "");
        }
    }
}