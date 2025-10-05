---
title: Sitecore ASP.NET SDK and slow performance on non-root paths
summary: Why is there a 10x slowdown in throughput on non-root paths when using ASP.NET OutputCache?
date: 2025-10-05
tags: sitecore, xmcloud, dotnet, performance
---

While working on a client solution using the [Sitecore ASP.NET SDK](https://github.com/Sitecore/ASP.NET-Core-SDK) I discovered a strange performance issue there the throughput **dropped 10x on all non-root paths**, home page `/` was blazing fast, but all other pages significant slower!

The solution is using the built-in [output caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-9.0) in ASP.NET and when running this:

`bombardier-windows-amd64.exe -n 5000 -c 50 -t 30s https://localhost:5001/`

I was getting something like **~40.000 req/sec** which is very nice indeed, but doing the same any *other* path like `/about` or `/about/contact` I was "only" getting **~4.000 req/sec**!

## The culprit

Could not find anything in our own implementation that was running when a page was in the output cache so I started to look into the ASP.NET SDK source code and found the offending code: <https://github.com/Sitecore/ASP.NET-Core-SDK/blob/b04d2c8ba039cdcfeb419b2514b69dafd9c4e408/src/Sitecore.AspNetCore.SDK.RenderingEngine/Routing/LanguageRouteConstraint.cs#L26>. The SDK is loads all known cultures from OS with `CultureInfo.GetCultures(CultureTypes.AllCultures)` on *every request*, and it does that *before* the output cache kicks in!

In the solution I was working on I could fix it by just removing the call to `Sitecore.AspNetCore.SDK.RenderingEngine.Extensions.MapSitecoreLocalizedRoute()` ([source](https://github.com/Sitecore/ASP.NET-Core-SDK/blob/b04d2c8ba039cdcfeb419b2514b69dafd9c4e408/src/Sitecore.AspNetCore.SDK.RenderingEngine/Extensions/ControllerEndpointExtensions.cs#L11)) since we are mapping the hostname to a culture and have no need to handle the culture from the path.

If this is not an option for you then you can pick one of these options:

1. Replace the [LanguageRouteConstraint](https://github.com/Sitecore/ASP.NET-Core-SDK/blob/b04d2c8ba039cdcfeb419b2514b69dafd9c4e408/src/Sitecore.AspNetCore.SDK.RenderingEngine/Routing/LanguageRouteConstraint.cs#L26) when a version that just uses the `IOptions<RequestLocalizationOptions>` that you must likely already have configured in `UseRequestLocalization`.
1. Wait on the issue I created is resolved: <https://github.com/Sitecore/ASP.NET-Core-SDK/issues/43>
1. Or even better, create a [PR](https://github.com/Sitecore/ASP.NET-Core-SDK/pulls) with a fix.

## Conclusion

Don't ever call `CultureInfo.GetCultures(CultureTypes.AllCultures)` on each and *every* request, highly unlikely the OS will get new cultures installed *that* often 😉 and always measure performance to catch issues like this early on!
