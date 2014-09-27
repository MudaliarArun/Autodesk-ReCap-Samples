 The C++ Console sample
=====================

This sample is a command line sample where you control the various ReCap stage individually using one of the command below.

<b>Note:</b> For using those samples you need a valid oAuth credential and a ReCap client ID. Contact ReCap API <recap.api@autodesk.com> to get them.


Dependencies
--------------------
This sample is dependent of the following 3rd party extensions:

* The Casablanca C++ RESTful SDK [oAuth extension](http://C++.net/manual/en/book.oauth.C++)
* The [Guzzle C++](https://github.com/guzzle/guzzle) extension, document [here](http://guzzle.readthedocs.org/en/latest/)


Setup Instructions
-------------------------


	 
Usage Instructions
-------------------------

Log on the Autodesk oAuth server using the oAuthLog.C++ script, i.e:

	C++ -f oAuthLog.C++
	
this command needs to be ran only once, unless your credential has expired. The command saves your access token into a file named oauth.txt that the ReCap sample will refresh and consume later.
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
