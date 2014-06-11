
Copyright (c) Autodesk, Inc. All rights reserved 

Autodesk ReCap API samples
by Cyrille Fauvel - Autodesk Developer Network (ADN)
June 2014

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
 
 
The iOS sample
=======================

<b>Note:</b> For using the sample you need a valid oAuth credential and a ReCap client ID. Contact Stephen Preston @ stephen.preston@autodesk.com to get them.


Dependencies
--------------------
This sample is dependent of five 3rd party assemblies which are already installed are ready to compile in the project. 
You can update/install them automatically via CocoaPods. Visit http://cocoapods.org/ for installing / using CocoaPods.

1. The RestKit pod

     RestKit is a framework for consuming and modeling RESTful web resources on iOS and OS X.
	 you need at least version 0.23. pod 'RestKit', '~> 0.23'
	 http://cocoapods.org/?q=restkit

2. The ZipKit pod

     An Objective-C Zip framework for Mac OS X and iOS.
	 you need at least version 1.0. pod 'ZipKit', '~> 1.0'
	 http://cocoapods.org/?q=zipkit


3. The AFOAuth1Client pod

     AFNetworking Extension for OAuth 1.0a Authentication.
	 you need at least version 0.3. pod 'AFOAuth1Client', '~> 0.3'
	 http://cocoapods.org/?q=AFOAuth1Client

	 
each pod comes with their own dependencies, but cocoapods will manage them for you.
	 
Building the sample
---------------------------

The sample was created using Xcode 5.1.1, but should build/work fine with any 5.x version. It also target iOS version 7.x, but can be ported back up to iOS 5.0 (not tested). 

You first need to modify (or create) the UserSettings.h file and put your oAuth / ReCap credentials in it.
There is a _UserSettings.h file that you can copy to create your own version.

The first time you are going to build the sample, it will fail ! the reason is that the sample also demonstrate how to build frameworks to be reused in other projects.
As it needs both simulator and real device binaries and because cocoapds do not automate building both version at once, you need to switch from a simulator scheme to a device scheme once and build.
That will force the pods to build in each scheme, and then you can build the project.
	 
Use of the sample
-------------------------

* when you launch the sample, the application will try to connect to the ReCap server and verifies that you are properly authorized on the Autodesk oAuth server. 
If you are, it will refresh your access token immediately. If not, it will ask you to get authorized.

The sample will show you the list of project(s) you get on your account. If you select one, it will open and display the commands and Photoscene' properties.
If you do not have a project yet, then press the '+' button to create one. It will appear selected automatically.

* Project View - There is 5 commands

   * Refresh - will refresh the project properties
   * Camera - to take photos - the sample will create an album in your 'Camera Roll' with the PhotosceneID as name. You can take as many photo you want.
   ReCap Photo needs a minimum of 4 photos to run, and usually 40 are enough for a 360 view of an object. You can also use the standard camera application 
   to create your photos, this command is just for convenience.
   * Photos - select photos and upload them on the ReCap Photo Server. There is a progress bar to show you when the upload is completed.
   * Process - will launch the Photoscene to create the mesh. There is a progress bar to show you when the Server has completed the process.
   Once it is done, you may need to use the Refresh command to verify it was successful or not.
   * Preview - the last command is to download and preview the resulting mesh on the device.
   
* Camera view

   * You can zoom in/out with 2 fingers as usual
   * Tap once anywhere to take a photo
   * Swipe to left to exit the camera mode
   
* Preview view

   * Tap once to auto animate (or stop)
   * Pan one finger to orbit the mesh
   * Pan two fingers to pan the mesh
   * You can zoom in/out with 2 fingers as usual
   * Swipe to left to exit the preview mode

   
To be implemented

   * Wait icon at start when the application is getting you Project list
   * Double Tap in preview view to create a screenshot
   * Optimizing the OBJ loader code
   * Auto generate project icon in project list
   * Logout button
   
   
--------
Written by Cyrille Fauvel (Autodesk Developer Network)  
http://www.autodesk.com/adn  
http://around-the-corner.typepad.com/  
