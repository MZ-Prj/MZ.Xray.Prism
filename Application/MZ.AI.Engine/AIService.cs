using MZ.Domain.Models;
using Prism.Mvvm;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoloDotNet.Models;

namespace MZ.AI.Engine
{
    public class AIService : BindableBase
    {
        private ObservableCollection<ObjectDetector> _detectors = [];
        public ObservableCollection<ObjectDetector> Detectors { get => _detectors; set => SetProperty(ref _detectors, value); }

        private int _selectedIndex = -1;
        public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }

        public void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption)
        {
            if (Detectors == null)
            {
                return;
            }

            var detector = new ObjectDetector
            {
                YoloOption = yoloOption,
                ObjectDetectionOption = objectDetectionOption,
                Categories = categories,
            };

            Detectors.Add(detector);
        }

        public void Predict(MemoryStream stream, Size imageSize, Size canvasSize)
        {
            Detectors[SelectedIndex].Predict(stream, imageSize, canvasSize);
        }

        /// <summary>
        /// ui에서 화면 갱신 후 이동 할때 같이 shift할 용도
        /// </summary>
        /// <param name="width"></param>
        public void Shift(int width)
        {
            foreach (var detection in Detectors[SelectedIndex].ObjectDetections)
            {
                detection.X -= (width * Detectors[SelectedIndex].ObjectDetectionOption.ScaleX);
            }
        }

        public void Add()
        {
            Detectors[SelectedIndex].ObjectDetectionsList.Add(Detectors[SelectedIndex].ObjectDetections);
        }

        public void Remove(int index = 0)
        {
            Detectors[SelectedIndex].ObjectDetectionsList.RemoveAt(index);
        }

        public int Count()
        {
            return Detectors[SelectedIndex].ObjectDetectionsList.Count;
        }

        public void Save(string root, int start)
        {
            Detectors[SelectedIndex].Save(root, start);
        }

        public ICollection<ObjectDetectionModel> Mapper(int start)
        {
            return Detectors[SelectedIndex].Mapper(start);
        }

        public ObservableCollection<CategoryModel> GetCategories()
        {
            return Detectors[SelectedIndex].Categories;
        }

        public void ChangeObjectDetections(int index)
        {
            Detectors[SelectedIndex].ObjectDetections = Detectors[SelectedIndex].ObjectDetectionsList[index];
        }

        public void ChangeCategoryColor(int index, string color)
        {
            Detectors[SelectedIndex].Categories[index].Color = color;
        }

        public void ChangeSelectedIndex(string onnxModel)
        {
            int index = Detectors.ToList().FindIndex(detector => detector.YoloOption.OnnxModel == onnxModel);
            SelectedIndex = index;
        }
    }
}