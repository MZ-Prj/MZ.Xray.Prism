using OpenCvSharp;
using Prism.Mvvm;

namespace MZ.Domain.Models
{
    public class CalibrationModel : BindableBase
    {
        private Mat _gain;
        public Mat Gain { get => _gain; set => SetProperty(ref _gain, value); }

        private Mat _offset;
        public Mat Offset { get => _offset; set => SetProperty(ref _offset, value); }

        private Mat _origin;
        public Mat Origin { get => _origin; set => SetProperty(ref _origin, value); }
    }
}