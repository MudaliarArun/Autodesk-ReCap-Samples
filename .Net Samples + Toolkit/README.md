Copyright (c) Autodesk, Inc. All rights reserved 

Autodesk ReCap API samples
by Philippe Leefsma - Autodesk Developer Network (ADN)
March2014

Permission to use, copy, modify, and distribute this software in
object code form for any purpose and without fee is hereby granted, 
provided that the above copyright notice appears in all copies and 
that both that copyright notice and the limited warranty and
restricted rights notice below appear in all supporting 
documentation.

AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
UNINTERRUPTED OR ERROR FREE.
 
 
.Net Samples + Toolkit
=======================

<b>Note:</b> For using those samples you need a valid oAuth credential and a ReCap client ID. Contact Stephen Preston @ stephen.preston@autodesk.com to get them.


Dependencies
--------------------
This sample is dependent of following 3rd party assemblies:

1. The RestSharp assembly

     RestSharp - Simple REST and HTTP Client for .NET
	 you need at least version 104.3.3. You can get this component source code [here](http://restsharp.org/)

2. The Xceed WPF Toolkit Community Edition

     this assembly is only used if you want to display the ReCap properties in a Property window. Properties are anyway dumped into a text window. 
	 You can get the binaries and documentation from [https://wpftoolkit.codeplex.com/](https://wpftoolkit.codeplex.com/)


Building the sample
---------------------------

The sample was created using Visual Studio 2012 Service Pack 1

You first need to modifythe UserSettings.cs file and put your oAuth & ReCap credentials in it.
	 
Use of the sample
-------------------------

* When you launch the sample, the application will try to connect to the ReCap server and verifies that you are properly authorized on the Autodesk oAuth server. 
If you are, it will refresh your access token immediately. If not, it will ask you to get authorized. 

* The samples display a treeview that contains existing ReCap photoscenes, right-click on the root node to display a context menu that allows to create a new scene.
Right-click on each scene node to display options.


--------
Written by Philippe Leefsma(Autodesk Developer Network)  

