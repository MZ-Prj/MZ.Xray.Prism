using MZ.Domain.Interfaces;
using Prism.Mvvm;
using System;

namespace MZ.Domain.Models
{
    public class ObjectDetectionOptionModel : BindableBase
    {
        private double _confidence = 0.5;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }

        private double _iou = 0.95;
        public double IoU { get => _iou; set => SetProperty(ref _iou, value); }

        private double _scaleX = 1.0;
        public double ScaleX { get => _scaleX; set => SetProperty(ref _scaleX, value); }

        private double _scaleY = 1.0;
        public double ScaleY { get => _scaleY; set => SetProperty(ref _scaleY, value); }
    }

    public class CategoryModel : BindableBase, ICategory
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }

        private bool _isUsing;
        public bool IsUsing { get => _isUsing; set => SetProperty(ref _isUsing, value); }

        private double _confidence;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }
    }

    public class ObjectDetectionModel : BindableBase, IObjectDetection
    {

        private int _index;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _color;
        public string Color { get => _color; set => SetProperty(ref _color, value); }

        private double _confidence;
        public double Confidence { get => _confidence; set => SetProperty(ref _confidence, value); }

        private double _x;
        public double X { get => _x; set => SetProperty(ref _x, value); }
        
        private double _y;
        public double Y { get => _y; set => SetProperty(ref _y, value); }

        private double _width;
        public double Width { get => _width; set => SetProperty(ref _width, value); }

        private double _height;
        public double Height { get => _height; set => SetProperty(ref _height, value); }

        private DateTime _createDate;
        public DateTime CreateDate { get => _createDate; set => SetProperty(ref _createDate, value); }

        //UI
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private bool _isVisibility = true;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }

        private bool _isBlink;
        public bool IsBlink { get => _isBlink; set => SetProperty(ref _isBlink, value); }

        private double _offsetX = 0;
        public double OffsetX { get => _offsetX; set => SetProperty(ref _offsetX, value); }

    }
}
