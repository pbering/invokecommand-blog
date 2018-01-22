---
title: solrwin - a Solr 6.6.2 Windows container with SSL
summary: Need a Solr server with SSL in a 1.5 GB nanoserver container that also works with Sitecore 9 and SIF?
date: 2018-01-22
tags: Solr, Sitecore, Containers, Docker, nanoserver
---

If you run Sitecore in Docker or just prefer to *not* having all kinds of dependencies (and often in multiple versions) installed on your workstation(s), then I have create a new Docker image with Solr *and* SSL just for you! ... if not (?) then I will recommend you to at least use [@kamsar's installation script](https://kamsar.net/index.php/2017/11/The-lazy-way-to-install-Sitecore-9/)

Anyways... clone and try out the Docker image from [https://github.com/pbering/solrwin](https://github.com/pbering/solrwin).

It also works with Sitecore 9.0.0 and 9.0.1 (where Solr now is the default search engine) and you can install the default cores using the Sitecore Installation Framework (SIF). The installed cores are persisted between restarts using volume mounts.

To use with SIF you need to copy the default "sitecore-solr.json" configuration and in that:

1. Remove the "StopSolr" and "StartSolr" task
1. Change the variable "Solr.Server" value to just: `[variable('Solr.FullRoot')]`

Then you can install the Solr cores with:

```` PowerShell
Install-SitecoreConfiguration @{
    Path        = "PATH-TO-SITECORE-SOLR.JSON-COPY"
    SolrUrl     = "https://solr:8983"
    SolrRoot    = "PATH-TO-CLONE-DIR/data/solrhome"
    SolrService = "void"
    CorePrefix  = "my-prefix"
}
````

As always, enjoy!