---
title: Automatically build and update Sitecore base Docker images
summary: A git repository that you can use directly and get private images of all Sitecore versions
date: 2018-02-08
tags: Sitecore, Docker, CI/CD, Series, Containers
---
Having a repository of base images for each Sitecore version that can be shared between machines (workstations, servers etc.) is the foundation for everything Docker and Sitecore. Base images can also serve as the base for specialized images, preconfigured for each of the different Sitecore roles (CM, CD, Reporting, Processing, xConnect etc.)

>This should *really* be something that Sitecore provided and I'm sure they will someday, but until then, please tell them that you want it when you get the opportunity and *also* go and vote on [https://sitecore.uservoice.com/forums/429883-sitecore-mvp/suggestions/18731035-support-installation-to-windows-2016-in-docker-and](https://sitecore.uservoice.com/forums/429883-sitecore-mvp/suggestions/18731035-support-installation-to-windows-2016-in-docker-and).

This is why I just made this: [https://github.com/sitecoreops/sitecore-images](https://github.com/sitecoreops/sitecore-images) public so we, at least, can share how to build and update Sitecore Docker images within the community. It can be used directly by your own build environment and it pushes the images build to your own private Docker repository.

All versions since 8.2 update 3 are present (this version was the first to official support Windows Server 2016). For the Sitecore 9.0.x version there is an image per role, for now only in the XM1 topology. But I hope to get images ready for the XP1 topology in the future.

## Prerequisites

* A **private** Docker repository. Any will do but the easiest is to sign-up for a private plan on [https://hub.docker.com](https://hub.docker.com), the cheapest one is $7/mo.
* A file share that your build agents can reach where you have placed zip files downloaded from [https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform.aspx](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform.aspx) **and** your license.xml.
* Some kind of build server is also preferred (you could run the PowerShell scripts manually you if *really* want to..., for example TeamCity, with agents that runs:
  * Windows 10 or Windows Server 2016 that is up to date and at least the 1709 build.
  * Hyper-V installed
  * Latest stable Docker engine and cli.
* Use directly or fork: `https://github.com/sitecoreops/sitecore-images`.

## How to use

Configure your build server to:

1. Trigger a build on changes to this git repository - to get new versions.
1. Trigger once a week - to get base images updated when Microsoft releases patched images.

./Build.ps1 should be called like this:

````PowerShell
# Login
"YOUR DOCKER REPOSITORY PASSWORD" | docker login --username "YOUR DOCKER REPOSITORY USERNAME" --password-stdin

# Build and push
. (Join-Path $PSScriptRoot "Build.ps1") `
    -InstallSourcePath "PATH TO WHERE YOU KEEP ALL SITECORE ZIP FILES AND LICENSE.XML" `
    -Organization "YOUR ORG NAME" ` # On Docker Hub it's your username unless you create an organization
    -Repository "sitecore"
````

## The build process

Each Sitecore version (and role) is define under ./sitecore/ with a build.json file that defines the final tag and which files that should be copied (from your file share) into the version folder before `docker image build` is called, and of course a Dockerfile.

1. Every base image used (the FROM line in all the Docker files) is pulled to ensure we have the latest OS patches. This is why you should schedule your build to run each week.
1. For each version folder under ./sitecore/
   1. Determine tag to use from convention: `ORGANIZATION/REPOSITORY:VERSION[-TOPOLOGY-ROLE]`, ex: `pbering/sitecore:9.0.171219-xm1-cm`. The part after the `:` is defined in `build.json`.
   1. Copy files defined in `build.json` if it's not already there from previous runs.
   1. Runs `docker image build`.
   1. Runs `docker image push`.

The Sitecore version number used in the tags *may* seem at bit strange at first glance. The reason behind is that the version used the *same* as the corresponding packages has on the official Sitecore NuGet feed, making it easy to identify which package and image versions to use together.

When it's done running, you should end up with something like this:

[![Sitecore Docker image repository](/content/images/docker-hub-sitecore-repository.png)](/content/images/docker-hub-sitecore-repository.png)