using MZ.Domain.Entities;
using MZ.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace MZ.DTO
{
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

    public record AIOptionSaveRequest(
        int AIOptionId,
        ICollection<CategoryEntity> Categories
    );

    public static class CategoryMapper
    {
        public static CategoryEntity ModelToEntity(CategoryModel model)
        {
            return new CategoryEntity()
            {
                Index = model.Index,
                Name = model.Name,
                Color = model.Color,
                IsUsing = model.IsUsing,
                Confidence = model.Confidence,
            };
        }

        public static ICollection<CategoryEntity> ModelsToEntities(ICollection<CategoryModel> model)
        {
            return [.. model.Select(c => ModelToEntity(c))];
        }
    }


    public static class ObjectDetectionMapper
    {
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

        public static ICollection<ObjectDetectionEntity> ModelsToEntities(ICollection<ObjectDetectionModel> model)
        {
            return [.. model.Select(c => ModelToEntity(c))];
        }
    }
}
