---
title: Sitecore 9, SIF and Docker
summary: Using SIF to install Sitecore 9 in a Docker image.
date: 2017-10-28
tags: Sitecore, Containers, Docker
---

So Sitecore 9.0 was released last week and also the new Sitecore Install Framework (SIF). SIF is a PowerShell module used for installing a Sitecore instance in a given configuration of different size and complexity. I have preferred using PowerShell scripts/modules to manage Sitecore instances for many years but it is really nice that Sitecore now supplies a good module "out-of-the-box".

For every Sitecore version I also build base Docker images and have this automated (a post for another day). Until now if has been pretty simple to do with in a Docker file that just unzip the version, setup permissions, IIS config etc. But now we have SIF as the "official" way to install Sitecore, so wouldn't it be great if it also worked in a Docker container?

Turns out it does! I basically just made a Docker file with required dependencies and executing the step described in the installation guide.

Here's a repository [https://github.com/pbering/sitecore-nine-docker](https://github.com/pbering/sitecore-nine-docker) with description on how to get it up and running and you can see the full Docker file here: [https://github.com/pbering/sitecore-nine-docker/blob/master/xm1/cm/Dockerfile](https://github.com/pbering/sitecore-nine-docker/blob/master/xm1/cm/Dockerfile).