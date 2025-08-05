using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MZ.Domain.Interfaces;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// AI 옵션(설정) Entity
    /// 
    /// - 인공지능 추론용 ONNX 모델, 디바이스(CPU/GPU) 정보, 성능 옵션 등 관리
    /// - 하나의 AIOption은 여러 Category(검출 클래스)와 연관됨 (1:N)
    /// - DB 테이블명: AIOption
    /// </summary>
    [Table("AIOption")]
    public class AIOptionEntity
    {
        /// <summary>
        /// PK. AI 옵션 고유 ID (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// ONNX 모델 파일 경로 또는 명칭
        /// </summary>
        public string OnnxModel { get; set; }

        /// <summary>
        /// 모델 타입
        /// </summary>
        public int ModelType { get; set; }

        /// <summary>
        /// GPU(CUDA) 사용 여부
        /// </summary>
        public bool Cuda { get; set; }

        /// <summary>
        /// Prime GPU(CUDA) 사용 여부(확장)
        /// </summary>
        public bool PrimeGpu { get; set; }

        /// <summary>
        /// 사용할 GPU ID
        /// </summary>
        public int GpuId { get; set; }

        /// <summary>
        /// 인공지능 예측 활성화 여부 
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// 객체 검출 신뢰도(Confidence) 임계값
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// IoU(Intersection over Union) 임계값
        /// </summary>
        public double IoU { get; set; }

        /// <summary>
        /// 설정 등록/수정 일시
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 연관 객체 분류(Category) 목록 (1:N)
        /// </summary>
        public ICollection<CategoryEntity> Categories { get; set; }

    }

    /// <summary>
    /// 객체 검출 카테고리 Entity
    /// 
    /// - AIOption과 1:N (여러 클래스)
    /// - DB 테이블명: Category
    /// </summary>
    [Table("Category")]
    public class CategoryEntity : ICategory
    {
        /// <summary>
        /// PK. 카테고리 고유 ID (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 클래스 인덱스(Name과 연동된 category_id)
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 클래스 명칭
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UI 표시 색상(HEX)
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 해당 클래스 사용 여부
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 클래스별 신뢰도(Confidence) 임계값(선택적)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// FK. 소속 AIOption ID
        /// </summary>
        public int AIOptionId { get; set; }

        [ForeignKey("AIOptionId")]
        public AIOptionEntity AIOption { get; set; }
    }

    /// <summary>
    /// 객체 검출 결과 Entity
    /// 
    /// - 실제 이미지에서 AI로 검출된 객체 정보
    /// - 이미지 1건에 여러 ObjectDetection이 1:N으로 매핑
    /// - DB 테이블명: ObjectDetection
    /// </summary>
    [Table("ObjectDetection")]
    public class ObjectDetectionEntity : IObjectDetection
    {
        /// <summary>
        /// PK. 검출 결과 고유 ID (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 클래스 인덱스(Name과 연동된 category_id) 
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 검출된 클래스 명칭 (ex: "person")
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UI 표시 색상(HEX)
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 검출 신뢰도
        /// </summary>
        public double Confidence { get; set; }
        /// <summary>
        /// Bounding Box 좌상단 X좌표 (픽셀)
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Bounding Box 좌상단 Y좌표 (픽셀)
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Bounding Box 가로 길이(Width, 픽셀)
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Bounding Box 세로 길이(Height, 픽셀)
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 검출 결과 생성 일시
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// FK. 원본 이미지 ID
        /// </summary>
        public int ImageId { get; set; }

        [ForeignKey("ImageId")]
        public ImageEntity Image { get; set; }
    }
}
