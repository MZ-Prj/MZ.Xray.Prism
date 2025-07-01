using MZ.Util;
using MZ.Vision;
using OpenCvSharp;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;

namespace MZ.Xray.Engine
{
    public class XrayDataSaveManager : BindableBase
    {

        private string _absoluteRoot = $".\\Save\\Image"; 
        public string AbsoluteRoot { get => _absoluteRoot; set => SetProperty(ref _absoluteRoot, value); }

        public void Image(Mat input, int start, int end, string path, string filename)
        {
            MZIO.TryMakeDirectory(path);

            string root = $"{path}/{filename}";

            if (!File.Exists(root))
            {
                var mat = VisionBase.SplitCol(input, start, end);
                VisionBase.SaveAsync(mat, root);
            }
        }

        public void Origin(Mat origin, Mat offset, Mat gain, int start, int end, string path, string filename)
        {
            MZIO.TryMakeDirectory(path);

            string root = $"{path}/{filename}";

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

        public void Video(List<Mat> list, string path, string filename)
        {
            MZIO.TryMakeDirectory(path);

            string root = $"{path}/{filename}";
            if (!File.Exists(root))
            {
                VisionBase.Save(list, root);
            }
        }

        public void AI()
        {

        }

        public void Database()
        {

        }


        public string GetPath()
        {
            return Path.GetFullPath($"{_absoluteRoot}\\{DateTime.Now:yyyy-MM-dd}");
        }

        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        }

        public (int, int) GetSplitPosition(int width, int sensorWidth, int frameCount)
        {
            int start = width - (sensorWidth * frameCount) >= 0 ? width - (sensorWidth * frameCount) : 0;
            start = start < 0 ? 0 : start;
            int end = width;

            return (start, end);
        }

    }
}
