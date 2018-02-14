---
title: The circle is complete, specialized Solr image also added
summary: Solr image for Sitecore 9.0.1 with embedded core and volume support added to the Sitecore Docker image repository.
date: 2018-02-13
tags: Sitecore, Docker, Solr, Automation, Containers
---
The other day I wrote about [a ready to use SQL server image](/posts/specialized-sql-images-added-to-sitecore-docker-repository) that was added to the [sitecoreops/sitecore-images](https://github.com/sitecoreops/sitecore-images) repository, now it's Solr's turn to get some love!

It works just like the SQL Server image as it will add all Sitecore cores [built using SIF](https://github.com/sitecoreops/sitecore-images/blob/b7a7c3f0458d6f589c40bd59ca2608651aeb322c/sitecore/9.0.1%20rev.%20171219%20Solr/Dockerfile) to the volume mount `C:\data` when starting up (if no cores exists already).

You can start a complete Sitecore 9.0.1 XM1 instance with just:

````YML
version: '2.1'

services:

  sql:
    image: your-org-or-username/sitecore:9.0.171219-sql
    volumes:
      - .\data\sql:C:\Data
    mem_limit: 3500MB # Recommended minimum (default is 2GB)

  solr:
    image: your-org-or-username/sitecore:9.0.171219-solr
    volumes:
      - .\data\solr:C:\Data

  cm:
    image: your-org-or-username/sitecore:9.0.171219-xm1-cm
    links:
      - sql
      - solr
````

>NOTE: First time only: Make a request to `/sitecore/admin/PopulateManagedSchema.aspx?indexes=all` (same as: Control Panel -> Indexing -> Populate Solr Managed Schema). This step is normally done by SIF when installing the `sitecore-XM1-cm.json` config, when running in non Docker environments.