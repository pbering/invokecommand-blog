---
title: Visual Studio 2017 Docker tooling and Sitecore
summary: Using the new built-in tooling to develop Sitecore solutions.
date: 2017-03-09
tags: Sitecore, Containers, Docker, Tools
---

Visual Studio 2017 is now RTM, go install it if you haven't already... I'll wait... :)

There a lot of new stuff to explore but this post is about the Docker tooling which is 
now built-in and works with both Linux and Windows containers and it's very nicely integrated in the IDE. 
You just right click and under "Add" choose Docker Support. If your project is .NET Core you can get 
Linux or Windows (depending on which is active in Docker for Windows) and if it's .NET 4.x you will get a Windows container.

Some time ago I made the [Socker](https://github.com/pbering/Socker) repository to show how to setup and run Sitecore solution using Docker compose to manage both IIS and SQL.

I've just pushed [SockerVS2017](https://github.com/pbering/SockerVS2017) to show how to use the new Visual Studio 2017 Docker tooling to do the same, just way nicer integrated.

Go try it out!