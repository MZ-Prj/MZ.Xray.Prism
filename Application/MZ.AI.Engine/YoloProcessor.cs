using MZ.Util;
using MZ.Logger;
using MZ.Model;
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
    /// <summary>
    /// YOLO 기반 객체 탐지 처리를 담당하는 프로세서
    /// </summary>
    public class YoloProcessor : BindableBase
    {
        #region Engine(YoloDotNet)
        public Yolo Yolo { get; set; }
        public YoloOptions YoloOption { get; set; }
        #endregion
        #region Params
        public List<ObservableCollection<ObjectDetectionModel>> ObjectDetectionsList { get; set; } = [];
        public ObjectDetectionOptionModel ObjectDetectionOption { get; set; } = new();

        private ObservableCollection<CategoryModel> _categories = [];
        public ObservableCollection<CategoryModel> Categories { get => _categories; set => SetProperty(ref _categories, value); }

        private ObservableCollection<ObjectDetectionModel> _objectDetections = [];
        public ObservableCollection<ObjectDetectionModel> ObjectDetections { get => _objectDetections; set => SetProperty(ref _objectDetections, value); }

        private bool _isVisibility = false;
        public bool IsVisibility { get => _isVisibility; set => SetProperty(ref _isVisibility, value); }
        #endregion

        public YoloProcessor() { }

        public YoloProcessor(YoloOptions yoloOptions, ObjectDetectionOptionModel objectDetectionOption, ObservableCollection<CategoryModel> categories = null)
        {
            Yolo = new Yolo(yoloOptions);
            ObjectDetectionOption = objectDetectionOption;
            Categories = [.. categories ?? ConvertCategories()];
        }

        /// <summary>
        /// YOLO 모델 및 카테고리, 옵션 생성 및 초기화
        /// </summary>
        public void Create(YoloOptions yoloOptions, ObjectDetectionOptionModel objectDetectionOption, ObservableCollection<CategoryModel> categories = null)
        {
            Yolo = new Yolo(yoloOptions);
            YoloOption = yoloOptions;
            ObjectDetectionOption = objectDetectionOption;
            Categories = [.. categories ?? ConvertCategories()];

        }

        /// <summary>
        /// ONNX 모델의 클래스 정보에서 CategoryModel 목록 생성
        /// </summary>
        /// <returns>ICollection<CategoryModel></returns>
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

        /// <summary>
        /// 이미지(스트림)에 대해 객체 탐지 수행 및 결과를 ObjectDetections에 저장
        /// </summary>
        /// <param name="stream">MemoryStream : 이미지 데이터 스트림</param>
        /// <param name="imageSize">imageSize : 원본 이미지 사이즈</param>
        /// <param name="canvasSize">canvasSize : 표시 캔버스 사이즈</param>
        /// <param name="offsetX">offsetX : X좌표 오프셋</param>
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

        /// <summary>
        /// 현재 탐지 결과, 옵션, 카테고리 등을 json으로 저장
        /// </summary>
        /// <param name="path">string : 저장 디렉토리</param>
        /// <param name="time">string : 파일명(시간 등)</param>
        public void Save(string path, string time)
        {
            try
            {
                string subPath = "Predict";
                
                MZIO.TryMakeDirectory(Path.Combine(path, subPath));

                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var objectDetections = ChangePositionCanvasToMat(); 
                var data = new
                {
                    objectDetections,
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

        /// <summary>
        /// 예측 결과(탐지 박스 포함 이미지)를 png로 저장
        /// </summary>
        /// <param name="path">string : 저장 경로</param>
        /// <param name="time">string : 파일명</param>
        /// <param name="stream">MemoryStream : 이미지 데이터 스트림</param>
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

        /// <summary>
        /// 탐지결과 좌표를 원본 이미지 기준으로 변환하여 반환 (Mapper) 
        /// </summary>
        /// <param name="start">int : 시작 X좌표</param>
        /// <returns>ICollection<ObjectDetectionModel></returns>
        public ICollection<ObjectDetectionModel> ChangePositionCanvasToMat(int start = 0)
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


        /// <summary>
        /// 객체 탐지 결과 표시/숨김 상태
        /// </summary>
        public void ChangedVisibility()
        {
            IsVisibility = !IsVisibility;
        }

        /// <summary>
        /// YOLO 엔진 리소스를 해제하고 참조 해제
        /// </summary>
        public void Clear()
        {
            Yolo?.Dispose();
            Yolo = null;
        }
    }
}