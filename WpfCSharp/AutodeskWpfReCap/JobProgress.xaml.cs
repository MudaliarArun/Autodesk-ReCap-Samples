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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Xml;

namespace AutodeskWpfReCap {

	public delegate void ProcessPhotosceneCompletedDelegate (string photoscene, string status) ;

	// http://codereview.stackexchange.com/questions/20820/use-and-understanding-of-async-await-in-net-4-5

	public partial class JobProgress : Window {
		protected CancellationTokenSource _cts =null ;
		protected AdskReCap _recap =null ;
		public ProcessPhotosceneCompletedDelegate _callback =null ;
		public string _photosceneid ;

		public JobProgress () {
			InitializeComponent () ;
		}

		#region Job Progress tasks
		private void ReportProgress (ProgressInfo value) {
			progressBar.Value =value.pct ;
			progressMsg.Content =value.msg ;
		}

		async Task ReCapJobProgress (string photosceneid, IProgress<ProgressInfo> progress, CancellationToken ct, TaskScheduler uiScheduler) {
			progress.Report (new ProgressInfo (0, "Initializing...")) ;
			while ( !ct.IsCancellationRequested ) {
				Task<ProgressInfo> task =Task<ProgressInfo>.Factory.StartNew (() => PhotosceneProgress (photosceneid)) ;
				await task ;

				if ( task.Result == null ) {
					progress.Report (new ProgressInfo (0, "Error")) ;
					break ;
				}
				progress.Report (task.Result) ;
				if ( task.Result.pct >= 100 ) {
					this.Dispatcher.Invoke (_callback, new Object [] { _photosceneid, task.Result.msg }) ;
					break ;
				}
			}
		}

		#endregion

		#region Window events
		private async void Window_Loaded (object sender, RoutedEventArgs e) {
			sceneid.Content =_photosceneid ;
			if ( !ConnectWithReCap () )
				return ;

			TaskScheduler uiScheduler =TaskScheduler.FromCurrentSynchronizationContext () ;
			var progressIndicator =new Progress<ProgressInfo> (ReportProgress) ;
			_cts =new CancellationTokenSource () ;
			try {
				await ReCapJobProgress (_photosceneid, progressIndicator, _cts.Token, uiScheduler) ;
			} catch ( OperationCanceledException ex ) {
			}
		}

		private void Button_Click (object sender, RoutedEventArgs e) {
			_cts.Cancel () ;
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
			return (_recap != null) ;
		}

		protected ProgressInfo PhotosceneProgress (string photosceneid) {
			if ( !ConnectWithReCap () )
				return (null) ;

			bool ret =_recap.SceneProgress (photosceneid) ;
			if ( !ret ) {
				return (null) ;
			}
			XmlDocument doc =_recap.xml ();
			XmlNode node =doc.SelectSingleNode ("/Response/Photoscene/progress") ;
			XmlNode nodemsg =doc.SelectSingleNode ("/Response/Photoscene/progressmsg") ;
			int pct =0 ;
			try {
				if ( node.InnerText != "" )
					pct =(int)Convert.ToDouble (node.InnerText) ;
			} catch {
			}
			return (new ProgressInfo (pct, nodemsg.InnerText)) ;
		}

		#endregion

	}

	public class ProgressInfo {

		public int pct { get; set; }
		public string msg { get; set; }

		public ProgressInfo (int pctValue, string msgValue) {
			pct =pctValue ;
			msg =msgValue ;
		}

	}

}
