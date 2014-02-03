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

	public partial class MainWindow : Window {
		protected AdskReCap _recap =null ;
		private bool _mouseDown =false ;
		private Point _lastPos ;

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

		// Examples referencing images on the WEB
		private void Tirelire_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 5 ; i++ )
				items.Add (new ReCapPhotoItem () { Name ="Tirelire" + i.ToString (), Type ="jpg", Image =@"https://raw.github.com/ADN-DevTech/Autodesk-ReCap-Samples/master/Examples/Tirelire/Tirelire" + i.ToString () + ".jpg" }) ;
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		private void KidSnail_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 63 ; i++ )
				items.Add (new ReCapPhotoItem () { Name ="KidSnail" + i.ToString (), Type ="jpg", Image =@"https://raw.github.com/ADN-DevTech/Autodesk-ReCap-Samples/master/Examples/KidSnail/KidSnail" + i.ToString () + ".jpg" }) ;
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		private void Calc_Click (object sender, RoutedEventArgs e) {
			if ( e != null )
				e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> () ;
			for ( int i =0 ; i < 60 ; i++ )
				items.Add (new ReCapPhotoItem () { Name ="KidSnail" + i.ToString (), Type ="jpg", Image =@"https://raw.github.com/ADN-DevTech/Autodesk-ReCap-Samples/master/Examples/Calc/Calc" + i.ToString () + ".jpg" }) ;
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		// Example taking images from the Application Resource
		private void Alligator_Click (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			ObservableCollection<ReCapPhotoItem> items =new ObservableCollection<ReCapPhotoItem> ();
			items.Add (new ReCapPhotoItem () { Name ="ReCap0", Type ="jpg", Image =@"Images\Alligator0.jpg" }) ;
			items.Add (new ReCapPhotoItem () { Name ="ReCap1", Type ="jpg", Image =@"Images\Alligator1.jpg" }) ;
			items.Add (new ReCapPhotoItem () { Name ="ReCap2", Type ="jpg", Image =@"Images\Alligator2.jpg" }) ;
			items.Add (new ReCapPhotoItem () { Name ="ReCap3", Type ="jpg", Image =@"Images\Alligator3.jpg" }) ;
			items.Add (new ReCapPhotoItem () { Name ="ReCap4", Type ="jpg", Image =@"Images\Alligator4.jpg" }) ;
			Thumbnails.ItemsSource =items ;
			Thumbnails.SelectAll () ;
		}

		private void Thumbnails_CreateNewScene (object sender, RoutedEventArgs e) {
			e.Handled =true ;
			if ( Thumbnails.SelectedItems.Count == 0 /*|| Thumbnails.SelectedItems.Count > 20*/ ) {
				//MessageBox.Show ("No images selected, or too many iamages selected (max 20 in one upload)!") ;
				MessageBox.Show ("No images selected!") ;
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
				//MessageBox.Show ("No images selected, or too many iamages selected (max 20 in one upload)!") ;
				MessageBox.Show ("No images selected!") ;
				return ;
			}
			if ( PhotoScenes.SelectedItems.Count != 1 ) {
				MessageBox.Show ("No Photoscene selected!") ;
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
			using ( ZipArchive zip =new ZipArchive (zipStream) ) {
				ZipArchiveEntry mesh =zip.GetEntry ("mesh.obj") ;
				ZipArchiveEntry mtl =zip.GetEntry ("mesh.mtl") ;
				ZipArchiveEntry texture =zip.GetEntry ("tex_0.jpg") ;

				using ( new CursorSwitcher (null) ) {
					var objLoaderFactory =new ObjLoaderFactory () ;
					var objLoader =objLoaderFactory.Create (new ObjMaterialStreamProvider (mtl)) ;
					var result =objLoader.Load (mesh.Open ()) ;
					// Reset the model & transform(s)
					this.model.Children.Clear () ;
					model.Children.Add (MakeVisualModel (result, texture)) ;
				}
			}

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
			Dictionary<string, string> filesRef =new Dictionary<string, string> () ;
			foreach ( ReCapPhotoItem item in Thumbnails.SelectedItems ) {
				//files.Add (item.Name, item.Image) ;
				if ( File.Exists (item.Image) ) {
					files.Add (item.Name, item.Image) ;
				} else if ( item.Image.Substring (0, 4).ToLower () == "http" || item.Image.Substring (0, 3).ToLower () == "ftp" ) {
					filesRef.Add (item.Name, item.Image) ;
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
					if ( UploadPhotosExecute (photosceneid, dict, null) )
						nRet-- ;
				}
			}
			if ( filesRef != null && filesRef.Count != 0 ) {
				int i =0 ;
				int n =1 + filesRef.Count / 20 ;
				nRet +=n ;
				var splits =(from item in filesRef
							 group item by i++ % n into part
							 select part).ToList () ; // ToDictionary (g => g.Key, g => g.Last ());
				foreach ( var grp in splits ) {
					Dictionary<string, string> dict =new Dictionary<string, string> () ;
					foreach ( var entry in grp )
						dict.Add (entry.Key, entry.Value) ;
					if ( UploadPhotosExecute (photosceneid, null, dict) )
						nRet-- ;
				}
			}
			return (nRet == 0) ;
		}

		protected bool UploadPhotosExecute (string photosceneid, Dictionary<string, string> files, Dictionary<string, string> filesRef) {
			// Synchronous sample
			//bool ret =_recap.UploadFiles (photosceneid, files, filesRef) ;
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
			var asyncHandle =_recap.UploadFilesAsync (photosceneid, files, filesRef, wnd.callback) ;
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

		#region Utilities to create a 3D model our of OBJ meshes
		//protected void LoadTest () {
		//	//FileStream zipStream =File.OpenRead ("kRi3rCR0kqu14I7Logtp7J1fYEE.zip") ;
		//	//FileStream zipStream =File.OpenRead ("D9hVhzPKXNyKs1nmwdkF06PfWrg.zip") ;
		//	FileStream zipStream =File.OpenRead ("TAY0M381eVQyEsRhLXHdTQZRGdE.zip") ;
		//	using ( ZipArchive zip =new ZipArchive (zipStream) ) {
		//		ZipArchiveEntry mesh =zip.GetEntry ("mesh.obj") ;
		//		ZipArchiveEntry mtl =zip.GetEntry ("mesh.mtl") ;
		//		ZipArchiveEntry texture =zip.GetEntry ("tex_0.jpg") ;

		//		using ( new CursorSwitcher (null) ) {
		//			var objLoaderFactory =new ObjLoaderFactory () ;
		//			var objLoader =objLoaderFactory.Create (new ObjMaterialStreamProvider (mtl)) ;
		//			var result =objLoader.Load (mesh.Open ()) ;
		//			// Reset the model & transform(s)
		//			this.model.Children.Clear () ;
		//			model.Children.Add (MakeVisualModel (result, texture)) ;
		//		}
		//	}
		//}

		protected void LoadAUcubeExample () {
			// This is coming from our resources
			StreamResourceInfo stri =Application.GetResourceStream (new Uri (
				@"Examples\AUquads.zip",
				UriKind.Relative
			)) ;
			using ( ZipArchive zip = new ZipArchive (stri.Stream) ) {
				ZipArchiveEntry mesh =zip.GetEntry ("mesh.obj") ;
				ZipArchiveEntry mtl =zip.GetEntry ("mesh.mtl") ;
				ZipArchiveEntry texture =zip.GetEntry ("tex_0.jpg") ;

				using ( new CursorSwitcher (null) ) {
					var objLoaderFactory =new ObjLoaderFactory () ;
					var objLoader =objLoaderFactory.Create (new ObjMaterialStreamProvider (mtl)) ;
					var result =objLoader.Load (mesh.Open ()) ;
					// Reset the model & transform(s)
					this.model.Children.Clear () ;
					model.Children.Add (MakeVisualModel (result, texture)) ;
				}
			}
		}

		public MeshGeometry3D MakeGeometry (LoadResult objmesh) {
			var r =new MeshGeometry3D () ;
			var mesh =new TriangleMeshAdapater (objmesh) ;
			r.Positions =mesh.Points ;
			r.TriangleIndices =mesh.Indices ;
			r.Normals =mesh.Normals ;
			r.TextureCoordinates =mesh.TexCoords ;
			return (r) ;
		}

		// MTL explanation: http://www.kixor.net/dev/objloader/
		public Material MakeMaterial (LoadResult objmesh, ZipArchiveEntry texture) {
			MaterialGroup matGroup =new MaterialGroup () ;

			ObjLoader.Loader.Data.Material material =objmesh.Materials [0] ;

			Stream imgStream =texture.Open () ;
			Byte [] buffer =new Byte [texture.Length] ;
			imgStream.Read (buffer, 0, buffer.Length) ;
			var byteStream =new System.IO.MemoryStream (buffer) ;

			//ImageBrush imgBrush =new ImageBrush (new BitmapImage (new Uri (@"C:\Users\cyrille\Documents\Visual Studio 2012\Projects\tex_0.jpg"))) ;
			BitmapImage bitmap =new BitmapImage () ;
			bitmap.BeginInit () ;
			bitmap.CacheOption =BitmapCacheOption.OnLoad ;
			bitmap.StreamSource =byteStream ;
			bitmap.EndInit () ;
			ImageBrush imgBrush =new ImageBrush (bitmap) ;
			imgBrush.ViewportUnits =BrushMappingMode.Absolute ;
			//imgBrush.ViewportUnits =BrushMappingMode.RelativeToBoundingBox ;

			//Brush brush =new SolidColorBrush (Color.FromScRgb (material.Transparency, material.DiffuseColor.X, material.DiffuseColor.Y, material.DiffuseColor.Z)) ;
			//brush.Opacity =material.Transparency ;

			DiffuseMaterial diffuse =new DiffuseMaterial (imgBrush) ;
			diffuse.AmbientColor =Color.FromScRgb (material.Transparency, material.AmbientColor.X, material.AmbientColor.Y, material.AmbientColor.Z) ;
			// no more attributes
			matGroup.Children.Add (diffuse) ;

			SpecularMaterial specular =new SpecularMaterial (new SolidColorBrush (Color.FromScRgb (material.Transparency, material.SpecularColor.X, material.SpecularColor.Y, material.SpecularColor.Z)), material.SpecularCoefficient) ;
			// no more attributes
			matGroup.Children.Add (specular);

			// Default to Blue
			if ( matGroup.Children.Count == 0 )
				matGroup.Children.Add (new DiffuseMaterial (new SolidColorBrush (Color.FromRgb (0, 0, 255)))) ;
			return (matGroup) ;
		}

		public GeometryModel3D MakeGeometryModel (Geometry3D geom, Material mat) {
			return (new GeometryModel3D (geom, mat)) ;
		}

		public Model3D MakeModel (LoadResult objmesh, ZipArchiveEntry texture) {
			return (MakeGeometryModel (MakeGeometry (objmesh), MakeMaterial (objmesh, texture))) ;
		}

		public ModelVisual3D MakeVisualModel (LoadResult objmesh, ZipArchiveEntry texture) {
			var r =new ModelVisual3D () ;
			r.Content =MakeModel (objmesh, texture) ;
			r.Transform =new Transform3DGroup () ;

			Transform3DGroup transformGroup = r.Transform as Transform3DGroup;
			TranslateTransform3D translation = new TranslateTransform3D (
				-(r.Content.Bounds.X + r.Content.Bounds.SizeX / 2),
				-(r.Content.Bounds.Y + r.Content.Bounds.SizeY / 2),
				-(r.Content.Bounds.Z + r.Content.Bounds.SizeZ / 2)
			);
			transformGroup.Children.Add (translation);

			double scale = Math.Abs (1 / (r.Content.Bounds.SizeX));
			scale = Math.Min (scale, Math.Abs (1 / (r.Content.Bounds.SizeY)));
			scale = Math.Min (scale, Math.Abs (1 / (r.Content.Bounds.SizeZ)));
			ScaleTransform3D scaletr = new ScaleTransform3D (scale, scale, scale);
			transformGroup.Children.Add (scaletr);

			return (r) ;
		}

		#endregion

		#region Controlling the 3D view camera
		private void Grid_MouseDown (object sender, MouseButtonEventArgs e) {
			if ( e.LeftButton != MouseButtonState.Pressed )
				return ;
			_mouseDown =true ;
			Point pos =Mouse.GetPosition (viewport) ;
			_lastPos =new Point (pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y) ;
		}

		private void Grid_MouseUp (object sender, MouseButtonEventArgs e) {
			_mouseDown =false ;
		}

		private void Grid_MouseMove (object sender, MouseEventArgs e) {
			if ( !_mouseDown )
				return ;
			Point pos =Mouse.GetPosition (viewport) ;
			Point actualPos =new Point (pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y) ;
			double dx =actualPos.X - _lastPos.X, dy = actualPos.Y - _lastPos.Y ;
			double mouseAngle =0 ;
			if ( dx != 0 && dy != 0 ) {
				mouseAngle =Math.Asin (Math.Abs (dy) / Math.Sqrt (Math.Pow (dx, 2) + Math.Pow (dy, 2))) ;
				if ( dx < 0 && dy > 0 )
					mouseAngle +=Math.PI / 2 ;
				else if ( dx < 0 && dy < 0 )
					mouseAngle +=Math.PI ;
				else if ( dx > 0 && dy < 0 )
					mouseAngle +=Math.PI * 1.5 ;
			} else if ( dx == 0 && dy != 0 ) {
				mouseAngle =Math.Sign (dy) > 0 ? Math.PI / 2 : Math.PI * 1.5 ;
			} else if ( dx != 0 && dy == 0 ) {
				mouseAngle =Math.Sign (dx) > 0 ? 0 : Math.PI ;
			}
			double axisAngle =mouseAngle + Math.PI / 2 ;
			Vector3D axis =new Vector3D (Math.Cos (axisAngle) * 4, Math.Sin (axisAngle) * 4, 0) ;
			double rotation =0.01 * Math.Sqrt (Math.Pow (dx, 2) + Math.Pow (dy, 2)) ;

			QuaternionRotation3D r =new QuaternionRotation3D (new Quaternion (axis, rotation * 180 / Math.PI)) ;
			foreach ( Visual3D child in model.Children ) {
				Transform3DGroup transformGroup = child.Transform as Transform3DGroup ;
				transformGroup.Children.Add (new RotateTransform3D (r)) ;
			}
			_lastPos =actualPos ;
		}

		private void Grid_MouseWheel (object sender, MouseWheelEventArgs e) {
			camera.Position =new Point3D (camera.Position.X - e.Delta / 250.0d, camera.Position.Y, camera.Position.Z) ;
		}

		#endregion

	}

}