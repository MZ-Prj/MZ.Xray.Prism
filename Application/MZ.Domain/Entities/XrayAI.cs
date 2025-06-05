using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MZ.Domain.Interfaces;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("AIOption")]
    public class AIOptionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OnnxModel { get; set; }
        public int ModelType { get; set; }
        public bool Cuda { get; set; }
        public bool PrimeGpu { get; set; }
        public int GpuId { get; set; }
        public bool IsChecked { get; set; }
        public double Confidence { get; set; }
        public double IoU { get; set; }
        public DateTime CreateDate { get; set; }

        // One-to-Many
        public ICollection<CategoryEntity> Categories { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    [Table("Category")]
    public class CategoryEntity : ICategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsUsing { get; set; }
        public double Confidence { get; set; }

        // Foreign Key
        public int AIOptionId { get; set; }

        // Navigation
        [ForeignKey("AIOptionId")]
        public AIOptionEntity AIOption { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("ObjectDetection")]
    public class ObjectDetectionEntity : IObjectDetection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Confidence { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public DateTime CreateDate { get; set; }

        // Foreign Key
        public int ImageId { get; set; }

        [ForeignKey("ImageId")]
        public ImageEntity Image { get; set; }
    }
}
