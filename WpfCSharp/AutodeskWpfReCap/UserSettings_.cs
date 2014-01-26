// (C) Copyright 2014 by Autodesk, Inc.
//
// The information contained herein is confidential, proprietary
// to Autodesk, Inc., and considered a trade secret as defined
// in section 499C of the penal code of the State of California.
// Use of this information by anyone other than authorized
// employees of Autodesk, Inc. is granted only under a written
// non-disclosure agreement, expressly prescribing the scope
// and manner of such use.

//- Written by Cyrille Fauvel, Autodesk Developer Network (ADN)
//- http://www.autodesk.com/joinadn
//- January 20th, 2014
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutodeskWpfReCap {

	public class UserSettings {

		// Hard coded consumer and secret keys and base URL.
		// In real world Apps, these values need to secured.
		// One approach is to encrypt and/or obfuscate these values
		public const string CONSUMER_KEY ="your consumer key" ;
		public const string CONSUMER_SECRET ="your consumer secret key" ;
		public const string O2_HOST ="https://accounts.autodesk.com/" ; // Autodesk pruction accounts server
		//public const string O2_HOST ="https://accounts-staging.autodesk.com/" ; // Autodesk staging accounts server

		// ReCap: Fill in these macros with the correct information (only the 2 first are important)
		public const string ReCapAPIURL ="http://rc-api-adn.autodesk.com/3.1/API/" ;
		public const string ReCapClientID ="your ReCap client ID" ;
		public const string ReCapKey ="your ReCap client key" ; // not used anymore
		public const string ReCapUserID ="your ReCap user ID" ; // Needed only for using the ReCapSceneList, otherwise bail

		public const string Email ="your email address" ; // used for notification

		// Do not edit
		public const string O2_REQUESTTOKEN ="OAuth/RequestToken" ;
		public const string O2_ACCESSTOKEN ="OAuth/AccessToken" ;
		public const string O2_AUTHORIZE ="OAuth/Authorize" ;
		public const string O2_INVALIDATETOKEN ="OAuth/InvalidateToken" ;
		public const string O2_ALLOW =O2_HOST + "OAuth/Allow" ;

	}

}
