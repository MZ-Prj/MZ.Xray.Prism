using Prism.Mvvm;
using System.IO;
using System.Linq;
using System.Data;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MZ.DTO;
using MZ.Util;
using MZ.Domain.Models;
using MZ.Domain.Entities;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using System;

namespace MZ.AI.Engine
{
    public class AIService : BindableBase, IAIService
    {
        #region Params
        private YoloProcessor _yolo = new();
        public YoloProcessor Yolo { get => _yolo; set => SetProperty(ref _yolo, value); }
        #endregion

        public AIService()
        {
        }

        public void Create(string path)
        {

            YoloOptions yoloOption = new()
            {
                OnnxModel = path,
                Cuda = true,
                PrimeGpu = true,
                ModelType = ModelType.ObjectDetection
            };

            try
            {
                _ = new Yolo(yoloOption);
            }
            catch 
            {
                yoloOption = new()
                {
                    OnnxModel = path,
                    Cuda = false,
                    ModelType = ModelType.ObjectDetection
                };
            }

            ObjectDetectionOptionModel objectDetectionOption = new();

            Yolo.Create(yoloOption, objectDetectionOption);
        }

        public void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption)
        {
            Yolo.Create(yoloOption, objectDetectionOption, categories);
        }

        public void Load(AIOptionEntity entity)
        {
            YoloOptions yoloOption = new()
            {
                OnnxModel = entity.OnnxModel,
                Cuda = entity.Cuda,
                ModelType = (ModelType)entity.ModelType,
                PrimeGpu = entity.PrimeGpu,
                GpuId = entity.GpuId,
            };

            ObjectDetectionOptionModel objectDetectionOption = new()
            {
                Confidence = entity.Confidence,
                IoU = entity.IoU
            };

            ObservableCollection<CategoryModel> categories = [.. entity.Categories?.Select(e => new CategoryModel
            {
                Id = e.Id,
                Index = e.Index,
                Name = e.Name,
                Color = e.Color,
                IsUsing = e.IsUsing,
                Confidence = e.Confidence,
            }) ?? []];

            Yolo.Create(yoloOption, objectDetectionOption, categories);
        }

        public void Predict(MemoryStream stream, Size size)
        {
            Yolo.Predict(stream, size, size);
        }

        public void Predict(MemoryStream stream, Size imageSize, Size canvasSize, int offsetX = 0)
        {
            Yolo.Predict(stream, imageSize, canvasSize, offsetX);
        }

        public async Task ShiftAsync(int width)
        {
            await Task.Run(() =>
            {
                Shift(width);
            });
        }

        public void Shift(int width)
        {
            foreach (var detection in Yolo.ObjectDetections)
            {
                detection.OffsetX -= (width * Yolo.ObjectDetectionOption.ScaleX);
            }
        }

        public void AddObjectDetection()
        {
            var objectDetections = new ObservableCollection<ObjectDetectionModel>(
                Yolo.ObjectDetections.Select(c =>
                {
                    var copy = new ObjectDetectionModel();
                    c.CopyTo(copy);
                    return copy;
                })
            );

            Yolo.ObjectDetectionsList.Add(objectDetections);
        }

        public void RemoveObjectDetection(int index = 0)
        {
            Yolo.ObjectDetectionsList.RemoveAt(index);
        }

        public int Count()
        {
            return Yolo.ObjectDetectionsList.Count;
        }

        public async Task Save(string path, string time, int start)
        {
            await Task.Run(() =>
            {
                Yolo.Save(path, time);
            });
        }

        public async Task Save(string path, string time, MemoryStream stream)
        {
            await Task.Run(() =>
            {
                Yolo.Save(path,time,stream);
            });
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
            Yolo.ObjectDetections = Yolo.ObjectDetectionsList[index-1];
        }

        public void ChangeCategoryColor(int index, string color)
        {
            Yolo.Categories[index].Color = color;
        }

        public bool IsSavedModel(string root)
        {
            return MZIO.IsFileExist(root);
        }

        public AIOptionCreateRequest YoloToRequest()
        {
            YoloOptions yoloOption = Yolo.YoloOption;
            ObjectDetectionOptionModel objectDetectionOption = Yolo.ObjectDetectionOption;

            return new(
                OnnxModel: yoloOption.OnnxModel,
                ModelType: (int)yoloOption.ModelType,
                Cuda: yoloOption.Cuda,
                PrimeGpu: yoloOption.PrimeGpu,
                GpuId: yoloOption.GpuId,
                IsChecked: true,
                Confidence: objectDetectionOption.Confidence,
                IoU: objectDetectionOption.IoU,
                Categories: [.. Yolo.Categories?.Select(e => new CategoryEntity
                    {
                        Id = e.Id,
                        Index = e.Index,
                        Name = e.Name,
                        Color = e.Color,
                        IsUsing = e.IsUsing,
                        Confidence = e.Confidence,
                    }) ?? []]
            );
        }

        public void Dispose()
        {
            Yolo.Clear();
        }
    }
}