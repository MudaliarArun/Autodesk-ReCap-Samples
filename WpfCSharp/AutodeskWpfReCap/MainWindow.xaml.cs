// (C) Copyright 2014 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

//- Written by Cyrille Fauvel, Autodesk Developer Network (ADN)
//- http://www.autodesk.com/joinadn
//- January 20th, 2014
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Resources;
using System.Resources;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Autodesk.ADN.Toolkit.Wpf.RestLogger;
using Autodesk.ADN.Toolkit.Wpf.Viewer;
using Autodesk.ADN.Toolkit.ReCap;

/*
 http://codereview.stackexchange.com/questions/20820/use-and-understanding-of-async-await-in-net-4-5

*/
namespace Autodesk.ADN.WpfReCap {

	public class ReCapPhotosceneProject {
		public string Name { get; set; }
		public string Type { get; set; }
		public string Image { get; set; }
	}

	public partial class MainWindow : Window {
		protected AdskReCap _recap =null ;
		protected RestLoggerWnd _logger =null ;
		
		public MainWindow () {
			InitializeComponent () ;
			PhotoScenes.View =PhotoScenes.FindResource ("recapView") as ViewBase ;

			serverLabel.Inlines.Clear () ;
			serverLabel.Inlines.Add (UserSettings.ReCapAPIURL) ;
			serverLabel.NavigateUri =new Uri (UserSettings.ReCapAPIURL.Replace ("/API/", "/api-docs/")) ;

			var values =Enum.GetValues (typeof (AdskReCap.MeshQuality)) ;
			foreach ( var value in values )
				outputQuality.Items.Add (value.ToString ()) ;
			outputQuality.SelectedItem =AdskReCap.MeshQuality.DRAFT.ToString () ;
			values =Enum.GetValues (typeof (AdskReCap.Format)) ;
			foreach ( var value in values )
				outputFormat.Items.Add (value.ToString ().TrimStart (new char [] { '_' })) ;
			outputFormat.SelectedItem =AdskReCap.Format.OBJ.ToString () ;
		}

		private void Window_Loaded (object sender, RoutedEventArgs e) {
			LoggerInit () ;
			Login () ;
		}

		public async void Login () {
			bool isNetworkAvailable =System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable () ;
			if ( !isNetworkAvailable ) {
				LogError ("Network Error: Check your network connection and try again...") ;
				return ;
			}

			// This isn't really secure as the tokens will be stored in the application settings
			// but not protected - This code is there to help testing the sample, but you should consider
			// securing the Access Tokens in a better way, or force login each time the application starts.
			LogInfo ("Authentification", LogIndent.PostIndent) ;
			if (   Properties.Settings.Default.oauth_token.Length == 0
				|| Properties.Settings.Default.oauth_token_secret.Length == 0
				|| Properties.Settings.Default.oauth_session_handle.Length == 0
				|| !await oAuth.AccessToken (true, null)
			) {
				oAuth wnd =new oAuth () ;
				wnd.ShowDialog () ;
			}
			ReCapLoginIcon.Source =(Properties.Settings.Default.oauth_token_secret.Length == 0 ?
				  new BitmapImage (new Uri (@"Images\Login.png", UriKind.Relative))
				: new BitmapImage (new Uri (@"Images\Logout.png", UriKind.Relative))
			) ;
			LogInfo ("", LogIndent.PostUnindent) ;
	
			await ConnectWithReCapServer () ;
			await ListPhotoScenes () ;
		}

		public async void Logout () {
			bool bRet =await oAuth.InvalidateToken () ;
			_recap =null ;
			ReCapLoginIcon.Source =(Properties.Settings.Default.oauth_token_secret.Length == 0 ?
				  new BitmapImage (new Uri (@"Images\Login.png", UriKind.Relative))
				: new BitmapImage (new Uri (@"Images\Logout.png", UriKind.Relative))
			) ;
		}

		#region Window events
		// UI - Menu Commands
		private void LoginMenu_Click (object sender, RoutedEventArgs e) {
			if (   Properties.Settings.Default.oauth_token.Length == 0
				|| Properties.Settings.Default.oauth_token_secret.Length == 0
				|| Properties.Settings.Default.oauth_session_handle.Length == 0
			)
				Login () ;
			else
				Logout () ;
		}

		private void LoggerMenu_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			LoggerInit () ;
		}

		private async void CreatePhotoscene_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			AdskReCap.Format format =AdskReCap.Format._3DP ;
			try {
				format =(AdskReCap.Format)Enum.Parse (typeof (AdskReCap.Format), (string)outputFormat.SelectedItem, true) ;
			} catch {
				format =(AdskReCap.Format)Enum.Parse (typeof (AdskReCap.Format), "_" + (string)outputFormat.SelectedItem, true) ;
			}
			string photosceneid =await CreateReCapPhotoscene (
				format,
				(AdskReCap.MeshQuality)Enum.Parse (typeof (AdskReCap.MeshQuality), (string)outputQuality.SelectedItem, true)
			) ;
			if ( photosceneid == "" )
				return ;
			
			if ( PhotoScenes.ItemsSource != null ) {
				ObservableCollection<ReCapPhotosceneProject> items =new ObservableCollection<ReCapPhotosceneProject> ((IEnumerable<ReCapPhotosceneProject>)PhotoScenes.ItemsSource) ;
				items.Add (new ReCapPhotosceneProject () {
					Name =photosceneid,
					Type ="CREATED",
					Image =@"Images\ReCap.jpg"
				}) ;
				PhotoScenes.ItemsSource =items ;
				PhotoScenes.Items.Refresh () ;
			}
		}

		private void PhotoScenesRefresh_Click (object sender, RoutedEventArgs e) {
			PhotoScenes_Refresh (sender, e) ;
		}

		// UI - Commands
		private void recapAPI_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
			e.Handled =true ;
			System.Diagnostics.Process.Start (new System.Diagnostics.ProcessStartInfo (e.Uri.AbsoluteUri)) ;
		}

		private void recap360_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
			e.Handled =true ;
			System.Diagnostics.Process.Start (new System.Diagnostics.ProcessStartInfo (e.Uri.AbsoluteUri)) ;
		}

		private async void PhotoScenes_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			e.Handled =true ;
			propertyGrid.SelectedObject =null ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneProject item =PhotoScenes.SelectedItem as ReCapPhotosceneProject ;
			if ( await PhotosceneProperties (item.Name) )
				propertyGrid.SelectedObject =new AdskReCapPhotoscene (_recap.xml ()) ;
		}

		#endregion

		#region PhotoScenes Context Menu events
		private async void PhotoScenes_TestConnection (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			await TestConnection () ;
		}

		private async void PhotoScenes_Refresh (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			await ListPhotoScenes () ;
		}

		private async void PhotoScenes_Properties (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneProject item =PhotoScenes.SelectedItem as ReCapPhotosceneProject ;
			if ( await PhotosceneProperties (item.Name) ) {
				PropertiesWnd wnd =new PropertiesWnd (new AdskReCapPhotoscene (_recap.xml ())) ;
				wnd.Owner =this ;
				wnd.Show () ;
			}
		}

		private void PhotoScenes_UploadPhotos (object sender, RoutedEventArgs e) {
			e.Handled = true;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return;
			ReCapPhotosceneProject item = PhotoScenes.SelectedItem as ReCapPhotosceneProject;
			ShotsWindow wnd = new ShotsWindow (item.Name);
			wnd.ShowDialog ();
		}

		private async void PhotoScenes_ProcessPhotoscene (object sender, RoutedEventArgs e) {
			e.Handled = true;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneProject item =PhotoScenes.SelectedItem as ReCapPhotosceneProject ;
			if ( await ProcessPhotoscene (item.Name) ) {
				JobProgress jobWnd =new JobProgress (item.Name) ;
				jobWnd._callback =new ProcessPhotosceneCompletedDelegate (this.ProcessPhotosceneCompleted) ;
				jobWnd.Show () ;
			}
		}

		public void ProcessPhotosceneCompleted (string photosceneid, string status) {
			foreach ( ReCapPhotosceneProject item in PhotoScenes.Items ) {
				if ( item.Name == photosceneid ) {
					item.Type =status ;
					PhotoScenes.Items.Refresh () ;
					break ;
				}
			}
		}

		private async void PhotoScenes_DownloadResult (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneProject item =PhotoScenes.SelectedItem as ReCapPhotosceneProject ;
			string link =await GetPhotosceneResult (item.Name) ;
			if ( link != "" ) {
				DownloadFileWnd wnd =new DownloadFileWnd (
					link,
					System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory)
						+ item.Name + System.IO.Path.GetExtension (link)
				) ;
				if ( e == null ) // Preview
					wnd._callback =new DownloadFileCompletedDelegate (this.DownloadResultForPreviewCompleted) ;
				else
					wnd._callback =new DownloadFileCompletedDelegate (this.DownloadResultCompleted) ;
				wnd.Show () ;
			}
		}

		public void DownloadResultCompleted (string photosceneid) {
			foreach ( ReCapPhotosceneProject item in PhotoScenes.Items ) {
				if ( item.Name == photosceneid ) {
					item.Image =photosceneid + ".zip:icon.png" ;
					PhotoScenes.Items.Refresh () ;
					break ;
				}
			}
		}

		public void DownloadResultForPreviewCompleted (string photosceneid) {
			DownloadResultCompleted (photosceneid) ;
			PhotoScenes_Preview (null, null) ;
		}

		// If not using .NET 4.5, http://dotnetzip.codeplex.com/
		private void PhotoScenes_Preview (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneProject item =PhotoScenes.SelectedItem as ReCapPhotosceneProject ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + item.Name + ".zip" ;
			if ( !File.Exists (location) ) {
				if ( e != null ) // Do not enter into an infinite loop
					PhotoScenes_DownloadResult (null, null) ;
				return ;
			}

			ViewerWindow wnd =new ViewerWindow () ;
			wnd.Owner =this ;
			wnd.Show () ;
			wnd.LoadModel (location) ;
		}

		private async void PhotoScenes_DeletePhotoscene (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			ObservableCollection<ReCapPhotosceneProject> items =new ObservableCollection<ReCapPhotosceneProject> ((IEnumerable<ReCapPhotosceneProject>)PhotoScenes.ItemsSource) ;
			foreach ( ReCapPhotosceneProject item in PhotoScenes.SelectedItems ) {
				if ( await DeletePhotoscene (item.Name) )
					items.Remove (item) ;
			}
			PhotoScenes.ItemsSource =items ;
			PhotoScenes.Items.Refresh () ;
		}

		#endregion

		#region ReCap Calls
		protected async Task<bool> ConnectWithReCapServer () {
			if ( _recap != null )
				return (true) ;

			_recap =new AdskReCap (
				UserSettings.ReCapClientID,
				UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
				Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret
			) ;
			System.Diagnostics.Debug.WriteLine ("tokens: " + Properties.Settings.Default.oauth_token + " - " + Properties.Settings.Default.oauth_token_secret, "Debug") ;
			if ( !await TestConnection () )
				_recap =null ;
			return (_recap != null) ;
		}

		protected async Task<bool> TestConnection () {
			// Test connection
			LogInfo ("Test Connection", LogIndent.PostIndent) ;
			if ( !await _recap.ServerTime () ) {
				connectedLabel.Content ="Connection to ReCap Server failed!" ;
				connectedTimeLabel.Content ="" ;
				LogError ("ReCap Error: Connection to ReCap Server failed!", LogIndent.PostUnindent) ;
				return (false) ;
			}
				
			// XML sample
			//XmlDocument doc =_recap.xml () ;
			//XmlNode node =doc.SelectSingleNode ("/Response/date") ; // doc.DocumentElement.SelectSingleNode ("/Response/date/text()").Value
			//DateTime dt =DateTime.Parse (node.InnerText) ;

			// XML LINQ sample
			//XDocument doc =_recap.xmlLinq () ;
			//XElement node =doc.Element ("Response").Element ("date") ;
			//DateTime dt =(DateTime)node ;

			// JSON sample (would require to use true in the call of _recap.ServerTime (true))
			//JObject doc =_recap.json () ;
			//JToken node =doc2 ["Response"] ["date"] ;
			//DateTime dt =(DateTime)node2 ;

			// ReCap Reponse dynamic object sample
			dynamic response =_recap.response () ;
			DateTime dt =DateTime.Parse (response.date) ;

			connectedLabel.Content =dt.ToLongDateString () ;
			connectedTimeLabel.Content =dt.ToLongTimeString () ;
			LogInfo ("ReCap Server date: " + dt.ToLongDateString () + " " + dt.ToLongTimeString (), LogIndent.PostUnindent) ;
			return (true) ;
		}

		private async Task ListPhotoScenes () {
			if ( !await ConnectWithReCapServer () )
				return ;

			// List Photoscene on the server
			LogInfo ("List Photoscenes", LogIndent.PostIndent) ;
			PhotoScenes.ItemsSource =null ;
			PhotoScenes.Items.Refresh () ;
			if ( !await _recap.SceneList ("userID", UserSettings.ReCapUserID) ) {
				LogError ("ListPhotoScenes failed", LogIndent.PostUnindent) ;
				return ;
			}

			// XML sample
			//ObservableCollection<ReCapPhotosceneProject> items =new ObservableCollection<ReCapPhotosceneProject> () ;
			//XmlDocument doc =_recap.xml () ;
			//XmlNodeList nodes =doc.SelectNodes ("/Response/Photoscenes/Photoscene") ;
			//string logText ="Photoscenes List:" ;
			//foreach ( XmlNode fnode in nodes ) {
			//	XmlNode p0 =fnode.SelectSingleNode ("deleted") ;
			//	if ( p0 != null && p0.InnerText == "true" )
			//		continue ;
			//	XmlNode p1 =fnode.SelectSingleNode ("photosceneid") ;
			//	string photosceneid =p1.InnerText ;
			//	XmlNode p2 =fnode.SelectSingleNode ("status") ;
			//	logText +=string.Format ("\n\t{0} [{1}]", p1.InnerText, p2.InnerText) ;
			//	// If we have the result downloaded, displays the resulting icon instead of the generic image
			//	items.Add (new ReCapPhotosceneProject () {
			//		Name =photosceneid,
			//		Type =p2.InnerText,
			//		Image =(File.Exists (photosceneid + ".zip") ? photosceneid + ".zip:icon.png" : @"Images\ReCap.jpg")
			//	}) ;
			//}

			// ReCap Reponse dynamic object sample (the beauty here is that this is the same code if you're using json vs xml)
			ObservableCollection<ReCapPhotosceneProject> items =new ObservableCollection<ReCapPhotosceneProject> () ;
			dynamic response =_recap.response () ;
			dynamic nodes =response.Photoscenes ; // if json, do doc.Photoscenes.Photoscene
			string logText ="Photoscenes List:" ;
			foreach ( KeyValuePair<string, object> pair in nodes.Dictionary ) {
				dynamic fnode =pair.Value ;
				try {
					if ( fnode.deleted == "true" ) // deleted might not be present in the response unless it is true
						continue;
				} catch { }
				logText +=string.Format ("\n\t{0} [{1}]", fnode.photosceneid, fnode.status) ;
				// If we have the result downloaded, displays the resulting icon instead of the generic image
				items.Add (new ReCapPhotosceneProject () {
					Name =fnode.photosceneid,
					Type =fnode.status,
					Image =(File.Exists (fnode.photosceneid + ".zip") ? fnode.photosceneid + ".zip:icon.png" : @"Images\ReCap.jpg")
				}) ;
			}

			PhotoScenes.ItemsSource =items ;
			PhotoScenes.Items.Refresh () ;
			LogInfo (logText, LogIndent.PostUnindent) ;
		}

		protected async Task<string> CreateReCapPhotoscene (AdskReCap.Format format, AdskReCap.MeshQuality quality) {
			if ( !await ConnectWithReCapServer () )
				return ("") ;

			//- Create Photoscene
			LogInfo (string.Format ("Create Photoscene {0} / {1}", format.ToString ().TrimStart (new char [] { '_' }), quality.ToString ()), LogIndent.PostIndent) ;
			Dictionary<string, string> options =new Dictionary<string, string> () {
				{ "callback", "email://" + UserSettings.Email }
			} ;
			if ( !await _recap.CreatePhotoscene (/*AdskReCap.Format.OBJ*/format, /*AdskReCap.MeshQuality.DRAFT*/quality, options) ) {
				LogError ("CreatePhotoscene failed - Failed to create a new Photoscene", LogIndent.PostUnindent) ;
				return ("") ;
			}
			dynamic response =_recap.response () ;
			LogInfo (string.Format ("CreatePhotoscene succeeded - PhotoSceneID = {0}", response.Photoscene.photosceneid), LogIndent.PostUnindent) ;
			return (response.Photoscene.photosceneid) ;
		}

		protected async Task<bool> PhotosceneProperties (string photosceneid) {
			if ( photosceneid == "" || !await ConnectWithReCapServer () )
				return (false) ;

			// Photoscene Properties
			LogInfo ("Photoscene Properties", LogIndent.PostIndent) ;
			bool bRet =await _recap.SceneProperties (photosceneid) ;
			Log (bRet ? "Photoscene Properties returned" : "PhotosceneProperties failed", LogIndent.PostUnindent, bRet ? "Info" : "Error") ;
			return (bRet) ;
		}

		protected async Task<bool> ProcessPhotoscene (string photosceneid) {
			if ( photosceneid == "" || !await ConnectWithReCapServer () )
				return (false) ;

			// Launch Photoscene
			LogInfo ("Launch Photoscene", LogIndent.PostIndent) ;
			bool bRet =await _recap.ProcessScene (photosceneid) ;
			Log (bRet ? "Photoscene processing request sent" : "ProcessPhotoscene failed", LogIndent.PostUnindent, bRet ? "Info" : "Error") ;
			return (bRet) ;
		}

		protected async Task<string> GetPhotosceneResult (string photosceneid) {
			if ( photosceneid == "" || !await ConnectWithReCapServer () )
				return ("") ;

			// Get Photoscene result (mesh)
			LogInfo ("Getting the Photoscene result (mesh)", LogIndent.PostIndent) ;
			if ( !await _recap.GetPointCloudArchive (photosceneid, AdskReCap.Format.OBJ) ) {
				LogError ("GetPointCloudArchive failed", LogIndent.PostUnindent) ;
				return ("") ;
			}
			dynamic response =_recap.response () ;
			LogInfo (string.Format ("CreatePhotoscene succeeded - PhotoSceneID = {0}", response.Photoscene.scenelink), LogIndent.PostUnindent) ;
			return (response.Photoscene.scenelink) ;
		}

		protected async Task<bool> DeletePhotoscene (string photosceneid) {
			if ( photosceneid == "" || !await ConnectWithReCapServer () )
				return (false) ;

			// Deleting the given Photoscene
			LogInfo ("Deleting a Photoscene", LogIndent.PostIndent) ;
			//bool ret =await _recap.DeleteScene (photosceneid) ;
			bool ret =_recap.DeleteSceneTempFix (photosceneid) ;
			if ( !ret ) {
				LogError ("DeletePhotoscene failed", LogIndent.PostUnindent) ;
				return (false) ;
			}
			LogInfo ("DeletePhotoscene call succeeded") ;
			dynamic response =_recap.response () ;
			try {
				string nb =response.Photoscene.deleted ;
				LogInfo (string.Format ("Photoscene {0} deleted - {1} resources deleted", photosceneid, nb), LogIndent.PostUnindent) ;
			} catch {
				LogInfo ("Failed deleting the Photoscene and resources", LogIndent.PostUnindent) ;
				return (false) ;
			}
			return (true) ;
		}

		#endregion

		#region Logger Window
		private void LoggerInit () {
			if ( _logger != null ) {
				_logger.Visibility =Visibility.Visible ;
				RestLoggerIcon.Source =new BitmapImage (new Uri (@"Images\RestLogger.png", UriKind.Relative)) ;
				return ;
			}
			_logger =new RestLoggerWnd () ;
			_logger.Owner =this ;
			_logger.Top =this.Top ;
			_logger.Left =this.Left + this.Width ;
			_logger.Closing +=_logger_Closing ;
			_logger.Updated +=_logger_Updated ;
			_logger.Show () ;
		}

		private void _logger_Closing (object sender, CancelEventArgs e) {
			e.Cancel =true ;
			_logger.Visibility =Visibility.Hidden ;
		}

		private void _logger_Updated () {
			if ( _logger.Visibility == Visibility.Hidden )
				RestLoggerIcon.Source =new BitmapImage (new Uri (@"Images\RestLoggerUpd.png", UriKind.Relative)) ;
		}

		protected enum LogIndent { None =0, PreIndent =1, PreUnindent =-1, PostIndent =2, PostUnindent =-2 } ;
		private static void LogInfo (string msg, LogIndent indent =LogIndent.None) {
			Log (msg, indent, "Info") ;
		}

		private static void LogError (string msg, LogIndent indent =LogIndent.None) {
			Log (msg, indent, "Error") ;
		}

		private static void Log (string msg, LogIndent indent =LogIndent.None, string category ="Info") {
			if ( Math.Abs ((int)indent) == 1 )
				Trace.IndentLevel +=(int)indent ;
			if ( msg != "" )
				Trace.WriteLine ("WpfRecap: " + msg, category) ;
			if ( Math.Abs ((int)indent) == 2 )
				Trace.IndentLevel +=((int)indent) / 2 ;
		}

		#endregion

	}

}