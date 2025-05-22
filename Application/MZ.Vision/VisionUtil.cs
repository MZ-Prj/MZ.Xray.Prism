using OpenCvSharp;
using System;

namespace MZ.Vision
{
    public class VisionUtil
    {
        public static string ScalarToHex(Scalar scalar)
        {
            byte b = (byte)scalar.Val0;
            byte g = (byte)scalar.Val1;
            byte r = (byte)scalar.Val2;
            byte a = (byte)scalar.Val3;

            return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
        }

        public static Scalar HexToScalar(string hex)
        {
            hex = hex.TrimStart('#');

            byte a = 255;
            int offset = 0;

            if (hex.Length == 8)
            {
                a = Convert.ToByte(hex.Substring(0, 2), 16);
                offset = 2;
            }

            byte r = Convert.ToByte(hex.Substring(offset, 2), 16);
            byte g = Convert.ToByte(hex.Substring(offset + 2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(offset + 4, 2), 16);

            return new Scalar(b, g, r, a);
        }
        public static OpenCvSharp.Point ToOpenCvPoint(System.Windows.Point point)
        {
            return new OpenCvSharp.Point((int)point.X, (int)point.Y);
        }
        public static System.Windows.Point ToWindowsPoint(OpenCvSharp.Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }
    }
}
