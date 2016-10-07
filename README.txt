******************************************************************************
Source code for The Definitive Guide to db4o, by Jim Paterson, Stefan Edlich, 
Henrik Hoerning and Reidar Hoerning, ISBN: 1-59059-656-0,Apress
******************************************************************************

This file contains source code for the examples in Part II, Working with db4o, 
which is the part of the book which contains a "hands-on" guide to db4o.

There are separate folders for code in C# and Java. The folder contents are as
follows:

C#
There is a separate folder for each chapter from Chapter05 to Chapter12. Each
folder contains a Visual Studio 2005, including .cs source files. Projects can
be opened and executed in VS2005 and Visual C# 2005 Express. Every project
required a reference to a suitable version of db4o.dll (versions 5.0 and 5.2
were used while writing the book)

There are three additional projects:
db4otools - includes db4o.tools which is distributed as source code rather than
in the dll. Chapter10 is dependent on this project.
MixedExample - allows the class MixedExample described in chapter 12 to be run
separately from the other examples from that chapter.
dg2db4oweb - ASP.NET project described in chapter 12.

JAVA
The java folder contains a source tree inside the src folder. Each chapter
has its own package which you can import into your IDE. For example, chapter 5
code is in the package com.db4o.dg2db4o.chapter5. A suitable version of the db4o
JAR needs to be added to your classpath or to your IDE project (versions 5.0 
and 5.2 for Java 5 were used while writing the book). The BLOAT libraries, 
described in chapter 6, should also be added to allow Native Query optimization.

Each package contains one
or more executable class, as follows:

chapter5: CompleteExample.java
chapter6: Main.java
chapter7: Main.java
chapter8: Main.java, RunServer.java, RunListClient.java, RunAddClient.java
chapter9: Main.java
chapter10: PicBlobWrapper.java, BackupDemo.java, ToolsDemo.java
chapter11: Main.java, RunServer.java, RunClient.java, RunReplicator.java, 
    StartHsqlServer.java, DRSBasicReplication.java, HibRep.java (the last three
    require the dRS and hsql libraries)
chapter12: Main.java, MixedExample.java, IndexBenchmark.java

There are additional folders as follows:
com/db4o/tools - includes db4o.tools which is distributed as source code rather 
than in the JAR. Some of the chapter 10 code is dependent on this.
dg2db4oweb - contains servlets and JSP for the web example described in
chapter 12. These can be imported into the appropriate locations in your
web application.

Note that in many examples, database file names are specified as Windows names -
these should be modified to your own preferred location appropriate for your
OS.
