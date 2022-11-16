---
title: .NET 7.0 released, impact on .NET based head implementations for Sitecore XM Cloud
summary: Lots of new stuff in .NET 7.0 (and Visual Studio 2022 17.4), easy upgrade and get even better performance!
date: 2022-11-16
tags: Sitecore, xmcloud, dotnet
---

Since I wrote ["Using .NET 6 as the head for headless sites in Sitecore XM Cloud"](/posts/dotnet-head-for-xmcloud) then [.NET 7.0 was released](https://devblogs.microsoft.com/dotnet/announcing-dotnet-7/) along side of Visual Studio 2022 17.4 bringing tons of improvements, most noticeable, lots of [performance improvements](https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/).

Upgrading the .NET 6 based head to .NET 7 was as simple as [updating the target framework](https://github.com/pbering/xmc-demo-dotnethead/commit/5c64cf1588de22e7994e652fb6b06b63319deff6#diff-1072ea6cd6bf0573e60399f664b5d4280a482e64a5e8264832537606a2ce02f0R4) and that was it.

## Performance

Let's see what happens with some simple local load testing by running `bombardier-windows-amd64.exe -n 50000 -c 1000 -t 1m -k https://localhost:5443/`, first on .NET 6:

```text
Statistics        Avg      Stdev        Max
  Reqs/sec      5421.35    4040.31   22713.18
  Latency      183.41ms    62.93ms      1.34s
  HTTP codes:
    1xx - 0, 2xx - 50000, 3xx - 0, 4xx - 0, 5xx - 0, others - 0
  Throughput:     5.20MB/s
```

then again on .NET 7:

```text
Statistics        Avg      Stdev        Max
  Reqs/sec      9033.82    4696.71   20839.73
  Latency       93.39ms   128.14ms      3.63s
  HTTP codes:
    1xx - 0, 2xx - 50000, 3xx - 0, 4xx - 0, 5xx - 0, others - 0
  Throughput:     8.71MB/s
```

THAT is impressive and we get that by *only* updating the head to use .NET 7.

## Visual Studio "dev tunnels"

Visual Studio "dev tunnels" are also now in [public preview](https://devblogs.microsoft.com/visualstudio/public-preview-of-dev-tunnels-in-visual-studio-for-asp-net-core-projects/) which means that you can easy enable it and use that instead of ngrok as suggested in ["Use local rendering host as an editing host in XM Cloud"](/posts/dotnet-head-for-xmcloud#use-local-rendering-host-as-an-editing-host-in-xm-cloud). The possibility to have a "durable" (ie. fixed) tunnel url should also be [coming soon](https://devblogs.microsoft.com/visualstudio/public-preview-of-dev-tunnels-in-visual-studio-for-asp-net-core-projects/#whats-next).
