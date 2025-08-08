using MZ.Model;
using MZ.Logger;
using MZ.Util;
using MZ.Vision;
using Prism.Mvvm;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenCvSharp.WpfExtensions;

namespace MZ.Producer.Engine
{
    public class XrayDataProcesser : BindableBase
    {
        #region Dispatcher
        protected readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        #endregion

        private int _currentIndex = 0;
        public int CurrentIndex { get => _currentIndex; set => SetProperty(ref _currentIndex, value); }

        public ObservableCollection<FileModel> Models { get; set; } = [];

        public XrayDataProcesser() {}

        public void LoadFiles(string path)
        {
            CurrentIndex = 0;

            _dispatcher.Invoke(() =>
            {
                List<FileModel> files = [.. MZIO.GetFilesWithExtension(path, ".png")
                    .OrderBy(file =>
                    {
                        var filename = Path.GetFileNameWithoutExtension(file);
                        return int.TryParse(filename, out int index) ? index : int.MaxValue;
                    })
                    .Select((file, index) =>
                    {
                        using var image = VisionBase.Load(file);
                        return new FileModel
                        {
                            Index = index,
                            Path = Path.GetDirectoryName(file),
                            Name = Path.GetFileName(file),
                            ImageSource = image.ToBitmapSource(),
                            Width = image.Width,
                            Height = image.Height,
                            Type = image.Type()
                        };
                    })];

                Models.Clear();
                foreach (var file in files)
                {
                    Models.Add(file);
                }
            });

            MZLogger.Information($"Load Files : {Models.Count}");
        }

        public FileModel GetFile(int index)
        {
            FileModel model = Models[index];
            var image = VisionBase.Load(Path.Combine(model.Path, model.Name));

            return new()
            {
                Index = model.Index,
                Path = model.Path,
                Name = model.Name,
                Image = image,
                Type = image.Type(),
                Width = image.Width,
                Height = image.Height,
                Message = model.Message,
            };
        }

        public FileModel GetCurrentFile()
        {
            return GetFile(CurrentIndex);
        }
    }
}
