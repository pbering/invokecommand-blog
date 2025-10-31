---
title: Introducing Experience Edge Emu (EEE)
summary: Local emulator for Sitecore Experience Edge.
date: 2025-10-31
tags: sitecore, xmcloud, graphql
---

Just released [Experience Edge Emu (EEE)](https://github.com/sitecoreops/experience-edge-emu)! The lightweight Experience Edge emulator for local (offline) cross-platform development and test automation.

This is a small application that emulates the [Sitecore Experience Edge](https://doc.sitecore.com/xmc/en/developers/xm-cloud/experience-edge.html) so you can use it as a complete replacement. You can add your own data files which is the JSON response from a GraphQL query *OR* you can crawl existing edge endpoints to seed the emulator with data!

Check the current [features](https://github.com/sitecoreops/experience-edge-emu?tab=readme-ov-file#features)!

Most of what you can do on a real Experience Edge endpoint is supported, `item`, `layout` and `site` queries, but `search` is not supported at the moment, see [limitations & known issues](https://github.com/sitecoreops/experience-edge-emu?tab=readme-ov-file#limitations--known-issues).

## Why use an emulator?

In a normal XM Cloud setup, all developers share the same Experience Edge endpoint, which makes isolated testing difficult. Or they are forced to run a local XM Cloud, but that is Windows only. EEE gives you your own private, cross-platform, local instance with (almost)full Experience Edge API compatibility.

Possible reasons to run EEE:

1. You want to work in isolation from other developers using the same shared XM Cloud instance.
1. You want to run some automated integration tests using fixed data.
1. You want to work offline.

## Try it out

EEE is released as [native binaries](https://github.com/sitecoreops/experience-edge-emu/releases) and a [Docker multi-platform image](https://github.com/sitecoreops/experience-edge-emu/pkgs/container/eee) that runs on both Linux and Windows. For this example I'm using Docker on Windows.

1. Let's start EEE, make sure `./data` exists beforehand since we are on Windows:

   ```powershell
   docker run -d --name eee -v "./data:C:/app/data" -e "EMU__MEDIAHOST=http://localhost:5710" -p 5710:8080 ghcr.io/sitecoreops/eee
   ```

1. Check the logs to see if everything looks good: `docker logs eee`, you will see a message like `[13:39:49 INF] Finished indexing, duration=00:00:00.004294, items=1, sites=0`.
1. Success! Let's query for that 1 item that was indexed: Open the UI at [http://localhost:5710/](<http://localhost:5710?query=%7B%0A%20%20item(path%3A%20%22%2Fsitecore%2Fcontent%2Ftests%2Fminimal%22%2C%20language%3A%20%22en%22)%20%7B%0A%20%20%20%20name%0A%20%20%20%20path%0A%20%20%20%20displayName%0A%20%20%7D%0A%7D>) and run the query (shortcut: `Ctrl+Enter`). You should see response similar to this:

   ```json
   {
     "data": {
       "item": {
         "name": "minimal",
         "path": "/sitecore/content/tests/minimal",
         "displayName": "Minimal Test Item"
       }
     }
   }
   ```

1. Now let's put some more data in there, try to execute the [crawl mutation](http://localhost:5710/?query=mutation%20%7B%0A%20%20crawl(%0A%20%20%20%20edgeContextId%3A%20%22%3CLIVE%20CONTEXT%20ID%3E%22%0A%20%20%20%20languages%3A%20%5B%22en%22%5D%0A%20%20)%20%7B%0A%20%20%20%20success%0A%20%20%20%20itemsProcessed%0A%20%20%20%20sitesProcessed%0A%20%20%20%20durationMs%0A%20%20%20%20message%0A%20%20%7D%0A%7D&operationName=Crawl) that crawls an existing edge endpoint for all sites in the languages defined. **Update** `<LIVE CONTEXT ID>` **with your own**. You can filter sites to crawl using the `siteNames` parameter if you only want to crawl some sites.
1. You are now ready to change your local head app edge endpoint to EEE endpoint: <http://localhost:5710/graphql>.

## Conclusion

Besides being fun to implement, could this be useful to you? Try it out and share your feedback or ideas - contributions are welcome!.

Maybe Sitecore announces something to get rid of local XM Cloud instances completely at Symposium 2025, who knows 🙂 - but till then, [EEE](https://github.com/sitecoreops/experience-edge-emu) could be a good fit if you want to work in isolation!
