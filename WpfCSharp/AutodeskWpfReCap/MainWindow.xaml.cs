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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
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

using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace AutodeskWpfReCap {

	public class ReCapPhotoItem {

		public string Name { get; set; }
		public string Type { get; set; }
		public string Image { get; set; }

	}

	public class ReCapPhotosceneidItem {

		public string Name { get; set; }
		public string Type { get; set; }
		public string Image { get; set; }

	}

	enum DisplayMode { Wireframe, SmoothShade, WireframeOnShaded, Textured } ;

	public partial class MainWindow : Window {
		protected AdskReCap _recap =null ;
		private Point _lastPos ;
		private DisplayMode _currentDisplayMode =DisplayMode.Textured ;
		private ModelVisual3D _currentMesh =null ;
		private Material _currentMeshMatGroup =null ;
		private ModelVisual3D _currentWireframe =null ;
		private bool _wireframeDirty =true ;
		private Vector3D _viewportT =new Vector3D () ;
		private Quaternion _viewportR =new Quaternion () ;
		
		public MainWindow () {
			InitializeComponent () ;
			Thumbnails.View =Thumbnails.FindResource ("tileView") as ViewBase ;
			PhotoScenes.View =PhotoScenes.FindResource ("recapView") as ViewBase ;
		}

		#region Window events
		private void Window_Loaded (object sender, RoutedEventArgs e) {
			if (   Properties.Settings.Default.oauth_token.Length == 0
				|| Properties.Settings.Default.oauth_token_secret.Length == 0
				|| Properties.Settings.Default.oauth_session_handle.Length == 0
				|| !oAuth.AccessToken (true, null)
			) {
				oAuth wnd =new oAuth () ;
				wnd.ShowDialog () ;
			}

			ConnectWithReCap () ;
			label5.Content =UserSettings.ReCapAPIURL ;
			LoadAUcubeExample () ;
			//LoadTest () ;
		}

		private void Window_SizeChanged (object sender, SizeChangedEventArgs e) {
			e.Handled =true ;
		}

		// In debug, Drag'nDrop will not work if you run Developer Studio as administrator
		private void Thumbnails_Drop (object sender, DragEventArgs e) {
			e.Handled =true ;
			if ( e.Data.GetDataPresent (DataFormats.FileDrop) ) {
				ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
				string [] files =(string [])e.Data.GetData (DataFormats.FileDrop) ;
				foreach ( string filename in files ) {
					items.Add (new ReCapPhotoItem () {
						Name =System.IO.Path.GetFileNameWithoutExtension (filename),
						Type =System.IO.Path.GetExtension (filename),
						Image =filename
					}) ;
				}
				Thumbnails.ItemsSource =items ;
				Thumbnails.SelectAll () ;
			}
		}

		// Example getting images with wildcard search on local disc
		/*private void Tirelire_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			DirectoryInfo folder =new DirectoryInfo (@"C:\Program Files\Autodesk\AutodeskWpfReCap\Images") ;
			FileInfo [] images =folder.GetFiles ("Tirelire*.jpg") ;
			foreach ( FileInfo img in images ) {
				items.Add (new ReCapPhotoItem () { Name =img.Name, Type =img.Extension, Image =img.FullName }) ;
				//new BitmapImage (new Uri (img.FullName))
			}
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}*/

		// Example taking images from the Application Resource
		private void Tirelire_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 6 ; i++ ) {
				items.Add (new ReCapPhotoItem () {
					Name ="Tirelire" + i.ToString (),
					Type ="jpg",
					Image =@"Images\Tirelire" + i.ToString () + ".jpg"
				}) ;
			}
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		// Examples referencing images on the WEB
		private void KidSnail_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 63 ; i++ ) {
				items.Add (new ReCapPhotoItem () {
					Name ="KidSnail" + i.ToString (),
					Type ="jpg",
					Image =@"https://raw.github.com/ADN-DevTech/Autodesk-ReCap-Samples/master/Examples/KidSnail/KidSnail" + i.ToString () + ".jpg"
				}) ;
			}
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		private void Calc_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 60 ; i++ ) {
				items.Add (new ReCapPhotoItem () {
					Name ="Calc" + i.ToString (),
					Type ="jpg",
					Image =@"https://raw.github.com/ADN-DevTech/Autodesk-ReCap-Samples/master/Examples/Calc/Calc" + i.ToString () + ".jpg"
				}) ;
				}
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		// Example getting images with images from a ZIP file on local disc
		// These examples come from the ReCap offical site that need to be downloaded first
		// as ReCap API cannot reference a ZIP file, or images in ZIP
		private void GetReCapExample (string url, string location) {
			if ( !File.Exists (location) ) {
				if ( url != null ) {
					if ( System.Windows.MessageBox.Show ("This sample is quite large, are you sure you want to proceed?\nThe file would be downloaded only once.", "ReCap Example download", MessageBoxButton.YesNo) == MessageBoxResult.Yes )
						ReCapExample_Download (url, location) ;
				}
				return ;
			}

			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			FileStream zipStream =File.OpenRead (location) ;
			using ( ZipArchive zip =new ZipArchive (zipStream) ) {
				foreach ( ZipArchiveEntry entry in zip.Entries ) {
					items.Add (new ReCapPhotoItem () {
						Name =System.IO.Path.GetFileNameWithoutExtension (entry.Name),
						Type =System.IO.Path.GetExtension (entry.Name).Trim (new char [] { '.' }),
						Image =location + ":" + entry.FullName 
					}) ;
				}
			}
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		private void ReCapExample_Download (string url, string location) {
			DownloadFileWnd wnd =new DownloadFileWnd () ;
			wnd._urlAddress =url ;
			wnd._location =location ;
			wnd._callback =new DownloadResultForPreviewCompletedDelegate (this.DownloadExampleCompleted) ;
			wnd.Show () ;
		}

		public void DownloadExampleCompleted (string zip) {
			GetReCapExample (null, System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + zip + ".zip") ;
		}

		private void Warrior_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=e0c8c37990674a24a561ba365009f5f4" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "Warrior.zip" ;
			GetReCapExample (url, location) ;
		}

		private void Horns_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=8eca9c8f22f8458b9ea35cec2e1dc7e3" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "Horns.zip" ;
			GetReCapExample (url, location) ;
		}

		private void Alligator_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=3e15e3b1064f41ff823d7e05ff8cae9b" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "Alligator.zip" ;
			GetReCapExample (url, location) ;
		}

		private void Mask_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=3e152c36f6e6438581b36e7c9f9eed5f" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "Mask.zip" ;
			GetReCapExample (url, location) ;
		}

		private void GymCenter_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=f92f89c676a7419a8e8bf9040a3280c7" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "GymCenter.zip" ;
			GetReCapExample (url, location) ;
		}

		private void Marriot_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			string url ="https://360.autodesk.com/Public/Download?hash=bb90ed0616ea4078b9ef4da27f8fa975" ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + "Marriot.zip" ;
			GetReCapExample (url, location) ;
		}

		// UI - Commands
		private void Thumbnails_CreateNewScene (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( Thumbnails.SelectedItems.Count == 0 /*|| Thumbnails.SelectedItems.Count > 20*/ ) {
				//System.Windows.MessageBox.Show ("No images selected, or too many iamages selected (max 20 in one upload)!") ;
				System.Windows.MessageBox.Show ("No images selected!") ;
				return ;
			}
			string photosceneid =CreateReCapPhotoscene () ;
			if ( photosceneid != "" ) {
				textBox1.Text +=string.Format ("\nCreatePhotoscene succeeded - PhotoSceneid = {0}", photosceneid) ;
				if ( PhotoScenes.ItemsSource != null ) {
					ObservableCollection<ReCapPhotosceneidItem> items =new ObservableCollection<ReCapPhotosceneidItem> ((IEnumerable<ReCapPhotosceneidItem>)PhotoScenes.ItemsSource) ;
					items.Add (new ReCapPhotosceneidItem () {
						Name =photosceneid,
						Type ="CREATED",
						Image =@"Images\ReCap.jpg"
					}) ;
					PhotoScenes.ItemsSource =items ;
					PhotoScenes.Items.Refresh () ;
				}
				UploadPhotos (photosceneid) ;
			}
			textBox1.ScrollToEnd () ;
		}

		private void Thumbnails_Remove (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			foreach ( ReCapPhotoItem item in Thumbnails.SelectedItems ) {
				Thumbnails.Items.Remove (item) ;
			}
			textBox1.ScrollToEnd () ;
		}

		private void Thumbnails_RemoveAll (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			Thumbnails.ItemsSource =null ;
			textBox1.ScrollToEnd () ;
		}

		private void Thumbnails_TestConnection (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			ConnectWithReCap () ;
			textBox1.ScrollToEnd () ;
		}

		private void PhotoScenes_UploadPhotos (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( Thumbnails.SelectedItems.Count == 0 /*|| Thumbnails.SelectedItems.Count > 20*/ ) {
				//System.Windows.MessageBox.Show ("No images selected, or too many iamages selected (max 20 in one upload)!") ;
				System.Windows.MessageBox.Show ("No images selected!") ;
				return ;
			}
			if ( PhotoScenes.SelectedItems.Count != 1 ) {
				System.Windows.MessageBox.Show ("No Photoscene selected!") ;
				return ;
			}
			ReCapPhotosceneidItem item =PhotoScenes.SelectedItem as ReCapPhotosceneidItem ;
			UploadPhotos (item.Name) ;
			textBox1.ScrollToEnd () ;
		}

		private void PhotoScenes_ProcessPhotoscene (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneidItem item =PhotoScenes.SelectedItem as ReCapPhotosceneidItem ;
			if ( ProcessPhotoscene (item.Name) ) {
				JobProgress jobWnd =new JobProgress () ;
				jobWnd._photosceneid =item.Name ;
				jobWnd._callback =new ProcessPhotosceneCompletedDelegate (this.ProcessPhotosceneCompleted) ;
				jobWnd.Show () ;
			}
			textBox1.ScrollToEnd () ;
		}

		public void ProcessPhotosceneCompleted (string photosceneid, string status) {
			foreach ( ReCapPhotosceneidItem item in PhotoScenes.Items ) {
				if ( item.Name == photosceneid ) {
					item.Type =status ;
					PhotoScenes.Items.Refresh () ;
					break ;
				}
			}
		}

		private void PhotoScenes_Properties (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneidItem item =PhotoScenes.SelectedItem as ReCapPhotosceneidItem ;
			if ( PhotosceneProperties (item.Name) ) {
				textBox1.ScrollToEnd () ;
				XmlDocument doc =_recap.xml () ;
				PropertiesWnd wnd =new PropertiesWnd () ;
				wnd.xml =doc ;
				wnd.ShowDialog () ;
			}
			textBox1.ScrollToEnd () ;
		}

		private void PhotoScenes_DownloadResult (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneidItem item =PhotoScenes.SelectedItem as ReCapPhotosceneidItem ;
			string link =GetPhotosceneResult (item.Name) ;
			if ( link != "" ) {
				DownloadFileWnd wnd =new DownloadFileWnd () ;
				wnd._urlAddress =link ;
				wnd._location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory)
					+ item.Name + System.IO.Path.GetExtension (link) ;
				if ( e == null ) // Preview
					wnd._callback =new DownloadResultForPreviewCompletedDelegate (this.DownloadResultForPreviewCompleted) ;
				wnd.Show () ;
			}
			textBox1.ScrollToEnd () ;
		}

		// If not using .NET 4.5, http://dotnetzip.codeplex.com/
		private void PhotoScenes_Preview (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			if ( PhotoScenes.SelectedItems.Count != 1 )
				return ;
			ReCapPhotosceneidItem item =PhotoScenes.SelectedItem as ReCapPhotosceneidItem ;
			string location =System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory) + item.Name + ".zip" ;
			if ( !File.Exists (location) ) {
				if ( e != null )
					PhotoScenes_DownloadResult (null, null) ;
				return ;
			}

			FileStream zipStream =File.OpenRead (location) ;
			LoadModel (zipStream) ;

			textBox1.ScrollToEnd () ;
			TabControl1.SelectedItem =tabItem3 ;
		}

		public void DownloadResultForPreviewCompleted (string photosceneid) {
			foreach ( ReCapPhotosceneidItem item in PhotoScenes.Items ) {
				if ( item.Name == photosceneid ) {
					item.Image =photosceneid + ".zip:icon.png" ;
					PhotoScenes.Items.Refresh () ;
					break ;
				}
			}
			PhotoScenes_Preview (null, null) ;
		}

		private void PhotoScenes_DeletePhotoscene (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			ObservableCollection<ReCapPhotosceneidItem> items =new ObservableCollection<ReCapPhotosceneidItem> ((IEnumerable<ReCapPhotosceneidItem>)PhotoScenes.ItemsSource) ;
			foreach ( ReCapPhotosceneidItem item in PhotoScenes.SelectedItems ) {
				if ( DeletePhotoscene (item.Name) )
					items.Remove (item) ;
			}
			PhotoScenes.ItemsSource =items ;
			PhotoScenes.Items.Refresh () ;
			textBox1.ScrollToEnd () ;
		}

		private void TabControl1_SelectionChanged (object sender, SelectionChangedEventArgs e) {
			e.Handled =true ;
			if ( TabControl1.SelectedIndex == 1 && ConnectWithReCap () ) { // If the 'ReCap Projects' panel was selected
				if ( PhotoScenes.Items.Count == 0 ) {
					bool ret =_recap.SceneList ("userID", UserSettings.ReCapUserID) ;
					textBox1.Text +="\n" + _recap._lastResponse.Content ;

					textBox1.Text +=string.Format ("\nSceneList succeeded") ;
					ObservableCollection<ReCapPhotosceneidItem> items =new ObservableCollection<ReCapPhotosceneidItem> () ;
					XmlDocument doc =_recap.xml () ;
					XmlNodeList nodes =doc.SelectNodes ("/Response/Photoscenes/Photoscene") ;
					foreach ( XmlNode fnode in nodes ) {
						XmlNode p0 =fnode.SelectSingleNode ("deleted") ;
						if ( p0 != null && p0.InnerText == "true" )
							continue ;
						XmlNode p1 =fnode.SelectSingleNode ("photosceneid") ;
						string photosceneid =p1.InnerText ;
						XmlNode p2 =fnode.SelectSingleNode ("status") ;
						textBox1.Text +=string.Format ("\n\t{0} [{1}]", p1.InnerText, p2.InnerText) ;

						// If we have the result downloaded, displays the resulting icon instead of the generic image
						items.Add (new ReCapPhotosceneidItem () {
							Name =photosceneid,
							Type =p2.InnerText,
							//Image =@"Images\ReCap.jpg"
							Image =(File.Exists (photosceneid + ".zip") ? photosceneid + ".zip:icon.png" : @"Images\ReCap.jpg")
						}) ;
					}
					PhotoScenes.ItemsSource =items ;
				}
				textBox1.ScrollToEnd () ;
				return ;
			}
		}

		#endregion

		#region ReCap Calls
		protected bool ConnectWithReCap () {
			if ( _recap != null )
				return (true) ;
			_recap =new AdskReCap (
				UserSettings.ReCapClientID,
				UserSettings.CONSUMER_KEY, UserSettings.CONSUMER_SECRET,
				Properties.Settings.Default.oauth_token, Properties.Settings.Default.oauth_token_secret
			) ;
			System.Diagnostics.Debug.WriteLine ("tokens: " + Properties.Settings.Default.oauth_token + " - " + Properties.Settings.Default.oauth_token_secret) ;
			if ( _recap.ServerTime () ) { // Test connection
				XmlDocument doc =_recap.xml () ;
				XmlNode node =doc.SelectSingleNode ("/Response/date") ; // doc.DocumentElement.SelectSingleNode ("/Response/date/text()").Value
				label3.Content =node.InnerText ;
			} else {
				label3.Content ="Connection to ReCap Server failed!" ;
				_recap =null ;
			}
			return (_recap != null) ;
		}

		protected string CreateReCapPhotoscene () {
			if ( !ConnectWithReCap () )
				return ("") ;

			//- Create Photoscene
			string photosceneid ;
			Dictionary<string, string> options =new Dictionary<string, string> () {
				{ "callback", "email://" + UserSettings.Email }
			} ;
			bool ret =_recap.CreatePhotoscene ("obj", "7", options) ;
			textBox1.Text +="\n" + _recap._lastResponse.Content ;
			if ( !ret ) {
				textBox1.Text +="\nCreatePhotoscene error" ;
				return ("") ;
			}
			XmlDocument doc =_recap.xml () ;
			XmlNode node =doc.SelectSingleNode ("/Response/Photoscene/photosceneid") ;
			return (node.InnerText) ;
		}

		protected bool UploadPhotos (string photosceneid) {
			if ( photosceneid == "" || !ConnectWithReCap () )
				return (false) ;
			//- Upload images to the project
			Dictionary<string, string> files =new Dictionary<string, string> () ;
			//Dictionary<string, string> filesRef =new Dictionary<string, string> () ;
			foreach ( ReCapPhotoItem item in Thumbnails.SelectedItems ) {
				//files.Add (item.Name, item.Image) ;
				if ( File.Exists (item.Image) ) {
					files.Add (item.Name, item.Image) ;
				} else if ( item.Image.Substring (0, 4).ToLower () == "http" || item.Image.Substring (0, 3).ToLower () == "ftp" ) {
					//filesRef.Add (item.Name, item.Image) ;
					files.Add (item.Name, item.Image) ;
				} else if ( item.Image.ToLower ().Contains (".zip:") == true ) {
 					// ReCap does not works with zip, we need to send images one by one
					string [] sts =item.Image.Split (':') ;
					if ( sts.Length == 3 ) {
						sts [1] =sts [0] + ":" + sts [1] ;
						sts =sts.Where (w => w != sts [0]).ToArray () ;
					}
					FileStream zipStream =File.OpenRead (sts [0]) ;
					using ( ZipArchive zip =new ZipArchive (zipStream) ) {
						ZipArchiveEntry entry =zip.GetEntry (sts [1]) ;
						DeflateStream str =entry.Open () as DeflateStream ;
						Byte [] byts =new Byte [entry.Length] ;
						str.Read (byts, 0, (int)entry.Length) ;
						files.Add (System.IO.Path.GetFileName (sts [1]), Convert.ToBase64String (byts)) ;
					}
				} else {
					// This is coming from our resources
					StreamResourceInfo stri =Application.GetResourceStream (new Uri (
						item.Image,
						UriKind.Relative
					)) ;
					if ( stri != null ) {
						Stream str =stri.Stream ;
						Byte [] byts =new Byte [str.Length] ;
						str.Read (byts, 0, (int)str.Length) ;
						files.Add (System.IO.Path.GetFileName (item.Image), Convert.ToBase64String (byts)) ;
					}
				}
			}

			// ReCap only accepts 20 uploads at a time with image not larger than 128Mb
			// Let's assume files size is ok and split calls by 20 max each time
			//     return (UploadPhotosExecute (photosceneid, files, filesRef)) ;
			int nRet =0 ;
			if ( files != null && files.Count != 0 ) {
				int i =0 ;
				int n =1 + files.Count / 20 ;
				nRet =+n ;
				var splits =(from item in files
							 group item by i++ % n into part
							 select part).ToList () ; // ToDictionary (g => g.Key, g => g.Last ());
				foreach ( var grp in splits ) {
					Dictionary<string, string> dict =new Dictionary<string, string> () ;
					foreach ( var entry in grp )
						dict.Add (entry.Key, entry.Value) ;
					if ( UploadPhotosExecute (photosceneid, dict) )
						nRet-- ;
				}
			}
			return (nRet == 0) ;
		}

		protected bool UploadPhotosExecute (string photosceneid, Dictionary<string, string> files) {
			// Synchronous sample
			//bool ret =_recap.UploadFiles (photosceneid, files) ;
			//textBox1.Text +="\n" + _recap._lastResponse.Content ;
			//if ( !ret ) {
			//	textBox1.Text +="\nUploadFiles error" ;
			//	return (false) ;
			//}
			//textBox1.Text +="\nUploadFiles succeeded" ;
			//XmlDocument doc =_recap.xml () ;
			//XmlNodeList nodes =doc.SelectNodes ("/Response/Files/file") ;
			//foreach ( XmlNode fnode in nodes ) {
			//	XmlNode p1 =fnode.SelectSingleNode ("filename") ;
			//	XmlNode p2 =fnode.SelectSingleNode ("fileid") ;
			//	textBox1.Text +=string.Format ("\n\t{0} [{1}]", p1.InnerText, p2.InnerText) ;
			//}

			// Async call
			UploadProgress wnd =new UploadProgress () ;
			wnd._photosceneid =photosceneid ;
			var asyncHandle =_recap.UploadFilesAsync (photosceneid, files, wnd.callback) ;
			if ( asyncHandle != null ) {
				textBox1.Text +="\nUploadFiles async successfully started" ;
				wnd._asyncHandle =asyncHandle ;
				wnd._callback =new UploadPhotosCompletedDelegate (this.UploadPhotosCompleted) ;
				wnd.Show () ;
			} else {
				textBox1.Text +="\nUploadFiles async error" ;
			}
			return (asyncHandle != null) ;
		}

		public void UploadPhotosCompleted (IRestResponse response) {
			textBox1.Text +="\n" + response.Content ;
			if (   response.StatusCode != HttpStatusCode.OK
				|| response.Content.IndexOf ("<error>") != -1
				|| response.Content.IndexOf ("<Error>") != -1
			) {
				textBox1.Text +="\nUploadFiles error" ;
				return ;
			}
			textBox1.Text +="\nUploadFiles succeeded" ;
			XmlDocument doc =new XmlDocument () ;
			doc.LoadXml (response.Content) ;
			XmlNodeList nodes =doc.SelectNodes ("/Response/Files/file") ;
			foreach ( XmlNode fnode in nodes ) {
				XmlNode p1 =fnode.SelectSingleNode ("filename") ;
				XmlNode p2 =fnode.SelectSingleNode ("fileid") ;
				textBox1.Text +=string.Format ("\n\t{0} [{1}]", p1.InnerText, p2.InnerText) ;
			}
		}

		protected bool ProcessPhotoscene (string photosceneid) {
			if ( photosceneid == "" || !ConnectWithReCap () )
				return (false) ;
			bool ret =_recap.ProcessScene (photosceneid) ;
			textBox1.Text +="\n" + _recap._lastResponse.Content ;
			if ( !ret ) {
				textBox1.Text +="\nError while lauching Photoscene!" ;
				return (false) ;
			}
			textBox1.Text +="\nPhotoscene processing request sent" ;
			return (true) ;
		}

		protected bool PhotosceneProperties (string photosceneid) {
			if ( photosceneid == "" || !ConnectWithReCap () )
				return (false) ;
			bool ret =_recap.SceneProperties (photosceneid) ;
			textBox1.Text +="\n" + _recap._lastResponse.Content ;
			if ( !ret ) {
				textBox1.Text += "\nError while getting properties!" ;
				return (false) ;
			}
			textBox1.Text +="\nPhotoscene properties query successful" ;
			return (true) ;
		}

		protected string GetPhotosceneResult (string photosceneid) {
			if ( photosceneid == "" || !ConnectWithReCap () )
				return ("") ;
			bool ret =_recap.GetPointCloudArchive (photosceneid, "obj") ;
			textBox1.Text +="\n" + _recap._lastResponse.Content ;
			if ( !ret ) {
				textBox1.Text +="\nGetPointCloudArchive error" ;
				return ("") ;
			}
			textBox1.Text += "\nGetPointCloudArchive succeeded" ;
			XmlDocument doc =_recap.xml () ;
			XmlNode node =doc.SelectSingleNode ("/Response/Photoscene/scenelink") ;
			return (node.InnerText) ;
		}

		protected bool DeletePhotoscene (string photosceneid) {
			if ( photosceneid == "" || !ConnectWithReCap () )
				return (false) ;
			//bool ret =_recap.DeleteScene (photosceneid) ;
			bool ret =_recap.DeleteScene2 (photosceneid) ;
			textBox1.Text +="\n" + _recap._lastResponse.Content ;
			if ( !ret ) {
				textBox1.Text +="\nError - DeletePhotoscene call failed!" ;
				return (false) ;
			}
			textBox1.Text +="\nDeletePhotoscene call succeeded" ;
			XmlDocument doc =_recap.xml () ;
			XmlNodeList nodes =doc.SelectNodes ("/Response/Photoscene/deleted") ;
			if ( nodes.Count != 0 )
				textBox1.Text +="\nMy ReCap Photoscene is now deleted\n" ;
			else
				textBox1.Text +="\nFailed deleting the Photoscene and resources!\n" ;
			return (true) ;
		}

		#endregion

		#region Utilities to create a 3D model out of OBJ meshes
		//protected void LoadTest () {
		//	//FileStream zipStream =File.OpenRead ("kRi3rCR0kqu14I7Logtp7J1fYEE.zip") ;
		//	//FileStream zipStream =File.OpenRead ("D9hVhzPKXNyKs1nmwdkF06PfWrg.zip") ;
		//	FileStream zipStream =File.OpenRead ("TAY0M381eVQyEsRhLXHdTQZRGdE.zip") ;
		//	LoadModel (zipStream) ;
		//}

		protected void LoadAUcubeExample () {
			// This is coming from our resources
			StreamResourceInfo stri =Application.GetResourceStream (new Uri (
				@"Examples\AUquads.zip",
				UriKind.Relative
			)) ;
			LoadModel (stri.Stream) ;
		}

		protected void LoadModel (Stream str) {
			using ( ZipArchive zip =new ZipArchive (str) ) {
				ZipArchiveEntry mesh =zip.GetEntry ("mesh.obj") ;
				ZipArchiveEntry mtl =zip.GetEntry ("mesh.mtl") ;
				ZipArchiveEntry texture =zip.GetEntry ("tex_0.jpg") ;

				using ( new CursorSwitcher (null) ) {
					var objLoaderFactory =new ObjLoaderFactory () ;
					var objLoader =objLoaderFactory.Create (new ObjMaterialStreamProvider (mtl)) ;
					var result =objLoader.Load (mesh.Open ()) ;
					// Reset the model & transform(s)
					_currentDisplayMode =DisplayMode.Textured ;
					this.model.Children.Clear () ;
					_currentWireframe =null ;
					_currentMesh =ObjTriangleMeshAdapater.BuildVisualModel (result, texture) ;
					_currentMeshMatGroup =(_currentMesh.Content as GeometryModel3D).Material ;
					model.Children.Add (_currentMesh) ;
				}
			}
			Home_Click (null, null) ;
		}

		protected void BuildWireframeModel (double depthOffset) {
			if ( _currentWireframe != null && _wireframeDirty == false )
				return ;
			_wireframeDirty =false ;
			bool isPresent =model.Children.Contains (_currentMesh) ;
			if ( !isPresent )
				model.Children.Add (_currentMesh) ;
			_currentWireframe =WireframeMeshAdapter.BuildVisualModel (_currentMesh, 1.0, depthOffset) ;
			if ( !isPresent )
				model.Children.Remove (_currentMesh) ;
		}

		#endregion

		#region Controlling the 3D view camera
		private void Grid_MouseDown (object sender, MouseButtonEventArgs e) {
			Mouse.Capture (canvas, CaptureMode.Element) ;
			_lastPos = Mouse.GetPosition (viewport) ; //e.GetPosition ()
		}

		private void Grid_MouseUp (object sender, MouseButtonEventArgs e) {
			Mouse.Capture (canvas, CaptureMode.None) ;
			if ( _wireframeDirty == true && (_currentDisplayMode == DisplayMode.Wireframe || _currentDisplayMode == DisplayMode.WireframeOnShaded ) ) {
				model.Children.Remove (_currentWireframe) ;
				BuildWireframeModel (0.0) ; //_currentDisplayMode == DisplayMode.Wireframe ? 0.0 : 1.0) ;
				model.Children.Add (_currentWireframe) ;

				foreach ( Visual3D child in model.Children )
					CleanChildTransforms (child) ;
			}
		}

		private void Grid_MouseMove (object sender, MouseEventArgs e) {
			Point pos =Mouse.GetPosition (viewport) ;
			if ( e.LeftButton == MouseButtonState.Pressed )
				Viewport_Rotate (pos) ;
			else if ( e.MiddleButton == MouseButtonState.Pressed )
				Viewport_Pan (pos) ;
			_lastPos =pos ;
		}

		private void Viewport_Pan (Point actualPos) {
			Vector3D lastPos3D =ProjectToTrackball (_lastPos) ;
			Vector3D pos3D =ProjectToTrackball (actualPos) ;

			//Length(original_position - cam_position) / Length(offset_vector) = Length(zNearA - cam_position) / Length(zNearB - zNearA)
			//offset_vector = Length(original_position - cam_position) / Length(zNearA - cam_position) * (zNearB - zNearA)
			double halfFOV =(camera.FieldOfView / 2.0f) * (Math.PI / 180.0) ;
			double distanceToObject =((Vector3D)camera.Position).Length ; // Compute the world space distance from the camera to the object you want to pan
			double projectionToWorldScale =distanceToObject * Math.Tan (halfFOV) ;
			Vector mouseDeltaInScreenSpace =actualPos - _lastPos ; // The delta mouse in pixels that we want to pan
			Vector mouseDeltaInProjectionSpace =new Vector (mouseDeltaInScreenSpace.X * 2 / viewport.ActualWidth, mouseDeltaInScreenSpace.Y * 2 / viewport.ActualHeight) ; // ( the "*2" is because the projection space is from -1 to 1)
			Vector cameraDelta =-mouseDeltaInProjectionSpace * projectionToWorldScale ; // Go from normalized device coordinate space to world space (at origin)

			Vector3D tr =new Vector3D (0.0d, -cameraDelta.Y, -cameraDelta.X) ; // Remember we are up=<0,-1,0>
			_viewportT +=tr ;

			foreach ( Visual3D child in model.Children ) {
				Transform3DGroup transformGroup =child.Transform as Transform3DGroup ;
				transformGroup.Children.Add (new TranslateTransform3D (tr)) ; // Remember we are up=<0,-1,0>
			}

			_wireframeDirty =true ;
		}

		private void Viewport_Rotate (Point actualPos) {
			Vector3D lastPos3D =ProjectToTrackball (_lastPos) ;
			Vector3D pos3D =ProjectToTrackball (actualPos) ;
			Vector3D axis =Vector3D.CrossProduct (lastPos3D, pos3D) ;
			double angle =Vector3D.AngleBetween (lastPos3D, pos3D) ;

			if ( axis.Length == 0 && angle == 0 )
				return ;
			Quaternion quat =new Quaternion (axis, -angle) ;
			_viewportR =quat * _viewportR ;

			QuaternionRotation3D r =new QuaternionRotation3D (quat) ;
			foreach ( Visual3D child in model.Children ) {
				Transform3DGroup transformGroup =child.Transform as Transform3DGroup ;
				transformGroup.Children.Add (new RotateTransform3D (r)) ;
			}

			_wireframeDirty =true ;
		}

		// http://scv.bu.edu/documentation/presentations/visualizationworkshop08/materials/opengl/trackball.c
		// http://curis.ku.dk/ws/files/38552161/01260772.pdf
		private Vector3D ProjectToTrackball (Point pos) { // Project an <x, y> pair onto a sphere
			// Translate 0,0 to the center, so <x, y> is [<-1, -1> - <1, 1>]
			double x =pos.X / (viewport.ActualWidth / 2) - 1 ;
			double y =1 - pos.Y / (viewport.ActualHeight / 2) ; // Flip Y - up instead of down
			double z2 =1 - Math.Pow (x, 2) - Math.Pow (y, 2) ; // z^2 =1 - x^2 - y^2
			double z =(z2 > 0 ? Math.Sqrt (z2) : 0) ;
			// Remember we are up=<0,-1,0>
			return (new Vector3D (-z, -y, x)) ;
		}

		private void Grid_MouseWheel (object sender, MouseWheelEventArgs e) {
			e.Handled =true ;

			Vector3D lookAt =camera.LookDirection ;
			//lookAt.Negate () ;
			lookAt.Normalize () ;
			lookAt *=e.Delta / 250.0d ;
			camera.Position +=lookAt ;

			_wireframeDirty =true ;
			if ( _currentDisplayMode == DisplayMode.Wireframe || _currentDisplayMode == DisplayMode.WireframeOnShaded ) {
				model.Children.Remove (_currentWireframe) ;
				BuildWireframeModel (0.0) ; //_currentDisplayMode == DisplayMode.Wireframe ? 0.0 : 1.0) ;
				model.Children.Add (_currentWireframe) ;

				foreach ( Visual3D child in model.Children )
					CleanChildTransforms (child) ;
			}
		}

		private void CleanChildTransforms (Visual3D child) {
			Transform3DGroup transformGroup =child.Transform as Transform3DGroup ;
			for ( int i =(child == _currentMesh || child == _currentWireframe ? 2 : 0) ; i < transformGroup.Children.Count ; /*i++*/ )
				transformGroup.Children.RemoveAt (i) ;
			if ( _viewportT.Length != 0 )
				transformGroup.Children.Add (new TranslateTransform3D (_viewportT)) ;
			if ( !_viewportR.IsIdentity )
				transformGroup.Children.Add (new RotateTransform3D (new QuaternionRotation3D (_viewportR))) ;
		}

		private void Home_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;

			_viewportT =new Vector3D () ;
			_viewportR =new Quaternion () ;

			// 0 and 1 are origin scale and translate
			foreach ( Visual3D child in model.Children ) {
				//Transform3DGroup transformGroup =child.Transform as Transform3DGroup ;
				//for ( int i =(child == _currentMesh ? 2 : 0) ; i < transformGroup.Children.Count ; /*i++*/ )
				//	transformGroup.Children.RemoveAt (i) ;
				CleanChildTransforms (child) ;
			}
			camera.Position =new Point3D (4.0d, 0.0d, 0.0d) ;
		}

		private void Wireframe_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;

			if ( _currentDisplayMode == DisplayMode.Wireframe )
				return ;
			model.Children.Remove (_currentWireframe) ;
			BuildWireframeModel (0.0) ;
			if ( !model.Children.Contains (_currentWireframe) )
				model.Children.Add (_currentWireframe) ;
			model.Children.Remove (_currentMesh) ;
			_currentDisplayMode =DisplayMode.Wireframe ;

			foreach ( Visual3D child in model.Children )
				CleanChildTransforms (child) ;
		}

		private void SmoothShade_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;

			if ( _currentDisplayMode == DisplayMode.SmoothShade )
				return ;
			if ( !model.Children.Contains (_currentMesh) )
				model.Children.Add (_currentMesh) ;
			model.Children.Remove (_currentWireframe) ;
			_currentDisplayMode =DisplayMode.SmoothShade ;
			(_currentMesh.Content as GeometryModel3D).Material =(_currentMesh.Content as GeometryModel3D).BackMaterial ;

			foreach ( Visual3D child in model.Children )
				CleanChildTransforms (child) ;
		}

		private void WireframeOnShaded_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;

			if ( _currentDisplayMode == DisplayMode.WireframeOnShaded )
				return ;
			if ( !model.Children.Contains (_currentMesh) )
				model.Children.Add (_currentMesh) ;
			BuildWireframeModel (0.0/*1.0*/) ;
			if ( !model.Children.Contains (_currentWireframe) )
				model.Children.Add (_currentWireframe) ;
			_currentDisplayMode =DisplayMode.WireframeOnShaded ;
			(_currentMesh.Content as GeometryModel3D).Material =(_currentMesh.Content as GeometryModel3D).BackMaterial ;

			foreach ( Visual3D child in model.Children )
				CleanChildTransforms (child) ;
		}

		private void Textured_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;

			if ( _currentDisplayMode == DisplayMode.Textured )
				return ;
			model.Children.Remove (_currentWireframe) ;
			if ( !model.Children.Contains (_currentMesh) ) 
				model.Children.Add (_currentMesh);
			_currentDisplayMode =DisplayMode.Textured ;
			(_currentMesh.Content as GeometryModel3D).Material =_currentMeshMatGroup ;

			foreach ( Visual3D child in model.Children )
				CleanChildTransforms (child) ;
		}

		private void ambiantlightToggle_Checked (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( !lights.Children.Contains (ambientLightMain) )
				lights.Children.Add (ambientLightMain) ;
		}

		private void ambiantlightToggle_Unchecked (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			lights.Children.Remove (ambientLightMain) ;
		}

		private void dirlightToggle_Checked (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( !lights.Children.Contains (dirLightMain) )
				lights.Children.Add (dirLightMain) ;
		}

		private void dirlightToggle_Unchecked (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			lights.Children.Remove (dirLightMain) ;
		}

		#endregion


	}

}