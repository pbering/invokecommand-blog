---
title: Live Mode, TDS and workflow settings on Standard Values gotcha
summary: When using TDS to manage a template with workflow settings on the standard values item, then you can't add it to a TDS project.
date: 2015-11-16
tags: TDS, Sitecore, Notes
---

### UPDATED 2015-11-19

> Hedgehog Development, the guys that makes TDS has fixed this issue in the next release, presumable v5.1.0.23 which I also tested. Thank you for the superb support!

### Lesson learned

Make sure to set `filterItem="false"` on you site if you need to add or sync a __Standard Values item that has workflow settings with TDS (Team Development for Sitecore).

### Scenario

Today I worked on solution that uses "Live Mode" locally using these settings:

````xml
<site name="website"
    database="master"
    filterItems="true"
    enableWorkflow="true" ... />
````

The template in question was a just "marker" template with no fields but it defined which workflow and which state should be used by future items of that template.

So I wanted to add that template and its the __Standard Values item to my TDS project (using latest version of TDS) but when I used the "Get Sitecore Items" dialog the __Standard values item was not visible from the list of items...

I was a bit puzzled at first and could not find the reason for why this was not working as expected. Since my TDS project was configured to use the master database, and I ***thought*** that the TDS connector was running in shell context and would then not be affected of any setting on the "website" site configuration.

Then a good college of mine (Thanks Jesper) pointed me in the right direction to simple set `filterItems="false"` and to my surprise it worked!