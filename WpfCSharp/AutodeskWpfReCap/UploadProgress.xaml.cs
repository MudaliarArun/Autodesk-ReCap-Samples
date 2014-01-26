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
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace AutodeskWpfReCap {

	// more to see progrees ?
	// http://stuff.seans.com/2009/01/05/using-httpwebrequest-for-asynchronous-downloads/

	public delegate void UploadPhotosCompletedDelegate (IRestResponse response) ;

	public partial class UploadProgress : Window {
		public string _photosceneid ;
		public RestRequestAsyncHandle _asyncHandle ;
		public UploadPhotosCompletedDelegate _callback =null ;
		private IProgress<ProgressInfo> _progressIndicator ;
	
		public UploadProgress () {
			InitializeComponent () ;
		}

		#region Job Progress tasks
		private void ReportProgress (ProgressInfo value) {
			progressBar.Value =value.pct ;
			progressMsg.Content =value.msg ;
			progressBar.IsIndeterminate =(value.pct != 0 && value.pct != 100) ;
		}

		public void callback (IRestResponse response, RestRequestAsyncHandle asyncHandle) {
			if (   response.StatusCode != HttpStatusCode.OK
				|| response.Content.IndexOf ("<error>") != -1
				|| response.Content.IndexOf ("<Error>") != -1
			) {
				_progressIndicator.Report (new ProgressInfo (0, "UploadFiles error")) ;
			} else {
				_progressIndicator.Report (new ProgressInfo (100, "UploadFiles succeeded")) ;
			}
			this.Dispatcher.Invoke (_callback, new Object [] { response }) ;
		}

		#endregion

		#region Window events
		private void Window_Loaded (object sender, RoutedEventArgs e) {
			sceneid.Content =_photosceneid ;
			ReportProgress (new ProgressInfo (1, "Uploading files to the ReCap server...")) ;
			_progressIndicator =new Progress<ProgressInfo> (ReportProgress) ;
		}

		private void Button_Click (object sender, RoutedEventArgs e) {
			_asyncHandle.Abort () ;
		}

		#endregion

	}

}
