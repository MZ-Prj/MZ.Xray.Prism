using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using static MZ.Vision.VisionEnum;

namespace MZ.Vision
{
    public class VisionBase
    {
        public static Scalar Make(MatType type)
        {
            Scalar scalar = new Scalar(0);

            if (type == MatType.CV_16UC1)
            {
                scalar = new Scalar(ushort.MaxValue);
            }
            if (type == MatType.CV_16UC3)
            {
                scalar = new Scalar(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
            }
            if (type == MatType.CV_8UC1)
            {
                scalar = new Scalar(byte.MaxValue);
            }
            if (type == MatType.CV_8UC3)
            {
                scalar = new Scalar(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            }
            return scalar;
        }
        public static Scalar MakeZeros(MatType type)
        {
            Scalar scalar = new Scalar(0);

            if (type == MatType.CV_16UC1)
            {
                scalar = new Scalar(ushort.MinValue);
            }
            if (type == MatType.CV_16UC3)
            {
                scalar = new Scalar(ushort.MinValue, ushort.MinValue, ushort.MinValue);
            }
            if (type == MatType.CV_8UC1)
            {
                scalar = new Scalar(byte.MinValue);
            }
            if (type == MatType.CV_8UC3)
            {
                scalar = new Scalar(byte.MinValue, byte.MinValue, byte.MinValue);
            }
            return scalar;
        }
        public static Mat CreateZeros(int height, int width, MatType type)
        {
            Mat result = new Mat(height, width, type, MakeZeros(type));
            return result;
        }

        public static Mat Create(int height, int width, MatType type)
        {
            Mat result = new Mat(height, width, type, Make(type));
            return result;
        }

        public static Mat Create(int height, int width, MatType type, Scalar value)
        {
            Mat result = new Mat(height, width, type, value);
            return result;
        }

        public static Mat Clone(Mat input)
        {
            return input.Clone();
        }

        public static void Save(Mat input, string root)
        {
            Cv2.ImWrite(root, input);
        }


        public static void Save(List<Mat> input, string root, double fps = 2.0)
        {
            Size frameSize = new(input[0].Width, input[0].Height);
            FourCC fourCC = FourCC.XVID;

            using var writer = new VideoWriter(root, fourCC, fps, frameSize, true);
            if (!writer.IsOpened())
            {
                throw new Exception("VideoWriter Open Error");
            }

            for (int i = 0; i < input.Count; i++)
            {
                Mat resizedImage = input[i].Clone();
                if (resizedImage.Size() != frameSize)
                {
                    Cv2.Resize(resizedImage, resizedImage, frameSize);
                }
                else
                {
                    resizedImage = BlendWithBackground(resizedImage);
                }

                writer.Write(resizedImage);
            }
        }

        public static void Save(List<Mat> input, List<DateTime> time, string root, double fps = 2.0)
        {
            Size frameSize = new(input[0].Width, input[0].Height);
            FourCC fourCC = FourCC.XVID;

            using var writer = new VideoWriter(root, fourCC, fps, frameSize, true);
            if (!writer.IsOpened())
            {
                throw new Exception("VideoWriter Open Error");
            }

            for (int i = 0; i < input.Count; i++)
            {
                Mat resizedImage = input[i].Clone();
                if (resizedImage.Size() != frameSize)
                {
                    Cv2.Resize(resizedImage, resizedImage, frameSize);
                }
                else
                {
                    resizedImage = BlendWithBackground(resizedImage);
                }

                //시간 추가
                string timeText = time[i].ToString("yyyy-MM-dd HH:mm:ss");
                Point textPosition = new(10, 20);
                Scalar textColor = Scalar.Red;
                double fontScale = 0.5;
                int thickness = 2;

                Cv2.PutText(resizedImage, timeText, textPosition, HersheyFonts.HersheySimplex, fontScale, textColor, thickness);

                writer.Write(resizedImage);
            }
        }

        public static Mat BlendWithBackground(Mat input)
        {
            var start = DateTime.Now;

            Mat[] channels = new Mat[4];
            Cv2.Split(input, out channels);

            Mat alpha = new Mat();
            channels[3].ConvertTo(alpha, MatType.CV_32F, 1.0 / byte.MaxValue);

            Mat background = new Mat(input.Size(), MatType.CV_32FC3, Scalar.White);

            Mat foreground = new Mat();
            Cv2.Merge(new[] { channels[0], channels[1], channels[2] }, foreground);
            foreground.ConvertTo(foreground, MatType.CV_32FC3);

            Mat result = new Mat();

            for (int c = 0; c < 3; c++)
            {
                using (Mat fore = new Mat())
                using (Mat back = new Mat())
                {
                    Mat oneMinusAlpha = new Mat();

                    Cv2.Multiply(foreground.ExtractChannel(c), alpha, fore);
                    Cv2.Subtract(Mat.Ones(alpha.Size(), alpha.Type()), alpha, oneMinusAlpha);
                    Cv2.Multiply(background.ExtractChannel(c), oneMinusAlpha, back);
                    Cv2.Add(fore, back, fore);

                    if (c == 0)
                    {
                        result = fore.Clone();
                    }
                    else
                    {
                        Cv2.Merge(new[] { result, fore }, result);
                    }
                }
            }

            result.ConvertTo(result, MatType.CV_8UC3);
            return result;
        }

        public static Mat CvtColor(Mat input, ColorConversionCodes code)
        {
            Cv2.CvtColor(input, input, ColorConversionCodes.BGRA2BGR);
            return input;
        }

        public static Task SaveAsync(Mat input, string root)
        {
            return Task.Run(() =>
            {
                Cv2.ImWrite(root, input);
            });
        }

        public static Mat Load(string filename)
        {
            Mat result = Cv2.ImRead(filename, ImreadModes.Unchanged);
            return result;
        }

        public static Mat RoI(Mat input, int x, int y, int width, int height)
        {
            var roiRect = new Rect(x, y, width, height);
            Mat roiMat = input.SubMat(roiRect);
            return roiMat;
        }

        public static (double, double) MinMax(Mat input)
        {
            if (input.Type() == MatType.CV_16UC3)
            {
                Mat[] channels = Cv2.Split(input);

                Cv2.MinMaxLoc(channels[0], out double redMinValue, out double redMaxValue, out Point redMinLocation, out Point redMaxLocation);
                Cv2.MinMaxLoc(channels[1], out double greenMinValue, out double greenMaxValue, out Point greenMinLocation, out Point greenMaxLocation);
                Cv2.MinMaxLoc(channels[2], out double blueMinValue, out double blueMaxValue, out Point blueMinLocation, out Point blueMaxLocation);

                return (
                    Math.Min(redMinValue, Math.Min(greenMinValue, blueMinValue)),
                    Math.Max(redMaxValue, Math.Max(greenMaxValue, blueMaxValue)));
            }
            if (input.Type() == MatType.CV_16UC1)
            {
                Cv2.MinMaxLoc(input, out double minValue, out double maxValue, out Point minLocation, out Point maxLocation);
                return (minValue, maxValue);
            }
            if (input.Type() == MatType.CV_8UC3)
            {
                Mat[] channels = Cv2.Split(input);

                Cv2.MinMaxLoc(channels[0], out double redMinValue, out double redMaxValue, out Point redMinLocation, out Point redMaxLocation);
                Cv2.MinMaxLoc(channels[1], out double greenMinValue, out double greenMaxValue, out Point greenMinLocation, out Point greenMaxLocation);
                Cv2.MinMaxLoc(channels[2], out double blueMinValue, out double blueMaxValue, out Point blueMinLocation, out Point blueMaxLocation);

                return (
                    Math.Min(redMinValue, Math.Min(greenMinValue, blueMinValue)),
                    Math.Max(redMaxValue, Math.Max(greenMaxValue, blueMaxValue)));
            }
            if (input.Type() == MatType.CV_8UC1)
            {
                Cv2.MinMaxLoc(input, out double minValue, out double maxValue, out Point minLocation, out Point maxLocation);
                return (minValue, maxValue);
            }
            if (input.Type() == MatType.CV_8UC4)
            {
                Mat gray = CvtColor(input, ColorConversionCodes.BGRA2GRAY);
                Cv2.MinMaxLoc(gray, out double minValue, out double maxValue, out Point minLocation, out Point maxLocation);
                return (minValue, maxValue);
            }
            return (0, 0);
        }


        public static Mat FillPoly(Mat image, List<Point> polygonPoints, Scalar color)
        {
            Point[][] pts = [[.. polygonPoints]];
            Cv2.FillPoly(image, pts, color);
            return image;
        }

        public static double CompareHistogram(Mat input1, Mat input2, HistCompMethods histCompMethods = HistCompMethods.Chisqr)
        {
            Mat hist1 = new();
            Mat hist2 = new();

            input1.ConvertTo(hist1, MatType.CV_32F);
            input2.ConvertTo(hist2, MatType.CV_32F);

            double result = Cv2.CompareHist(hist1, hist2, histCompMethods);
            return result;
        }

        public static Mat Histogram(Mat input)
        {
            Mat result = new();

            Cv2.MinMaxLoc(input, out double minValue, out double maxValue, out Point minLocation, out Point maxLocation);
            Cv2.EqualizeHist(input, result);

            Mat srcHist = new();
            Mat[] srcChannels = { input };
            int[] srcChannelsIdx = { 0 };
            int[] histSize = { 256 };
            Rangef[] ranges = { new Rangef((int)minValue, (int)maxValue) };
            Cv2.CalcHist(srcChannels, srcChannelsIdx, null, srcHist, 1, histSize, ranges);

            Mat dstHist = new();
            Mat[] dstChannels = { result };
            int[] dstChannelsIdx = { 0 };
            Cv2.CalcHist(dstChannels, dstChannelsIdx, null, dstHist, 1, histSize, ranges);

            return result;
        }

        public static Mat PlotFilledHistogram(Mat input, double lower = 0, double upper = byte.MaxValue)
        {
            int channels = input.Channels();

            int histSize = 256;
            int graphHeight = 256;
            Mat histImage = new(new Size(histSize, graphHeight), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));

            int[] histSizes = [histSize];
            Rangef[] ranges = [new(0, 256)];

            for (int i = 0; i < channels; i++)
            {
                Mat hist = new();
                Cv2.CalcHist([input], [i], null, hist, 1, histSizes, ranges);

                double maxVal = 0;
                Cv2.MinMaxLoc(hist, out _, out maxVal);

                Scalar color = new(0, 0, 0, 128);
                if (i == 0) color = new Scalar(255, 0, 0, 128);
                else if (i == 1) color = new Scalar(0, 255, 0, 128);
                else if (i == 2) color = new Scalar(0, 0, 255, 128);
                else if (i == 3) color = new Scalar(128, 128, 128, 128);

                Point[] points = new Point[histSize + 2];
                for (int j = 0; j < histSize; j++)
                {
                    int x = j;
                    int y = (int)(hist.Get<float>(j) * graphHeight / maxVal);
                    y = graphHeight - y;
                    points[j] = new Point(x, y);
                }

                points[histSize] = new Point(histSize - 1, graphHeight);
                points[histSize + 1] = new Point(0, graphHeight);

                Cv2.FillConvexPoly(histImage, points, color);
            }

            if (lower > 0)
            {
                int lowerX = (int)lower;
                Mat overlay = new Mat(histImage.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
                Cv2.Rectangle(overlay, new Point(0, 0), new Point(lowerX, graphHeight), new Scalar(0, 0, 0, 30), -1);
                Cv2.Add(histImage, overlay, histImage);
            }

            if (upper < byte.MaxValue)
            {
                int upperX = (int)upper;
                Mat overlay = new Mat(histImage.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
                Cv2.Rectangle(overlay, new Point(upperX, 0), new Point(histSize, graphHeight), new Scalar(0, 0, 0, 30), -1);
                Cv2.Add(histImage, overlay, histImage);
            }


            return histImage;
        }


        public static Mat Contrast(Mat input, int level)
        {
            Mat result = new();
            //double slop = 1;
            //slop += level;
            double slope = Math.Pow(1.2, level);
            //double slope = (double)level;

            input.ConvertTo(result, input.Type(), slope);
            return result;
        }


        public static Mat Sharp(Mat input, int level)
        {
            Mat blur = new();
            Mat result = new();

            Cv2.GaussianBlur(input, blur, new Size(0, 0), 3);
            Cv2.AddWeighted(input, 1 + level, blur, -level, 0, result);

            return result;
        }

        public static Mat Blur(Mat input, int level)
        {
            Mat result = new();
            //Cv2.GaussianBlur(input, result, new Size(level*2+1, level * 2 + 1), 0);
            Cv2.GaussianBlur(input, result, new Size(0, 0), level);
            return result;
        }

        public static Mat Bright(Mat input, int level)
        {
            Mat result = new();

            if (MatType.CV_16U == input.Depth())
            {
                level *= 4096;
            }
            else
            {
                level *= 16;
            }
            Cv2.Add(input, level, result);
            return result;
        }

        public static Mat Convert16BitTo8Bit(Mat input)
        {
            Mat result = new();
            if (MatType.CV_16U == input.Depth())
            {
                input.ConvertTo(result, MatType.CV_8UC1, 1.0 / (double)(byte.MaxValue + 1));
            }
            else
            {
                result = input;
            }
            return result;
        }

        public static Mat Resize(Mat input, int width, int height)
        {
            Mat result = new();
            Size size = new(width, height);
            Cv2.Resize(input, result, size, 0, 0, InterpolationFlags.Linear);
            return result;
        }

        public static Mat ResizeWithAspectRatio(Mat input, int size)
        {
            int originalWidth = input.Width;
            int originalHeight = input.Height;

            double scale = Math.Min((double)size / originalWidth, (double)size / originalHeight);

            int newWidth = (int)(originalWidth * scale);
            int newHeight = (int)(originalHeight * scale);

            Mat resized = new();
            Cv2.Resize(input, resized, new Size(newWidth, newHeight), 0, 0, InterpolationFlags.Linear);

            Mat result = Mat.Zeros(new Size(size, size), input.Type());

            int xOffset = (size - newWidth) / 2;
            int yOffset = (size - newHeight) / 2;

            Rect roi = new Rect(xOffset, yOffset, newWidth, newHeight);
            resized.CopyTo(result[roi]);

            return result;
        }

        public static Mat Rotate(Mat input, RotateFlags rotate = RotateFlags.Rotate90Counterclockwise)
        {
            Mat result = new();
            Cv2.Rotate(input, result, rotate);
            return result;
        }


        public static Mat CenterCol(Mat input, int width, int height, Scalar scalar, int start, int end)
        {
            Mat result = new(height, width, input.Type(), scalar);
            input.ColRange(0, input.Width).CopyTo(result.ColRange(start, end));
            return result;
        }

        public static Mat OverlapCol(Mat input, Mat line)
        {
            int cols = input.Cols;
            int offset = line.Width;

            input.ColRange(cols - offset, cols).CopyTo(line);
            return input;
        }

        public static Mat ShiftCol(Mat input, Mat line)
        {
            int cols = input.Cols;
            int rows = input.Rows;
            int offset = line.Width;
            Mat result = new(rows, cols, input.Type());

            input.ColRange(offset, cols).CopyTo(result.ColRange(0, cols - offset));
            line.CopyTo(result.ColRange(cols - offset, cols));

            return result;
        }

        public static Mat ShiftColLeft(Mat input, Mat line)
        {
            int cols = input.Cols;
            int rows = input.Rows;
            int offset = line.Width;

            Mat result = new(rows, cols, input.Type());

            input.ColRange(offset, cols).CopyTo(result.ColRange(0, cols - offset));
            line.CopyTo(result.ColRange(cols - offset, cols));

            return result;
        }

        public static Mat ShiftColRight(Mat input, Mat line)
        {
            int cols = input.Cols;
            int rows = input.Rows;
            int offset = line.Width;

            Mat result = new(rows, cols, input.Type());

            // 왼쪽에서 오른쪽으로 데이터를 이동
            input.ColRange(0, cols - offset).CopyTo(result.ColRange(offset, cols));
            line.CopyTo(result.ColRange(0, offset));

            return result;
        }

        public static Mat ShiftRow(Mat input, Mat line)
        {
            int cols = input.Cols;
            int rows = input.Rows;
            int offset = line.Height;

            Mat result = new(rows, cols, input.Type());
            input.RowRange(offset, rows).CopyTo(result.RowRange(0, rows - offset));
            line.CopyTo(result.RowRange(rows - offset, rows));

            return result;
        }

        public static Mat SplitCol(Mat input, int start, int end)
        {
            int cols = end - start;
            int rows = input.Rows;
            Mat result = new(rows, cols, input.Type());
            input.ColRange(start, end).CopyTo(result.ColRange(0, cols));
            return result;
        }

        public static Mat SplitRow(Mat input, int start, int end)
        {
            int rows = end - start;
            int cols = input.Cols;
            Mat result = new(rows, cols, input.Type());
            input.RowRange(start, end).CopyTo(result.RowRange(0, rows));
            return result;
        }

        public static double Mean(Mat input)
        {
            return (double)Cv2.Mean(input);
        }

        public static Mat Flip(Mat input, FlipMode mode)
        {
            Mat result = new();
            Cv2.Flip(input, result, mode);
            return result;
        }

        public static Mat ExpandArrayToMat(ushort[] array, int width, MatType matType)
        {
            int rows = array.Length;
            int cols = width;

            Mat expandedMat = new(rows, cols, matType);

            for (int i = 0; i < rows; i++)
            {
                expandedMat.Row(i).SetTo(new Scalar(array[i]));
            }

            return expandedMat;
        }

        public static Mat Crop(Mat input, int x, int y, int width, int height)
        {
            Rect rect = new Rect(x, y, width, height);
            Mat result = new(input, rect);

            return result;
        }

        public static Mat CropPaste(Mat input, Mat cropped, int x, int y)
        {
            Mat result = input.Clone();
            Rect roi = new Rect(x, y, cropped.Width, cropped.Height);
            cropped.CopyTo(result[roi]);

            return result;
        }

        public static Mat Convert1DArrayToMat(ushort[] array, int height = 64)
        {
            int width = array.Length;

            Mat mat = new(height, width, MatType.CV_16UC1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mat.Set(i, j, array[j]);
                }
            }

            return mat;
        }

        public static string InformationOnPixel(Mat input, System.Windows.Point point)
        {
            string result = string.Empty;
            try
            {

                var size = $"Size : {input.Size()}";
                var type = $"type : {input.Type()}";
                var position = $"Position : (x:{(int)point.X}, y:{(int)point.Y})";
                var pixel = string.Empty;

                if (input.Type() == MatType.CV_16UC1)
                {
                    pixel = $"Pixel : {input.Get<ushort>((int)point.Y, (int)point.X)}";
                }
                if (input.Type() == MatType.CV_16UC3)
                {
                    pixel = $"Pixel : {input.Get<Vec3w>((int)point.Y, (int)point.X)}";
                }
                result = $"{size}, {type}, {position}, {pixel}";
            }
            catch { }
            return result;
        }

        public static double[] RowAverage(Mat mat)
        {
            int rows = mat.Rows;
            int cols = mat.Cols;

            double[] average = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                average[i] = Cv2.Mean(mat.Row(i))[0];
            }

            return average;
        }

        public static double[] ColAverage(Mat mat)
        {
            int rows = mat.Rows;
            int cols = mat.Cols;

            double[] average = new double[cols];

            for (int i = 0; i < cols; i++)
            {
                average[i] = Cv2.Mean(mat.Col(i))[0];
            }

            return average;
        }

        public static void View(Mat input, string title = "title")
        {
            Mat output = new Mat();
            Cv2.Resize(input, output, new Size(input.Width / 2, input.Height / 2));
            Cv2.NamedWindow(title, WindowFlags.AutoSize);
            Cv2.MoveWindow(title, 100, 100);
            Cv2.ImShow(title, output);

            Cv2.WaitKey(0);
        }

        public static Mat ImageSourceToMat(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                return BitmapSourceConverter.ToMat(bitmapSource);
            }
            return new Mat();
        }

        public static Mat BitmapSourceToMat(BitmapSource bitmapSource)
        {
            return bitmapSource.ToMat();
        }

        public static Mat Threshold(Mat mat, double threshold, ThresholdTypes type)
        {
            Mat result = new();
            Cv2.Threshold(mat, result, threshold, byte.MaxValue, type);
            return result;
        }

        public static Mat Threshold(Mat mat, double lower, double upper)
        {
            Mat result = new();

            int channels = mat.Channels();

            Mat[] channelsArray = Cv2.Split(mat);

            for (int i = 0; i < channels; i++)
            {
                Mat mask = new();
                Cv2.InRange(channelsArray[i], new Scalar(lower), new Scalar(upper), mask);
                Cv2.BitwiseAnd(channelsArray[i], mask, channelsArray[i]);
            }

            Cv2.Merge(channelsArray, result);

            return result;
        }

        public static Mat ColorBalance(Mat mat, double red, double green, double blue)
        {
            Mat[] channels = Cv2.Split(mat);

            Cv2.Add(channels[0], new Scalar(blue), channels[0]);
            Cv2.Add(channels[1], new Scalar(green), channels[1]);
            Cv2.Add(channels[2], new Scalar(red), channels[2]);

            Mat result = new();
            Cv2.Merge(channels, result);
            return result;
        }

        public static Mat SelectColor(Mat mat, ColorEnum color)
        {
            Mat result = new();

            switch (color)
            {
                case ColorEnum.Color:
                    result = mat.Clone();
                    break;

                case ColorEnum.Gray:
                    {
                        Mat bgr = new();
                        Cv2.CvtColor(mat, bgr, ColorConversionCodes.BGRA2BGR);
                        Mat gray = new();
                        Cv2.CvtColor(bgr, gray, ColorConversionCodes.BGR2GRAY);
                        Mat[] splitChannels = Cv2.Split(mat);
                        Mat alpha = splitChannels[3];

                        Mat[] mergedChannels =
                        [
                            gray,
                            gray,
                            gray,
                            alpha
                        ];
                        Cv2.Merge(mergedChannels, result);
                        break;
                    }

                case ColorEnum.Red:
                    {
                        Mat[] channels = Cv2.Split(mat);
                        Mat zero = new(mat.Size(), MatType.CV_8UC1, Scalar.All(0));
                        Mat[] mergedChannels =
                        [
                            zero,
                            zero,
                            channels[2],
                            channels[3]
                        ];
                        Cv2.Merge(mergedChannels, result);
                        break;
                    }

                case ColorEnum.Green:
                    {
                        Mat[] channels = Cv2.Split(mat);
                        Mat zero = new(mat.Size(), MatType.CV_8UC1, Scalar.All(0));
                        Mat[] mergedChannels =
                        [
                            zero,
                            channels[1],
                            zero,
                            channels[3]
                        ];
                        Cv2.Merge(mergedChannels, result);
                        break;
                    }

                case ColorEnum.Blue:
                    {
                        Mat[] channels = Cv2.Split(mat);
                        Mat zero = new(mat.Size(), MatType.CV_8UC1, Scalar.All(0));
                        Mat[] mergedChannels =
                        [
                            channels[0],
                            zero,
                            zero,
                            channels[3]
                        ];
                        Cv2.Merge(mergedChannels, result);
                        break;
                    }

                case ColorEnum.Alpha:
                    {
                        Mat[] channels = Cv2.Split(mat);
                        Mat zero = new(mat.Size(), MatType.CV_8UC1, Scalar.All(0));
                        Mat[] mergedChannels =
                        [
                            zero,
                            zero,
                            zero,
                            channels[3]
                        ];
                        Cv2.Merge(mergedChannels, result);
                        break;
                    }

                default:
                    result = mat.Clone();
                    break;
            }

            return result;
        }

        public static Mat Exposure(Mat mat, double exposure, double offset, double gammaCorrection)
        {
            Mat result = new();

            int channels = mat.Channels();
            // 채널 분리
            Mat[] channelsArray = Cv2.Split(mat);

            double exposureScale = 10.0;
            double exposureFactor = Math.Pow(2.0, exposure / exposureScale);

            // alpha 미적용
            for (int i = 0; i < channels - 1; i++)
            {
                channelsArray[i].ConvertTo(channelsArray[i], MatType.CV_32F, 1.0 / 255.0);

                // exposure & offset : (2^exposure) + offset
                Cv2.Multiply(channelsArray[i], exposureFactor, channelsArray[i]);
                Cv2.Add(channelsArray[i], offset, channelsArray[i]);

                // clamp
                Cv2.Min(channelsArray[i], new Scalar(1.0), channelsArray[i]);
                Cv2.Max(channelsArray[i], new Scalar(0.0), channelsArray[i]);

                // gamma : pow(pixel, 1 / gammaCorrection)
                Cv2.Pow(channelsArray[i], 1.0 / gammaCorrection, channelsArray[i]);

                // clamp
                Cv2.Min(channelsArray[i], new Scalar(1.0), channelsArray[i]);
                Cv2.Max(channelsArray[i], new Scalar(0.0), channelsArray[i]);

                // scaling
                channelsArray[i].ConvertTo(channelsArray[i], mat.Type(), 255.0);
            }

            Cv2.Merge(channelsArray, result);

            return result;
        }


        public static Mat Curve(Mat mat, Point[] points)
        {
            if (points.Length == 0)
            {
                return mat;
            }

            Mat result = Create(mat.Height, mat.Width, mat.Type());
            int channels = mat.Channels();
            Mat[] channelsArray = Cv2.Split(mat);

            //byte[] lut = GenerateLinearLUT(points); 
            byte[] lut = GenerateSplineLUT(points);
            for (int i = 0; i < channels - 1; i++)
            {
                Cv2.LUT(channelsArray[i], lut, channelsArray[i]);

            }
            Cv2.Merge(channelsArray, result);

            return result;
        }

        /// <summary>
        /// 선형 LUT 생성
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static byte[] GenerateLinearLUT(Point[] points)
        {
            byte[] lut = new byte[256];

            // 점을 X 좌표 기준으로 정렬
            var sortedPoints = points.OrderBy(p => p.X).ToList();

            // 첫 점과 끝 점 추가 (0,0)과 (255,255)
            if (sortedPoints[0].X != 0)
            {
                sortedPoints.Insert(0, new Point(0, 0));
            }
            if (sortedPoints[sortedPoints.Count - 1].X != 255)
            {
                sortedPoints.Add(new Point(255, 255));
            }

            // 선형 보간으로 LUT 생성
            for (int i = 0; i < 256; i++)
            {
                double input = i;
                double output = LinearInterpolate(sortedPoints, input);
                lut[i] = (byte)Math.Clamp(output, 0, 255);
            }

            return lut;
        }

        /// <summary>
        /// 선형 보간
        /// </summary>
        /// <param name="points"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static double LinearInterpolate(List<Point> points, double input)
        {
            if (points.Count == 0)
            {
                return input; // 점이 없으면 입력값을 그대로 반환
            }

            if (input <= points[0].X)
            {
                return points[0].Y; // 첫 점보다 작으면 첫 점의 Y 값 반환
            }

            if (input >= points[points.Count - 1].X)
            {
                return points[points.Count - 1].Y; // 끝 점보다 크면 끝 점의 Y 값 반환
            }

            // 선형 보간
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (input >= points[i].X && input <= points[i + 1].X)
                {
                    double t = (input - points[i].X) / (points[i + 1].X - points[i].X);
                    return points[i].Y + t * (points[i + 1].Y - points[i].Y);
                }
            }

            return input; // 보간 실패 시 입력값 반환
        }

        /// <summary>
        /// 스플라인 곡선 LUT 생성
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static byte[] GenerateSplineLUT(Point[] points)
        {
            byte[] lut = new byte[256];

            var splinePoints = GenerateSpline([.. points]);

            if (splinePoints.Count == 0)
            {
                for (int i = 0; i < 256; i++)
                {
                    lut[i] = (byte)i;
                }
            }
            else
            {
                if (splinePoints[0].X != 0 || splinePoints[0].Y != 0)
                {
                    splinePoints.Insert(0, new Point(0, 0));
                }
                if (splinePoints[splinePoints.Count - 1].X != 255 || splinePoints[splinePoints.Count - 1].Y != 255)
                {
                    splinePoints.Add(new Point(255, 255));
                }

                for (int i = 0; i < 256; i++)
                {
                    double outputValue = InterpolateFromSpline(splinePoints, i);
                    lut[i] = (byte)Math.Clamp(outputValue, 0, 255);
                }
            }

            return lut;
        }

        private static List<Point> GenerateSpline(List<Point> points)
        {
            var splinePoints = new List<Point>();
            if (points.Count < 2)
            {
                return splinePoints;
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                var p0 = i > 0 ? points[i - 1] : points[i];
                var p1 = points[i];
                var p2 = points[i + 1];
                var p3 = i < points.Count - 2 ? points[i + 2] : points[i + 1];

                for (double t = 0; t <= 1; t += 0.05)
                {
                    double x = CalculateSplineValue(t, p0.X, p1.X, p2.X, p3.X);
                    double y = CalculateSplineValue(t, p0.Y, p1.Y, p2.Y, p3.Y);
                    splinePoints.Add(new Point(x, y));
                }
            }

            return splinePoints;
        }

        private static double CalculateSplineValue(double t, double p0, double p1, double p2, double p3)
        {
            return 0.5 * (
                (-p0 + 3 * p1 - 3 * p2 + p3) * Math.Pow(t, 3) +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * Math.Pow(t, 2) +
                (-p0 + p2) * t +
                2 * p1
            );
        }

        /// <summary>
        /// 스플라인 곡선 보간법
        /// </summary>
        /// <param name="splinePoints"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static double InterpolateFromSpline(List<Point> splinePoints, double input)
        {
            if (splinePoints.Count == 0)
            {
                return input;
            }

            if (input <= splinePoints[0].X)
            {
                return splinePoints[0].Y;
            }

            if (input >= splinePoints[splinePoints.Count - 1].X)
            {
                return splinePoints[splinePoints.Count - 1].Y;
            }

            for (int i = 0; i < splinePoints.Count - 1; i++)
            {
                if (input >= splinePoints[i].X && input <= splinePoints[i + 1].X)
                {
                    double t = (input - splinePoints[i].X) / (splinePoints[i + 1].X - splinePoints[i].X);
                    return splinePoints[i].Y + t * (splinePoints[i + 1].Y - splinePoints[i].Y);
                }
            }

            return input;
        }

        public static Mat GaussianBlur(Mat mat, int size)
        {
            Mat result = new();
            Cv2.GaussianBlur(mat, result, new Size(size, size), 1);
            return result;
        }

        public static List<Rect> ContourRectangles(Mat mat, int minArea)
        {
            double maxArea = mat.Width * mat.Height * 0.9;
            // Contour 탐지
            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            //계산
            List<Rect> rectangles = new List<Rect>();
            foreach (var contour in contours)
            {
                Point[] approx = Cv2.ApproxPolyDP(contour, 0.02 * Cv2.ArcLength(contour, true), true);
                double area = Cv2.ContourArea(contour);
                if (approx.Length == 4
                    && Cv2.IsContourConvex(approx)
                    && area > minArea
                    && maxArea > area)
                {
                    rectangles.Add(Cv2.BoundingRect(approx));
                }
            }

            return rectangles;
        }


        public static List<Rect> ContourRectangles(Mat mat, int minArea, int maxArea)
        {
            List<Rect> rectangles = new List<Rect>();
            Mat matGray = new Mat();

            if (mat.Channels() > 1)
            {
                Cv2.CvtColor(mat, matGray, ColorConversionCodes.BGR2GRAY);
            }
            else
            {
                matGray = mat.Clone();
            }

            Cv2.FindContours(matGray, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);

                if (area > minArea && area < maxArea)
                {
                    rectangles.Add(Cv2.BoundingRect(contour));
                }
            }

            return rectangles;
        }

        private static Random _random = new();
        public static Scalar GetRandomColor()
        {

            byte r = (byte)_random.Next(0, 256);
            byte g = (byte)_random.Next(0, 256);
            byte b = (byte)_random.Next(0, 256);
            return new Scalar(b, g, r);
        }

        public static Scalar GetColorFromSpectrom(double input, double min, double max)
        {
            if (max == min)
            {
                return new Scalar(0, 0, 255);
            }

            double normalized = (input - min) / (max - min);
            normalized = Math.Max(0, Math.Min(1, normalized));

            double hue = normalized * 240;

            byte h = (byte)(hue / 2);
            byte s = 255;
            byte v = 255;

            byte[] hsv = new byte[] { h, s, v };
            var hsvMat = new Mat(1, 1, MatType.CV_8UC3);
            Marshal.Copy(hsv, 0, hsvMat.Data, hsv.Length);

            Mat bgrMat = new Mat();
            Cv2.CvtColor(hsvMat, bgrMat, ColorConversionCodes.HSV2BGR);

            Vec3b bgrColor = bgrMat.At<Vec3b>(0, 0);

            return new Scalar(bgrColor.Item0, bgrColor.Item1, bgrColor.Item2);
        }

        public static Mat HConcat(params Mat[] mats)
        {
            Mat result = new();
            Cv2.HConcat(mats, result);
            return result;
        }

        public static Mat BrightnessAndContrast(Mat mat, double brightness, double contrast)
        {
            double contrastFactor = (100.0 + contrast) / 100.0;
            contrastFactor *= contrastFactor;

            Mat result = new();
            mat.ConvertTo(result, -1, contrastFactor, brightness);

            return result;
        }

        public static Type GetTypeFromMatType(MatType matType)
        {
            string type = matType.ToString();

            return type switch
            {
                "CV_8UC1" => typeof(byte),
                "CV_8UC2" => typeof(Vec2b),
                "CV_8UC3" => typeof(Vec3b),
                "CV_8UC4" => typeof(Vec4b),
                "CV_32FC1" => typeof(float),
                "CV_32FC2" => typeof(Vec2f),
                "CV_32FC3" => typeof(Vec3f),
                "CV_32FC4" => typeof(Vec4f),
                "CV_64FC1" => typeof(double),
                "CV_64FC2" => typeof(Vec2d),
                "CV_64FC3" => typeof(Vec3d),
                "CV_64FC4" => typeof(Vec4d),
                _ => typeof(byte)
            };
        }

        public static bool IsEmpty(Mat mat)
        {
            if (mat == null || mat.Empty())
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// ImageSource Freeze
        /// </summary>
        public static BitmapSource CanFreezeImageSource(BitmapSource bitmap)
        {
            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }
            return bitmap;
        }


    }
}
