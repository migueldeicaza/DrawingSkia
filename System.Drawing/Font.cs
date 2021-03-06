using System;
using System.Runtime.Serialization;

namespace System.Drawing
{

	public sealed partial class Font : MarshalByRefObject, ISerializable, ICloneable, IDisposable 
	{
	
		const byte DefaultCharSet = 1;

		float sizeInPoints = 0;
		GraphicsUnit unit = GraphicsUnit.Point;
		float size;
		bool underLine = false;
		bool strikeThrough = false;
		FontFamily fontFamily;
		FontStyle fontStyle;
		byte gdiCharSet = 1;
		bool  gdiVerticalFont;

		static float dpiScale = 96f / 72f;

		public Font (FontFamily family, float emSize,  GraphicsUnit unit)
			: this (family, emSize, FontStyle.Regular, unit, DefaultCharSet, false)
		{
		}

		public Font (string familyName, float emSize,  GraphicsUnit unit)
			: this (new FontFamily (familyName, true), emSize, FontStyle.Regular, unit, DefaultCharSet, false)
		{
		}

		public Font (FontFamily family, float emSize)
			: this (family, emSize, FontStyle.Regular, GraphicsUnit.Point, DefaultCharSet, false)
		{
		}

		public Font (FontFamily family, float emSize, FontStyle style)
			: this (family, emSize, style, GraphicsUnit.Point, DefaultCharSet, false)
		{
		}

		public Font (FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
			: this (family, emSize, style, unit, DefaultCharSet, false)
		{
		}

		public Font (FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
			: this (family, emSize, style, unit, gdiCharSet, false)
		{
		}

		public Font (string familyName, float emSize)
			: this (new FontFamily (familyName, true), emSize, FontStyle.Regular, GraphicsUnit.Point, DefaultCharSet, false)
		{
		}

		public Font (string familyName, float emSize, FontStyle style)
			: this (new FontFamily (familyName, true), emSize, style, GraphicsUnit.Point, DefaultCharSet, false)
		{
		}

		public Font (string familyName, float emSize, FontStyle style, GraphicsUnit unit)
			: this (new FontFamily (familyName, true), emSize, style, unit, DefaultCharSet, false)
		{
		}

		public Font (string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
			: this (new FontFamily (familyName, true), emSize, style, unit, gdiCharSet, false)
		{
		}

		public Font (string familyName, float emSize, FontStyle style,
		           GraphicsUnit unit, byte gdiCharSet, bool  gdiVerticalFont)
			: this (new FontFamily (familyName, true), emSize, style, unit, gdiCharSet, gdiVerticalFont)
		{
		}

		public Font (FontFamily familyName, float emSize, FontStyle style,
		             GraphicsUnit unit, byte gdiCharSet, bool  gdiVerticalFont )
		{


			if (emSize <= 0)
				throw new ArgumentException("emSize is less than or equal to 0, evaluates to infinity, or is not a valid number.","emSize");

			fontFamily = familyName;
			fontStyle = style;
			this.gdiVerticalFont = gdiVerticalFont;
			this.gdiCharSet = gdiCharSet;

#if TODO
			CreateNativeFont (familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
#else
			throw new NotImplementedException ();
#endif

		}

		#region ISerializable implementation
		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region ICloneable implementation
		public object Clone ()
		{
			return new Font (fontFamily, size, fontStyle, unit, gdiCharSet, gdiVerticalFont);
		}
		#endregion

		#region IDisposable implementation
		~Font ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
		}
		
		internal void Dispose (bool disposing)
		{
			if (disposing){
				#if TODO
				if (nativeFont != null){
					nativeFont.Dispose ();
					nativeFont = null;
				}
				#endif
			}
		}
		
#endregion
		
		public float SizeInPoints 
		{
			get { return sizeInPoints; }
		}
		
		public GraphicsUnit Unit 
		{
			get { return unit; }
			
		}
		
		public float Size 
		{
			get { 
				return size; 
			}
			
		}

		public bool Bold 
		{ 
			get { return bold; }
		}

		public bool Italic
		{ 
			get { return italic; }
		}

		public bool Underline
		{ 
			get { return underLine; }
		}

		public bool Strikeout
		{ 
			get { return strikeThrough; }
		}

		public int Height {
			get { return (int)Math.Round (GetHeight ()); }
		}

		public FontFamily FontFamily
		{
			get { return fontFamily; }
		}

		public FontStyle Style 
		{ 
			get { return fontStyle; }
		}

		public float GetHeight() 
		{
			return GetNativeheight ();
		}
	}
}

