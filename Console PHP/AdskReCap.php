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
 
 */
require 'vendor/autoload.php' ;
use Guzzle\Http\Client ;
use Guzzle\Plugin\Oauth\OauthPlugin ;

class AdskReCap {
	private $_clientID ;
	private $_Client =null ;
	public $_lastResponse =null ;
	public $_outputlog =false ;

	public function AdskReCap ($clientID, $tokens, $outputlog =false) {
		$this->_clientID =$clientID ;
		$this->_outputlog =$outputlog ;
		// @"oauth_consumer_key" @"oauth_consumer_secret" @"oauth_token" @"oauth_token_secret"
		$this->_tokens =$tokens ;
		$this->_Client =new Client (ReCapAPIURL) ;

		//- http://guzzlephp.org/guide/plugins.html
		//- The Guzzle oAuth plug-in will put the oAuth signature in the HTML header automatically
		$oauthClient =new OauthPlugin (array (
			'consumer_key' => CONSUMER_KEY,
			'consumer_secret' => CONSUMER_SECRET,
			'token' => $tokens ['oauth_token'], //- access_token
			'token_secret' => $tokens ['oauth_token_secret'], //- access_token_secret
		)) ;
		$this->_Client->addSubscriber ($oauthClient) ;
	}

	/*public function AdskReCap ($clientID, $consumerKey, $consumerSecret, $oauth, $oauthSecret) {
		$this->_clientID =$clientID ;
		// @"oauth_consumer_key" @"oauth_consumer_secret" @"oauth_token" @"oauth_token_secret"
		$this->_Client =new Client (ReCapAPIURL) ;
		//- http://guzzlephp.org/guide/plugins.html
		//- The Guzzle oAuth plug-in will put the oAuth signature in the HTML header automatically
		$oauthClient =new OauthPlugin (array (
			'consumer_key' => $consumerKey,
			'consumer_secret' => $consumerSecret,
			'token' => $oauth, //- access_token
			'token_secret' => $oauthSecret, //- access_token_secret
		)) ;
		$this->_Client->addSubscriber ($oauthClient) ;
	}*/

	public function ServerTime () {
		//- Requesting the ReCap service/date to start and check our connection/authentication
		$request =$this->_Client->get ('service/date') ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array ( 'clientID' => $this->_clientID, 'timestamp' => time (), )) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("service/date raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function Version () {
		//- Requesting the ReCap version
		$request =$this->_Client->get ('version') ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array ( 'clientID' => $this->_clientID, 'timestamp' => time (), )) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("version raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function SetNotificationMessage ($emailType, $emailTxt) {
		$request =$this->_Client->post ("notification/template") ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array (
			'emailType' => $emailType,
			'emailTxt' => $emailTxt,
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("notification/template raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function CreateSimplePhotoscene ($format, $meshQuality) {
		$request =$this->_Client->post ("photoscene") ;
		$request->addPostFields (array (
			'scenename' => 'MyPhotoScene' . time (),
			'meshquality' => $meshQuality,
			'format' => $format,
			'callback' => Email,
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("photoscene raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function CreatePhotoscene ($format, $meshQuality, $options) {
		$request =$this->_Client->post ("photoscene") ;
		$params =array (
			'scenename' => 'MyPhotoScene' . time (),
			'meshquality' => $meshQuality,
			'format' => $format,
			'callback' => Email,
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		) ;
		//foreach ( $options as $key => $value ) {}
		$params =array_merge ($params, $options) ;
		$request->addPostFields ($params) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("photoscene raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function SceneList ($attributeName, $attributeValue) {
		$request =$this->_Client->get ("photoscene/properties") ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array (
			'clientID' => $this->_clientID, 'timestamp' => time (),
			'attributeName' => $attributeName,
			'attributeValue' => $attributeValue,
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("photoscene/properties raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function SceneProperties ($photosceneid) {
		$request =$this->_Client->get ("photoscene/{$photosceneid}/properties") ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array ( 'clientID' => $this->_clientID, 'timestamp' => time (), )) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("photoscene/.../properties raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function UploadFiles ($photosceneid, $files) {
		// ReCap returns the following if no file uploaded (or referenced), setup an error instead
		//<Response>
		//        <Usage>0.81617307662964</Usage>
		//        <Resource>/file</Resource>
		//        <photosceneid>  your scene ID  </photosceneid>
		//        <Files>
		//
		//        </Files>
		//</Response>
		if ( $files == null || count ($files) == 0 ) {
			$this->_lastResponse =null ;
			return (false) ;
		}
		$request =$this->_Client->post ("file") ;
		$params =array (
			'photosceneid' => $photosceneid,
			'type' => 'image',
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		) ;
		$request->getQuery ()->clear () ;
		//$request->getQuery ()->merge ($params) ; // was needed before because of a bug
		$request->addPostFields ($params) ;
		$request->addPostFiles ($files) ; //- Local Files
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("file raw response: ", $this->_lastResponse) ;
		
		return ($this->isOk ()) ;
	}

	public function ProcessScene ($photosceneid) {
		$request =$this->_Client->post ("photoscene/{$photosceneid}") ;
		$request->addPostFields (array (
			'photosceneid' => $photosceneid,
			'forceReprocess' => "1",
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("(post) photoscene/... raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function SceneProgress ($photosceneid) {
		$request =$this->_Client->get ("photoscene/{$photosceneid}/progress") ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array (
			'photosceneid' => $photosceneid,
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("photoscene/.../progress raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function GetPointCloudArchive ($photosceneid, $format) {
		$request =$this->_Client->get ("photoscene/{$photosceneid}") ;
		$request->getQuery ()->clear () ; // Not needed as this is a new request object, but.as I am going to use merge(), it is safer
		$request->getQuery ()->merge (array (
			'photosceneid' => $photosceneid,
			'format' => $format,
			'clientID' => $this->_clientID,
			'timestamp' => time (),
		)) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("(get) photoscene/... raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}

	public function DeleteScene ($photosceneid) {
		//$this->_Client->setDefaultOption ('headers', array ('Content-Type' => 'multipart/form-data')) ;
		//curl_setopt($this->ch, CURLOPT_HTTPHEADER, array("Content-type: multipart/form-data"));
		$request =$this->_Client->delete ("photoscene/{$photosceneid}") ; //, null, null, array ( CURLOPT_HTTPHEADER, array("Content-type: multipart/form-data") )) ;
		$request->addPostFields (array ( 'clientID' => $this->_clientID, 'timestamp' => time (), )) ;
		$this->_lastResponse =$request->send () ;
		if ( $this->_outputlog == true )
			$this->NSLog ("(delete) photoscene/... raw response: ", $this->_lastResponse) ;
		return ($this->isOk ()) ;
	}
	
	/*public function signIt ($relativurl, &$data, $method) {
		// in our case we need to urldecode the URL in case the ID is encoded
		$relativurl =rtrim (urldecode ($relativurl), "/") ;
		$toSign ="/" . $relativurl ;
		$toSign .= "?";
		foreach ( $data as $name => $value ) {
			if ( $name != "signature" )
				$toSign .=$name . "=" . $value . "&" ;
		}
		$toSign =trim ($toSign, "&") ;
		$oauth =new OAuth (CONSUMER_KEY, CONSUMER_SECRET, OAUTH_SIG_METHOD_HMACSHA1) ;
		$oauth->setToken ($this->_tokens ['oauth_token'], $this->_tokens ['oauth_token_secret']) ;
		return ($oauth->getRequestHeader (strtoupper ($method), ReCapAPIURL . $toSign)) ;
	}

	public function DeleteScene ($photosceneid) {
		$url =ReCapAPIURL . "photoscene/$photosceneid" ;

		$cURL =curl_init () ;
		curl_setopt ($cURL, CURLOPT_USERAGENT, "ServerUserAgent") ;
		curl_setopt ($cURL, CURLOPT_CUSTOMREQUEST, "DELETE") ;
		$data =array ( 'clientID' => $this->_clientID, 'timestamp' => time (), ) ;
		$authorization =$this->signIt ("photoscene/$photosceneid", $data, 'delete') ;
		$co ="" ;
		$d ="" ;
		foreach ( $data as $key => $value ) {
			$d .=$co . urlencode ($key) . "=" . urlencode ($value) ;
			$co ='&' ;
		}
		curl_setopt ($cURL, CURLOPT_POSTFIELDS, $d) ;
		curl_setopt ($cURL, CURLOPT_URL, $url) ;
		curl_setopt ($cURL, CURLOPT_RETURNTRANSFER, TRUE) ;
		curl_setopt ($cURL, CURLOPT_HEADER, 0) ;
		curl_setopt ($cURL, CURLOPT_HTTPHEADER, array ("Authorization: $authorization")) ;
		curl_setopt ($cURL, CURLOPT_TIMEOUT, 100) ;
		curl_setopt ($cURL, CURLOPT_VERBOSE, TRUE) ;
		curl_setopt ($cURL, CURLOPT_PROXY, '127.0.0.1:8888') ;

		$res =curl_exec ($cURL) ;
		$info =curl_getinfo ($cURL) ;
		curl_close ($cURL) ;
		
		print_r ($info) ;
	}*/

	public function ErrorMessage ($display) {
		if ( $this->_lastResponse == null )
			return ("") ;
		$errmsg ="" ;
		$xmlDoc =xml () ;
		if ( $xmlDoc != null )
			$errmsg ="{$xml->Error->msg} (# {$xml->Error->code})" ;
		else
			$errmsg ="Not an XML response." ;
		if ( $display ) 
			echo "ReCap Error", $errmsg, "\n" ;
		return ($errmsg) ;
	}

	public function xml () {
		if ( empty ($this->_lastResponse) )
			return (null) ;
		return ($this->_lastResponse->xml ()) ;
	}

	public function isOk () {
		if ( empty ($this->_lastResponse) )
			return (false) ;
		$st =$this->ToString () ;
		return (stripos ($st, "<error>") === false) ;
	}

	public function ToString () {
		if ( $this->_lastResponse == null )
			return ("<error>") ;
		return ($this->_lastResponse->getBody ()) ;
	}

	public static function NSLog ($message, $response) {
		if ( $message == null )
			$message ="Response Body" ;
		echo "$message\n-------\n", $response->getBody (), "======\n\n" ;
	}

}

?>