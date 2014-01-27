<?php
/*
 Copyright (c) Autodesk, Inc. All rights reserved 

 PHP Autodesk ReCap Console Sample
 by Cyrille Fauvel - Autodesk Developer Network (ADN)
 August 2013

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
 
 This sample is a modified version of the Autodesk oAuth sample that you can find here:
 https://github.com/ADN-DevTech/AutodeskOAuthSamples/tree/master/AdskOAuth%20PHP
 
 This sample is to authentify you on the Autodesk Oxygen server .It demo'es the '3 legs' process to authentify
 a user on the Autodesk Cloud infrastructure.
 
 After installing PHP on your system, you need to install the Google PHP OAuth library
 The Google OAuth Client library found here: (http://code.google.com/p/oauth-php/)
 
 You also need to modify the Google OAuth library code like this:
    file oauth\OAuthRequester.php line #285, add
           return ($token) ;

 The PHP oAuth API is documented here:
 http://php.net/manual/en/book.oauth.php

 The '3 legs' process is as follow:
 a- Get a 'request token' from the system
 b- Authorize the received token. Note here that Autodesk currently require you to manual log on Oxygen
      for authorization. This is why the sample is using your default browser for logging.
 c- Get an 'access token' and a session
 
 In case you want to use Fiddler to debug the HTTP request, you can add
 CURLOPT_PROXY => '127.0.0.1:8888'
 to the curl options.
 But you would also need to modify the library code to cope with the Fiddler header.
 Modify oauth/OAuthRequester.php
  a- requestRequestToken() line # 161
  b- requestAccessToken() line # 248
 with this
         $text        = $oauth->curl_raw($curl_options);
if ( isset ($curl_options [CURLOPT_PROXY]) ) {
        $text =substr ($text, 4) ;
        $text =strstr ($text, 'HTTP') ;
}                
        if (empty($text))

*/
require 'vendor/autoload.php' ;
use Guzzle\Http\Client ;
use Guzzle\Plugin\Oauth\OauthPlugin ;
require_once ('UserSettings.php') ;

//- Oxygen
$token ='' ;
$access ='' ;

//- Prepare the PHP OAuth for consuming our Oxygen service
//- Disable the SSL check to avoid an exception with invalidate certificate on the server
$oauth =new OAuth (CONSUMER_KEY, CONSUMER_SECRET, OAUTH_SIG_METHOD_HMACSHA1, OAUTH_AUTH_TYPE_URI) ;
$oauth->enableDebug () ;
$oauth->disableSSLChecks () ;

try {
	//- 1st leg: Get the 'request token'
	$token =$oauth->getRequestToken (O2_REQUESTTOKEN) ;
	//- Set the token and secret for subsequent requests.
	$oauth->setToken ($token ['oauth_token'], $token ['oauth_token_secret']) ;

	//- 2nd leg: Authorize the token
	//- Currently, Autodesk Oxygen service requires you to manually log into the system, so we are using your default browser
	$url =O2_AUTHORIZE . "?oauth_token=" . urlencode (stripslashes ($token ['oauth_token'])) ;
	exec (DEFAULT_BROWSER . $url) ;
	//- We need to wait for the user to have logged in
	echo "Press [Enter] when logged" ;
	$psLine =fgets (STDIN, 1024) ;
	
	//- 3rd leg: Get the 'access token' and session
	$access =$oauth->getAccessToken (O2_ACCESSTOKEN) ;
	//- Set the token and secret for subsequent requests.
	$oauth->setToken ($access ['oauth_token'], $access ['oauth_token_secret']) ;
	
	//- To refresh the 'Access token' before it expires, just call again
	//- $access =$oauth->getAccessToken (BaseUrl . "OAuth/AccessToken") ;
	//- Note that at this time the 'Access token' never expires
	
	echo "'oauth_token' => '{$access ['oauth_token']}',\n'oauth_token_secret' => '{$access ['oauth_token_secret']}', \n" ;
	
	//- Save the Access tokens to disk
	$fname =realpath(dirname(__FILE__)) . '/oauth.txt' ;
	file_put_contents ($fname, serialize ($access)) ;
	
} catch (OAuthException $e) {
	echo "OAuth\n", 'Caught exception: ',  $e->getMessage (), "\n" ;
	exit ;
} catch (Exception $e) {
	echo "OAuth/Authorize\n", 'Caught exception: ',  $e->getMessage (), "\n" ;
	exit ;
}

//- Done
exit ;

?>