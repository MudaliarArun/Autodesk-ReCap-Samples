Copyright (c) Autodesk, Inc. All rights reserved 

Autodesk ReCap API samples
by Cyrille Fauvel - Autodesk Developer Network (ADN)
January 2014

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
 
 
The PHP Console sample
=====================

<b>Note:</b> For using the sample you need a valid oAuth credential and a ReCap client ID. Contact Stephen Preston @ stephen.preston@autodesk.com to get them.


Dependencies
--------------------
This sample is dependent of a 3rd party extension:

1. The Guzzle PHP extension

     
	 
Use of the sample
-------------------------

This sample is a command line sample where you control the various ReCap stage individually using one of the command below.

Usage:    ReCap [-d] [-r] [-h] [-i photosceneid] [-c command] [-p photo(s)]

	-r		Refresh Access token only
	-d	Debug mode. Display the RESTful response

	-c	Command list
			version - Displays the current ReCap server version
			current - Displays the current photosceneid in use
			create - Create a new Photoscene
			set - Set the current Photoscene ID - requires -i option
			release - Release the current photosceneid
			list - List all photoscenes present on your account
			properties - Displays current Photoscene properties
			upload - Upload photo(s) on your current Photoscene - requires -p option (could be a single file, a folder, or a search string)
			start - Launch your Photoscene
			progress - Launch your Photoscene
			result - Get the result
			delete - Delete the Photoscene and resources from server

	-h	Help - this message

--------
Written by Cyrille Fauvel (Autodesk Developer Network)  
http://www.autodesk.com/adn  
http://around-the-corner.typepad.com/  
