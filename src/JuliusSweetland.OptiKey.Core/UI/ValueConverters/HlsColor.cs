using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace JuliusSweetland.OptiKey.UI.ValueConverters
{
    public class HlsColor
    {
        public double H { get; set; }
        public double L { get; set; }
        public double S { get; set; }
        public double A { get; set; }

        public static Color Fade(Color color, double amount)
        {
            var hlsColor = RgbToHls(color);
            hlsColor.L = (hlsColor.L < amount) ? hlsColor.L + amount : hlsColor.L - amount;
            return HlsToRgb(hlsColor);
        }
    
        // Converts a WPF RGB color to an HSL color
        public static HlsColor RgbToHls(Color rgbColor)
        {
            // Initialize result
            var hlsColor = new HlsColor();

            // Convert RGB values to percentages
            var r = (double)rgbColor.R / 255;
            var g = (double)rgbColor.G / 255;
            var b = (double)rgbColor.B / 255;
            hlsColor.A = (double)rgbColor.A / 255;

            // Find min and max RGB values
            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;

            /* If max and min are equal, that means we are dealing with 
             * a shade of gray. So we set H and S to zero, and L to either
             * max or min (it doesn't matter which), and  then we exit. */

            //Special case: Gray
            if (max == min)
            {
                hlsColor.H = 0;
                hlsColor.S = 0;
                hlsColor.L = max;
                return hlsColor;
            }

            /* If we get to this point, we know we don't have a shade of gray. */

            // Set L
            hlsColor.L = (min + max) / 2;

            // Set S
            if(hlsColor.L < 0.5)
            {
                hlsColor.S = delta / (max + min);
            }
            else
            {
                hlsColor.S = delta / (2.0 - max - min);
            }

            // Set H
            if (r == max) hlsColor.H = (g - b) / delta;
            if (g == max) hlsColor.H = 2.0 + (b - r) / delta;
            if (b == max) hlsColor.H = 4.0 + (r - g) / delta;
            hlsColor.H *= 60;
            if (hlsColor.H < 0) hlsColor.H += 360;

            // Set return value
            return hlsColor;

        }

       /// Converts a WPF HSL color to an RGB color
        public static Color HlsToRgb(HlsColor hlsColor)
        {
            // Initialize result
            var rgbColor = new Color();

            /* If S = 0, that means we are dealing with a shade 
             * of gray. So, we set R, G, and B to L and exit. */

            // Special case: Gray
            if (hlsColor.S == 0)
            {
                rgbColor.R = (byte)(hlsColor.L  * 255);
                rgbColor.G = (byte)(hlsColor.L * 255);
                rgbColor.B = (byte)(hlsColor.L * 255);
                rgbColor.A = (byte)(hlsColor.A * 255);
                return rgbColor;
            }

            double t1;
            if (hlsColor.L < 0.5)
            {
                t1 = hlsColor.L*(1.0 + hlsColor.S);
            }
            else
            {
                t1 = hlsColor.L + hlsColor.S - (hlsColor.L * hlsColor.S);
            }

            var t2 = 2.0*hlsColor.L - t1;

            // Convert H from degrees to a percentage
            var h = hlsColor.H / 360;

            // Set colors as percentage values
            var tR = h + (1.0/3.0);
            var r = SetColor(t1, t2, tR);

            var tG = h;
            var g = SetColor(t1, t2, tG);

            var tB = h - (1.0 / 3.0);
            var b = SetColor(t1, t2, tB);

            // Assign colors to Color object
            rgbColor.R = (byte)(r * 255);
            rgbColor.G = (byte)(g * 255);
            rgbColor.B = (byte)(b * 255);
            rgbColor.A = (byte)(hlsColor.A * 255);

            // Set return value
            return rgbColor;
        }

        /// <summary>
        /// Used by the HSL-to-RGB converter.
        /// </summary>
        /// <param name="t1">A temporary variable.</param>
        /// <param name="t2">A temporary variable.</param>
        /// <param name="t3">A temporary variable.</param>
        /// <returns>An RGB color value, in decimal format.</returns>
        private static double SetColor(double t1, double t2, double t3)
        {
            if (t3 < 0) t3 += 1.0;
            if (t3 > 1) t3 -= 1.0;

            double color;
            if (6.0 * t3 < 1)
            {
                color = t2 + (t1 - t2) * 6.0 * t3;
            }
            else if(2.0 * t3 < 1)
            {
                color = t1;
            }
            else if(3.0*t3 < 2)
            {
                color = t2 + (t1 - t2) * ((2.0/3.0) - t3) * 6.0;
            }
            else
            {
                color = t2;
            }

            // Set return value
            return color;
        }

        public static bool TryParse(string color, out SolidColorBrush colorBrush)
        {
            if (!string.IsNullOrEmpty(color)
                && (Regex.IsMatch(color, "^(#[0-9A-Fa-f]{3})$|^(#[0-9A-Fa-f]{6})$")
                || System.Drawing.Color.FromName(color).IsKnownColor))
            {
                colorBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
                return true;
            }
            colorBrush = null;
            return false;
        }
    }
}
