using MZ.Domain.Entities;
using System.Collections.Generic;

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


}
