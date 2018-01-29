---
title: Ultimate legacy-in-a-box test
summary: Go old school and run Sitecore 4.3.2 with Docker!
date: 2017-04-13
tags: Sitecore, Containers, Docker
---

I love the [legacy in a box](https://www.thoughtworks.com/radar/techniques/legacy-in-a-box) technique to isolate (legacy) systems 
using [Docker](/tags/docker) so I don't need to pollute my environment with all kinds of crazy dependencies.

So I wanted to see just how "legacy" I could go and get the very first Sitecore version I ever used to run in a 
container... Behold! The rock solid performer: Sitecore v4.3.2 from October 2005, based on .NET 1.0!

Here's a few screenshots:

[![Sitecore 4.3.2 Login](/content/images/sitecore432-login.png)](/content/images/sitecore432-login.png)

[![Sitecore 4.3.2 Content Editor](/content/images/sitecore432-contenteditor.png)](/content/images/sitecore432-contenteditor.png)

Besides the glorious UI, Sitecore 4.3.2 is only ~1500 files and ~14 MB on disk, including the xml file based databases, so it's a bit more lightweight than recent versions :)

## Getting it to work

What you need:

1. Windows 10 with Docker running (for example using Docker for Windows)
1. VM running Windows XP and Internet Explorer 6.0 (don't even think about getting it to work with anything else).
1. A "dist.4.3.2.*.zip", I found one in a old backup but if you don't have one you could try to call your local Sitecore office :P
1. Modify a few files, see [https://github.com/pbering/SockerOldSchool/tree/master/Sitecore/Website](https://github.com/pbering/SockerOldSchool/tree/master/Sitecore/Website)

Since this effectively runs .NET 4.5 even though it's .NET 1.0 there is a lot changed around security and encryption so I had to replace a few static methods (HasModule check, some auth stuff) in Sitecore.Kernel.dll at runtime, from within global.asax.

Here's how the Docker file looks:

````docker
FROM microsoft/aspnet

ADD *.zip .

RUN powershell -Command \
    Expand-Archive -Path 'C:\dist*.zip' -DestinationPath 'C:\\'; \
    Remove-Item 'C:\dist*.zip' -Force;

RUN powershell -Command \    
    New-Item -Path 'C:\Sitecore\Website' -ItemType Directory -Force ^| Out-Null; \
    Get-ChildItem -Path 'C:\dist*\*' ^| Copy-Item -Destination 'C:\Sitecore\Website' -Recurse -Force; \
    Remove-Item 'C:\dist*' -Recurse -Force; 

ADD Sitecore /Sitecore

# Application Pool *needs* to run 64 bit for some reason, on 32 bit the application pool just crashes hard.
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name 'physicalPath' -Value 'C:\Sitecore\Website'; \
    Set-ItemProperty -Path 'IIS:\apppools\DefaultAppPool' -Name 'enable32BitAppOnWin64' -Value 'False';
````

You can get everything needed here if you dare: [https://github.com/pbering/SockerOldSchool](https://github.com/pbering/SockerOldSchool)

## Conclusion

Is this crazy? Yes it is! But the point here is besides the fun of it, it's possible run very old legacy applications without much effort in a *clean* and *isolated* way.

