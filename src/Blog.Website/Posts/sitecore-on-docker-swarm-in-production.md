---
title: Sitecore on Docker Swarm in production
summary: Retrospective on running Sitecore on Docker Swarm in production and a look on what's next.
date: 2019-06-02
tags: Sitecore, Docker, Containers, Swarm, Kubernetes
---

With the announcement of [Windows Server Container *preview* support on Azure AKS (Azure Kubernetes Service)](https://azure.microsoft.com/en-us/blog/announcing-the-preview-of-windows-server-containers-support-in-azure-kubernetes-service/) last week I think it is a good time to reflect on how it has been to run Sitecore in Docker Swarm mode, in production for the last year. Just in time before diving deeper into Sitecore on AKS :)

... but couldn't help my self, had to try it out! I have been waiting so long for this to be possible and production ready, so here is a sneak peak of a running AKS cluster with Sitecore 9.1.1 XM (**spoiler alert**: Your current images works **just as fine and without any modifications**, in AKS as they do in Swarm):

[![Sitecore on Kubernetes](/content/images/sitecore-on-aks.png)](/content/images/sitecore-on-aks.png)

Anyhow, that is for another blog post! Back to Swarm...

## Goals

Besides the benefits of using Docker locally for development we also wanted:

- To use the same images in all upstream environments as we do locally.
- That anyone can do *fast* deployments of *any* branch.
- Fast to scale.
- Low maintenance and *cheap* environments.
- Log analytics and easy search in Sitecore logs.
- Reuse CI / CD processes across solutions and technology stacks.

## Implementation

Besides doing a talk at [SUGCON 2018 (Painless deployments of Sitecore using Docker Swarm)](https://github.com/sitecoreops/sugcon2018-docker) we had the intention to use the same approach both for dev/test workloads as well as production.

Here is a screenshot of one of those dev/test clusters where we run multiple Sitecore based solutions (and different branches here of). Each little box is a container running on a VM in Azure:

[![Sitecore on Swarm](/content/images/sitecore-on-swarm.png)](/content/images/sitecore-on-swarm.png)

In Swarm there is a concept of "stacks". A stack is a collection of containers (tasks in Swarm terms), for example defined in a Docker Compose file. This compose file can be deployed as a stack and containers in the stack is isolated from other stacks. Each stack form its own little network so if one container asks for a service called "sql" it only gets the "sql" service within its own stack. This is what makes it useful for deploying multiple different solutions and branches onto the same cluster:

[![Stacks](/content/images/stacks.png)](/content/images/stacks.png)

Today any developer can deploy any branch they whish in a few seconds. Depending on if the branch has been deployed before it will take a few minutes before Sitecore and related services are warm and ready. If it's the first deployment, SQL and Solr containers will automatically seed stock databases and cores for the version of Sitecore running. We use [Traefik](https://traefik.io) that listens on events in Swarm and automatically creates proxy rules with host names using a convention of `stackname-servicename.example.com` and since our convention for stack names is `customer-product-branch` we can then access any service in the stack, for example the CM on `https://acme-website-feature42-cm.example.com`.

The minimal infrastructure (IaaS) supporting this looks like this:

[![Swarm on Azure](/content/images/swarm-on-azure.png)](/content/images/swarm-on-azure.png)

All Azure resources is completely handled as "Infrastructure as Code" using [Terraform](https://www.terraform.io) and VM's are not updated but deleted and added again so we always have fresh and updated nodes running. This process is today manually *executed* and so is scaling the nodes or the number of nodes. More about this tradeoff later...

Besides running Sitecore related Windows containers (CM, CD, Solr, Sql etc) we also run operational containers (on Linux) for Sitecore log analytics ([ElasticSearch / Kibana](https://www.elastic.co/)) and [Traefik](https://traefik.io) for proxying traffic to the right containers.

## Going forward

I think we achieved all of our goals state above and we are quite happy about the current implementation, but there are areas on which we want to improve on.

### Storage

The Docker engine on Windows does not currently support volume plugins as it does on Linux. This means that we have very limited options on where we can mount data volumes. Limited as in either we use local volumes on each host which is can be bad if/when a container moves to another host or we can use [SMB Mounts](https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/container-storage#smb-mounts) that maps a Azure File Share.

SMB mounts *kind of works* but only if you create/update your services using `docker service` commands and not using Docker Compose files. Another showstopper for using SMB in our case is that the directories on the shares needs to exist *before* a container/task is started which is not really practical when we want to deploy any branch and stack dynamically. 

Today we store SQL and Solr data on dedicated host and use labels to ensure that a container/task is *pinned* to those hosts which works fine, but it can require some manual work if containers/task needs to move to another host. Comparing to AKS there are more storage options, also for Windows container as both Azure Disks (read/write one pod only) and Azure File Shares (read/write by many pods) are available.

### IaaS vs. SaaS

Keeping everything in Terraform and using it to deploy changes to the infrastructure helps a lot to mitigate the shortcomings of IaaS compared to SaaS but we still have to manually update the VM's. We do so by starting up new machines from fresh VM images, draining the old, move data. Moving the data is the annoying part, not so much in production as the SQL databases are run on Azure SQL but still a hassle (and manually) for Solr.

Comparing to AKS, the VM's still needs updating but the OS patching process can be handled by Kubernetes itself (using [kured](https://github.com/weaveworks/kured)) that will handle rescheduling pods to other nodes in case of reboots are needed. Not sure how to Windows works yet, but it's easy to add and remove node pools (collections of VM's) so I guess it can be automated also.

In regards of pricing I think that AKS would be a bit cheaper that what we do today. In Docker Swarm it's recommended to have at least 3 manager nodes for the cluster to keep consensus on the state where you can get by with only 1 in Kubernetes.

### Auto scaling

There is no auto scaling in Docker Swarm, we monitor CPU, Memory and disk usage and then add or remove machines as needed by changing a variable in the Terraform. Containers/tasks are also scaled manually by increasing / decreasing the replica count of service. In Kubernetes it's possible to configure auto scaling of pods using metric and in the AKS preview there its also possible to configure auto scaling of node pools. The VM's in the node pools are created in a Azure VM scale sets so you can also set an upper limit on the number of VM's.

## Closing

Even with the few downsides described above I would always prefer to run Sitecore in Docker containers (also in production) using Swarm, instead of a classic Sitecore Azure App service deployments or on-prem. I'm sure that we will end up moving our clusters to AKS in near future to mitigate the downsides and also to get auto scaling. As long as Sitecore (or any other Windows based app) runs in containers, we can avoid long slow deployments and we are not bound which orchestration we can use - or which cloud provider for that matter, so our dev teams can move faster with more confidence.
