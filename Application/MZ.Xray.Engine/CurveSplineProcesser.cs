using MZ.Model;
using MZ.Vision;
using OpenCvSharp;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace MZ.Xray.Engine
{
    public class CurveSplineProcesser : BindableBase
    {

        private ObservableCollection<CurveControlModel> _points = [];
        public ObservableCollection<CurveControlModel> Points { get => _points; set => SetProperty(ref _points, value); }

        public CurveSplineProcesser()
        {
            InitializeMaterial();
        }
        /// <summary>
        /// 초기 설정
        /// </summary>
        public void InitializeMaterial()
        {
            Points.Add(new() { X = 0, Y = 0 });
            Points.Add(new() { X = 145, Y = 100 });
            Points.Add(new() { X = 255, Y = 255 });
        }

        public void AddPoint(CurveControlModel point)
        {
            Points.Add(point);
            RemoveDuplicatePoints();
            SortPointsByX();
        }

        public void RemovePoint(CurveControlModel point)
        {
            Points.Remove(point);
        }

        public void SortPointsByX()
        {
            var sortedPoints = Points.OrderBy(p => p.X).ToList();
            Points.Clear();
            foreach (var point in sortedPoints)
            {
                Points.Add(point);
            }
        }

        public Point[] ConvertPointToCVPoint()
        {
            return [.. Points.Select(p => new Point(p.X, p.Y))];
        }

        public Mat UpdateMat(Mat color)
        {
            return VisionBase.Curve(color, ConvertPointToCVPoint());
        }

        public void RemoveDuplicatePoints()
        {
            var unique = Points.GroupBy(p => new { p.X, p.Y })
                               .Select(g => g.First())
                               .ToList();

            Points.Clear();
            foreach (var point in unique)
            {
                Points.Add(point);
            }
        }
    }
}
