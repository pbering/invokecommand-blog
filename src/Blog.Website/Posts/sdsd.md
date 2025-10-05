---
title: Effective observability using OpenTelemetry and Sitecore MVC
summary: Implementing support for OpenTelemetry metrics, tracing and logging in Sitecore MVC
date: 2024-08-22
tags: monitoring, otel, sitecore
---

[OpenTelemetry](https://opentelemetry.io) (OTel for short) is a open source, cross platform, vendor neutral framework and specification/protocol for making systems observable. This is done through instrumenting code so it emits traces, metrics and logs in a common format so that can be sent to a observability backend. Backends can be open source systems as <https://www.jaegertracing.io/>, <https://prometheus.io/>, <https://signoz.io/> or commercial offerings that also supports the [OpenTelemetry Protocol (OTLP) specification](https://opentelemetry.io/docs/specs/otlp/) such has New Relic, DataDog, Azure App Insights etc.

## Sitecore MVC vs Sitecore rendering hosts

If you do a headless implementation using either the [Sitecore ASP.NET Core SDK](https://github.com/Sitecore/ASP.NET-Core-SDK) (now open source yay!) or the [Sitecore JavaScript Rendering SDK (JSS)](https://github.com/Sitecore/jss) then it is easy to add OTel to you head implementation in a few minutes, see [.NET 6+ guide](https://opentelemetry.io/docs/languages/net/getting-started/) or [Node.js guide](https://opentelemetry.io/docs/languages/js/getting-started/nodejs/). But for classic .NET 4.8 framework / Sitecore MVC sites it's another story which this post is all about!

.NET 4.8 *is* supported but Sitecore Platform DXP is making it more difficult then it should be mostly due to lots of very old dependencies used in Sitecore Platform DXP, even in the latest (in time of writing) 10.4.0 release.

## Implementing OTel support in Sitecore MVC sites

I have create a complete example implementation [here](https://github.com/pbering/sitecore-mvc-otel/) that is wired up to use the [ASP.NET Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/explore) as a OTel compatible backend to store traces, metrics and logs in memory only (good for local development environments).

[![ASP.NET Aspire Dashboard showing a trace](/content/images/sitecore-mvc-otel-aspire-trace.png)](/content/images/sitecore-mvc-otel-aspire-trace.png)

What you get is:

- ASP.NET traces and spans for requests and calls to SQL, Solr and other HttpClient/WebRequests
- ASP.NET metrics such as CPU/Memory/GC
- Traces are decorated with Sitecore context data such as site name, item id, template id, language etc.
- All Sitecore CM/CD logs
- Correlation between logs and traces
- Traefik traces and metrics (OTel support built into v3.x)
- Solr traces and metrics (uses OTel Java auto instrumentation agent <https://github.com/open-telemetry/opentelemetry-java-instrumentation>)

Some interesting implementation details:

1. Biggest challenge was to get the [OTel nuget packages](https://github.com/pbering/sitecore-mvc-otel/blob/main/Directory.Packages.props) to work with Sitecore due to Sitecore has lots of old (and deprecated) dependencies, see this [transform](https://github.com/pbering/sitecore-mvc-otel/blob/main/src/Platform/Web.config.SitecoreMvcOtel.xdt).
1. Since Sitecore uses log4net and forwards Microsoft.Extensions.Logging logs into that I needed to *reverse* that process so that log4net messages is forwarded to Microsoft.Extensions.Logging instead as it's automatically picked up by OTel! See the [ServiceConfigurator](https://github.com/pbering/sitecore-mvc-otel/blob/53689543f2b03c2128b3dbdabeb02249be8dbf74/src/SitecoreMvcOtel/ServiceConfigurator.cs#L56) and [MsForwardingAppender](https://github.com/pbering/sitecore-mvc-otel/blob/main/src/SitecoreMvcOtel/Logging/MsForwardingAppender.cs)
1. The whole OTel configuration is wired up in an old school [HttpApplication](https://github.com/pbering/sitecore-mvc-otel/blob/main/src/SitecoreMvcOtel/HttpApplication.cs) that you need to inherit in your own [Global.asax](https://github.com/pbering/sitecore-mvc-otel/blob/main/src/Platform/Global.asax)

You can also find more useful OTel extensions here: <https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src>

## Conclusion

I really like the fact that OTel is a vendor neutral and agreed upon (and [CNCF backed](https://www.cncf.io/projects/opentelemetry/)) approach to observability across different technology stacks which is very useful in these composable times.

Checkout even more possibilities on the [OpenTelemetry ecosystem](https://opentelemetry.io/ecosystem/) page.
