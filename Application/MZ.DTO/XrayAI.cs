using MZ.Domain.Entities;
using MZ.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace MZ.DTO
{
    #region Reqeust
    /// <summary>
    /// AIOption 신규 생성 요청 DTO
    /// </summary>
    /// <param name="OnnxModel">ONNX 모델 파일명 또는 경로</param>
    /// <param name="ModelType">모델 타입 (ex: 0: 기본, 1: 커스텀 등)</param>
    /// <param name="Cuda">CUDA 가속 사용 여부</param>
    /// <param name="PrimeGpu">GPU(Multi) 사용 여부</param>
    /// <param name="GpuId">사용할 GPU ID</param>
    /// <param name="IsChecked">해당 모델 사용 여부</param>
    /// <param name="Confidence">신뢰도(탐지 임계값)</param>
    /// <param name="IoU">IoU(탐지 중복 허용 임계값)</param>
    /// <param name="Categories">분류(Category) 리스트</param>
    public record AIOptionCreateRequest(
        string OnnxModel,
        int ModelType,
        bool Cuda,
        bool PrimeGpu,
        int GpuId,
        bool IsChecked,
        double Confidence,
        double IoU,
        ICollection<CategoryEntity> Categories
    );


    /// <summary>
    /// AIOption Category 변경/저장 요청 DTO
    /// </summary>
    /// <param name="Categories">카테고리(Entity) 리스트</param>
    public record AIOptionSaveRequest(
        ICollection<CategoryEntity> Categories
    );
    #endregion

    #region Mapper
    /// <summary>
    /// Category Mapper 유틸리티 클래스
    /// </summary>
    public static class CategoryMapper
    {
        /// <summary>
        /// CategoryModel을 CategoryEntity로 변환
        /// </summary>
        /// <param name="model">CategoryModel</param>
        /// <returns>CategoryEntity</returns>
        public static CategoryEntity ModelToEntity(CategoryModel model)
        {
            return new CategoryEntity()
            {
                Id = model.Id,
                Index = model.Index,
                Name = model.Name,
                Color = model.Color,
                IsUsing = model.IsUsing,
                Confidence = model.Confidence,
            };
        }
        /// <summary>
        /// CategoryModel 컬렉션을 CategoryEntity 컬렉션으로 변환
        /// </summary>
        /// <param name="model"><ICollection<CategoryModel>/param>
        /// <returns>ICollection<CategoryEntity></returns>
        public static ICollection<CategoryEntity> ModelsToEntities(ICollection<CategoryModel> model)
        {
            return [.. model.Select(ModelToEntity)];
        }

        /// <summary>
        /// CategoryModel 컬렉션을 AIOptionSaveRequest로 변환
        /// </summary>
        /// <param name="model">ICollection<CategoryModel></param>
        /// <returns>AIOptionSaveRequest</returns>
        public static AIOptionSaveRequest ModelToRequest(ICollection<CategoryModel> model)
        {
            return new(ModelsToEntities(model));
        }
    }


    /// <summary>
    /// ObjectDetection Mapper 유틸리티 클래스
    /// </summary>
    public static class ObjectDetectionMapper
    {
        /// <summary>
        /// ObjectDetectionModel을 ObjectDetectionEntity로 변환
        /// </summary>
        /// <param name="model">ObjectDetectionModel</param>
        /// <returns>ObjectDetectionEntity</returns>
        public static ObjectDetectionEntity ModelToEntity(ObjectDetectionModel model)
        {
            return new ObjectDetectionEntity()
            {
                Index = model.Index,
                Name = model.Name,
                Color = model.Color,
                Confidence = model.Confidence,
                X  = model.X,
                Y  = model.Y,
                Width = model.Width,
                Height = model.Height,
            };
        }

        /// <summary>
        /// ObjectDetectionEntity로 ObjectDetectionModel 변환
        /// </summary>
        /// <param name="entity">ObjectDetectionEntity</param>
        /// <returns>ObjectDetectionModel</returns>
        public static ObjectDetectionModel EntityToModel(ObjectDetectionEntity entity)
        {
            return new ObjectDetectionModel()
            {
                Index = entity.Index,
                Name = entity.Name,
                Color = entity.Color,
                Confidence = entity.Confidence,
                X = entity.X,
                Y = entity.Y,
                Width = entity.Width,
                Height = entity.Height,
            };
        }

        /// <summary>
        /// ObjectDetectionModel 컬렉션을 ObjectDetectionEntity 컬렉션으로 변환
        /// </summary>
        /// <param name="model">ICollection<ObjectDetectionModel></param>
        /// <returns>ICollection<ObjectDetectionEntity></returns>
        public static ICollection<ObjectDetectionEntity> ModelsToEntities(ICollection<ObjectDetectionModel> model)
        {
            return [.. model.Select(ModelToEntity)];
        }

        /// <summary>
        /// ObjectDetectionEntity 컬렉션을 ObjectDetectionModel 컬렉션으로 변환
        /// </summary>
        /// <param name="model">ICollection<ObjectDetectionEntity></param>
        /// <returns>ICollection<ObjectDetectionModel></returns>
        public static ICollection<ObjectDetectionModel> EntitiesToModels(ICollection<ObjectDetectionEntity> model)
        {
            return [.. model.Select(EntityToModel)];
        }
    }
    #endregion
}
