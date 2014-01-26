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
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

// http://wpftoolkit.codeplex.com/wikipage?title=PropertyGrid

namespace AutodeskWpfReCap {

	public class AdskReCapSceneProperties {

		public string PhotoSceneID { get; set; }
		public string Name { get; set; }
		public DateTime CreationDate { get; set; }
		public string MeshQuality { get; set; }
		public string Status { get; set; }
		public string ConvertFormat { get; set; }
		public string ConvertStatus { get; set; }
		public string NbFaces { get; set; }
		public string NbVertices { get; set; }
		public string Nb3dPoints { get; set; }
		public string NbShots { get; set; }
		public string NbStitchedShots { get; set; }

		public AdskReCapSceneProperties (XmlDocument doc) {
			try {
				XmlNode node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/photosceneid") ;
				PhotoSceneID =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/name") ;
				Name =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/creationDate") ;
				CreationDate =DateTime.Parse (Uri.UnescapeDataString (node.InnerText)) ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/meshQuality") ;
				MeshQuality =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/status") ;
				Status =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/convertFormat") ;
				ConvertFormat =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/convertStatus") ;
				ConvertStatus =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/nbfaces") ;
				NbFaces =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/nbvertices") ;
				NbVertices =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/nb3Dpoints") ;
				Nb3dPoints =node.InnerText ;
				node = doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/nbStitchedShots");
				NbStitchedShots =node.InnerText ;
				node =doc.SelectSingleNode ("/Response/Photoscenes/Photoscene/nbShots") ;
				NbShots =node.InnerText ;
			} catch {
			}
		}

	}

	public partial class PropertiesWnd : Window {

		public XmlDocument xml { get; set; }

		public PropertiesWnd () {
			InitializeComponent () ;
		}

		private void Window_Loaded (object sender, RoutedEventArgs e) {
			propertyGrid.SelectedObject =new AdskReCapSceneProperties (xml) ;
		}

	}

}
