---
title: Specialized SQL Server images added to the Sitecore Docker image repository
summary: Ready to use images with embedded Sitecore databases and volume support.
date: 2018-02-11
tags: Sitecore, Docker, SQL, Automation, Containers
---
Just updated the [sitecoreops/sitecore-images](https://github.com/sitecoreops/sitecore-images) repository with specialized SQL Server images for each Sitecore 9 version (see the [Automatically build and update Sitecore base Docker images](/posts/automatically-build-and-update-base-images) post for details on how to set it up).

The new images is based on the official [Microsoft SQL Server 2016 SP1 Developer Edition image](https://hub.docker.com/r/microsoft/mssql-server-windows-developer/) but it have as few additional features:

1. Automatically adds all Sitecore databases to the volume mount `C:\data` when starting up, if no databases was found.
1. Automatically attaches all databases in the volume mount `C:\data`.

Since the volume mount is specified in the image, the databases will **always** be persisted on your host machine, either in the default Docker volume location or another location of your choosing!

With this you can now start a fresh Sitecore instance with *only* this compose file and nothing else:

````YML
version: '2.1'

services:

  sql:
    image: your-org-or-username/sitecore:9.0.171219-sql
    volumes:
      - .\data:C:\Data

  cm:
    image: your-org-or-username/sitecore:9.0.171219-xm1-cm
    ports:
      - 80:80
    links:
      - sql
````