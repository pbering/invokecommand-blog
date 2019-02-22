---
title: Sitecore 9 Experience Platform (XP) images released
summary: XP joins the Docker party!
date: 2019-02-21
tags: Sitecore, Docker, Containers
---
The Sitecore 9 Experience Management (XM) images has been around for a while and they are pretty stable and fit for production use, currently used on at least two customer solutions.

Until now I haven't been involved in any projects that had the need for the full blown XP on Docker, but that is about to change. So in preparation for this I have released images for Sitecore 9.1 and 9.0.2 XP out on [https://github.com/sitecoreops/sitecore-images](https://github.com/sitecoreops/sitecore-images).

Sitecore 9.1 XP is both for 1803 *and* ltsc2019 since it's the first Sitecore version that **officially** supports Windows Server 2019, see the [Sitecore compatibility table](https://kb.sitecore.net/articles/087164). **UPDATE:** I was either dreaming or it was removed again, but now it seem that Windows Server 2019 is **not supported yet**. That said I have not seen any issues on the images and only the issue below on the host OS.

> Please be aware of this [issue](https://github.com/moby/moby/issues/38484) if you plan on running Sitecore 9.1 on ltsc2019 in a hybrid Docker swarm cluster. Thankfully a Windows update should be available soon.

Sitecore 9.0.2 XP is only for 1803 (for now, ltsc2019 could be made if there is a need. There are working XM images on ltsc2019).

This is the images for 9.1 on ltsc2019:

```text
REPOSITORY                            TAG                                     SIZE
sitecore-xp-xconnect                  9.1.001564-windowsservercore-ltsc2019   4.9GB
sitecore-xp-xconnect-indexworker      9.1.001564-windowsservercore-ltsc2019   4.78GB
sitecore-xp-xconnect-automationengine 9.1.001564-windowsservercore-ltsc2019   4.77GB
sitecore-xp-standalone                9.1.001564-windowsservercore-ltsc2019   5.48GB
sitecore-xp-cd                        9.1.001564-windowsservercore-ltsc2019   5.48GB
sitecore-xp-sqldev                    9.1.001564-windowsservercore-ltsc2019   7.04GB
sitecore-xp-solr                      9.1.001564-nanoserver-1809              695MB
```

... and here for 1803:

```text
REPOSITORY                            TAG                                     SIZE
sitecore-xp-xconnect                  9.1.001564-windowsservercore-1803       5.81GB
sitecore-xp-xconnect-indexworker      9.1.001564-windowsservercore-1803       5.68GB
sitecore-xp-xconnect-automationengine 9.1.001564-windowsservercore-1803       5.68GB
sitecore-xp-standalone                9.1.001564-windowsservercore-1803       6.39GB
sitecore-xp-cd                        9.1.001564-windowsservercore-1803       6.39GB
sitecore-xp-sqldev                    9.1.001564-windowsservercore-1803       7.74GB
sitecore-xp-solr                      9.1.001564-nanoserver-1803              749MB
```

## Try it out

1. Clone [https://github.com/sitecoreops/sitecore-images](https://github.com/sitecoreops/sitecore-images).
1. Build the images, see the [instructions](https://github.com/sitecoreops/sitecore-images#how-to-use).
1. Create the folder structure under `.\data` for the volume mounts, see [example](https://github.com/sitecoreops/sitecore-images/tree/master/tests/9.1.0%20rev.%20001564/ltsc2019).
1. If you built the images on another machine you need to prefix the images with your repository name else you can leave it as is.
1. Start with: `docker-compose up -d`

You now have a fully functional Sitecore 9.1 XP running :)

```YML
version: '2.4'

services:

  sql:
    image: sitecore-xp-sqldev:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\sql:C:\Data
    mem_limit: 2GB
    isolation: process
    ports:
      - "44010:1433"

  solr:
    image: sitecore-xp-solr:9.1.001564-nanoserver-1809
    volumes:
      - .\data\solr:C:\Data
    mem_limit: 1GB
    isolation: process
    ports:
      - "44011:8983"

  xconnect:
    image: sitecore-xp-xconnect:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\xconnect:C:\inetpub\xconnect\App_Data\logs
    mem_limit: 1GB
    isolation: process
    links:
      - sql
      - solr

  xconnect-automationengine:
    image: sitecore-xp-xconnect-automationengine:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\xconnect-automationengine:C:\AutomationEngine\App_Data\logs
    mem_limit: 500MB
    isolation: process
    links:
      - sql
      - xconnect

  xconnect-indexworker:
    image: sitecore-xp-xconnect-indexworker:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\xconnect-indexworker:C:\IndexWorker\App_Data\logs
    mem_limit: 500MB
    isolation: process
    links:
      - sql
      - solr

  cd:
    image: sitecore-xp-cd:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\cd:C:\inetpub\sc\App_Data\logs
    isolation: process
    ports:
      - "44002:80"
    links:
      - sql
      - solr
      - xconnect

  cm:
    image: sitecore-xp-standalone:9.1.001564-windowsservercore-ltsc2019
    volumes:
      - .\data\cm:C:\inetpub\sc\App_Data\logs
    isolation: process
    ports:
      - "44001:80"
    links:
      - sql
      - solr
      - xconnect
```
