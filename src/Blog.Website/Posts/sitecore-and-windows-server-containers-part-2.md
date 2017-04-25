---
title: Sitecore running in a Windows Server Container - Part 2
summary: Tips and tricks for working with containers and hosts.
date: 2015-12-18
tags: Sitecore, Containers, Docker, Notes
---

Here you can find some tips and tricks I picked up while playing around with this awesome new  Windows feature. You should also read [Part 1](/posts/sitecore-and-windows-server-containers) before this makes any sense :)

### Host VM tweaks

- To manage the host from your workstation you can use `Enter-PSSession -VMName "wscdev"`, more about the new PowerShell Direct feature on [MSDN](https://msdn.microsoft.com/en-us/virtualization/hyperv_on_windows/user_guide/vmsession "MSDN").
- Remove Windows Defender, totally cripples build and running images:

  `Uninstall-WindowsFeature -Name Windows-Defender-Features -Restart`

- To set the keyboard language to something else than en-US:

  `Set-WinUserLanguageList -LanguageList da-DK`

- Place the VHD on SSD for maximum disk performance, most image/container operations are very disk I/O hungry.
- Soon you will run out of disks space in the host VM (is 20 GB). Here is how to make a larger host with more space:

1. Copy `ServerDatacenterCore_en-us_TP4_Container.vhd` to `ServerDatacenterCore_en-us_TP4_Container_Extended.vhd`.
1. Use the "Edit disk" feature in Hyper-V to expand it.
1. Double-click the new expanded VHD to mount it.
1. Use "Disk Management" to re-size the OS volume.
1. Use "Explorer" to eject the mounted VHD.
1. Use VhdPath parameter of New-ContainerHost.ps1 to reference the expanded VHD.

### Connecting with the host VM from your workstation

1. On the host add **" -H 0.0.0.0:2375"** right after the first occurrence of **"-b "Virtual Switch"** and before the **"goto"** in the file **"c:\ProgramData\docker\runDockerDaemon.cmd"**

1. Run `net stop docker` and `net start docker` to restart the Docker daemon
1. Open the new port with: `New-NetFirewallRule -Name "TCP2375" -DisplayName "TCP2375" -Protocol tcp -LocalPort 2375 -Action Allow -Enabled True`
1. Now set the DOCKER_HOST environment variable to your host VM IP: `$env:DOCKER_HOST = "tcp://10.20.34.227:2375"`

You can now use `docker.exe` from you workstation!

### Cleaning up

Delete all stopped containers:

    docker rm $(docker ps -a -q)

Delete all tagged images tagged with "none":

    docker rmi $(docker images -q -f dangling=true)

### Other stuff

- Modifying NTFS permissions in containers [does not work](https://social.msdn.microsoft.com/Forums/en-US/fbb8bd89-d86d-4609-8607-558b28f93592/ntfs-permissions-not-committed?forum=windowscontainers) - Everything must run as LocalSystem...
- Please go vote on [Native container support on Windows 10](https://windowsserver.uservoice.com/forums/304624-containers/suggestions/11120520-native-container-support-on-windows-10) so we can run containers without an extra host VM layer.
