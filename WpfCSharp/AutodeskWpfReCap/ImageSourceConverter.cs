﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Resources;

namespace AutodeskWpfReCap {

	public class ImageSourceConverter : IValueConverter {

		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			if ( value == null || !(value is string) || value == @"Images\ReCap.jpg"
				|| File.Exists (value as string)
				|| (value as string).Substring (0, 4).ToLower () == "http"
				|| (value as string).Substring (0, 3).ToLower () == "ftp"
			)
				return (value) ;

			string [] sts =(value as string).Split (':') ;
			BitmapImage bitmap =null ;
			if ( File.Exists (sts[0]) ) {
				FileStream zipStream =File.OpenRead (sts [0]) ;
				using ( ZipArchive zip =new ZipArchive (zipStream) ) {
					ZipArchiveEntry icon =zip.GetEntry (sts [1]) ;
					Stream imgStream =icon.Open () ;
					Byte [] buffer =new Byte [icon.Length] ;
					imgStream.Read (buffer, 0, buffer.Length) ;
					var byteStream =new System.IO.MemoryStream (buffer) ;
					bitmap =new BitmapImage () ;
					bitmap.BeginInit () ;
					bitmap.CacheOption =BitmapCacheOption.OnLoad ;
					bitmap.StreamSource =byteStream ;
					bitmap.EndInit () ;
				}
			}
			return (AutoCropBitmap (bitmap)) ;
		}

		public object ConvertBack (object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			return (value) ;
		}

		// http://wpftutorial.net/Images.html
		public static ImageSource AutoCropBitmap (BitmapSource source) {
			if ( source == null )
				throw new ArgumentException ("source") ;

			if ( source.Format != PixelFormats.Bgra32 )
				source =new FormatConvertedBitmap (source, PixelFormats.Bgra32, null, 0) ;

			int width =source.PixelWidth ;
			int height =source.PixelHeight ;
			int bytesPerPixel =source.Format.BitsPerPixel / 8 ;
			int stride =width * bytesPerPixel ;

			var pixelBuffer =new byte [height * stride] ;
			source.CopyPixels (pixelBuffer, stride, 0) ;

			int cropTop =height, cropBottom =0, cropLeft =width, cropRight =0 ;
			for ( int y =0 ; y < height ; y++ ) {
				for ( int x =0 ; x < width ; x++ ) {
					int offset =(y * stride + x * bytesPerPixel) ;
					byte blue =pixelBuffer [offset] ;
					byte green =pixelBuffer [offset + 1] ;
					byte red =pixelBuffer [offset + 2] ;
					byte alpha =pixelBuffer [offset + 3] ;

					// Define a threshold when a pixel has a content
					bool hasContent =alpha > 10 ;
					if ( hasContent ) {
						cropLeft =Math.Min (x, cropLeft) ;
						cropRight =Math.Max (x, cropRight) ;
						cropTop =Math.Min (y, cropTop) ;
						cropBottom =Math.Max (y, cropBottom) ;
					}
				}
			}
			return (new CroppedBitmap (source, new Int32Rect (cropLeft, cropTop, cropRight - cropLeft, cropBottom - cropTop))) ;
		}

	}

}

