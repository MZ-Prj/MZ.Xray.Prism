using System;

namespace MZ.Domain.Interfaces
{
    public interface IAIOption
    {
        public string OnnxModel { get; set; }
        public int ModelType { get; set; }
        public bool Cuda { get; set; }
        public bool PrimeGpu { get; set; }
        public int GpuId { get; set; }
        public bool IsChecked { get; set; }
        public double Confidence { get; set; }
        public double IoU { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public interface ICategory
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsUsing { get; set; }
        public double Confidence { get; set; }
    }

    public interface IObjectDetection
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Confidence { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
