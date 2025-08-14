using Microsoft.Extensions.Configuration;
using Moq;
using MZ.AI.Engine;
using MZ.Domain.Interfaces;
using MZ.Infrastructure;
using MZ.Util;
using MZ.Vision;
using OpenCvSharp;
using Prism.Events;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Xray.Engine.Test
{
    public class XrayServiceTest
    {

        private static string LinePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Line");
        }
        private static string OffsetPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Offset", "offset.png");
        }

        private static string GainPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Gain", "gain.png");
        }

        private static XrayService CreateAlgorithmOnlyService()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AI:Path"] = Path.Combine(Path.GetTempPath(), "_xray_ai_"),
                    ["AI:DownloadLink"] = "about:blank",
                })
                .Build();

            var ev = new Mock<IEventAggregator>(MockBehavior.Loose);
            var db = new Mock<IDatabaseService>(MockBehavior.Loose);
            var ai = new Mock<IAIService>(MockBehavior.Loose);
            
            return new XrayService(cfg, ev.Object, db.Object, ai.Object);
        }

        /// <summary>
        /// Calculation 알고리즘 검증 수행
        /// </summary>
        [Fact]
        public void Calculation_FromLine_Calculation()
        {
            string linePath = LinePath();
            var files = MZIO.GetFilesWithExtension(linePath, "png").ToList();
            Assert.NotEmpty(files);

            // 랜덤 선택 100 회 시행
            int sampleCount = Math.Min(100, files.Count);
            var rng = new Random(unchecked((int)DateTime.UtcNow.Ticks));
            var selected = files
                .OrderBy(_ => rng.Next()) 
                .Take(sampleCount)
                .ToList();

            var svc = CreateAlgorithmOnlyService();
            // 실 offset/gain 설정
            svc.Calibration.Offset = VisionBase.Load(OffsetPath());
            svc.Calibration.Gain = VisionBase.Load(GainPath());

            foreach (var file in selected)
            {
                // Arrange
                var line = VisionBase.Load(file);
                Assert.True(line.Type() == MatType.CV_16UC1 || line.Type() == MatType.CV_16U);

                // Act
                var (high, low, color, z) = svc.Calculation(line);

                // Assert
                Assert.Equal(line.Cols, high.Cols);
                Assert.Equal(line.Rows / 2, high.Rows);
                Assert.Equal(MatType.CV_16UC1, high.Type());
                Assert.Equal(MatType.CV_16UC1, low.Type());
                Assert.Equal(MatType.CV_8UC4, color.Type());
                Assert.Equal(MatType.CV_8UC1, z.Type());
            }
        }



        /// <summary>
        /// Process 알고리즘 검증 수행
        /// </summary>
        [Fact]
        public async Task Process_AllLine_Run()
        {
            string linePath = LinePath();
            var files = MZIO.GetFilesWithExtension(linePath, "png").ToList();

            Assert.NotEmpty(files);

            var svc = CreateAlgorithmOnlyService();

            svc.Calibration.Offset = VisionBase.Load(OffsetPath());
            svc.Calibration.Gain = VisionBase.Load(GainPath());

            foreach (var file in files)
            {
                var origin = VisionBase.Load(file);
                var ex = await Record.ExceptionAsync(() => svc.ProcessTest(origin));

                // Assert
                Assert.Null(ex);
            }
        }
    }
}
