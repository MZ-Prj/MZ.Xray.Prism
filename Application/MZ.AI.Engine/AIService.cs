using MZ.Domain.Models;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoloDotNet.Models;

namespace MZ.AI.Engine
{
    public class AIService : BindableBase, IAIService
    {
        private YoloProcessor _yolo;
        public YoloProcessor Yolo { get => _yolo; set => SetProperty(ref _yolo, value); }

        public void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption)
        {
            if (Yolo == null)
            {
                return;
            }

            var yolo = new YoloProcessor
            {
                YoloOption = yoloOption,
                ObjectDetectionOption = objectDetectionOption,
                Categories = categories,
            };

            Yolo = yolo;
        }

        public void Predict(MemoryStream stream, Size imageSize, Size canvasSize)
        {
            Yolo.Predict(stream, imageSize, canvasSize);
        }

        public void Shift(int width)
        {
            foreach (var detection in Yolo.ObjectDetections)
            {
                detection.X -= (width * Yolo.ObjectDetectionOption.ScaleX);
            }
        }

        public void Add()
        {
            Yolo.ObjectDetectionsList.Add(Yolo.ObjectDetections);
        }

        public void Remove(int index = 0)
        {
            Yolo.ObjectDetectionsList.RemoveAt(index);
        }

        public int Count()
        {
            return Yolo.ObjectDetectionsList.Count;
        }

        public void Save(string root, int start)
        {
            Yolo.Save(root, start);
        }

        public ICollection<ObjectDetectionModel> Mapper(int start)
        {
            return Yolo.Mapper(start);
        }

        public ObservableCollection<CategoryModel> GetCategories()
        {
            return Yolo.Categories;
        }

        public void ChangeObjectDetections(int index)
        {
            Yolo.ObjectDetections = Yolo.ObjectDetectionsList[index];
        }

        public void ChangeCategoryColor(int index, string color)
        {
            Yolo.Categories[index].Color = color;
        }

    }
}