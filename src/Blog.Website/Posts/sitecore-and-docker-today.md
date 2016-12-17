---
title: Sitecore and Docker - Today!
summary: It is today possible (and usable) to run and develop Sitecore solutions in Docker.
date: 2016-10-30
tags: Sitecore, Containers, Docker
---

A lot has happend since my last post on Sitecore and Windows containers:

1. Windows Server 2016 is now released with builtin Docker CS (commercially supported). For free!
2. "windowsservercore" based containers is now supported on Windows 10 besides "nanoserver". Full .NET Framework support!
3. "Docker for Windows" in the beta channel now supports switching between Linux and Windows containers.
4. Docker Compose is starting to become usable with Windows containers also.

There are some issue though:

1. Windows NAT and Docker is not working as with Linux container ie. you can't publish ports from containes to localhost. 
 So from your host you need to access the containers on the container IP instead of for example localhost:8000. You can access publish ports externally with the IP of the host though.
 More details here [https://blog.sixeyed.com/published-ports-on-windows-containers-dont-do-loopback/](https://blog.sixeyed.com/published-ports-on-windows-containers-dont-do-loopback/) - I expect this to be "fixed" in future Windows version or for local development in "Docker for Windows".
2. Mixing Linux and Windows containers is not supoprted in Compose (yet?), so it is not possible to have both Linux and Windows containers in the same compose file communicating.

But still we are now at a place where Sitecore and Docker can be used to easily and fast get a solution up and running, with data persistance through mapped volumes and also executing code inside the containers during development! All running natively on Windows :)

You can see a complete end to end example of how to setup a Sitecore solution here: [https://github.com/pbering/Socker/](https://github.com/pbering/Socker/)