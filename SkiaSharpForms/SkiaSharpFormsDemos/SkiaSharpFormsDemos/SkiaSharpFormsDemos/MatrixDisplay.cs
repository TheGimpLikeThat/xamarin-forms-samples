using System;
using SkiaSharp;

namespace SkiaSharpFormsDemos
{
    static class MatrixDisplay
    {
        static SKPaint matrixPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 48,
            StrokeWidth = 2
        };

        public static SKSize Measure(SKMatrix matrix, SKPaint paint = null)
        {
            return MeasureAndPaint(null, matrix, new SKPoint(), paint ?? matrixPaint);
        }

        public static void Paint(SKCanvas canvas, SKMatrix matrix, SKPoint location, SKPaint paint = null)
        {
            MeasureAndPaint(canvas, matrix, location, paint ?? matrixPaint, true);
        }

        static SKSize MeasureAndPaint(SKCanvas canvas, SKMatrix matrix, SKPoint location, SKPaint paint, bool doPaint = false)
        {
            float[] values = matrix.Values;
            string[] texts = new string[9];
            SKRect[] bounds = new SKRect[9];
            float[] widths = new float[3]; 

            for (int i = 0; i < 9; i++)
            {
                int row = i % 3;
                int col = i / 3;

                // Format string differently based on row
                texts[i] = values[i].ToString(row == 2 ? "F0" : (col == 2 ? "F4" : "F2"));

                // Measure string with a '-' even if one is not present
                bool isNegative = texts[i][0] == '-';
                string text = (isNegative ? "" : "-") + texts[i];
                paint.MeasureText(text, ref bounds[i]);

                // Get maximum width for each column
                widths[col] = Math.Max(widths[col], bounds[i].Width);

                // Measure the text again without the '-' in front
                paint.MeasureText(texts[i], ref bounds[i]);
            }

            // Some formatting constants 
            float horzGap = paint.TextSize;
            float horzMarg = paint.TextSize;
            float vertMarg = paint.FontSpacing / 4;

            // Calculate the total width and height of the matrix display
            float totalWidth = widths[0] + widths[1] + widths[2] + 2 * horzGap + 2 * horzMarg;
            float totalHeight = 3 * paint.FontSpacing + 2 * vertMarg;

            if (doPaint)
            {
                SKPaintStyle saveStyle = paint.Style;

                for (int i = 0; i < 9; i++)
                {
                    int row = i % 3;
                    int col = i / 3;

                    // Find x, y of upper-left corner of text
                    float x = location.X + horzMarg;

                    for (int c = 0; c < col; c++)
                    {
                        x += widths[c] + horzGap;
                    }

                    float y = location.Y + vertMarg + row * paint.FontSpacing;

                    // Adjust for right-justified text
                    x += widths[col] - bounds[i].Width;
                    y += (paint.FontSpacing - bounds[i].Height) / 2 - bounds[i].Top;

                    // Draw the text
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawText(texts[i], x, y, paint);
                }

                // Display vertical lines at the sides of the matrix
                paint.Style = SKPaintStyle.Stroke;
                canvas.DrawLine(location.X + horzMarg / 2, location.Y + vertMarg, 
                                location.X + horzMarg / 2, location.Y + totalHeight - vertMarg, paint);
                canvas.DrawLine(location.X + totalWidth - horzMarg / 2, location.Y + vertMarg, 
                                location.X + totalWidth - horzMarg / 2, location.Y + totalHeight - vertMarg, paint);

                paint.Style = saveStyle;
            }
            return new SKSize(totalWidth, totalHeight);
        }
    }
}
