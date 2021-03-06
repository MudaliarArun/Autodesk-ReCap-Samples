﻿// (C) Copyright 2014 by Autodesk, Inc.
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

using Autodesk.ADN.Toolkit.ReCap;

// http://wpftoolkit.codeplex.com/wikipage?title=PropertyGrid

namespace Autodesk.ADN.WpfReCap {

	public partial class PropertiesWnd : Window {
		protected AdskReCapPhotoscene _obj ;

		protected PropertiesWnd () {
			InitializeComponent () ;
		}

		public PropertiesWnd (AdskReCapPhotoscene obj) {
			_obj =obj ;
			InitializeComponent () ;
		}

		private void Window_Loaded (object sender, RoutedEventArgs e) {
			propertyGrid.SelectedObject =_obj ;
		}

	}

}
