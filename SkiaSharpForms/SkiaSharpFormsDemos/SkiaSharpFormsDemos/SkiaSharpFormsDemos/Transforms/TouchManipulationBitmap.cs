using System;
using System.Collections.Generic;
using System.Linq;

using SkiaSharp;

using TouchTracking;

namespace SkiaSharpFormsDemos.Transforms
{
    class TouchManipulationBitmap
    {
        SKBitmap bitmap;
        Dictionary<long, TouchManipulationInfo> touchDictionary = 
            new Dictionary<long, TouchManipulationInfo>();

        public TouchManipulationBitmap(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
            Matrix = SKMatrix.MakeIdentity();
        }

        public SKMatrix Matrix { set; get; }

        public void Paint(SKCanvas canvas)
        {
            canvas.Save();
            SKMatrix matrix = Matrix;
            canvas.Concat(ref matrix);
            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
        }

        public bool HitTest(SKPoint location)
        {
            SKRect rect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            rect = Matrix.MapRect(rect);
            return rect.Contains(location);
        }

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    touchDictionary.Add(id, new TouchManipulationInfo
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        // Enable anisotropic scaling
        // Enable two-finger rotate
        // Enable one-finger rotate


        // Touch Manipulation Manager

            // Enable properties

            // SKMatrix OneTouch(SKPoint prevPoint, SKPoint newPoint, SKPoint pivot)

            // SKMatrix TwoTouch(SKPoint prevPoint, SKPoint newPoint, SKPoint pivot)






        void Manipulate()
        {
            if (touchDictionary.Count == 1)
            {
                TouchManipulationInfo info = touchDictionary.Values.First();
                float xTranslate = info.NewPoint.X - info.PreviousPoint.X;
                float yTranslate = info.NewPoint.Y - info.PreviousPoint.Y;

                SKMatrix matrix = Matrix;
                SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(xTranslate, yTranslate));
                Matrix = matrix;
            }

            else if (touchDictionary.Count >= 2)
            {
                TouchManipulationInfo[] infos = // new TouchManipulationInfo[touchDictionary.Count];
                touchDictionary.Values.ToArray(); // .CopyTo(infos, 0);
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;

                SKPoint pivot = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;


                // Anisotropic
                /*
                                float scaleX = (newPoint.X - pivot.X) / (prevPoint.X - pivot.X);
                                float scaleY = (newPoint.Y - pivot.Y) / (prevPoint.Y - pivot.Y);
                */

                // Isotropic
                float scale = Magnitude(newPoint - pivot) / Magnitude(prevPoint - pivot);
                float scaleX = scale;
                float scaleY = scale;



                SKMatrix matrix = Matrix;
                SKMatrix.PostConcat(ref matrix, SKMatrix.MakeScale(scaleX, scaleY, pivot.X, pivot.Y));
                Matrix = matrix;
            }
        }

        float Magnitude(SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }
    }
}
