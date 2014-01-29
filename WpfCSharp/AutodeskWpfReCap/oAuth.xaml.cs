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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Diagnostics;

using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace AutodeskWpfReCap {

	public partial class oAuth : Window {
		private static RestClient _Client =null ;

		public oAuth () {
			InitializeComponent () ;
		}

		public void startLogin () {
			if ( RequestToken () ) // Leg 1
                Authorize () ; // Leg 2
		}

		public static bool isOOB {
			get { return (false) ; } // Return false always in this example
		}

		//- First Leg: The first step of authentication is to request a token
		protected bool RequestToken () {
			if ( _Client == null )
				_Client =new RestClient (UserSettings.O2_HOST) ;
			
			Properties.Settings.Default.oauth_token = "";
			Properties.Settings.Default.oauth_token_secret ="" ;
			Properties.Settings.Default.oauth_session_handle ="" ;
			Properties.Settings.Default.Save () ;

			if ( isOOB )
				_Client.Authenticator =OAuth1Authenticator.ForRequestToken (UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET, "oob") ;
			else
				_Client.Authenticator =OAuth1Authenticator.ForRequestToken (UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET) ;
			// Build the HTTP request for a Request token and execute it against the OAuth provider
            var request =new RestRequest (UserSettings.O2_REQUESTTOKEN, Method.POST) ;
            var response =_Client.Execute (request) ;
			if ( response.StatusCode != HttpStatusCode.OK ) {
				System.Diagnostics.Trace.WriteLine ("Failure!\nHTTP request did not work!\nMaybe you are having a connection problem?") ;
				return (false) ;
			}
        
			// The HTTP request succeeded. Get the request token and associated parameters.
			var requestToken =HttpUtility.ParseQueryString (response.Content) ;
			if ( requestToken.Count < 2 ) {
				System.Diagnostics.Trace.WriteLine ("Failure!\nCould not get request token!\nMaybe the credentials are incorrect?") ;
				return (false) ;
			}
			Properties.Settings.Default.oauth_token =requestToken ["oauth_token"] ;
			Properties.Settings.Default.oauth_token_secret =requestToken ["oauth_token_secret"] ;
			Properties.Settings.Default.Save () ;
			return (true) ;
		}

		//- Second Leg: The second step is to authorize the user using the Autodesk login server
		protected void Authorize () {
			var request =new RestRequest (UserSettings.O2_AUTHORIZE) ;
			request.AddParameter ("oauth_token", Properties.Settings.Default.oauth_token) ;
			if ( isOOB ) {
				// In case of out-of-band authorization, let's show the authorization page which will provide the user with a PIN
				// in the default browser. Then here in our app request the user to type in that PIN.
				Uri authorizeUri =_Client.BuildUri (request) ;
				Process.Start (authorizeUri.ToString ()) ;
			} else {
				// Otherwise let's load the page in our web viewer so that
				// we can catch the URL that it gets redirected to
				request.AddParameter ("viewmode", "desktop") ;
				webView.Source =_Client.BuildUri (request) ;
			}
		}

		//- When a new URL is being shown in the browser then we can check the URL
		//- This is needed in case of in-band authorization which will redirect us to a given
		//- URL (O2_ALLOW) in case of success
		private void webView_LoadCompleted (object sender, System.Windows.Navigation.NavigationEventArgs e) {
			// In case of out-of-band login we do not need to check the callback URL
			// Instead we'll need the PIN that the webpage will provide for the user
			if ( isOOB )
				return ;
			// Let's check if we got redirected to the correct page
			if ( isAuthorizeCallBack () ) {
				System.Diagnostics.Trace.WriteLine ("Success!\nYou are authorized!\nYOu can now request your Access token.") ;
				AccessToken (false, null) ;
			} else {
				System.Diagnostics.Trace.WriteLine ("Failure!\nYou are not authorized!") ;
			}
		}

		//- Third leg: The third step is to authenticate using the request tokens
		//- Once you get the access token and access token secret you need to use those to make your further REST calls
		//- Same in case of refreshing the access tokens or invalidating the current session. To do that we need to pass
		//- in the acccess token and access token secret as the accessToken and tokenSecret parameter of the
		//- [AdskRESTful URLRequestForPath] function
		public static bool AccessToken (bool refresh, string PIN) {
			if ( _Client == null )
				_Client =new RestClient (UserSettings.O2_HOST) ;
			var request =new RestRequest (UserSettings.O2_ACCESSTOKEN, Method.POST) ;

			// If we already got access tokens and now just try to refresh
			// them then we need to provide the session handle
			if ( refresh ) {
				if ( Properties.Settings.Default.oauth_session_handle.Length == 0 )
					return (false) ;
				if ( isOOB )
					_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh (
						UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
						Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret,
						PIN, Properties.Settings.Default.oauth_session_handle
					);
				else
					_Client.Authenticator = OAuth1Authenticator.ForAccessTokenRefresh (
						UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
						Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret,
						Properties.Settings.Default.oauth_session_handle
					) ;
			} else {
				if ( Properties.Settings.Default.oauth_token.Length == 0 )
					return (false) ;
				if ( isOOB )
					// Use PIN to request access token for users account for an out of band request.
					_Client.Authenticator =OAuth1Authenticator.ForAccessToken (
						UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
						Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret,
						PIN
					) ;
				else
					_Client.Authenticator =OAuth1Authenticator.ForAccessToken (
						UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
						Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret
					) ;
			}

			Properties.Settings.Default.oauth_token ="" ;
			Properties.Settings.Default.oauth_token_secret ="" ;
			Properties.Settings.Default.oauth_session_handle ="" ;
			Properties.Settings.Default.Save () ;

			var response =_Client.Execute (request) ;
			if ( response.StatusCode != HttpStatusCode.OK ) {
				System.Diagnostics.Trace.WriteLine ("Failure!\nHTTP request did not work!\nMaybe you are having a connection problem?") ;
				return (false) ;
			}

			// The HTTP request succeeded. Get the request token and associated parameters.
			var accessToken =HttpUtility.ParseQueryString (response.Content) ;
			if ( accessToken.Count < 3 || accessToken ["oauth_session_handle"] == null ) {
				if ( refresh )
					System.Diagnostics.Trace.WriteLine ("Failure!\nCould not refresh token!") ;
				else
					System.Diagnostics.Trace.WriteLine ("Failure!\nCould not get access token!") ;
				return (false) ;
			}
			if ( refresh )
				System.Diagnostics.Trace.WriteLine ("Success!\nManaged to refresh token!") ;
			else
				System.Diagnostics.Trace.WriteLine ("Success!\nManaged to log in and get access token!") ;
			Properties.Settings.Default.oauth_token =accessToken ["oauth_token"] ;
			Properties.Settings.Default.oauth_token_secret =accessToken["oauth_token_secret"] ;
			Properties.Settings.Default.oauth_session_handle =accessToken ["oauth_session_handle"] ;
			Properties.Settings.Default.Save () ;
			return (true) ;
		}

		//- If we do not want to use the service anymore then
		//- the best thing is to log out, i.e. invalidate the tokens we got
		public static void InvalidateToken () {
			if ( _Client == null )
				_Client = new RestClient (UserSettings.O2_HOST);

			var request = new RestRequest (UserSettings.O2_INVALIDATETOKEN, Method.POST);
			_Client.Authenticator =OAuth1Authenticator.ForAccessTokenRefresh (
				UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
				Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret,
				Properties.Settings.Default.oauth_session_handle
			) ;
			var response =_Client.Execute (request) ;
			if ( response.StatusCode != HttpStatusCode.OK ) {
				System.Diagnostics.Trace.WriteLine ("Failure!\nHTTP request did not work!\nMaybe you are having a connection problem?") ;
				return ;
			}

			// If Invalidate was successful, we will not get back any data
			if ( response.Content.Length == 0 ) {
				Properties.Settings.Default.oauth_token ="" ;
				Properties.Settings.Default.oauth_token_secret ="" ;
				Properties.Settings.Default.oauth_session_handle ="" ;
				Properties.Settings.Default.Save () ;
				System.Diagnostics.Trace.WriteLine ("Success!\nManaged to log out!") ;
			} else {
				System.Diagnostics.Trace.WriteLine ("Failure!\nCould not log out!") ;
			}
		}

		//- Check if the URL is O2_ALLOW, which means that the user could log in successfully
		private bool isAuthorizeCallBack () {
			string fullUrlString =webView.Source.AbsoluteUri ;
			if ( fullUrlString.Length == 0 )
				return (false) ;
			string [] arr =fullUrlString.Split ('?') ;
			if ( arr == null || arr.Length != 2 )
				return (false) ;
			// If we were redirected to the O2_ALLOW URL then the user could log in successfully
			if ( arr [0] == UserSettings.O2_ALLOW )
				return (true) ;
			// If we got to this page then probably there is an issue
			if ( arr [0] == UserSettings.O2_AUTHORIZE ) {
				// If the page contains the word "oauth_problem" then there is clearly a problem
				//string content =webView stringByEvaluatingJavaScriptFromString:@"document.body.innerHTML"] ;
				//if ( content.IndexOf ("oauth_problem") > -1 )
				//	System.Diagnostics.Trace.WriteLine ("Failure!\nCould not log in!\nTry again!") ;
			}
			return (false) ;
		}

		private void Window_Loaded (object sender, RoutedEventArgs e) {
			startLogin () ;
		}

	}
}
