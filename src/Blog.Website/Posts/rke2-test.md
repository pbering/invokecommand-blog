---
title: Trying out Windows container support on RKE2
summary: Ranchers next-generation Kubernetes distribution.
date: 2023-10-07
tags: kubernetes, rke2, rancher, docker, sitecore
---

While distributions like [AKS Edge Essentials](/posts/aks-ee-test) *can* be the right fit for Windows users and server admins, it may not be optimal for maximum performance due to the fact that it is based on nodes running *inside* VM's, furthermore there are limits on how much memory and CPU is allowed for each of the nodes and how many nodes you are allowed to run.

If you want full flexibility but also avoid installing (and maintaining) Kubernetes by hand, then [RKE2 (Rancher Kubernetes Engine 2)](https://ranchergovernment.com/products/rke2) is a very good fit.

From the official sales pitch [emphasis mine]:

>"RKE2 is a **certified Kubernetes distribution** focused on security to adhere to the robust compliance requirements of the U.S. government. Leveraging the best components of RKE and K3s, RKE2 is **easy to install**, **secure by default**, and built to run on mission critical infrastructure **whether on prem, in the cloud, or at the edge**. RKE2 is Federal Information Processing Standards (FIPS) enabled, SELinux supported, and the only DISA STIG certified Kubernetes distribution for the Department of Defense (DoD), Intelligence Community and civilian agencies."

The [installation is very easy](https://docs.rke2.io/install/quickstart) and supports both Linux and Windows so let's try it out! We will be using [Vagrant](https://www.vagrantup.com/) to manage the VM's in our test lab.

## Setting up a minimal cluster

First thing first, some prerequisites:

- Hyper-V installed
- Vagrant **>= 2.8.3** (<https://github.com/hashicorp/vagrant/releases>)
- kubectl (`winget install --id Kubernetes.kubectl`)
- helm (`winget install --id Helm.Helm`)
- mkcert (`winget install --id FiloSottile.mkcert`)
- [SitecoreDockerTools](https://github.com/Sitecore/docker-tools/blob/main/README.md#powershell-module) installed

> Please note that Vagrant commands needs to *run as admin* to be allowed to communicate with Hyper-V!

Fire up your terminal and then:

1. run `git clone https://github.com/pbering/rke2-test.git` and `cd rke2-test`
1. run `vagrant up`

This will pull VM images and provision 1 server and 1 Windows agent, you can adjust the number of servers and Linux/Windows agents in <https://github.com/pbering/rke2-test/blob/main/Vagrantfile>.

> TIP: if you want a [HA](https://docs.rke2.io/install/ha) setup you need to adjust the server count to at least 3, but please be aware that if you go nuts at try to start many servers and agents in *one* go, it can fail due to Docker Hub throttling. Better to start with the minimal amount, then increase and run `vagrant up` again to get the new machines provisioned (already running machines are kept).

Using Vagrant is a very convenient way to build out infrastructure and automation **before** doing it on real servers and it can also serve as documentation on what needs to be done.

When finished you now have a running cluster, to connect you need grab the Kubernetes cluster config:

1. run `(vagrant ssh nixs1 --no-tty -c "cat /etc/rancher/rke2/rke2.yaml") -replace "server: https://127.0.0.1:6443", "server: https://nixs1.rke2.lab:6443" | Out-File -Path ./.kubeconfig`, to grab a copy from the server.
1. run `$env:KUBECONFIG="./.kubeconfig"`, to set your terminal to use it.
1. try it with `kubectl get nodes`, you should see a list of nodes in ready state.

You can also run [k9s](https://k9scli.io/) directly from inside the server by running: `vagrant ssh nixs1 -c k9s`.

### Deploy sample Windows workload

1. run `kubectl create namespace sample`
1. run `kubectl apply -f .\workloads\sample\ --namespace sample`
1. [optional] run `kubectl get pods --namespace sample -w` to wait for the pods to be running, this takes a while since all Windows images needs to be pulled.
1. open <http://sample.rke2-test.lab/>

### Deploy Sitecore workload

The Sitecore workload is this repo uses [Longhorn](https://longhorn.io/)  distributed block storage for SQL and Solr data.

1. run `helm repo add longhorn https://charts.longhorn.io`
1. run `helm repo update`
1. run `helm install longhorn longhorn/longhorn --namespace longhorn-system --create-namespace --version 1.5.1`
1. run `kubectl apply -f .\workloads\longhorn\ --namespace longhorn-system`
1. when running, open <http://longhorn.rke2-test.lab/> to access the dashboard.

> TIP: if you add more Linux agents so that there are at least 3 Linux nodes in the cluster, then longhorn will automatically distribute all storage claims among the nodes making persistence [HA](https://longhorn.io/docs/1.5.1/high-availability/auto-balance-replicas/).

Now install Sitecore XM:

1. run `Import-Module SitecoreDockerTools;ConvertTo-CompressedBase64String -Path "C:\License\license.xml" | Out-File -Encoding ascii -NoNewline -FilePath .\workloads\xm\sitecore-license.txt` to encode your Sitecore license file.
1. run `mkcert -cert-file ".\workloads\xm\cm-tls.crt" -key-file ".\workloads\xm\cm-tls.key" "cm-xm.rke2-test.lab"`
1. run `mkcert -cert-file ".\workloads\xm\id-tls.crt" -key-file ".\workloads\xm\id-tls.key" "id-xm.rke2-test.lab"`
1. run `kubectl create namespace xm`
1. run `kubectl apply -n xm -k .\workloads\xm`
1. when running, open <https://cm-xm.rke2-test.lab/>

## Conclusion

When it comes to choosing a Kubernetes distribution for your Windows based workloads, you have various options to consider. While distributions like AKS Edge Essentials can serve the needs of Windows users and server admins, they may not offer optimal performance due to their reliance on nodes running inside virtual machines and resource limitations.

This test lab shows how relative easy it is to install and use RKE2 in real life scenarios for both Linux and Windows workloads, hope you found it useful.
