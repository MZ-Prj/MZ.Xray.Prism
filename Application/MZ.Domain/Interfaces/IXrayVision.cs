using MZ.Domain.Enums;
using System;

namespace MZ.Domain.Interfaces
{
    public interface ICalibration
    {
        public double RelativeWidthRatio { get; set; }
        public double OffsetRegion { get; set; }
        public double GainRegion { get; set; }
        public double BoundaryArtifact { get; set; }
        public double ActivationThresholdRatio { get; set; }
        public int MaxImageWidth { get; set; }
        public int SensorImageWidth { get; set; }
    }
    public interface ICalibrationLine
    {
        public double Offset { get; set; }
        public double Gain { get; set; }
    }

    public interface IImage
    {
        public string Path { get; set; }
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public interface IVideo
    {
        public int Count { get; set; }

    }

    public interface IFilter
    {
        public float Zoom { get; set; }
        public float Sharpness { get; set; }
        public float Brightness { get; set; }
        public float Contrast { get; set; }
        public ColorRole ColorMode { get; set; }
    }

    public interface IMaterial
    {
        public double Blur { get; set; }
        public double HighLowRate { get; set; }
        public double Density { get; set; }
        public double EdgeBinary { get; set; }
        public double Transparency { get; set; }

    }

    public interface IMaterialControl
    {
        public double Y { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }

    }

    public interface IZeffectControl
    {
        public bool Check { get; set; }
        public string Content { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }

}
