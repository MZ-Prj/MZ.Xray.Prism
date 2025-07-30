using MZ.Domain.Enums;
using MZ.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("Calibration")]
    public class CalibrationEntity : ICalibration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 이미지 가로 방향의 상대적 비율
        /// </summary>
        public double RelativeWidthRatio { get; set; } = 1.25;
        /// <summary>
        /// 센서값 신호 Off : 방사선이 꺼져있을때의 신호값
        /// </summary>
        public double OffsetRegion { get; set; } = 2600;

        /// <summary>
        /// 센서값 신호 On : 방사선이 켜져있을때의 신호값
        /// </summary>
        public double GainRegion { get; set; } = 15000;

        /// <summary>
        /// Detector 사이의 빈 영역에서 발생하는 저신호 경계
        /// </summary>
        public double BoundaryArtifact { get; set; } = 5000;

        /// <summary>
        /// 센서가 물체를 검출하기 위한 최소 신호 값(비율)
        /// </summary>
        public double ActivationThresholdRatio { get; set; } = 0.9;

        /// <summary>
        /// 이미지 최대 가로 길이(픽셀)
        /// </summary>
        public int MaxImageWidth { get; set; } = 1600;

        /// <summary>
        /// 센서에서 받아온 이미지 가로 길이(픽셀)
        /// </summary>
        public int SensorImageWidth { get; set; } = 16;

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("Image")]
    public class ImageEntity : IImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public ICollection<ObjectDetectionEntity> ObjectDetections { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("Filter")]
    public class FilterEntity : IFilter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public float Zoom { get; set; }
        public float Sharpness{ get; set; }
        public float Brightness{ get; set; }
        public float Contrast { get; set; }
        public ColorRole ColorMode { get; set; }

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("Material")]
    public class MaterialEntity : IMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Blur { get; set; }
        public double HighLowRate { get; set; }
        public double Density { get; set; }
        public double EdgeBinary { get; set; }
        public double Transparency { get; set; }

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

        // One-to-Many
        public ICollection<MaterialControlEntity> MaterialControls { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("MaterialControl")]
    public class MaterialControlEntity : IMaterialControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Y { get; set; }
        public double XMin { get; set; }
        public double XMax { get; set; }
        public string Color { get; set; }

        // Foreign Key
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public MaterialEntity Material { get; set; }
    }

    [Table("ZeffectControl")]
    public class ZeffectControlEntity : IZeffectControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Check { get; set; }
        public string Content { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string Color { get; set; }
    }
}
