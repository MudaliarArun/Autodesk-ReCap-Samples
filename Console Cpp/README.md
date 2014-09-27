 The C++ Console sample
=====================

This sample is a command line sample where you control the various ReCap stage individually using one of the command below.

<b>Note:</b> For using those samples you need a valid oAuth credential and a ReCap client ID. Contact ReCap API <recap.api@autodesk.com> to get them.


Dependencies
--------------------
This sample is dependent of the following 3rd party extension:

* The Casablanca C++ RESTful SDK [Source code](https://casablanca.codeplex.com/)

Because the oAuth1.0a experimental implementation is buggy, you will need to modify the source code and recompile the library.
Required code changes has been submitted to Microsoft for future release.


Setup Instructions
-------------------------

* Recompile Casablanca

	1. Open the ReCap project in Visual Studio 2012 (or 2013)

	2. Compile (this is required to force NuGet to create the Casablanca NuGet local package)

	3. Download Casablanca source code from [https://casablanca.codeplex.com/SourceControl/latest#](https://casablanca.codeplex.com/SourceControl/latest#)

	4. Open the Casablanca project in Visual Studio 2012 (or 2013)
	
		- casablanca110.desktop.sln = Visual Studio 2012
		- casablanca120.desktop.sln = Visual Studio 2013

	5. Patch the:

		- Release/include/http_constants.dat
		- Release/include/oauth1.h
		- Release/src/http/oauth/oauth1.cpp

	  files with the ones from the Casablanca directory from the ReCap download
	  
	6. Recompile Casablanca in the configurations you need
	
	7. Copy for each configurations the following files (target folder may change depending of your platform/configuration)
	
		- Release/include/oauth1.h and Release/include/http_constants.dat to 'Autodesk-ReCap-Samples\Console Cpp\packages\cpprestsdk.2.2.0\build\native\include\cpprest'
		- <Your_Casablanca>\Binaries\x64\Debug\*.lib to 'Autodesk-ReCap-Samples\Console Cpp\packages\cpprestsdk.2.2.0\build\native\lib\x64\v110\Debug\Desktop'
		- <Your_Casablanca>\Binaries\x64\Debug\*.* to 'Autodesk-ReCap-Samples\Console Cpp\packages\cpprestsdk.2.2.0\build\native\bin\x64\v110\Debug\Desktop'
	
	
* Compile the ReCap sample

	1. Copy the UserSettings_.h into UserSettings.h
	
	2. Open the ReCap project in Visual Studio 2012 (or 2013)

	3. Edit UserSettings.h, and replace the following key strings with appropriate value
     * CONSUMER_KEY
	 * CONSUMER_SECRET
	 * ReCapClientID
	 * Email
	 
	4. Compile and you are ready to go.
	  use the -h command argument to get help.
	
Usage Instructions
-------------------------

The command will save your current PhotosceneID and oAuth token in local files, you can delete these files to reset the system.

```
Usage:    ReCap [-d] [-y] [-r] [-h] [-i photosceneid] [-c command] [-p photo(s)]

	-r	Refresh Access token only
	-d	Debug mode. Display the RESTful response
	-y	Enable proxy debug when using Fiddler
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
```

Typical scenario:
```
ReCap -c create
ReCap -c upload -p ../Examples/Tirelire
ReCap -c properties
ReCap -c start
ReCap -c progress

once 'progress' reports no error and completion at 100%

ReCap -c properties
ReCap -c result
```

--------

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.


## Written by

Cyrille Fauvel (Autodesk Developer Network)  
http://www.autodesk.com/adn  
http://around-the-corner.typepad.com/  
