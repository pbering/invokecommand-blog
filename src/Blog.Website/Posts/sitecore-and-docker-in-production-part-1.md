---
title: Sitecore and Docker in production - part 1
summary: Introduction and automated repository of Sitecore Docker base images
date: 2018-01-31
tags: CI/CD, Sitecore, Docker, Series, Containers
---
In the recent years I have been experimenting a lot with the Windows Containers support on Windows 10 and Windows Server 2016. My primary goal is to use it to both develop and to host Sitecore solutions, on-premise or *any* cloud provider.

>The Docker image and runtime specifications has recently been opened up and is now governed by the ([Open Containers Initiative](https://www.opencontainers.org/)) which is backed by Docker Inc, Microsoft, Amazon, Google and a lot of other major leaders in the cloud space. **This is good for us as it makes it possible to move container based solutions easier between products, vendors and platforms.** An example of this is that you can build, run and publish an image on your Windows workstation, run it on on-premise, Azure, AWS, Google Cloud using just Docker or rkt, Docker Swarm or Kubernetes - all without changing or rebuilding your images.

Now it time to put it all together and show how it can be done.

Just to make it clear, everything in this series can also used for other applications than Sitecore and that is the beauty of containers - tooling, packaging and runtime execution is the same regardless of platform and technology stack.

## Goals

1. **Smooth on-boarding of developers on existing solutions**, ie. clone source repository and run, no messing around setting up IIS, SQL, Solr or SIF/SIM/Custom PowerShell scripts - all which, in my experience, is *always* different from solution to solution (if they work at all) and not to mention differences between workstations and versions installed of all the different components. Which again is almost never the exact same as in production...
1. **Easy to start a new solutions**, on any recent version of Sitecore, handles same issues as above.
1. **Infrastructure as Code**, *everything* is versioned controlled and easily reproducible, remember your servers are cattle not pets. No more [snowflake servers](https://martinfowler.com/bliki/SnowflakeServer.html)!
1. **CI/CD pipeline is fast and reusable**, once established it's reusable since the output is container images and not a mix of Sitecore files, application files and configuration files. It is also the same images that moves through the pipeline.
1. **Scaling Sitecore roles and other services is dynamic and requires no configuration** nor changes to the images or the CI/CD pipeline.

In this first post I will lay the foundation for reaching those goals.

## Automated repository of Sitecore Docker base images

Having a repository of base images for each Sitecore version that can be shared between machines (developer workstations, servers etc.) is the foundation of it all. Base images will also serve as the base for specialized images for the different roles, ie. CM, CD, Reporting, Processing, xConnect etc.

### Prerequisites

...

### Build process

...

### Update process

...

## Posts in this series

1. [Automated repository of Sitecore Docker base images (this)](/posts/sitecore-and-docker-in-production-part-1)
1. The sample solution
1. Setting up a hybrid Docker Swarm
1. CI/CD pipeline
1. Logging and metrics
1. Conclusion
