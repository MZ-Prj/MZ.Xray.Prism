using MZ.Domain.Models;
using MZ.Util;
using MZ.Vision;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// Xray 데이터 저장 관리
    /// </summary>
    public class XrayDataSaveManager 
    {
        /// <summary>
        /// .exe 기준 경로
        /// </summary>
        public static string AbsoluteRoot = Path.Combine(AppContext.BaseDirectory, "Save");

        /// <summary>
        /// 일반 이미지 저장(검증용)
        /// </summary>
        public static void Base(Mat input, string root)
        {
            VisionBase.Save(input, root);
        }

        /// <summary>
        /// 이미지 저장
        /// </summary>
        public static void Base(Mat input, string path, string subPath, string filename)
        {
            MZIO.TryMakeDirectory(Path.Combine(path, subPath));
            string root = Path.Combine(path, subPath, filename);

            if (!File.Exists(path))
            {
                VisionBase.Save(input, root);
            }
        }
        /// <summary>
        /// 이미지 저장 (비동기)
        /// </summary>
        public static async Task BaseAsync(Mat input, string path, string subPath, string filename)
        {
            await Task.Run(() =>
            {
                Base(input, path, subPath, filename);
            });
        }
        /// <summary>
        /// 이미지 저장(App 경로 지정)
        /// </summary>
        public static void Image(Mat input, int start, int end, string path, string filename)
        {
            string subPath = "Image";

            MZIO.TryMakeDirectory(Path.Combine(path, subPath));
            string root = Path.Combine(path, subPath, filename);

            if (!File.Exists(path))
            {
                var mat = VisionBase.SplitCol(input, start, end);
                VisionBase.Save(mat, root);
            }
        }
        /// <summary>
        /// 이미지 저장(App 경로 지정) (비동기)
        /// </summary>
        public static async Task ImageAsync(Mat input, int start, int end, string path, string filename)
        {
            await Task.Run(() =>
            {
                Image(input, start, end, path, filename);
            });
        }
        /// <summary>
        /// 원본 16bit 저장(App 경로 지정)
        /// </summary>
        public static void Origin(Mat origin, Mat offset, Mat gain, int start, int end, string path, string filename)
        {
            string subPath = "Origin";

            MZIO.TryMakeDirectory(Path.Combine(path, subPath));
            string root = Path.Combine(path, subPath, filename);

            if (!File.Exists(root))
            {
                var split = VisionBase.SplitCol(origin, start, end);

                int height = offset.Height;

                var offsetInfo = VisionBase.Create(height, 1, MatType.CV_16UC1, new Scalar(offset.Width));
                var gainInfo = VisionBase.Create(height, 1, MatType.CV_16UC1, new Scalar(gain.Width));

                var mat = VisionBase.HConcat(offsetInfo, gainInfo, offset, gain, split);

                VisionBase.Save(mat, root);
            }
        }
        /// <summary>
        /// 원본 16bit 저장(App 경로 지정) (비동기)
        /// </summary>
        public static async Task OriginAsync(Mat origin, Mat offset, Mat gain, int start, int end, string path, string filename)
        {
            await Task.Run(() =>
            {
                Origin(origin, offset, gain, start, end, path, filename);
            });
        }
        /// <summary>
        /// 화면(UI) 저장(App 경로 지정)
        /// </summary>
        public static void Screen(Mat input, string path, string filename)
        {
            string subPath = "Screen";

            MZIO.TryMakeDirectory(Path.Combine(path, subPath));
            string root = Path.Combine(path, subPath, filename);

            if (!File.Exists(path))
            {
                VisionBase.Save(input, root);
            }
        }
        /// <summary>
        /// 화면(UI) 저장(App 경로 지정) (비동기)
        /// </summary>
        public static async Task ScreenAsync(Mat input, string path, string filename)
        {
            await Task.Run(() =>
            {
                Screen(input, path, filename);
            });
        }
        /// <summary>
        /// 비디오 저장(App 경로 지정)
        /// </summary>
        public static void Video(List<FrameModel> list, string path, string filename)
        {
            string subPath = "Video";

            MZIO.TryMakeDirectory(Path.Combine(path, subPath));
            string root = Path.Combine(path, subPath, filename);

            if (!File.Exists(root))
            {
                VisionBase.Save([.. list.Select(item => item.Image)], root);
            }
        }
        /// <summary>
        /// 비디오 저장(App 경로 지정) (비동기)
        /// </summary>
        public static async Task VideoAsync(List<FrameModel> list, string path, string filename)
        {
            await Task.Run(() =>
            {
                Video(list, path, filename);
            });
        }
        /// <summary>
        /// Path : ./yyyy-mm-dd
        /// </summary>
        public static string GetPath()
        {
            return Path.GetFullPath(Path.Combine(AbsoluteRoot, DateTime.Now.ToString("yyyy-MM-dd")));
        }
        /// <summary>
        /// CurrentTime : yyyy-MM-dd_HH-mm-ss-fff
        /// </summary>
        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        }

        /// <summary>
        /// Image 전체가 아닌 센서에서 판단한 기준하에 자를 영역 시작, 종료 값 계산
        /// </summary>
        public static  (int, int) GetSplitPosition(int width, int sensorWidth, int frameCount)
        {
            int start = width - (sensorWidth * frameCount) >= 0 ? width - (sensorWidth * frameCount) : 0;
            start = start < 0 ? 0 : start;
            int end = width;

            return (start, end);
        }

    }
}
