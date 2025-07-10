using System;
using System.Windows;
using System.Windows.Media;

namespace MZ.Util
{
    public class MZBrush
    {

        public static RadialGradientBrush CreateHsvCircularGradientBrush()
        {
            var gradientBrush = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5),
                RadiusX = 0.5,
                RadiusY = 0.5
            };

            int hueStep = 30;
            for (double hue = 0; hue <= 360; hue += hueStep)
            {
                var color = HsvToRgb(hue, 1.0, 1.0);
                gradientBrush.GradientStops.Add(new GradientStop(color, hue / 360.0));
            }

            return gradientBrush;
        }
        public static LinearGradientBrush CreateHsvGradientBrush()
        {
            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            int hueStep = 30;
            for (double hue = 0; hue <= 360; hue += hueStep)
            {
                var color = HsvToRgb(hue, 1.0, 1.0);
                gradientBrush.GradientStops.Add(new GradientStop(color, hue / 360.0));
            }

            return gradientBrush;
        }

        public static Color HsvToRgb(double hue, double saturation, double value)
        {
            int hi = (int)(hue / 60) % 6;
            double f = (hue / 60) - Math.Floor(hue / 60);

            double v = value * 255;
            double p = v * (1 - saturation);
            double q = v * (1 - f * saturation);
            double t = v * (1 - (1 - f) * saturation);

            byte r = 0, g = 0, b = 0;
            switch (hi)
            {
                case 0: r = (byte)v; g = (byte)t; b = (byte)p; break;
                case 1: r = (byte)q; g = (byte)v; b = (byte)p; break;
                case 2: r = (byte)p; g = (byte)v; b = (byte)t; break;
                case 3: r = (byte)p; g = (byte)q; b = (byte)v; break;
                case 4: r = (byte)t; g = (byte)p; b = (byte)v; break;
                case 5: r = (byte)v; g = (byte)p; b = (byte)q; break;
            }

            return Color.FromRgb(r, g, b);
        }
    }
}
