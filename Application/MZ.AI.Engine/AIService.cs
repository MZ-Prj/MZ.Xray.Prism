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
using MZ.Model;
using MZ.Domain.Entities;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using System;
using HarfBuzzSharp;
using MZ.Domain.Interfaces;

namespace MZ.AI.Engine
{
    /// <summary>
    /// 실제 YOLO 기반 AI 객체 탐지 엔진 
    /// </summary>
    public class AIService : BindableBase, IAIService
    {
        #region Params
        private YoloProcessor _yolo = new();
        public YoloProcessor Yolo { get => _yolo; set => SetProperty(ref _yolo, value); }
        #endregion

        public AIService()
        {
        }

        /// <summary>
        /// 지정된 모델 파일 경로로 객체탐지 엔진 생성
        /// </summary>
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


        /// <summary>
        /// 모델 옵션, 카테고리, 탐지옵션으로 엔진 초기화
        /// </summary>
        public void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption)
        {
            Yolo.Create(yoloOption, objectDetectionOption, categories);
        }

        /// <summary>
        /// DB의 AI 옵션 엔티티로 엔진 세팅(옵션 불러오기)
        /// </summary>
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

        /// <summary>
        /// 입력 이미지(스트림)로 객체탐지 실행
        /// </summary>
        public void Predict(MemoryStream stream, Size size)
        {
            Yolo.Predict(stream, size, size);
        }

        /// <summary>
        /// 입력 이미지(스트림)로 객체탐지 실행 (UI 매핑용)
        /// </summary>
        public void Predict(MemoryStream stream, Size imageSize, Size canvasSize, int offsetX = 0)
        {
            Yolo.Predict(stream, imageSize, canvasSize, offsetX);
        }
        /// <summary>
        /// 예측 결과 객체의 X 좌표 Shift (비동기)
        /// </summary>
        public async Task ShiftAsync(int width)
        {
            await Task.Run(() =>
            {
                Shift(width);
            });
        }
        /// <summary>
        /// 예측 결과 객체의 X 좌표 Shift
        /// </summary>
        public void Shift(int width)
        {
            var objectDetections = new ObservableCollection<ObjectDetectionModel>(
                Yolo.ObjectDetections.Select(c =>
                {
                    var copy = new ObjectDetectionModel();
                    c.CopyTo(copy);
                    copy.OffsetX -= (width * Yolo.ObjectDetectionOption.ScaleX);
                    return copy;
                })
            );
            Yolo.ObjectDetections = objectDetections;
        }
        /// <summary>
        /// 객체탐지 결과 추가
        /// </summary>
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
        /// <summary>
        /// 인덱스 기반 객체탐지 결과 삭제
        /// </summary>
        public void RemoveObjectDetection(int index = 0)
        {
            Yolo.ObjectDetectionsList.RemoveAt(index);
        }
        /// <summary>
        /// 현재 객체탐지 결과 개수 반환
        /// </summary>
        public int Count()
        {
            return Yolo.ObjectDetectionsList.Count;
        }
        /// <summary>
        /// 탐지 결과 이미지 저장
        /// </summary>
        public async Task Save(string path, string time, int start)
        {
            await Task.Run(() =>
            {
                Yolo.Save(path, time);
            });
        }
        /// <summary>
        /// 스트림 이미지로 결과 저장
        /// </summary>
        public async Task Save(string path, string time, MemoryStream stream)
        {
            await Task.Run(() =>
            {
                Yolo.Save(path,time,stream);
            });
        }
        /// <summary>
        /// ObjectDetectionModel 리스트로 매핑
        /// </summary>
        public ICollection<ObjectDetectionModel> ChangePositionCanvasToMat(int start)
        {
            return Yolo.ChangePositionCanvasToMat(start);
        }
        /// <summary>
        /// 현재 카테고리 리스트 반환
        /// </summary>
        public ObservableCollection<CategoryModel> GetCategories()
        {
            return Yolo.Categories;
        }
        /// <summary>
        /// 객체탐지 결과 특정 인덱스 값 변경 (UI동기화용)
        /// </summary>
        public void ChangeObjectDetections(int index)
        {
            Yolo.ObjectDetections = Yolo.ObjectDetectionsList[index-1];
        }
        /// <summary>
        /// 카테고리 색상 변경
        /// </summary>
        public void ChangeCategoryColor(int index, string color)
        {
            Yolo.Categories[index].Color = color;
        }
        /// <summary>
        /// 지정 루트경로에 저장된 모델이 있는지 확인
        /// </summary>
        public bool IsSavedModel(string root)
        {
            return MZIO.IsFileExist(root);
        }
        /// <summary>
        /// 현재 설정값들을 AIOption 생성용 DTO로 변환
        /// </summary>
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
        /// <summary>
        /// 엔진 및 리소스 해제
        /// </summary>
        public void Dispose()
        {
            Yolo.Clear();
        }
    }
}