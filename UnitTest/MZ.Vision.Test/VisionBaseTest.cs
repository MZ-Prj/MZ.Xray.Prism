using OpenCvSharp;
using Xunit;
using static MZ.Vision.VisionEnum;
using Assert = Xunit.Assert;

namespace MZ.Vision.Test
{
    public class VisionTest
    {
        private static Mat MakeBgra(int w, int h, byte b, byte g, byte r, byte a)
        {
            return new Mat(h, w, MatType.CV_8UC4, new Scalar(b, g, r, a));
        }

        private static Mat MakeBgr(int w, int h, byte b, byte g, byte r)
        {
            return new Mat(h, w, MatType.CV_8UC3, new Scalar(b, g, r));
        }

        private static double InvokeLinearInterpolate(List<Point> p, double x)
        {
            var lut = VisionBase.GenerateLinearLUT([.. p]);
            return lut[(int)x];
        }

        #region 생성
        public static IEnumerable<object[]> MakeTestData =>
            [
                [MatType.CV_8UC1, 255],
                [MatType.CV_8UC3, 255],
                [MatType.CV_16UC1, (double)ushort.MaxValue],
                [MatType.CV_16UC3, (double)ushort.MaxValue],
            ];

        [Theory]
        [MemberData(nameof(MakeTestData))]
        public void Make_ReturnsMaxValue(MatType type, double expected)
        {
            Scalar result = VisionBase.Make(type);
            Assert.Equal(expected, result.Val0);
        }

        [Fact]
        public void MakeZeros_ReturnsZeros_ByType()
        {
            Assert.Equal(0, VisionBase.MakeZeros(MatType.CV_8UC1).Val0);
            Assert.Equal(0, VisionBase.MakeZeros(MatType.CV_8UC3).Val0);
            Assert.Equal(0, VisionBase.MakeZeros(MatType.CV_16UC1).Val0);
            Assert.Equal(0, VisionBase.MakeZeros(MatType.CV_16UC3).Val0);
        }

        [Fact]
        public void Create_WithScalarValue_SetsCorrectValue()
        {
            int height = 5, width = 5;
            Scalar value = new(100, 100, 100);
            Mat mat = VisionBase.Create(height, width, MatType.CV_8UC3, value);
            Vec3b pixel = mat.At<Vec3b>(0, 0);

            Assert.Equal(100, pixel.Item0);
            Assert.Equal(100, pixel.Item1);
            Assert.Equal(100, pixel.Item2);
        }

        [Fact]
        public void Create_FillsWithMaxScalar()
        {
            var mat = VisionBase.Create(3, 2, MatType.CV_8UC1);
            Assert.Equal(255, mat.At<byte>(0, 0));
        }

        #endregion

        #region Clone, Save/Load

        [Fact]
        public void Clone_MatCloneIsEqualButDifferentInstance()
        {
            Mat original = new(5, 5, MatType.CV_8UC1, Scalar.All(50));
            Mat clone = VisionBase.Clone(original);

            Assert.Equal(original.Size(), clone.Size());
            Assert.False(ReferenceEquals(original, clone));
            Assert.NotEqual(original.Data, clone.Data);
        }

        [Fact]
        public void SaveAndLoad_ReturnsNonEmptyMat()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            Mat original = new(10, 10, MatType.CV_8UC3, new Scalar(255, 0, 0));
            VisionBase.Save(original, tempFile);
            Mat loaded = VisionBase.Load(tempFile);

            Assert.False(loaded.Empty());
            File.Delete(tempFile);
        }

        [Fact]
        public async Task SaveAsync_SavesAndLoad_ReturnsNonEmptyMat()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            Mat original = new(10, 10, MatType.CV_8UC3, new Scalar(0, 255, 0));
            await VisionBase.SaveAsync(original, tempFile);
            Mat loaded = VisionBase.Load(tempFile);

            Assert.False(loaded.Empty());
            File.Delete(tempFile);
        }

        #endregion

        #region Mat 처리

        [Fact]
        public void RoI_ReturnsSubMatOfCorrectSize()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1, Scalar.All(100));
            Mat roi = VisionBase.RoI(mat, 2, 2, 5, 5);

            Assert.Equal(new Size(5, 5), roi.Size());
        }

        [Fact]
        public void MinMax_CV_8UC1_ReturnsCorrectMinMax()
        {
            Mat mat = new(1, 5, MatType.CV_8UC1);
            byte[] values = [10, 50, 20, 5, 255];
            for (int i = 0; i < 5; i++)
            {
                mat.Set(0, i, values[i]);
            }
            (double min, double max) = VisionBase.MinMax(mat);

            Assert.Equal(5, min);
            Assert.Equal(255, max);
        }

        [Fact]
        public void Contrast_AdjustsContrast()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1, new Scalar(100));
            Mat contrasted = VisionBase.Contrast(mat, 1);

            Assert.NotEqual(mat.At<byte>(0, 0), contrasted.At<byte>(0, 0));
        }

        [Fact]
        public void Blur_BlursImage()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1);
            Cv2.Randu(mat, 0, 255);
            Mat blurred = VisionBase.Blur(mat, 5);

            Assert.NotEqual(mat.At<byte>(0, 0), blurred.At<byte>(0, 0));
        }

        [Fact]
        public void Bright_IncreasesBrightness()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1, new Scalar(50));
            Mat brightened = VisionBase.Bright(mat, 10);

            Assert.True(brightened.At<byte>(0, 0) > mat.At<byte>(0, 0));
        }

        [Fact]
        public void Convert16BitTo8Bit_ConvertsProperly()
        {
            Mat mat16 = new(10, 10, MatType.CV_16UC1, new Scalar(300));
            Mat mat8 = VisionBase.Convert16BitTo8Bit(mat16);

            Assert.Equal(MatType.CV_8UC1, mat8.Type());
        }

        [Fact]
        public void Resize_ResizesMatCorrectly()
        {
            Mat mat = new(20, 20, MatType.CV_8UC1, new Scalar(100));
            Mat resized = VisionBase.Resize(mat, 10, 10);

            Assert.Equal(new Size(10, 10), resized.Size());
        }

        [Fact]
        public void ResizeWithAspectRatio_ProducesSquareOutput()
        {
            Mat mat = new(100, 200, MatType.CV_8UC3, new Scalar(50, 100, 150));
            int size = 150;
            Mat resized = VisionBase.ResizeWithAspectRatio(mat, size);

            Assert.Equal(new Size(size, size), resized.Size());
        }

        [Fact]
        public void Rotate_RotatesMatCorrectly()
        {
            Mat mat = new(10, 20, MatType.CV_8UC1, Scalar.All(0));
            Mat rotated = VisionBase.Rotate(mat, RotateFlags.Rotate90Clockwise);

            Assert.Equal(new Size(10, 20), rotated.Size());
        }

        [Fact]
        public void BlendWithBackground_DropsAlphaAndPreservesSize()
        {
            var mat = MakeBgra(10, 10, 10, 20, 30, 0);
            var result = VisionBase.BlendWithBackground(mat);

            Assert.Equal(MatType.CV_8UC3, result.Type());
            Assert.Equal(mat.Size(), result.Size());

            var px = result.At<Vec3b>(0, 0);
            Assert.True(px.Item0 >= 250 && px.Item1 >= 250 && px.Item2 >= 250);
        }
        #endregion

        #region 배열 처리

        [Fact]
        public void ShiftCol_ShiftsAndAppendsLine()
        {
            Mat mat = new(1, 10, MatType.CV_8UC1);
            for (int i = 0; i < 10; i++)
            {
                mat.Set(0, i, (byte)i);
            }
            Mat line = new(1, 2, MatType.CV_8UC1, new Scalar(100));
            Mat result = VisionBase.ShiftCol(mat, line);

            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(mat.At<byte>(0, i + 2), result.At<byte>(0, i));
            }
            for (int i = 8; i < 10; i++)
            {
                Assert.Equal(100, result.At<byte>(0, i));
            }
        }

        [Fact]
        public void ShiftColLeftAndRight_WorkCorrectly()
        {
            Mat mat = new(1, 10, MatType.CV_8UC1);
            for (int i = 0; i < 10; i++)
            {
                mat.Set(0, i, (byte)i);
            }
            Mat line = new(1, 2, MatType.CV_8UC1, new Scalar(50));
            Mat leftShift = VisionBase.ShiftColLeft(mat, line);

            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(mat.At<byte>(0, i + 2), leftShift.At<byte>(0, i));
            }
            for (int i = 8; i < 10; i++)
            {
                Assert.Equal(50, leftShift.At<byte>(0, i));
            }

            Mat rightShift = VisionBase.ShiftColRight(mat, line);
            for (int i = 0; i < 2; i++)
            {
                Assert.Equal(50, rightShift.At<byte>(0, i));
            }
            for (int i = 2; i < 10; i++)
            {
                Assert.Equal(mat.At<byte>(0, i - 2), rightShift.At<byte>(0, i));
            }
        }

        [Fact]
        public void ShiftRow_ShiftsAndAppendsLine()
        {
            Mat mat = new(10, 1, MatType.CV_8UC1);
            for (int i = 0; i < 10; i++)
            {
                mat.Set(i, 0, (byte)i);
            }
            Mat line = new(2, 1, MatType.CV_8UC1, new Scalar(99));
            Mat result = VisionBase.ShiftRow(mat, line);
            for (int i = 0; i < 8; i++)
            {
                Assert.Equal(mat.At<byte>(i + 2, 0), result.At<byte>(i, 0));
            }
            for (int i = 8; i < 10; i++)
            {
                Assert.Equal(99, result.At<byte>(i, 0));
            }
        }

        [Fact]
        public void ExpandArrayToMat_CreatesExpectedMat()
        {
            ushort[] array = [1, 2, 3, 4, 5];
            Mat expanded = VisionBase.ExpandArrayToMat(array, 5, MatType.CV_16UC1);

            for (int i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], expanded.At<ushort>(i, 0));
            }
        }

        [Fact]
        public void CropAndCropPaste_WorkCorrectly()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1, new Scalar(0));
            Mat cropped = new(5, 5, MatType.CV_8UC1, new Scalar(100));
            Mat pasted = VisionBase.CropPaste(mat, cropped, 2, 2);

            Assert.Equal(100, pasted.At<byte>(2, 2));
        }

        [Fact]
        public void Convert1DArrayToMat_CreatesMatWithCorrectDimensions()
        {
            ushort[] array = [10, 20, 30, 40];

            Mat mat = VisionBase.Convert1DArrayToMat(array, 2);

            Assert.Equal(2, mat.Rows);
            Assert.Equal(array.Length, mat.Cols);
        }

        #endregion

        #region 기타 기능

        [Fact]
        public void InformationOnPixel_ReturnsExpectedInfo()
        {
            Mat mat = new(5, 5, MatType.CV_16UC1);

            mat.Set(2, 2, (ushort)123);

            string info = VisionBase.InformationOnPixel(mat, new System.Windows.Point(2, 2));

            Assert.Contains("123", info);
        }

        [Fact]
        public void RowAverageAndColAverage_ReturnCorrectAverages()
        {
            Mat mat = new(2, 3, MatType.CV_8UC1);

            byte[,] values = { { 10, 20, 30 }, { 40, 50, 60 } };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mat.Set(i, j, values[i, j]);
                }
            }

            double[] rowAvg = VisionBase.RowAverage(mat);
            double[] colAvg = VisionBase.ColAverage(mat);

            Assert.Equal(20, rowAvg[0], 1);
            Assert.Equal(50, rowAvg[1], 1);
            Assert.Equal(25, colAvg[0], 1);
            Assert.Equal(35, colAvg[1], 1);
            Assert.Equal(45, colAvg[2], 1);
        }

        [Fact]
        public void Threshold_AppliesThresholding()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1, new Scalar(100));
            Mat thresh = VisionBase.Threshold(mat, 50, ThresholdTypes.Binary);

            Assert.Equal(255, thresh.At<byte>(0, 0));
        }

        [Fact]
        public void GaussianBlur_BlursImage()
        {
            Mat mat = new(10, 10, MatType.CV_8UC1);
            Cv2.Randu(mat, 0, 255);
            Mat blurred = VisionBase.GaussianBlur(mat, 3);

            Assert.NotEqual(mat.At<byte>(0, 0), blurred.At<byte>(0, 0));
        }

        [Fact]
        public void ContourRectangles_WithMaxArea_FindsRectangles()
        {
            Mat mat = Mat.Zeros(100, 100, MatType.CV_8UC1);
            Cv2.Rectangle(mat, new Rect(10, 10, 30, 30), Scalar.White, -1);
            List<Rect> rects = VisionBase.ContourRectangles(mat, 50, 1000);

            Assert.NotEmpty(rects);
        }

        [Fact]
        public void GetRandomColor_ReturnsValidScalar()
        {
            Scalar color = VisionBase.GetRandomColor();

            Assert.InRange(color.Val0, 0, 255);
            Assert.InRange(color.Val1, 0, 255);
            Assert.InRange(color.Val2, 0, 255);
        }

        [Fact]
        public void GetColorFromSpectrom_ReturnsNonZeroColor()
        {
            Scalar color = VisionBase.GetColorFromSpectrom(50, 0, 100);

            Assert.NotEqual(new Scalar(0, 0, 0), color);
        }

        [Fact]
        public void FillPoly_FillsGivenPolygon()
        {
            var mats = new Mat(50, 50, MatType.CV_8UC1, Scalar.All(0));
            var points = new List<Point> { new(10, 10), new(40, 10), new(40, 40), new(10, 40) };

            VisionBase.FillPoly(mats, points, Scalar.All(200));

            Assert.True(mats.Sum().Val0 > 0);
        }

        [Fact]
        public void CompareHistogram_SameImagesReturnZeroOnChiSquare()
        {
            var a = MakeBgr(20, 10, 10, 10, 10);
            var b = a.Clone();
            var score = VisionBase.CompareHistogram(a, b, HistCompMethods.Chisqr);

            Assert.InRange(score, 0, 1e-6);
        }

        [Fact]
        public void Histogram_EqualizesContrastOn8bitGray()
        {
            var gray = new Mat(1, 256, MatType.CV_8UC1);
            for (int x = 0; x < 256; x++)
            {
                gray.Set(0, x, (byte)x);
            }
            var histogram = VisionBase.Histogram(gray);
            Assert.Equal(gray.Size(), histogram.Size());
            Assert.Equal(MatType.CV_8UC1, histogram.Type());

        }

        [Fact]
        public void PlotFilledHistogram_ReturnsOverlayImageWithAlpha()
        {
            var mat = MakeBgr(64, 64, 10, 10, 10);
            var histogram = VisionBase.PlotFilledHistogram(mat);

            Assert.Equal(new Size(256, 256), histogram.Size());
            Assert.Equal(MatType.CV_8UC4, histogram.Type());
        }

        [Fact]
        public void Sharp_IncreasesEdgeEnergy()
        {
            var mat = new Mat(64, 64, MatType.CV_8UC1, Scalar.All(120));
            Cv2.Rectangle(mat, new Rect(16, 16, 32, 32), Scalar.All(200), -1);

            var before = new Mat(); Cv2.Laplacian(mat, before, MatType.CV_16S);
            var sharp = VisionBase.Sharp(mat, level: 1);
            var after = new Mat(); Cv2.Laplacian(sharp, after, MatType.CV_16S);

            double s1 = Cv2.Mean(Cv2.Abs(before)).Val0;
            double s2 = Cv2.Mean(Cv2.Abs(after)).Val0;

            Assert.True(s2 > s1);
        }

        [Fact]
        public void CenterCol_PlacesInputIntoGivenRangeAndPadsWithScalar()
        {
            var mat = new Mat(10, 6, MatType.CV_8UC1, Scalar.All(50));
            var result = VisionBase.CenterCol(mat, width: 12, height: 10, scalar: Scalar.All(0), start: 3, end: 9);

            Assert.Equal(0, result.At<byte>(0, 0));

            Assert.Equal(50, result.At<byte>(0, 3));
            Assert.Equal(50, result.At<byte>(0, 8));
        }

        [Fact]
        public void OverlapCol_CopiesInputsLastColumnsIntoLine_AndReturnsInput()
        {
            var mat = new Mat(5, 8, MatType.CV_8UC1, Scalar.All(10));
            var line = new Mat(5, 3, MatType.CV_8UC1, Scalar.All(0));

            var ret = VisionBase.OverlapCol(mat, line);

            for (int r = 0; r < 5; r++)
            {
                Assert.Equal(mat.At<byte>(r, 5), line.At<byte>(r, 0));
                Assert.Equal(mat.At<byte>(r, 7), line.At<byte>(r, 2));
            }

            Assert.True(ReferenceEquals(mat, ret));
        }

        [Fact]
        public void SplitCol_ReturnsRequestedRange()
        {
            var mat = new Mat(4, 10, MatType.CV_8UC1, Scalar.All(7));
            var part = VisionBase.SplitCol(mat, 2, 6);

            Assert.Equal(4, part.Rows);
            Assert.Equal(4, part.Cols);
        }

        [Fact]
        public void SplitRow_ReturnsRequestedRange()
        {
            var mat = new Mat(10, 4, MatType.CV_8UC1, Scalar.All(7));
            var part = VisionBase.SplitRow(mat, 3, 8);

            Assert.Equal(5, part.Rows);
            Assert.Equal(4, part.Cols);
        }

        [Fact]
        public void Mean_ReturnsAverageOfSingleChannel()
        {
            var mat = new Mat(1, 4, MatType.CV_8UC1);
            mat.Set(0, 0, 0); mat.Set(0, 1, 10); mat.Set(0, 2, 20); mat.Set(0, 3, 30);
            var avg = VisionBase.Mean(mat);
            Assert.InRange(avg, 15 - 1e-6, 15 + 1e-6);
        }

        [Fact]
        public void Flip_FlipsHorizontally()
        {
            var mat = new Mat(1, 3, MatType.CV_8UC1);
            mat.Set(0, 0, 1); mat.Set(0, 1, 2); mat.Set(0, 2, 3);
            var flipped = VisionBase.Flip(mat, FlipMode.Y);

            Assert.Equal(3, flipped.At<byte>(0, 0));
            Assert.Equal(1, flipped.At<byte>(0, 2));
        }

        [Fact]
        public void Crop_ReturnsSubmatrix()
        {
            var mat = new Mat(10, 10, MatType.CV_8UC1, Scalar.All(5));
            var c = VisionBase.Crop(mat, 2, 3, 4, 5);

            Assert.Equal(5, c.Rows);
            Assert.Equal(4, c.Cols);
            Assert.Equal(5, c.At<byte>(0, 0));
        }

        [Fact]
        public void Threshold_ZeroesOutsideRange_PerChannel()
        {
            var mat = new Mat(1, 3, MatType.CV_8UC3);
            mat.Set(0, 0, new Vec3b(10, 100, 200));
            mat.Set(0, 1, new Vec3b(50, 150, 250));
            mat.Set(0, 2, new Vec3b(0, 255, 128));

            var r = VisionBase.Threshold(mat, lower: 100, upper: 200);

            var p0 = r.At<Vec3b>(0, 0);
            Assert.Equal(0, p0.Item0);
            Assert.Equal(100, p0.Item1);
            Assert.Equal(200, p0.Item2);
        }

        [Fact]
        public void ColorBalance_AddsPerChannel_BgrOrder()
        {
            var mat = MakeBgr(2, 1, 10, 20, 30);
            var r = VisionBase.ColorBalance(mat, red: 5, green: 4, blue: 3);
            var p = r.At<Vec3b>(0, 0);
            Assert.Equal(13, p.Item0);
            Assert.Equal(24, p.Item1);
            Assert.Equal(35, p.Item2);
        }

        [Fact]
        public void SelectColor_Gray_ProducesGrayWithAlphaPreserved()
        {
            var mat = MakeBgra(2, 2, 10, 20, 30, 200);
            var gray = VisionBase.SelectColor(mat, ColorEnum.Gray);
            Assert.Equal(MatType.CV_8UC4, gray.Type());
            var a = gray.Split()[3];
            Assert.Equal(200, a.At<byte>(0, 0));
            var ch = gray.Split();
            Assert.Equal(0, Cv2.CountNonZero(ch[0] - ch[1]));
            Assert.Equal(0, Cv2.CountNonZero(ch[1] - ch[2]));
        }


        [Fact]
        public void SelectColor_Red_KeepsRedAndAlpha_ZeroesOthers()
        {
            var mat = MakeBgra(1, 1, 5, 6, 200, 123);
            var r = VisionBase.SelectColor(mat, ColorEnum.Red);
            var ch = r.Split();
            Assert.Equal(0, ch[0].At<byte>(0, 0));
            Assert.Equal(0, ch[1].At<byte>(0, 0));
            Assert.Equal(200, ch[2].At<byte>(0, 0));
            Assert.Equal(123, ch[3].At<byte>(0, 0));
        }

        [Fact]
        public void Exposure_ClampsAndModifiesRgb_NotAlpha()
        {
            var mat = MakeBgra(1, 1, 50, 100, 150, 77);
            var r = VisionBase.Exposure(mat, exposure: 5, offset: 0.1, gammaCorrection: 1.0);
            Assert.Equal(77, r.Split()[3].At<byte>(0, 0));
        }

        [Fact]
        public void GenerateLinearLUT_ClampsAndInterpolates()
        {
            var lut = VisionBase.GenerateLinearLUT([new Point(0, 0), new Point(255, 255)]);
            Assert.Equal(0, lut[0]);
            Assert.Equal(255, lut[255]);
            Assert.Equal(128, lut[128]);
        }

        [Fact]
        public void LinearInterpolate_BetweenTwoPoints()
        {
            var pts = new List<Point> { new(0, 0), new(100, 200) };
            var y = InvokeLinearInterpolate(pts, 50); 
            Assert.Equal(100, y, 1);

        }

        [Fact]
        public void GenerateSpline_ReturnsDensePoints()
        {
            var s = VisionBase.GenerateSpline([new(0, 0), new(128, 200), new(255, 255)]);
            Assert.True(s.Count > 0);
            Assert.True(s.First().X >= 0 && s.Last().X <= 255);
        }

        [Fact]
        public void CalculateSplineValue_ProducesContinuousCurve()
        {
            var v1 = VisionBase.CalculateSplineValue(0.0, 0, 0, 100, 100);
            var v2 = VisionBase.CalculateSplineValue(1.0, 0, 0, 100, 100);
            Assert.InRange(v1, -1, 256);
            Assert.InRange(v2, -1, 256);
        }

        [Fact]
        public void Curve_AppliesLUT_ToRgbChannelsOnly()
        {
            var mat = MakeBgra(1, 1, 10, 20, 30, 77);
            var points = new[] { new Point(0, 0), new Point(255, 255) };
            var r = VisionBase.Curve(mat, points);
            Assert.Equal(77, r.Split()[3].At<byte>(0, 0)); 
        }

        [Fact]
        public void ContourRectangles_FindsConvexRectsWithinArea()
        {
            var mat = new Mat(200, 200, MatType.CV_8UC1, Scalar.All(0));
            Cv2.Rectangle(mat, new Rect(50, 50, 80, 60), Scalar.All(255), -1);
            HierarchyIndex[] h = [];
            var rects = VisionBase.ContourRectangles(mat, minArea: 100, hierarchy: h);
            Assert.Contains(rects, r => r.Width == 80 && r.Height == 60);
        }

        [Fact]
        public void HConcat_ConcatenatesWidth()
        {
            var a = MakeBgr(10, 5, 0, 0, 0);
            var b = MakeBgr(7, 5, 0, 0, 0);
            var c = VisionBase.HConcat(a, b);
            Assert.Equal(17, c.Width);
            Assert.Equal(5, c.Height);
        }

        [Fact]
        public void BrightnessAndContrast_AppliesAffineTransform()
        {
            var mat = MakeBgr(1, 1, 10, 10, 10);
            var r = VisionBase.BrightnessAndContrast(mat, brightness: 10, contrast: 0);
            var p = r.At<Vec3b>(0, 0);

            Assert.True(p.Item0 >= 10 && p.Item1 >= 10 && p.Item2 >= 10);
        }

        [Fact]
        public void GetTypeFromMatType_MapsKnownTypes()
        {
            Assert.Equal(typeof(byte), VisionBase.GetTypeFromMatType(MatType.CV_8UC1));
            Assert.Equal(typeof(Vec3b), VisionBase.GetTypeFromMatType(MatType.CV_8UC3));
            Assert.Equal(typeof(float), VisionBase.GetTypeFromMatType(MatType.CV_32FC1));
            Assert.Equal(typeof(double), VisionBase.GetTypeFromMatType(MatType.CV_64FC1));
        }

        [Fact]
        public void IsEmpty_TrueForNullOrEmpty_FalseForNonEmpty()
        {
            Mat? nullMat = null;
            Assert.True(VisionBase.IsEmpty(nullMat));
            Assert.True(VisionBase.IsEmpty(new Mat()));

            var mat = MakeBgr(2, 2, 1, 2, 3);
            Assert.False(VisionBase.IsEmpty(mat));
        }

        #endregion

    }
}