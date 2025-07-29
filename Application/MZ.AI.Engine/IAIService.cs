using MZ.Domain.Entities;
using MZ.Domain.Models;
using MZ.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using YoloDotNet.Models;

namespace MZ.AI.Engine
{
    public interface IAIService
    {
        YoloProcessor Yolo { get; set; }
        void Create(string path);
        void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption);
        void Load(AIOptionEntity entity);
        void Predict(MemoryStream stream, Size imageSize);
        void Predict(MemoryStream stream, Size imageSize, Size canvasSize, int offsetX = 0);
        void Shift(int width);
        Task ShiftAsync(int width);
        void Add();
        void Remove(int index = 0);
        int Count();
        Task Save(string path, string time, int start = 0);
        Task Save(string path, string time, MemoryStream stream);
        ICollection<ObjectDetectionModel> Mapper(int start);
        ObservableCollection<CategoryModel> GetCategories();
        void ChangeObjectDetections(int index);
        void ChangeCategoryColor(int index, string color);
        bool IsSavedModel(string root);
        AIOptionCreateRequest YoloToRequest();
    }
}
