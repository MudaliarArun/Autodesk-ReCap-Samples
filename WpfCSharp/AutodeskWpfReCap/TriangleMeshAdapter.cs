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
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;

namespace AutodeskWpfReCap {

	public class ObjMaterialStreamProvider : IMaterialStreamProvider {
		public ZipArchiveEntry _mtl ;

		public ObjMaterialStreamProvider (ZipArchiveEntry mtl) {
			_mtl =mtl ;
		}

		public Stream Open (string materialFilePath) {
			return (_mtl.Open ()) ;
		}

	}
	
	// Utility Class for converting data containing a Maya MFnMesh into an object that is compatible
	// with the Windows Presentation framework. 
	public class TriangleMeshAdapater {
		public Point3DCollection Points ;
		public Vector3DCollection Normals ;
		public Int32Collection Indices ;
		public PointCollection TexCoords ;

		//public TriangleMeshAdapater (LoadResult objmesh) {
		//	Points =new Point3DCollection (objmesh.Vertices.Count) ;
		//	foreach ( ObjLoader.Loader.Data.VertexData.Vertex vertex in objmesh.Vertices )
		//		Points.Add (new Point3D (vertex.X, vertex.Y, vertex.Z)) ;

		//	Normals =new Vector3DCollection () ;

		//	Indices =new Int32Collection () ;
		//	foreach ( Face face in objmesh.Groups [0].Faces ) {
		//		if ( face.Count != 3 )
		//			throw new NotImplementedException ("Only triangles are supported");
		//		for ( int i =0 ; i < face.Count ; i++ ) {
		//			var faceVertex =face [i] ;
		//			Indices.Add (faceVertex.VertexIndex - 1) ;
		//		}
		//	}

		//	TexCoords =new PointCollection (Indices.Count) ;
		//	foreach ( ObjLoader.Loader.Data.VertexData.Texture t in objmesh.Textures )
		//		TexCoords.Add (new System.Windows.Point (t.X, t.Y)) ;
		//}

		public TriangleMeshAdapater (LoadResult objmesh) {
			HashSet<string> uniqPairs =new HashSet<string> () ;
			List<Face> newFaces =new List<Face> () ;
			foreach ( Face face in objmesh.Groups [0].Faces ) {
				// Create vertex/tex pairs list
				for ( int i =0 ; i < face.Count ; i++ ) {
					FaceVertex fv =face [i] ;
					string pairName =string.Format ("{0}/{1}", fv.VertexIndex, fv.TextureIndex < 0 ? 0 : fv.TextureIndex) ;
					uniqPairs.Add (pairName) ;
				}
				// Split quads into triangles
				if ( face.Count == 4 ) { // a quad
					//throw new NotImplementedException ("Face needs to be triangulated!"); 
					Face glface =new Face () ;
					glface.AddVertex (new FaceVertex (face [0].VertexIndex, face [0].TextureIndex, face [0].NormalIndex)) ;
					glface.AddVertex (new FaceVertex (face [2].VertexIndex, face [2].TextureIndex, face [2].NormalIndex)) ;
					glface.AddVertex (new FaceVertex (face [3].VertexIndex, face [3].TextureIndex, face [3].NormalIndex)) ;
					// Added the following in Face.cs
					//public void RemoveVertexAt (int index) { _vertices.RemoveAt (index); }
					face.RemoveVertexAt (3) ;
					newFaces.Add (glface) ;
				} else if ( face.Count > 4 ) {
					throw new NotImplementedException ("Face needs to be triangulated!") ;
				}
			}
			((List<Face>)(objmesh.Groups [0].Faces)).AddRange (newFaces) ;
			
			// Build OpenGL vertex / tex arrrays
			int nbPairs =uniqPairs.Count ;
			string [] pairs =new string [nbPairs] ;
			uniqPairs.CopyTo (pairs) ;
			Points =new Point3DCollection (nbPairs) ;
			TexCoords =new PointCollection (nbPairs) ;
			foreach ( string pairName in pairs ) {
				string [] def =pairName.Split ('/') ;
				ObjLoader.Loader.Data.VertexData.Vertex vertex =objmesh.Vertices [Convert.ToInt32 (def [0]) - 1] ;
				Points.Add (new Point3D (vertex.X, vertex.Y, vertex.Z)) ;
				ObjLoader.Loader.Data.VertexData.Texture t =objmesh.Textures [Convert.ToInt32 (def [1]) == 0 ? 0 : Convert.ToInt32 (def [1]) - 1] ;
				TexCoords.Add (new System.Windows.Point (t.X, 1.0 - t.Y)) ;
				//System.Diagnostics.Debug.Print ("{0}\t- {1},\t{2},\t{3}\t- {4}\t{5}", Points.Count, vertex.X, vertex.Y, vertex.Z, t.X, t.Y) ;
			}
			//System.Diagnostics.Debug.Print (" ") ;

			Normals =new Vector3DCollection () ;
			Indices =new Int32Collection () ;
			foreach ( Face face in objmesh.Groups [0].Faces ) {
				for ( int i =0 ; i < face.Count ; i++ ) {
					FaceVertex fv =face [i] ;
					string pairName =string.Format ("{0}/{1}", fv.VertexIndex, fv.TextureIndex < 0 ? 0 : fv.TextureIndex) ;
					int index =Array.IndexOf (pairs, pairName) ;
					Indices.Add (index) ;
					//System.Diagnostics.Debug.Print ("{0}\t/{1}\t= {2}", i, pairName, index) ;
				}
			}
		}

	}

}
