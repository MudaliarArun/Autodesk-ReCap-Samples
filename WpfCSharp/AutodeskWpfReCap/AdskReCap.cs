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
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace AutodeskWpfReCap {

	public class AdskReCap {
		protected string _clientID ;
		//protected Dictionary<string, string> _tokens =null ;
		private RestClient _Client =null ;
		public IRestResponse _lastResponse =null ;

		public AdskReCap (string clientID, Dictionary<string, string> tokens) {
			_clientID =clientID ;
			// @"oauth_consumer_key" @"oauth_consumer_secret" @"oauth_token" @"oauth_token_secret"
			//_tokens =tokens ;
			_Client =new RestClient (UserSettings.ReCapAPIURL) ;
			_Client.Authenticator =OAuth1Authenticator.ForProtectedResource (
				tokens ["oauth_consumer_key"], tokens ["oauth_consumer_secret"],
				tokens ["oauth_token"], tokens ["oauth_token_secret"]
			) ;
		}

		public AdskReCap (string clientID, string consumerKey, string consumerSecret, string oauth, string oauthSecret) {
			_clientID =clientID ;
			// @"oauth_consumer_key" @"oauth_consumer_secret" @"oauth_token" @"oauth_token_secret"
			//_tokens =Dictionary<string, string> () ;
			//_tokens.Add ("oauth_consumer_key", consumerKey) ;
			//_tokens.Add ("oauth_consumer_secret", consumerSecret) ;
			//_tokens.Add ("oauth_token", oauth) ;
			//_tokens.Add ("oauth_token_secret", oauthSecret) ;
			_Client =new RestClient (UserSettings.ReCapAPIURL) ;
			_Client.Authenticator = OAuth1Authenticator.ForProtectedResource (
				consumerKey, consumerSecret, oauth, oauthSecret
			);
		}

		public bool ServerTime () {
			var request =new RestRequest ("service/date", Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog("service/date response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool Version () {
			var request =new RestRequest ("version", Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("version response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool SetNotificationMessage (string emailType, string msg) {
			var request =new RestRequest ("notification/template", Method.POST) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("emailType", emailType) ;
			request.AddParameter ("emailTxt", msg) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("notification/template response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool CreateSimplePhotoscene (string format, string meshQuality) {
			var request =new RestRequest ("photoscene", Method.POST) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("format", format) ;
			request.AddParameter ("meshquality", meshQuality) ;
			request.AddParameter ("scenename", string.Format ("MyPhotoScene{0}", DateTime.Now.Ticks.ToString ())) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("photoscene response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool CreatePhotoscene (string format, string meshQuality, Dictionary<string, string> options) {
			var request =new RestRequest ("photoscene", Method.POST) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("format", format) ;
			request.AddParameter ("meshquality", meshQuality) ;
			request.AddParameter ("scenename", string.Format ("MyPhotoScene{0}", DateTime.Now.Ticks.ToString ())) ;
			foreach ( KeyValuePair<string, string> entry in options )
				request.AddParameter (entry.Key, entry.Value) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("photoscene response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool SceneList (string attributeName, string attributeValue) {
			var request =new RestRequest ("photoscene/properties", Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("attributeName", attributeName) ;
			request.AddParameter ("attributeValue", attributeValue) ;
			_lastResponse =_Client.Execute (request) ;
			//NSLog ("photoscene/properties response: {0}", _lastResponse) ; // Can be very big
			return (isOk ()) ;
		}
		
		public bool SceneProperties (string photosceneid) {
			var request =new RestRequest (string.Format ("photoscene/{0}/properties", photosceneid), Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("photoscene/.../properties response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool UploadFiles (string photosceneid, Dictionary<string, string> files) {
			// ReCap returns the following if no file uploaded (or referenced), setup an error instead
			//<Response>
			//        <Usage>0.81617307662964</Usage>
			//        <Resource>/file</Resource>
			//        <photosceneid>  your scene ID  </photosceneid>
			//        <Files>
			//
			//        </Files>
			//</Response>
			if ( files == null || files.Count == 0 ) {
				_lastResponse =null ;
				return (false) ;
			}
			var request =new RestRequest (string.Format ("file", photosceneid), Method.POST) ;
			request.Timeout =1 * 60 * 60 * 1000 ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
			request.AddParameter ("type", "image") ;
			foreach ( KeyValuePair<string, string> entry in files )
				request.AddFile (entry.Key, entry.Value) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("file response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public RestRequestAsyncHandle UploadFilesAsync (string photosceneid, Dictionary<string, string> files, Action<IRestResponse, RestRequestAsyncHandle> callback) {
			// ReCap returns the following if no file uploaded (or referenced), setup an error instead
			//<Response>
			//        <Usage>0.81617307662964</Usage>
			//        <Resource>/file</Resource>
			//        <photosceneid>  your scene ID  </photosceneid>
			//        <Files>
			//
			//        </Files>
			//</Response>
			if ( files == null || files.Count == 0 ) {
				_lastResponse =null ;
				return (null) ;
			}
			var request =new RestRequest (string.Format ("file", photosceneid), Method.POST) ;
			request.Timeout =1 * 60 * 60 * 1000 ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
			request.AddParameter ("type", "image") ;
			foreach ( KeyValuePair<string, string> entry in files )
				request.AddFile (entry.Key, entry.Value) ;
			// Inline example
			//var asyncHandle =_Client.ExecuteAsync (request, response => { if ( response.StatusCode == HttpStatusCode.OK ) {} else {} }) ;
			var asyncHandle =_Client.ExecuteAsync (request, callback) ;
			Console.WriteLine ("async file call started") ;
			return (asyncHandle) ;
		}

		public bool ProcessScene (string photosceneid) {
			var request =new RestRequest (string.Format ("photoscene/{0}", photosceneid), Method.POST) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
			request.AddParameter ("forceReprocess", "1") ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("(post) photoscene/... response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool SceneProgress (string photosceneid) {
			var request =new RestRequest (string.Format ("photoscene/{0}/progress", photosceneid), Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("photoscene/.../progress response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool GetPointCloudArchive (string photosceneid, string format) {
			var request =new RestRequest (string.Format ("photoscene/{0}", photosceneid), Method.GET) ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
			request.AddParameter ("format", format) ;
			_lastResponse =_Client.Execute (request) ;
			NSLog ("photoscene/... response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public bool DeleteScene (string photosceneid) {
			var request =new RestRequest (string.Format ("photoscene/{0}", photosceneid), Method.DELETE) ;
			request.AlwaysMultipartFormData =true ;
			request.AddParameter ("clientID", UserSettings.ReCapClientID) ;
			request.AddParameter ("timestamp", DateTime.Now.Ticks.ToString ()) ;
			request.AddParameter ("photosceneid", photosceneid) ;
request.AddParameter ("debug", "1");
			//_lastResponse =_Client.Execute (request) ;
			var method =Enum.GetName (typeof (Method), Method.DELETE) ;
			_lastResponse =_Client.ExecuteAsPost (request, method) ;
			NSLog ("(delete) photoscene/... response: {0}", _lastResponse) ;
			return (isOk ()) ;
		}

		public string ErrorMessage (bool display) {
			if ( _lastResponse == null )
				return ("") ;
			string errmsg ="" ;
			if ( _lastResponse.ErrorMessage != null && _lastResponse.ErrorMessage != "" ) {
				errmsg =_lastResponse.ErrorMessage ;
			} else {
				Console.WriteLine (ToString ()) ;
				XmlDocument xmlDoc =xml () ;
				if ( xmlDoc != null ) {
					XmlNode errorCode =xmlDoc.SelectSingleNode ("/Response/Error/code") ;
					XmlNode results =xmlDoc.SelectSingleNode ("/Response/Error/msg") ;
					errmsg =string.Format ("{0} (# {1})", results.InnerText, errorCode.InnerText) ;
				} else {
					errmsg ="Not an XML response." ;
				}
			}
			if ( display ) 
				MessageBox.Show (errmsg, "ReCap Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) ;
			return (errmsg) ;
		}

		public XmlDocument xml () {
			if ( _lastResponse == null || _lastResponse.ErrorMessage != null )
				return (null) ;
			XmlDocument theDocument =new XmlDocument () ;
			theDocument.LoadXml (ToString ()) ;
			return (theDocument) ;
		}

		public bool isOk () {
			if ( _lastResponse == null || _lastResponse.StatusCode != HttpStatusCode.OK )
				return (false) ;
			string st =this.ToString () ;
			return (st.IndexOf ("<error>") == -1 && st.IndexOf ("<Error>") == -1) ;
		}

		override public string ToString () {
			if ( _lastResponse == null )
				return ("<error>") ;
			return (_lastResponse.Content) ;
		}

		public static void NSLog (string format, IRestResponse message) {
			Console.WriteLine (string.Format (format, message.Content)) ;
		}

	}

}

//string jsonToSend =JsonHelper.ToJson (json) ;
//request.AddParameter ("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody) ;
//request.RequestFormat =DataFormat.Json ;
