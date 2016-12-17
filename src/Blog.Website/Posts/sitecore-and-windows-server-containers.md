---
title: Sitecore running in a Windows Server Container
summary: The day finally came! It is now possible to run Sitecore (and other ASP.NET 4.6 apps) in a Windows Server Container.
date: 2015-11-23
tags: Sitecore, Containers, Docker
---

### Be sure to also read... ###

Part 2: [Tips and trick](/posts/sitecore-and-windows-server-containers-part-2)

### Say what? ###

Today it's possible to run .NET applications built with ASP.NET 5 targeting CoreCLR, natively on Linux. **But for applications like Sitecore that is based on ASP.NET 4.5 and the full .NET framework, Windows and IIS on bare metal or VM's are our only options, until now that is.** 

With the release of Windows Server 2016 TP4 it is now finally possible to run Sitecore in a container! Exiting times ahead!

In case you don't know what Windows Server Containers (WSC) is about:

>Windows Containers provide operating system level virtualization that allows multiple isolated applications to be run on a single system. Two different types of container runtime are included with the feature, each with a different degree of application isolation. Windows Server Containers achieve isolation through namespace and process isolation. Hyper-V Containers encapsulates each container in a light weight virtual machine. In addition to two runtimes, both can be managed with either PowerShell or Docker. (Source [MSDN](https://msdn.microsoft.com/virtualization/windowscontainers/containers_welcome)) 

In TP4 is is now for the first time possible to install and use the ASP.NET 4.5 IIS feature, read more about application compatibility on [MSDN]( https://msdn.microsoft.com/en-us/virtualization/windowscontainers/reference/app_compat).

### Getting started ###

The focus in this article is on Windows Server Containers so using Hyper-V on Windows 10 RTM can be used if you wish to try this out on you own box as I did. It's faster to copy the 450 MB  of Sitecore files locally than to, for example a host in Azure, at least while your are building images.

1. Start PowerShell as an administrator
2. Check that you have a switch configured with: `Get-VMSwitch | ? {$_.SwitchType –eq “External”}` if not, create one first.
3. Run this to get the install script: `wget -Uri https://aka.ms/tp4/New-ContainerHost -OutFile c:\New-ContainerHost.ps1`
4. Run: `c:\New-ContainerHost.ps1 –VmName wscdev -WindowsImage ServerDatacenterCore`, this takes a while...
5. Use the Hyper-V manager and connect to the new VM `wscdev` and verify that you have the base OS images available with `docker images` - There should be a `windowsservercore` in the list.
6. Start a new container interactively with: `docker run --name iisbase -it windowsservercore PowerShell`
7. Inside this new container you can now make your server ready for Sitecore, first install IIS: `Install-WindowsFeature Web-Server` then ASP.NET with `Install-WindowsFeature Web-Asp-Net45`.
8. Type `exit` when done fiddling and then `docker commit iisbase iisaspnet` to save the container as an image
9. Type `docker images` to verify that you see the image `iisaspnet`
10. Add a new firewall rule so you can access your container inside your host from you desktop: `New-NetFirewallRule -Name "TCP80" -DisplayName "TCP80" -Protocol tcp -LocalPort 80 -Action Allow -Enabled True`
11.  Make and directory `c:\builds` and create a file named `Dockerfile` with contents:

		FROM iisaspnet
		ADD ./Sitecore /inetpub/wwwroot
12. Copy a Sitecore website to `c:\builds\Sitecore`
13. Type `docker build -t sitecore c:\build`, this takes a while...
14. Type `ipconfig` and take note of the host VM's IP address, mine is `10.20.34.200`
15. Type `docker run --name sitecoredemo -it -p 80:80 sitecore PowerShell` to start you new container!
16. You can now access Sitecore on `http://10.20.34.200` from you desktop!  

### Why is this important?###

You properly already read about and maybe used containers before with Docker and aware of the benefits and possibilities. 

Both Docker and Windows Server Containers are built around images, consider this:


	Image 1. Windows Server Core	 : 1000 MB *
	Image 2. IIS, ASP.NET 4.5/4.6	 :  315 MB
	Image 3. Clean Sitecore 8.1		 :  420 MB
	Image 4. Sitecore implementation :   50 MB
	------------------------------------------
	                                   1785 MB
	
	* Currently "docker images" returns the size 0 B for the 
	base image "windowsservercore" so 1 GB is a guess.
	
Image 4 is based on 3, 3 on 2 and so forth. Images are immutable and it is only the container running based on image 4 that is writable. Image 4 contains only the code base excluding Sitecore distributed files.

So with image 4 you will only be deploying a 50 MB image through your pipeline rather than 1785 MB **and** that image is a **complete server** with a Sitecore implementation ready to run. In continuous integration and deployment pipelines, the speed and size matters.  

Imagine a 50 MB container image that boots in seconds right on your dev or test box!  Containers are also way more lightweight in terms of both size and resource consumption than VM's, so you can have many isolated containers running on a single host. I does not matter if that host is your local machine, your own data center or the cloud, containers are portable across hosts.

### Summary ###

**Sitecore in a container, this is indeed possible today** and something I will dig much more into. Imagine a future where Sitecore delivers containers through there own Docker repository, ready to use containers of every version of Sitecore, that we could build our own images from. Imagine Sitecore delivers ready made CD and CM optimized containers!
