using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using static MZ.Vision.VisionEnum;

namespace MZ.Vision
{
    /// <summary>
    /// OpenCvSharp(Mat) 기반 영상 처리, 변환, 생성, 정보조회 등 주요 함수 제공 유틸리티
    /// </summary>
    public class VisionBase
    {
        private static Random _random = new();

        /// <summary>
        /// 타입에 맞는 최대값 스칼라 반환
        /// </summary>
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

        /// <summary>
        /// 타입에 맞는 0값 스칼라 반환
        /// </summary>
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

        /// <summary>
        /// 0으로 채운 Mat 생성
        /// </summary>
        public static Mat CreateZeros(int height, int width, MatType type)
        {
            Mat result = new Mat(height, width, type, MakeZeros(type));
            return result;
        }

        /// <summary>
        /// 타입에 맞는 최대값으로 채운 Mat 생성
        /// </summary>
        public static Mat Create(int height, int width, MatType type)
        {
            Mat result = new Mat(height, width, type, Make(type));
            return result;
        }

        /// <summary>
        /// 특정 값으로 Mat 생성
        /// </summary>
        public static Mat Create(int height, int width, MatType type, Scalar value)
        {
            Mat result = new Mat(height, width, type, value);
            return result;
        }

        /// <summary>
        /// Mat 복제
        /// </summary>
        public static Mat Clone(Mat input)
        {
            return input.Clone();
        }

        /// <summary>
        /// Mat을 파일로 저장
        /// </summary>
        public static void Save(Mat input, string root)
        {
            Cv2.ImWrite(root, input);
        }

        /// <summary>
        /// 영상 목록을 영상 파일로 저장
        /// </summary>
        public static void Save(List<Mat> input, string root, double fps = 2.0)
        {
            try
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
            catch (Exception e)
            {
                throw new Exception($"Exception :{e}");
            }
            
        }

        /// <summary>
        /// 영상 목록+시간 리스트 영상 파일로 저장(시간 워터마크 추가)
        /// </summary>
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

        /// <summary>
        /// 알파 채널을 흰 배경으로 블렌딩
        /// </summary>
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

        /// <summary>
        /// 컬러 변환 (ex: BGRA -> BGR 등)
        /// </summary>
        public static Mat CvtColor(Mat input, ColorConversionCodes code)
        {
            Cv2.CvtColor(input, input, ColorConversionCodes.BGRA2BGR);
            return input;
        }

        /// <summary>
        /// 파일 저장 (비동기)
        /// </summary>
        public static Task SaveAsync(Mat input, string root)
        {
            return Task.Run(() =>
            {
                Cv2.ImWrite(root, input);
            });
        }

        /// <summary>
        /// 파일에서 이미지 불러오기
        /// </summary>
        public static Mat Load(string filename)
        {
            Mat result = Cv2.ImRead(filename, ImreadModes.Unchanged);
            return result;
        }

        /// <summary>
        /// ROI 추출
        /// </summary>
        public static Mat RoI(Mat input, int x, int y, int width, int height)
        {
            var roiRect = new Rect(x, y, width, height);
            Mat roiMat = input.SubMat(roiRect);
            return roiMat;
        }

        /// <summary>
        /// 최소/최대값 반환 (채널별 처리)
        /// </summary>
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

        /// <summary>
        /// 다각형 채우기
        /// </summary>
        public static Mat FillPoly(Mat image, List<Point> polygonPoints, Scalar color)
        {
            Point[][] pts = [[.. polygonPoints]];
            Cv2.FillPoly(image, pts, color);
            return image;
        }

        /// <summary>
        /// 두 영상의 히스토그램 비교
        /// </summary>
        public static double CompareHistogram(Mat input1, Mat input2, HistCompMethods histCompMethods = HistCompMethods.Chisqr)
        {
            Mat hist1 = new();
            Mat hist2 = new();

            input1.ConvertTo(hist1, MatType.CV_32F);
            input2.ConvertTo(hist2, MatType.CV_32F);

            double result = Cv2.CompareHist(hist1, hist2, histCompMethods);
            return result;
        }

        /// <summary>
        /// 히스토그램 평활화 결과 반환
        /// </summary>
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

        /// <summary>
        /// 히스토그램 이미지(Mat) 생성 
        /// </summary>
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

                Cv2.MinMaxLoc(hist, out _, out double maxVal);

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
                Mat overlay = new(histImage.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
                Cv2.Rectangle(overlay, new Point(0, 0), new Point(lowerX, graphHeight), new Scalar(0, 0, 0, 30), -1);
                Cv2.Add(histImage, overlay, histImage);
            }

            if (upper < byte.MaxValue)
            {
                int upperX = (int)upper;
                Mat overlay = new(histImage.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
                Cv2.Rectangle(overlay, new Point(upperX, 0), new Point(histSize, graphHeight), new Scalar(0, 0, 0, 30), -1);
                Cv2.Add(histImage, overlay, histImage);
            }
            return histImage;
        }

        /// <summary>
        /// 명암 조정 (레벨 적용)
        /// </summary>
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

        /// <summary>
        /// 샤프닝(선명도) 조정 (레벨 적용)
        /// </summary>
        public static Mat Sharp(Mat input, int level)
        {
            Mat blur = new();
            Mat result = new();

            Cv2.GaussianBlur(input, blur, new Size(0, 0), 3);
            Cv2.AddWeighted(input, 1 + level, blur, -level, 0, result);

            return result;
        }

        /// <summary>
        /// 블러(가우시안 블러) 적용
        /// </summary>
        public static Mat Blur(Mat input, int level)
        {
            Mat result = new();
            //Cv2.GaussianBlur(input, result, new Size(level*2+1, level * 2 + 1), 0);
            Cv2.GaussianBlur(input, result, new Size(0, 0), level);
            return result;
        }

        /// <summary>
        /// 밝기 조정 (레벨 적용)
        /// </summary>
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

        /// <summary>
        /// 16비트 이미지를 8비트로 변환
        /// </summary>
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

        /// <summary>
        /// Resize
        /// </summary>
        public static Mat Resize(Mat input, int width, int height)
        {
            Mat result = new();
            Size size = new(width, height);
            Cv2.Resize(input, result, size, 0, 0, InterpolationFlags.Linear);
            return result;
        }

        /// <summary>
        /// 주어진 크기로 비율 맞춰서 가운데 정렬 Resize
        /// </summary>
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

        /// <summary>
        /// Rotate
        /// </summary>
        public static Mat Rotate(Mat input, RotateFlags rotate = RotateFlags.Rotate90Counterclockwise)
        {
            Mat result = new();
            Cv2.Rotate(input, result, rotate);
            return result;
        }

        /// <summary>
        /// 주어진 위치에 컬럼 삽입
        /// </summary>
        public static Mat CenterCol(Mat input, int width, int height, Scalar scalar, int start, int end)
        {
            Mat result = new(height, width, input.Type(), scalar);
            input.ColRange(0, input.Width).CopyTo(result.ColRange(start, end));
            return result;
        }

        /// <summary>
        /// 마지막 컬럼에 선(Line) 삽입
        /// </summary>
        public static Mat OverlapCol(Mat input, Mat line)
        {
            int cols = input.Cols;
            int offset = line.Width;

            input.ColRange(cols - offset, cols).CopyTo(line);
            return input;
        }

        /// <summary>
        /// 컬럼 시프트(우측)
        /// </summary>
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

        /// <summary>
        /// 컬럼 시프트(좌측)
        /// </summary>
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

        /// <summary>
        /// 컬럼 시프트(우측, 다른 방식)
        /// </summary>
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

        /// <summary>
        /// 행(Row) 시프트
        /// </summary>
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

        /// <summary>
        /// 컬럼 범위만 추출
        /// </summary>
        public static Mat SplitCol(Mat input, int start, int end)
        {
            int cols = end - start;
            int rows = input.Rows;
            Mat result = new(rows, cols, input.Type());
            input.ColRange(start, end).CopyTo(result.ColRange(0, cols));
            return result;
        }

        /// <summary>
        /// 행 범위만 추출
        /// </summary>
        public static Mat SplitRow(Mat input, int start, int end)
        {
            int rows = end - start;
            int cols = input.Cols;
            Mat result = new(rows, cols, input.Type());
            input.RowRange(start, end).CopyTo(result.RowRange(0, rows));
            return result;
        }

        /// <summary>
        /// 전체 평균값 반환
        /// </summary>
        public static double Mean(Mat input)
        {
            return (double)Cv2.Mean(input);
        }

        /// <summary>
        /// Flip(상하좌우 뒤집기)
        /// </summary>
        public static Mat Flip(Mat input, FlipMode mode)
        {
            Mat result = new();
            Cv2.Flip(input, result, mode);
            return result;
        }

        /// <summary>
        /// 1D 배열 -> Mat 변환
        /// </summary>
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

        /// <summary>
        /// 지정 영역(Crop)
        /// </summary>
        public static Mat Crop(Mat input, int x, int y, int width, int height)
        {
            Rect rect = new Rect(x, y, width, height);
            Mat result = new(input, rect);

            return result;
        }

        /// <summary>
        /// Crop된 결과를 입력 이미지에 덮어쓰기
        /// </summary>
        public static Mat CropPaste(Mat input, Mat cropped, int x, int y)
        {
            Mat result = input.Clone();
            Rect roi = new Rect(x, y, cropped.Width, cropped.Height);
            cropped.CopyTo(result[roi]);

            return result;
        }

        /// <summary>
        /// 1D 배열을 행단위로 복제하여 Mat 생성
        /// </summary>
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

        /// <summary>
        /// 지정 위치 픽셀 값 정보 문자열로 반환
        /// </summary>
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

        /// <summary>
        /// 행별 평균 반환
        /// </summary>
        public static double[] RowAverage(Mat mat)
        {
            int rows = mat.Rows;

            double[] average = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                average[i] = Cv2.Mean(mat.Row(i))[0];
            }

            return average;
        }

        /// <summary>
        /// 열별 평균 반환
        /// </summary>
        public static double[] ColAverage(Mat mat)
        {
            int cols = mat.Cols;

            double[] average = new double[cols];

            for (int i = 0; i < cols; i++)
            {
                average[i] = Cv2.Mean(mat.Col(i))[0];
            }

            return average;
        }

        /// <summary>
        /// ImShow
        /// </summary>
        public static void View(Mat input, string title = "title")
        {
            Mat output = new();
            Cv2.Resize(input, output, new Size(input.Width / 2, input.Height / 2));
            Cv2.NamedWindow(title, WindowFlags.AutoSize);
            Cv2.MoveWindow(title, 100, 100);
            Cv2.ImShow(title, output);

            Cv2.WaitKey(0);
        }

        /// <summary>
        /// ImageSource -> Mat 변환
        /// </summary>
        public static Mat ImageSourceToMat(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                return BitmapSourceConverter.ToMat(bitmapSource);
            }
            return new Mat();
        }

        /// <summary>
        /// BitmapSource -> Mat 변환
        /// </summary>
        public static Mat BitmapSourceToMat(BitmapSource bitmapSource)
        {
            return bitmapSource.ToMat();
        }

        /// <summary>
        /// 임계값(Threshold)
        /// </summary>
        public static Mat Threshold(Mat mat, double threshold, ThresholdTypes type)
        {
            Mat result = new();
            Cv2.Threshold(mat, result, threshold, byte.MaxValue, type);
            return result;
        }

        /// <summary>
        /// 임계값 처리 (채널별)
        /// </summary>
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

        /// <summary>
        /// 컬러 밸런스(RGB)
        /// </summary>
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

        /// <summary>
        /// 컬러 채널 분리 및 선택
        /// </summary>
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

        /// <summary>
        /// 노출, 오프셋, 감마 보정
        /// </summary>
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

        /// <summary>
        /// 커브(LUT) 적용 (포인트 지정)
        /// </summary>
        public static Mat Curve(Mat mat, Point[] points)
        {
            DateTime start = DateTime.Now;
            if (points.Length == 0)
            {
                return mat;
            }

            Mat result = Create(mat.Height, mat.Width, mat.Type());
            int channels = mat.Channels();
            Mat[] channelsArray = Cv2.Split(mat);

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
        public static byte[] GenerateLinearLUT(Point[] points)
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

        /// <summary>
        /// 주어진 점 목록을 바탕으로 스플라인 곡선 포인트 목록을 생성
        /// </summary>
        public static List<Point> GenerateSpline(List<Point> points)
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

        /// <summary>
        /// Catmull-Rom 스플라인 공식에 따라 주어진 t에서 좌표값 계산
        /// </summary>
        public static double CalculateSplineValue(double t, double p0, double p1, double p2, double p3)
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

        /// <summary>
        /// 가우시안 블러
        /// </summary>
        public static Mat GaussianBlur(Mat mat, int size)
        {
            Mat result = new();
            Cv2.GaussianBlur(mat, result, new Size(size, size), 1);
            return result;
        }

        /// <summary>
        /// 컨투어(윤곽선) 사각형 추출
        /// </summary>
        public static List<Rect> ContourRectangles(Mat mat, int minArea)
        {
            double maxArea = mat.Width * mat.Height * 0.9;
            // Contour 탐지
            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            //계산
            List<Rect> rectangles = [];
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

        /// <summary>
        /// 컨투어(윤곽선) 사각형 추출 (최대영역 제한)
        /// </summary>
        public static List<Rect> ContourRectangles(Mat mat, int minArea, int maxArea)
        {
            List<Rect> rectangles = [];
            Mat matGray = new();

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

        
        /// <summary>
        /// 랜덤 컬러 반환
        /// </summary>
        public static Scalar GetRandomColor()
        {

            byte r = (byte)_random.Next(0, 256);
            byte g = (byte)_random.Next(0, 256);
            byte b = (byte)_random.Next(0, 256);
            return new Scalar(b, g, r);
        }

        /// <summary>
        /// 입력값에 따라 스펙트럼에서 색상 반환
        /// </summary>
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

            byte[] hsv = [h, s, v];
            var hsvMat = new Mat(1, 1, MatType.CV_8UC3);
            Marshal.Copy(hsv, 0, hsvMat.Data, hsv.Length);

            Mat bgrMat = new ();
            Cv2.CvtColor(hsvMat, bgrMat, ColorConversionCodes.HSV2BGR);

            Vec3b bgrColor = bgrMat.At<Vec3b>(0, 0);

            return new Scalar(bgrColor.Item0, bgrColor.Item1, bgrColor.Item2);
        }

        /// <summary>
        /// Mat 배열 수평 합치기
        /// </summary>
        public static Mat HConcat(params Mat[] mats)
        {
            Mat result = new();
            Cv2.HConcat(mats, result);
            return result;
        }

        /// <summary>
        /// 밝기/명암 동시 조정
        /// </summary>
        public static Mat BrightnessAndContrast(Mat mat, double brightness, double contrast)
        {
            double contrastFactor = (100.0 + contrast) / 100.0;
            contrastFactor *= contrastFactor;

            Mat result = new();
            mat.ConvertTo(result, -1, contrastFactor, brightness);

            return result;
        }

        /// <summary>
        /// MatType에 맞는 타입 반환
        /// </summary>
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

        /// <summary>
        /// 비었는지 확인
        /// </summary>
        public static bool IsEmpty(Mat mat)
        {
            if (mat == null || mat.Empty())
            {
                return true;
            }
            return false;
        }

        public static Point ToOpenCvPoint(System.Windows.Point point)
        {
            return new Point((int)point.X, (int)point.Y);
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

        /// <summary>
        /// ImageSource Freeze (비동기)
        /// </summary>
        public static async Task<BitmapSource> CanFreezeImageSourceAsync(BitmapSource bitmap)
        {
            if (bitmap.CanFreeze)
            {
                await Task.Run(() => bitmap.Freeze());
            }
            return bitmap;
        }

    }
}
