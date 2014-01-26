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
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using Autodesk.Maya;

namespace AutodeskWpfReCap {

	public partial class App : Application {

		public void App_Startup (object sender, StartupEventArgs args) {
			try {
				bool bSuccess =MayaTheme.Initialize (this) ;

			} catch ( System.Exception ex ) {
				MessageBox.Show (ex.Message, "Error during initialization. This program will exit") ;
				Application.Current.Shutdown () ;
			}
		}

	}
}