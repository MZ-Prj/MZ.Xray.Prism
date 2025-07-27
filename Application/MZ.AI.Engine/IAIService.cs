using MZ.Domain.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using YoloDotNet.Models;

namespace MZ.AI.Engine
{
    public interface IAIService
    {
        YoloProcessor Yolo { get; set; }

        void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption);
        void Predict(MemoryStream stream, Size imageSize);
        void Predict(MemoryStream stream, Size imageSize, Size canvasSize);
        void Shift(int width);
        void Add();
        void Remove(int index = 0);
        int Count();
        void Save(string root, int start);
        ICollection<ObjectDetectionModel> Mapper(int start);
        ObservableCollection<CategoryModel> GetCategories();
        void ChangeObjectDetections(int index);
        void ChangeCategoryColor(int index, string color);
    }
}
