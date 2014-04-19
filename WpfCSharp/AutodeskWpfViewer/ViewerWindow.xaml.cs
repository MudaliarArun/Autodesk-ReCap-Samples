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

namespace Autodesk.ADN.Toolkit.Wpf.Viewer {

	enum DisplayMode { Wireframe, SmoothShade, WireframeOnShaded, Textured } ;

	public partial class ViewerWindow : Window {
		private Point _lastPos ;
		private DisplayMode _currentDisplayMode =DisplayMode.Textured ;
		private ModelVisual3D _currentMesh =null ;
		private Material _currentMeshMatGroup =null ;
		private ModelVisual3D _currentWireframe =null ;
		private bool _wireframeDirty =true ;
		private Vector3D _viewportT =new Vector3D () ;
		private Quaternion _viewportR =new Quaternion () ;
		
		public ViewerWindow () {
			InitializeComponent () ;
		}

		public void LoadModel (string location) {
			FileStream zipStream =File.OpenRead (location) ;
			LoadModel (zipStream) ;
		}

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
			Transform3DGroup transformGroup =camera.Transform as Transform3DGroup ;
			transformGroup.Children.Add (new TranslateTransform3D (lookAt)) ;

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

			Transform3DGroup transformGroup =camera.Transform as Transform3DGroup ;
			transformGroup.Children.Clear () ;
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