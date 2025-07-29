using MZ.Util;
using MZ.Logger;
using MZ.Domain.Models;
using Prism.Mvvm;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoloDotNet;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using Newtonsoft.Json;

namespace MZ.AI.Engine
{
    public class YoloProcessor : BindableBase
    {
        public Yolo Yolo { get; set; }
        public YoloOptions YoloOption { get; set; }
        public ObservableCollection<CategoryModel> Categories { get; set; } = [];
        public List<ObservableCollection<ObjectDetectionModel>> ObjectDetectionsList { get; set; } = [];
        public ObjectDetectionOptionModel ObjectDetectionOption { get; set; } = new();

        private ObservableCollection<ObjectDetectionModel> _objectDetections = [];
        public ObservableCollection<ObjectDetectionModel> ObjectDetections { get => _objectDetections; set => SetProperty(ref _objectDetections, value); }

        private bool _isVisibility = false;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }


        public YoloProcessor() { }

        public YoloProcessor(YoloOptions yoloOptions, ObjectDetectionOptionModel objectDetectionOption, ObservableCollection<CategoryModel> categories = null)
        {
            Yolo = new Yolo(yoloOptions);
            ObjectDetectionOption = objectDetectionOption;
            Categories = [.. categories ?? ConvertCategories()];
        }

        public void Create(YoloOptions yoloOptions, ObjectDetectionOptionModel objectDetectionOption, ObservableCollection<CategoryModel> categories = null)
        {
            Yolo = new Yolo(yoloOptions);
            YoloOption = yoloOptions;
            ObjectDetectionOption = objectDetectionOption;
            Categories = [.. categories ?? ConvertCategories()];

        }

        public ICollection<CategoryModel> ConvertCategories()
        {
            ICollection<CategoryModel> categories = [.. Yolo.OnnxModel.Labels.Select((category, index) => new       CategoryModel
                {
                    Index = index,
                    Color = category.Color,
                    Name = category.Name,
                    IsUsing = true,
                    Confidence = ObjectDetectionOption.Confidence
                })];

            return categories;
        }

        public void Predict(MemoryStream stream, Size imageSize, Size canvasSize, int offsetX = 0)
        {
            ObjectDetectionOption.ScaleX = canvasSize.Width / (double)imageSize.Width;
            ObjectDetectionOption.ScaleY = canvasSize.Height / (double)imageSize.Height;

            var result = new ObservableCollection<ObjectDetectionModel>();
            var labels = Categories.Where(l => l.IsUsing).ToDictionary(l => l.Index, l => l);

            using(stream)
            using(var image = SKImage.FromEncodedData(stream))
            {
                var objectDetections = Yolo.RunObjectDetection(image, ObjectDetectionOption.Confidence, ObjectDetectionOption.IoU);
                var predicts = objectDetections.Select((detection, index) => new {detection, index});

                foreach (var predict in predicts)
                {
                    if (labels.TryGetValue(predict.detection.Label.Index, out var label) &&
                        predict.detection.Confidence >= label.Confidence)
                    {
                        var boundingBox = predict.detection.BoundingBox;

                        result.Add(new ObjectDetectionModel
                        {
                            Id = predict.index,
                            Index = predict.detection.Label.Index,
                            Name = label.Name,
                            Color = label.Color,
                            Confidence = predict.detection.Confidence,
                            X = boundingBox.Left * ObjectDetectionOption.ScaleX,
                            Y = boundingBox.Top * ObjectDetectionOption.ScaleY,
                            Width = boundingBox.Width * ObjectDetectionOption.ScaleX,
                            Height = boundingBox.Height * ObjectDetectionOption.ScaleY,
                            IsVisibility = true, 
                            IsBlink = false,
                            OffsetX = (offsetX + boundingBox.Left) * ObjectDetectionOption.ScaleX,
                            CreateDate = DateTime.Now,
                        });
                    }
                }
            }

            ObjectDetections = [.. result.OrderBy(p => p.Name).ToList()];
        }

        public void Save(string path, string time, int start = 0)
        {
            try
            {
                string subPath = "Predict";
                
                MZIO.TryMakeDirectory(Path.Combine(path, subPath));

                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };

                //var predict = Mapper(start);

                var data = new
                {
                    ObjectDetections,
                    YoloOption,
                    Categories
                };
                string json = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(Path.Combine(path, subPath, time), json);
            }
            catch (Exception ex) 
            {
                MZLogger.Warning(ex.ToString());
            }
        }


        public void Save(string path, string time, MemoryStream stream)
        {
            string subPath = "Predict";
            MZIO.TryMakeDirectory(Path.Combine(path, subPath));

            using (stream)
            using (var image = SKImage.FromEncodedData(stream))
            {
                var predicts = Yolo.RunObjectDetection(image, ObjectDetectionOption.Confidence, ObjectDetectionOption.IoU);

                var predictImage = image.Draw(predicts);
                predictImage.Save(Path.Combine(path, subPath, time), SKEncodedImageFormat.Png);
            }
        }

        public ICollection<ObjectDetectionModel> Mapper(int start = 0)
        {
            ICollection<ObjectDetectionModel> objectDetections = [];
            foreach (var item in ObjectDetections)
            {
                double x = item.X / ObjectDetectionOption.ScaleX - start;
                if (x > 0)
                {
                    objectDetections.Add(new ObjectDetectionModel()
                    {
                        Id = item.Id,
                        Index = item.Index,
                        Name = item.Name,
                        Color = item.Color,
                        Confidence = item.Confidence,
                        X = x,
                        Y = item.Y / ObjectDetectionOption.ScaleY,
                        Width = item.Width / ObjectDetectionOption.ScaleX,
                        Height = item.Height / ObjectDetectionOption.ScaleY,
                        CreateDate = DateTime.Now,
                    });
                }
            }
            return objectDetections;
        }

        public void ChangedVisibility()
        {
            IsVisibility = !IsVisibility;
        }

    }
}