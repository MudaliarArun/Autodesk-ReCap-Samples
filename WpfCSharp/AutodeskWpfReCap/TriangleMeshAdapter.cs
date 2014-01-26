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

		public TriangleMeshAdapater (LoadResult objmesh) {
			Points =new Point3DCollection (objmesh.Vertices.Count) ;
			foreach ( ObjLoader.Loader.Data.VertexData.Vertex vertex in objmesh.Vertices )
				Points.Add (new Point3D (vertex.X, vertex.Y, vertex.Z)) ;

			Normals =new Vector3DCollection () ;

			Indices =new Int32Collection () ;
			foreach ( Face face in objmesh.Groups [0].Faces ) {
				if ( face.Count != 3 )
					throw new NotImplementedException ("Only triangles are supported");
				for ( int i =0 ; i < face.Count ; i++ ) {
					var faceVertex =face [i] ;
					Indices.Add (faceVertex.VertexIndex - 1) ;
				}
			}

			TexCoords =new PointCollection (Indices.Count) ;
			foreach ( ObjLoader.Loader.Data.VertexData.Texture t in objmesh.Textures )
				TexCoords.Add (new System.Windows.Point (t.X, t.Y)) ;
		}

	}

}
