// (C) Copyright 2014 by Autodesk, Inc.
//
// The information contained herein is confidential, proprietary
// to Autodesk, Inc., and considered a trade secret as defined
// in section 499C of the penal code of the State of California.
// Use of this information by anyone other than authorized
// employees of Autodesk, Inc. is granted only under a written
// non-disclosure agreement, expressly prescribing the scope
// and manner of such use.

//- http://www.autodesk.com/joinadn
//- January 20th, 2014
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutodeskWpfReCap {

	public class TileView : ViewBase {

		public static readonly DependencyProperty ItemContainerStyleProperty =ItemsControl.ItemContainerStyleProperty.AddOwner (typeof (TileView)) ;

		public Style ItemContainerStyle {
			get { return ((Style)GetValue (ItemContainerStyleProperty)) ; }
			set { SetValue (ItemContainerStyleProperty, value) ; }
		}

		public static readonly DependencyProperty ItemTemplateProperty =ItemsControl.ItemTemplateProperty.AddOwner (typeof (TileView)) ;

		public DataTemplate ItemTemplate {
			get { return ((DataTemplate)GetValue (ItemTemplateProperty)) ; }
			set { SetValue (ItemTemplateProperty, value) ; }
		}

		public static readonly DependencyProperty ItemWidthProperty =WrapPanel.ItemWidthProperty.AddOwner (typeof (TileView)) ;

		public double ItemWidth {
			get { return ((double)GetValue (ItemWidthProperty)) ; }
			set { SetValue (ItemWidthProperty, value) ; }
		}

		public static readonly DependencyProperty ItemHeightProperty =WrapPanel.ItemHeightProperty.AddOwner (typeof (TileView)) ;

		public double ItemHeight {
			get { return ((double)GetValue (ItemHeightProperty)) ; }
			set { SetValue (ItemHeightProperty, value) ; }
		}

		protected override object DefaultStyleKey {
			get { return (new ComponentResourceKey (GetType (), "myTileView")) ; }
		}

	}

}
