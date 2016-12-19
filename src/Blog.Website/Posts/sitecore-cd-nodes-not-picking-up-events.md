---
title: Sitecore CD nodes not picking up events after replication re-initialization
summary: What happens with CD nodes after replication re-initialization and why do they ignore all events?
date: 2015-10-04
tags: Sitecore, SQL, Notes
---

### TL;DR

**Always** clear all rows where the Key starts with "EQStamp_" in the Properties table after re-initializing SQL Server replication, so that the CD nodes knows the "timestamp" of the latest event in the EventQueue.

	DELETE FROM [Properties] WHERE [Key] LIKE 'EQStamp_%'

### Scenario

You are using SQL Server replication on one or more Sitecore databases, typically the web database to keep databases in sync across data centers. Things runs fine but then you need to make changes to the replication where you need to remove subscribers and reinitialize them again. Could be to physically move databases to another cluster or during recovery.

This happened to me twice now, so I figured I needed to write it down :) 

In my case I had the web database on-premise as the publisher and two subscribers, one located in a data center in Europe and one in the US. During a routine task the DBA had to remove the replication and reinitialize subscribers from a snapshot. After that our editors started to report that they could not see their changes after publish. While investigating logs on the CD nodes I found that no remote events was being fired at all, not even publish:end:remote where HtmlCacheClearer normally would write an entry when it does. 

But still we could see that data *was* being replicated and all events in the EventQueue tables was up to date.

### So what happed?

On a Sitecore CD node the EventQueue is being checked for new events every 2 second by default (configured in "processingInterval"), in this check Sitecore uses a value from the Properties table named:

	EQStamp_<instance name>
	
It has the string value (the type is ntext) of the latest raised event from the [Stamp] column (the type is [timestamp]) in the EventQueue table. So with each check Sitecore asks only for events **newer** that the last raised event.

The problem with this is the use of [timestamp], the value of [timestamp] on insert and deleted is a binary counter that is relative in the database, from [MSDN](https://msdn.microsoft.com/en-us/library/ms182776.aspx):

>Each database has a counter that is incremented for each insert or update operation that is performed on a table that contains a rowversion column within the database. This counter is the database rowversion. **This tracks a relative time within a database, not an actual time that can be associated with a clock**

So what happens here is that the CD nodes *thinks* it has checked events from a certain point in time, but all new events is "older" since SQL Server is starting over with the value of [timestamp].

Theoretical the CD nodes would start working again when event [Stamp] values catches up to the value in the Properties table. But that could takes years depending on how long your previous replication had been running. I doubt that any editor could live with that!

By the way, the **timestamp** data type has been deprecated since SQL Server 2008 and should not be used, check [MSDN](https://msdn.microsoft.com/en-us/library/ms182776.aspx):

>The timestamp syntax is deprecated. This feature will be removed in a future version of Microsoft SQL Server. Avoid using this feature in new development work, and plan to modify applications that currently use this feature.
