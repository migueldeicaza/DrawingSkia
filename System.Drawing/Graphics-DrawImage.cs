using System;
using System.Drawing.Imaging;
using SkiaSharp;

namespace System.Drawing
{
	public partial class Graphics {
		public delegate bool DrawImageAbort (IntPtr callbackData);
#if TODO
		private CIContext ciContext;

		private void DrawImage(RectangleF rect, CGImage image, CGAffineTransform transform)
		{
			var trans = transform;
			// Do our translation on the image transform
			trans.Translate (rect.X, rect.Height - image.Height + rect.Y);

			// The translation is already taken care of in the transform
			rect.Y = 0;
			rect.X = 0;

			// Apply our transform to the context
			context.ConcatCTM (trans);

			// we are getting an error somewhere and not sure where
			// I think the image bitmapBlock is being corrupted somewhere
			try {
				context.DrawImage (rect.ToCGRect (), image);
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.Message);
			}

			// Now we revert our image transform from the context 
			var revert = CGAffineTransform.CGAffineTransformInvert (trans);
			context.ConcatCTM (revert);
		}


		/// <summary>
		/// Draws the specified Image at the specified location and with the specified size.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="rect">Rect.</param>
		public void DrawImage (Image image, RectangleF rect)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

				DrawImage (rect, image.NativeCGImage, image.imageTransform);

		}

		/// <summary>
		/// Draws the specified Image, using its original physical size, at the specified location.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="point">Point.</param>
		public void DrawImage (Image image, PointF point)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage(image, point.X, point.Y);
		}

		/// <summary>
		/// Draws the specified Image at the specified location and with the specified shape and size.
		/// 
		/// The destPoints parameter specifies three points of a parallelogram. The three Point structures 
		/// represent the upper-left, upper-right, and lower-left corners of the parallelogram. The fourth 
		/// point is extrapolated from the first three to form a parallelogram.  
		/// 
		/// The image represented by the image parameter is scaled and sheared to fit the shape of the 
		/// parallelogram specified by the destPoints parameters.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destPoints">Destination points.</param>
		public void DrawImage (Image image, Point [] destPoints)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");

			if (destPoints.Length < 3)
				throw new ArgumentException ("Destination points must be an array with a length of 3 or 4. " +
					"A length of 3 defines a parallelogram with the upper-left, upper-right, " +
					"and lower-left corners. A length of 4 defines a quadrilateral with the " +
					"fourth element of the array specifying the lower-right coordinate.");

			// Windows throws a Not Implemented error if the points are more than 3
			if (destPoints.Length > 3)
				throw new NotImplementedException ();

			var destPointsF = new PointF[destPoints.Length];
			for (int p = 0; p < destPoints.Length; p++)
				destPointsF [p] = destPoints [p];

			DrawImage (image, destPointsF);
		}

		/// <summary>
		/// Draws the specified Image, using its original physical size, at the specified location.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="point">Point.</param>
		public void DrawImage (Image image, Point point)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			DrawImage (image, point.X, point.Y);
		}

		/// <summary>
		/// Draws the specified Image at the specified location and with the specified size.
		/// 
		/// The image represented by the image object is scaled to the dimensions of the rect rectangle.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="rect">Rect.</param>
		public void DrawImage (Image image, Rectangle rect)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			DrawImage (image, (RectangleF)rect);
		}

		/// <summary>
		/// Draws the specified Image at the specified location and with the specified shape and size.
		/// 
		/// The destPoints parameter specifies three points of a parallelogram. The three PointF structures 
		/// represent the upper-left, upper-right, and lower-left corners of the parallelogram. The fourth point 
		/// is extrapolated from the first three to form a parallelogram.
		/// 
		/// The image represented by the image object is scaled and sheared to fit the shape of the parallelogram 
		/// specified by the destPoints parameter.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destPoints">Destination points.</param>
		public void DrawImage (Image image, PointF [] destPoints)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");
			if (destPoints.Length < 3)
				throw new ArgumentException ("Destination points must be an array with a length of 3 or 4. " +
				                             "A length of 3 defines a parallelogram with the upper-left, upper-right, " +
				                             "and lower-left corners. A length of 4 defines a quadrilateral with the " +
				                             "fourth element of the array specifying the lower-right coordinate.");

			// Windows throws a Not Implemented error if the points are more than 3
			if (destPoints.Length > 3)
				throw new NotImplementedException ();

			// create our rectangle.  Offset is 0 because the CreateGeometricTransform bakes our x,y offset in there.
			var rect = new RectangleF (0,0, destPoints [1].X - destPoints [0].X, destPoints [2].Y - destPoints [0].Y);

			// We need to flip our Y axis so the image appears right side up
			var geoTransform = new CGAffineTransform (1, 0, 0, -1, 0, rect.Height);
			//var geott = GeomUtilities.CreateGeometricTransform (rect, destPoints);
			geoTransform.Multiply (GeomUtilities.CreateGeometricTransform (rect, destPoints));

			// Apply our transform to the context
			context.ConcatCTM (geoTransform);

			// now we draw our image.
			context.DrawImage(rect.ToCGRect (), image.NativeCGImage);

			// Now we revert our image transform from the context 
			var revert = CGAffineTransform.CGAffineTransformInvert (geoTransform);
			context.ConcatCTM (revert);

		}

		/// <summary>
		/// Draws the specified image, using its original physical size, at the location specified by a coordinate pair.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void DrawImage (Image image, int x, int y)
		{
			var width = image.physicalSize.Width;
			var height = image.physicalSize.Height;

			if (graphicsUnit != GraphicsUnit.Pixel) 
			{
				width = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.HorizontalResolution, width);
				height = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.VerticalResolution, height);
			}

			DrawImage (image, new RectangleF(x,y,width,height));
		}

		/// <summary>
		/// Draws the specified image, using its original physical size, at the location specified by a coordinate pair.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void DrawImage (Image image, float x, float y)
		{
			var width = image.physicalSize.Width;
			var height = image.physicalSize.Height;

			if (graphicsUnit != GraphicsUnit.Pixel) 
			{
				width = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.HorizontalResolution, width);
				height = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.VerticalResolution, height);
			}
			DrawImage (image, new RectangleF(x,y,width,height));
		}

		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The srcRect parameter specifies a rectangular portion of the image object to draw. This portion is scaled 
		/// to fit inside the rectangle specified by the destRect parameter.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destRect">Destination rect.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage (image, (RectangleF)destRect, (RectangleF)srcRect, srcUnit);

		}

		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The srcRect parameter specifies a rectangular portion of the image object to draw. This portion is scaled 
		/// up or down (in the case where source rectangle overruns the bounds of the image) to fit inside the rectangle 
		/// specified by the destRect parameter.  
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destRect">Destination rect.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
		{			
			if (image == null)
				throw new ArgumentNullException ("image");

			var srcRect1 = srcRect;

			// If the source units are not the same we need to convert them
			// The reason we check for Pixel here is that our graphics already has the Pixel's baked into the model view transform
			if (srcUnit != graphicsUnit && srcUnit != GraphicsUnit.Pixel) 
			{
				ConversionHelpers.GraphicsUnitConversion (srcUnit, graphicsUnit, image.HorizontalResolution, image.VerticalResolution,  ref srcRect1);
			} 

			// Obtain the subImage
			var subImage = image.NativeCGImage.WithImageInRect (srcRect1. ToCGRect ());

			// If we do not have anything to draw then we exit here
			if (subImage.Width == 0 || subImage.Height == 0)
				return;

			var transform = image.imageTransform;
			// Reset our height on the transform to account for subImage
			transform.y0 = subImage.Height;

			// Make sure we scale the image in case the source rectangle
			// overruns our subimage bouncs width and/or height
			float scaleX = subImage.Width/srcRect1.Width;
			float scaleY = subImage.Height/srcRect1.Height;
			transform.Scale (scaleX, scaleY);

			// Now draw our image
			DrawImage (destRect, subImage, transform);

		}


		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The destPoints specifies a parallelogram with the first point specifying the upper left corner, 
		/// second point specifying the upper right corner and the third point specifying the lower left corner.
		/// 
		/// The srcRect parameter specifies a rectangular portion of the image object to draw. This portion is scaled 
		/// up or down (in the case where source rectangle overruns the bounds of the image) to fit inside the rectangle 
		/// specified by the destRect parameter.  
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destPoints">Destination points.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, Point [] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");

			PointF[] pointfs = new PointF[destPoints.Length];
			for (var p = 0; p < pointfs.Length; p++)
				pointfs [p] = destPoints [p];

			DrawImage (image, pointfs, (RectangleF)srcRect, srcUnit);

		}

		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The destPoints specifies a parallelogram with the first point specifying the upper left corner, 
		/// second point specifying the upper right corner and the third point specifying the lower left corner.
		/// 
		/// The srcRect parameter specifies a rectangular portion of the image object to draw. This portion is scaled 
		/// up or down (in the case where source rectangle overruns the bounds of the image) to fit inside the rectangle 
		/// specified by the destRect parameter.  
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destPoints">Destination points.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, PointF [] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");

			if (destPoints.Length < 3)
				throw new ArgumentException ("Destination points must be an array with a length of 3 or 4. " +
				                             "A length of 3 defines a parallelogram with the upper-left, upper-right, " +
				                             "and lower-left corners. A length of 4 defines a quadrilateral with the " +
				                             "fourth element of the array specifying the lower-right coordinate.");

			// Windows throws a Not Implemented error if the points are more than 3
			if (destPoints.Length > 3)
				throw new NotImplementedException ();

			var srcRect1 = srcRect;

			// If the source units are not the same we need to convert them
			// The reason we check for Pixel here is that our graphics already has the Pixel's baked into the model view transform
			if (srcUnit != graphicsUnit && srcUnit != GraphicsUnit.Pixel) 
			{
				ConversionHelpers.GraphicsUnitConversion (srcUnit, graphicsUnit, image.HorizontalResolution, image.VerticalResolution,  ref srcRect1);
			} 

			// Obtain the subImage
			var subImage = image.NativeCGImage.WithImageInRect (srcRect1.ToCGRect ());

			// If we do not have anything to draw then we exit here
			if (subImage.Width == 0 || subImage.Height == 0)
				return;

			// create our rectangle.  Offset is 0 because the CreateGeometricTransform bakes our x,y offset in there.
			var rect = new RectangleF (0,0, destPoints [1].X - destPoints [0].X, destPoints [2].Y - destPoints [0].Y);

			// We need to flip our Y axis so the image appears right side up
			var geoTransform = new CGAffineTransform (1, 0, 0, -1, 0, rect.Height);

			// Make sure we scale the image in case the source rectangle
			// overruns our subimage bounds (width and/or height)
			float scaleX = subImage.Width/srcRect1.Width;
			float scaleY = subImage.Height/srcRect1.Height;
			geoTransform.Scale (scaleX, scaleY);

			//var geott = GeomUtilities.CreateGeometricTransform (rect, destPoints);
			geoTransform.Multiply (GeomUtilities.CreateGeometricTransform (rect, destPoints));

			// Apply our transform to the context
			context.ConcatCTM (geoTransform);

			// now we draw our image.
			context.DrawImage(rect.ToCGRect (), subImage);

			// Now we revert our image transform from the context 
			var revert = CGAffineTransform.CGAffineTransformInvert (geoTransform);
			context.ConcatCTM (revert);


		}

		public void DrawImage (Image image, Point [] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, 
                                ImageAttributes imageAttr)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImagePointsRectI (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y,
			//	srcRect.Width, srcRect.Height, srcUnit,
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, null, IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}
		
		public void DrawImage (Image image, float x, float y, float width, float height)
		{
			DrawImage (image, new RectangleF (x, y, width, height));
		}

		public void DrawImage (Image image, PointF [] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, 
                                ImageAttributes imageAttr)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImagePointsRect (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y,
			//	srcRect.Width, srcRect.Height, srcUnit, 
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, null, IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}

		/// <summary>
		/// Draws a portion of an image at a specified location.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
		{			
			if (image == null)
				throw new ArgumentNullException ("image");

			var width = image.physicalSize.Width;
			var height = image.physicalSize.Height;

			if (graphicsUnit != GraphicsUnit.Pixel) 
			{
				width = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.HorizontalResolution, width);
				height = ConversionHelpers.GraphicsUnitConversion (GraphicsUnit.Pixel, graphicsUnit, image.VerticalResolution, height);
			}

			DrawImage (image, new RectangleF (x, y, width, height), srcRect, srcUnit);
		}

		/// <summary>
		/// Draws the specified Image at the specified location and with the specified size.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage(image, new RectangleF(x,y,width, height));
		}

		/// <summary>
		/// Draws a portion of an image at a specified location.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="srcRect">Source rect.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
		{			
			if (image == null)
				throw new ArgumentNullException ("image");

			var srcRect1 = srcRect;

			// If the source units are not the same we need to convert them
			// The reason we check for Pixel here is that our graphics already has the Pixel's baked into the model view transform
			if (srcUnit != graphicsUnit && srcUnit != GraphicsUnit.Pixel) 
			{
				ConversionHelpers.GraphicsUnitConversion (srcUnit, graphicsUnit, image.VerticalResolution, image.HorizontalResolution, ref srcRect1);
			}

			DrawImage (image, new RectangleF (x, y, srcRect1.Width, srcRect1.Height), srcRect1, graphicsUnit);		
		}

		public void DrawImage (Image image, PointF [] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImagePointsRect (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y,
			//	srcRect.Width, srcRect.Height, srcUnit, 
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, callback, IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}

		public void DrawImage (Image image, Point [] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");
			
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImagePointsRectI (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y,
			//	srcRect.Width, srcRect.Height, srcUnit, 
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, callback, IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}

		public void DrawImage (Image image, Point [] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			if (destPoints == null)
				throw new ArgumentNullException ("destPoints");

			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImagePointsRectI (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y, 
			//	srcRect.Width, srcRect.Height, srcUnit, 
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, callback, (IntPtr) callbackData);
			//GDIPlus.CheckStatus (status);
		}

		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The parameters srcX, srcY, srcWidth and srcHeight define the rectangular source portion of the image object 
		/// to draw. This portion is scaled up or down (in the case where source rectangle overruns the bounds of the image)
		/// to fit inside the rectangle specified by the destRect parameter.  
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destRect">Destination rect.</param>
		/// <param name="srcX">Source x.</param>
		/// <param name="srcY">Source y.</param>
		/// <param name="srcWidth">Source width.</param>
		/// <param name="srcHeight">Source height.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage (image, (RectangleF)destRect, new RectangleF (srcX, srcY, srcWidth, srcHeight), srcUnit);
		}
		
		public void DrawImage (Image image, PointF [] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
		{
			//Status status = GDIPlus.GdipDrawImagePointsRect (nativeObject, image.NativeObject,
			//	destPoints, destPoints.Length , srcRect.X, srcRect.Y,
			//	srcRect.Width, srcRect.Height, srcUnit, 
			//	imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, callback, (IntPtr) callbackData);
			//GDIPlus.CheckStatus (status);
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Draws the specified portion of the specified Image at the specified location and with the specified size.
		/// 
		/// The parameters srcX, srcY, srcWidth and srcHeight define the rectangular source portion of the image object 
		/// to draw. This portion is scaled up or down (in the case where source rectangle overruns the bounds of the image)
		/// to fit inside the rectangle specified by the destRect parameter.  
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="destRect">Destination rect.</param>
		/// <param name="srcX">Source x.</param>
		/// <param name="srcY">Source y.</param>
		/// <param name="srcWidth">Source width.</param>
		/// <param name="srcHeight">Source height.</param>
		/// <param name="srcUnit">Source unit.</param>
		public void DrawImage (Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage (image, destRect, new Rectangle (srcX, srcY, srcWidth, srcHeight), srcUnit);
		}

		public void DrawImage (Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			var srcRect1 = new RectangleF(srcX, srcY,srcWidth,srcHeight);

			// If the source units are not the same we need to convert them
			// The reason we check for Pixel here is that our graphics already has the Pixel's baked into the model view transform
			if (srcUnit != graphicsUnit && srcUnit != GraphicsUnit.Pixel) 
			{
				ConversionHelpers.GraphicsUnitConversion (srcUnit, graphicsUnit, image.HorizontalResolution, image.VerticalResolution,  ref srcRect1);
			} 

			// Obtain the subImage
			var subImage = image.NativeCGImage.WithImageInRect (srcRect1.ToCGRect ());

			// If we do not have anything to draw then we exit here
			if (subImage.Width == 0 || subImage.Height == 0)
				return;

//			var transform = image.imageTransform;
////			// Reset our height on the transform to account for subImage
//			transform.y0 = subImage.Height;
////
////			// Make sure we scale the image in case the source rectangle
////			// overruns our subimage bouncs width and/or height
//			float scaleX = subImage.Width/srcRect1.Width;
//			float scaleY = subImage.Height/srcRect1.Height;
//			transform.Scale (scaleX, scaleY);
			bool attributesSet = imageAttrs.isColorMatrixSet || imageAttrs.isGammaSet;

			if (attributesSet) {
				InitializeImagingContext ();
				CIImage result = subImage;

				if (imageAttrs.isColorMatrixSet) {

					var ciFilter = CIFilter.FromName ("CIColorMatrix");
					ciFilter.SetDefaults ();

					ciFilter.SetValueForKey (result, new NSString ("inputImage"));

					var inputRVector = new CIVector (imageAttrs.colorMatrix.Matrix00, imageAttrs.colorMatrix.Matrix01, imageAttrs.colorMatrix.Matrix02, imageAttrs.colorMatrix.Matrix03);
					var inputGVector = new CIVector (imageAttrs.colorMatrix.Matrix10, imageAttrs.colorMatrix.Matrix11, imageAttrs.colorMatrix.Matrix12, imageAttrs.colorMatrix.Matrix13);
					var inputBVector = new CIVector (imageAttrs.colorMatrix.Matrix20, imageAttrs.colorMatrix.Matrix21, imageAttrs.colorMatrix.Matrix22, imageAttrs.colorMatrix.Matrix23);
					var inputAVector = new CIVector (imageAttrs.colorMatrix.Matrix30, imageAttrs.colorMatrix.Matrix31, imageAttrs.colorMatrix.Matrix32, imageAttrs.colorMatrix.Matrix33);
					var inputBiasVector = new CIVector (imageAttrs.colorMatrix.Matrix40, imageAttrs.colorMatrix.Matrix41, imageAttrs.colorMatrix.Matrix42, imageAttrs.colorMatrix.Matrix43);

					ciFilter.SetValueForKey (inputRVector, new NSString ("inputRVector"));
					ciFilter.SetValueForKey (inputGVector, new NSString ("inputGVector"));
					ciFilter.SetValueForKey (inputBVector, new NSString ("inputBVector"));
					ciFilter.SetValueForKey (inputAVector, new NSString ("inputAVector"));
					ciFilter.SetValueForKey (inputBiasVector, new NSString ("inputBiasVector"));
					result = (CIImage)ciFilter.ValueForKey (new NSString ("outputImage"));
				}

				if (imageAttrs.isGammaSet) {

					var ciFilter = CIFilter.FromName ("CIGammaAdjust");
					ciFilter.SetDefaults ();

					ciFilter.SetValueForKey (result, new NSString ("inputImage"));

					var inputPower = NSNumber.FromFloat (imageAttrs.gamma);

					ciFilter.SetValueForKey (inputPower, new NSString ("inputPower"));
					result = (CIImage)ciFilter.ValueForKey (new NSString ("outputImage"));
				}


				subImage = ciContext.CreateCGImage (result, result.Extent);
			}

			transform = image.imageTransform;
			transform.y0 = subImage.Height;
			float scaleX1 = subImage.Width/srcRect1.Width;
			float scaleY1 = subImage.Height/srcRect1.Height;
			transform.Scale (scaleX1, scaleY1);
			// Now draw our image
			DrawImage (destRect, subImage, transform);

		}

		public void DrawImage (Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{			
			if (image == null)
				throw new ArgumentNullException ("image");

			DrawImage (image, destRect, (float)srcX, (float)srcY, (float)srcWidth, (float)srcHeight, srcUnit, imageAttr);
		}
		
		public void DrawImage (Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImageRectRectI (nativeObject, image.NativeObject, 
            //                            destRect.X, destRect.Y, destRect.Width, 
			//		destRect.Height, srcX, srcY, srcWidth, srcHeight,
			//		srcUnit, imageAttr != null ? imageAttr.NativeObject : IntPtr.Zero, callback,
			//		IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}
		
		public void DrawImage (Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImageRectRect (nativeObject, image.NativeObject, 
            //                            destRect.X, destRect.Y, destRect.Width, 
			//		destRect.Height, srcX, srcY, srcWidth, srcHeight,
			//		srcUnit, imageAttrs != null ? imageAttrs.NativeObject : IntPtr.Zero, 
			//		callback, IntPtr.Zero);
			//GDIPlus.CheckStatus (status);
		}

		public void DrawImage (Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImageRectRect (nativeObject, image.NativeObject, 
			//	destRect.X, destRect.Y, destRect.Width, destRect.Height,
			//	srcX, srcY, srcWidth, srcHeight, srcUnit, 
			//	imageAttrs != null ? imageAttrs.NativeObject : IntPtr.Zero, callback, callbackData);
			//GDIPlus.CheckStatus (status);
		}

		public void DrawImage (Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			throw new NotImplementedException ();
			//Status status = GDIPlus.GdipDrawImageRectRect (nativeObject, image.NativeObject, 
            //           		destRect.X, destRect.Y, destRect.Width, destRect.Height,
			//	srcX, srcY, srcWidth, srcHeight, srcUnit,
			//	imageAttrs != null ? imageAttrs.NativeObject : IntPtr.Zero, callback, callbackData);
			//GDIPlus.CheckStatus (status);
		}		
		public void DrawImageUnscaled (Image image, Point point)
		{
			DrawImageUnscaled (image, point.X, point.Y);
		}
		
		public void DrawImageUnscaled (Image image, Rectangle rect)
		{
			DrawImageUnscaled (image, rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void DrawImageUnscaled (Image image, int x, int y)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			DrawImage (image, x, y, image.Width, image.Height);
		}

		public void DrawImageUnscaled (Image image, int x, int y, int width, int height)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			// avoid creating an empty, or negative w/h, bitmap...
			if ((width <= 0) || (height <= 0))
				return;

			using (Image tmpImg = new Bitmap (width, height)) {
				using (Graphics g = FromImage (tmpImg)) {
					g.DrawImage (image, 0, 0, image.Width, image.Height);
					DrawImage (tmpImg, x, y, width, height);
				}
			}
		}

		public void DrawImageUnscaledAndClipped (Image image, Rectangle rect)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			int width = (image.Width > rect.Width) ? rect.Width : image.Width;
			int height = (image.Height > rect.Height) ? rect.Height : image.Height;

			DrawImageUnscaled (image, rect.X, rect.Y, width, height);			
		}

		private void InitializeImagingContext ()
		{
#if MONOTOUCH
			if (ciContext == null)
				ciContext = CIContext.FromOptions(null);
#else
			if (ciContext == null)
				ciContext = CIContext.FromContext (context);
#endif
		}
	#endif
	}
}

