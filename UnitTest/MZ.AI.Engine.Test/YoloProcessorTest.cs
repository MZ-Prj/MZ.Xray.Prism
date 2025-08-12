using MZ.Vision;
using SkiaSharp;
using OpenCvSharp;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.AI.Engine.Test
{
    public class YoloProcessorTest
    {
        private static string ImagePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Model", "sample.png");
        }

        private static string ModelPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Model", "sample.onnx");
        }

        private static MemoryStream LoadSample(string path)
        {
            Assert.True(File.Exists(path), path);
            return new MemoryStream(File.ReadAllBytes(path));
        }

        [Fact]
        public void Load_SampleImage_CanCreateSkImage()
        {
            var sample = LoadSample(ImagePath());
            using var image = SKImage.FromEncodedData(sample);

            Assert.NotNull(image);
            Assert.True(image.Width > 0 && image.Height > 0);
        }

        [Fact]
        public void Predict_WithSampleImageAndModel_ObjectDetects()
        {
            var image = ImagePath();
            var model = ModelPath();

            var yolo = new YoloProcessor();

            YoloOptions yoloOption = new()
            {
                OnnxModel = model,
                ModelType = ModelType.ObjectDetection,
                Cuda = false,
                PrimeGpu = false,
            };

            yolo.Create(yoloOption, new (), null);

            var mat = VisionBase.Load(image);
            if (mat.Type() == MatType.CV_8UC4)
            {
                mat = VisionBase.BlendWithBackground(mat);
            }

            var imageSize = new System.Windows.Size(mat.Width, mat.Height);
            var canvasSize = new System.Windows.Size(mat.Width, mat.Height);

            yolo.Predict(mat.ToMemoryStream(), imageSize, canvasSize, offsetX: 0);

            var result = yolo.ObjectDetections.ToList();

            Assert.True(result.Count > 0);
            Assert.NotNull(result);
            Assert.All(result, d => Assert.True(d.Confidence >= 0));
            Assert.All(result, d => Assert.False(string.IsNullOrWhiteSpace(d.Name)));
        }
    }
}
