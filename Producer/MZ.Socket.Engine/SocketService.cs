using MZ.Domain.Models;
using MZ.Logger;
using MZ.Util;
using MZ.Vision;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MZ.Socket.Engine
{
    public class SocketService : BindableBase
    {
        #region Model
        private IpNetworkModel _ipNetwork = new();
        public IpNetworkModel IpNetwork { get => _ipNetwork; set => SetProperty(ref _ipNetwork, value); }   

        private ObservableCollection<FileModel> _files = [];
        public ObservableCollection<FileModel> Files { get => _files; set => SetProperty(ref _files, value); }
        #endregion

        public SocketService() { }


        public void LoadFile(string path)
        {
            var files = MZIO.GetFilesWithExtension(path, ".png")
                .OrderBy(file =>
                {
                    var filename = Path.GetFileNameWithoutExtension(file);
                    return int.TryParse(filename, out int index) ? index : int.MaxValue;
                })
                .ToList();

            VirtualDataParamters.Images.Clear();
            foreach (var file in files)
            {
                VirtualDataParamters.Images.Add(VisionBase.Load(file));
            }
            MZLogger.Information($"Load Files : {files.Count}");
        }



    }
}
