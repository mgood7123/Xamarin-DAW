using System;
using SkiaSharp;

namespace Xamarin_DAW.Skia_UI_Kit
{
    public static class SKCanvasExtensions
	{
		/// <summary>Returns true if this canvas is Hardware Accelerated.</summary>
		public static bool isHardwareAccelerated(this SKCanvas this_canvas)
		{
			return (bool)this_canvas.ExtensionProperties_GetValue("HardwareAccelerated", false);
		}

		/// <summary>Creates a Hardware Accelerated canvas.</summary>
		public static SKCanvas CreateHardwareAcceleratedCanvas(this SKCanvas this_canvas, GRContext context, int width, int height)
		{
			SKSurface s = SKSurface.Create(context, false, new SKImageInfo(width, height));
			SKCanvas c = s.Canvas;
			c.ExtensionProperties_SetValue("GRContext", context);
			c.ExtensionProperties_SetValue("Width", width);
			c.ExtensionProperties_SetValue("Height", height);
			c.ExtensionProperties_SetValue("Surface", s);
			c.ExtensionProperties_SetValue("HardwareAccelerated", true);
			return c;
		}

		/// <summary>Creates a software canvas, this is not Hardware Accelerated.</summary>
		public static SKCanvas CreateSoftwareCanvas(this SKCanvas this_canvas, int width, int height)
		{
            SKCanvas c = new SKCanvas(new SKBitmap(width, height));
			c.ExtensionProperties_SetValue("Width", width);
			c.ExtensionProperties_SetValue("Height", height);
			return c;
		}

		/// <summary>Creates a canvas, whether the created canvas is Hardware Accelerated or not depends
        /// on the canvas of which this method is invoked from.</summary>
		public static SKCanvas CreateCanvas(this SKCanvas this_canvas, int width, int height)
		{
			if (this_canvas.isHardwareAccelerated())
            {
				GRContext context = (GRContext)this_canvas.ExtensionProperties_GetValue("GRContext", null);
				if (context == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no GRContext");
				return this_canvas.CreateHardwareAcceleratedCanvas(context, width, height);
            }
			else
            {
				return this_canvas.CreateSoftwareCanvas(width, height);
            }
		}

		public static int Width(this SKCanvas this_canvas)
        {
			return (int)this_canvas.ExtensionProperties_GetValue("Width", this_canvas.LocalClipBounds.Width);
		}

		public static int Height(this SKCanvas this_canvas)
		{
			return (int)this_canvas.ExtensionProperties_GetValue("Height", this_canvas.LocalClipBounds.Height);
		}

		/// <summary>Draws a canvas on this canvas.</summary>
		public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, 0, 0, paint);

		/// <summary>Draws a canvas on this canvas.</summary>
		public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, int x, int y, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, x, y, paint);

		/// <summary>Draws a canvas on this canvas.</summary>
		public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPoint point, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, point, paint);

		/// <summary>Draws this canvas to a canvas.</summary>
		public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPaint paint = null) => this_canvas.DrawToCanvas(canvas, 0, 0, paint);

        /// <summary>Draws this canvas to a canvas.</summary>
        public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, int x, int y, SKPaint paint = null)
		{
			if (this_canvas.isHardwareAccelerated())
			{
				SKSurface s = (SKSurface)this_canvas.ExtensionProperties_GetValue("Surface", null);
				if (s == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no surface");
				canvas.DrawSurface(s, x, y, paint);
			}
		}

		/// <summary>Draws this canvas to a canvas.</summary>
		public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPoint point, SKPaint paint = null)
		{
			if (this_canvas.isHardwareAccelerated())
			{
				SKSurface s = (SKSurface)this_canvas.ExtensionProperties_GetValue("Surface", null);
				if (s == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no surface");
				canvas.DrawSurface(s, point, paint);
			}
		}

		public static SKRectI GetDeviceClipBounds(this SKCanvas canvas)
		{
			SKRectI bounds;
			canvas.GetDeviceClipBounds(out bounds);
			return bounds;
		}

		public static SKRect GetLocalClipBounds(this SKCanvas canvas)
        {
			SKRect bounds;
			canvas.GetLocalClipBounds(out bounds);
			return bounds;
		}

		//static int only_axis_aligned_saveBehind(SKCanvas canvas, SKRect bounds)
		//{
		//	if (!canvas.GetLocalClipBounds().IntersectsWith(bounds))
		//	{
		//		// Assuming clips never expand, if the request bounds is outside of the current clip
		//		// there is no need to copy/restore the area, so just devolve back to a regular save.
		//		canvas.Save();
		//	}
		//	else
  //          {
		//		bool doTheWork = false;
		//		internalSave();
		//		if (doTheWork)
		//		{
		//			internalSaveBehind(bounds);
		//		}
		//	}
		//	return canvas.SaveCount - 1;
		//}

		//static int SaveBehind(SKCanvas canvas, SKRect subset)
  //      {
		//	return only_axis_aligned_saveBehind(canvas, subset);
		//}


		public static int saveUnclippedLayer(this SKCanvas canvas, int left, int top, int right, int bottom)
		{
			return canvas.Save();
			//	SKRect bounds = SKRect.Create(left, top, right, bottom);
			//	return SaveBehind(this_canvas, bounds);
		}

		public static void restoreUnclippedLayer(this SKCanvas canvas, int restoreCount, SKPaint paint)
		{

            while (canvas.SaveCount > restoreCount + 1) {
				canvas.Restore();
            }

            //	if (mCanvas->getSaveCount() == restoreCount + 1) {
            //		SkCanvasPriv::DrawBehind(mCanvas, filterPaint(paint));

            //		this->restore();

            //	}
        }

		public static void ClipRect(this SKCanvas canvas, float left, float top, float right, float bottom)
        {
			canvas.ClipRect(new SKRect(left, top, right, bottom));
        }

    }
}