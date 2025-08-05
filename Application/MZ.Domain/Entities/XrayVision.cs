using MZ.Domain.Enums;
using MZ.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MZ.Domain.Entities
{
    /// <summary>
    /// Xray Calibration(센서 보정값) Entity
    /// 
    /// - Xray 센서/이미지 입력 시 각종 보정값, 임계값, 비율 정보 관리
    /// - 각 사용자의 환경/장치별 교정값 저장
    /// - DB 테이블: Calibration
    /// </summary>
    [Table("Calibration")]
    public class CalibrationEntity : ICalibration
    {
        /// <summary>
        /// PK. 교정값 고유 ID (Auto-increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 이미지 가로 방향의 상대적 비율(표준값 대비)
        /// </summary>
        public double RelativeWidthRatio { get; set; } = 1.25;
        /// <summary>
        /// 방사선이 꺼진 상태의 센서 최대 신호값
        /// </summary>
        public double OffsetRegion { get; set; } = 2600;

        /// <summary>
        /// 방사선이 켜진 상태의 센서 최소 신호값
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

        /// <summary>
        /// FK. 사용자 ID
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

    }

    /// <summary>
    /// Xray 촬영 이미지 Entity
    /// 
    /// - 단일 이미지 파일 정보, 사이즈, 생성일시, 검출 결과와 1:N 관계
    /// - DB 테이블: Image
    /// </summary>
    [Table("Image")]
    public class ImageEntity : IImage
    {
        /// <summary>
        /// PK. 이미지 고유 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 파일 경로(절대/상대경로)
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 파일명
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// 이미지 가로 픽셀
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 이미지 세로 픽셀
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 이미지 저장 일시
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;
        /// <summary>
        /// AI 객체 검출 결과 (1:N)
        /// </summary>
        public ICollection<ObjectDetectionEntity> ObjectDetections { get; set; }
    }

    /// <summary>
    /// 이미지 필터 정보 Entity
    /// 
    /// - 밝기, 명암, 컬러 등 후처리(뷰어) 옵션 관리
    /// - 각 사용자별 개별 저장
    /// - DB 테이블: Filter
    /// </summary>
    [Table("Filter")]
    public class FilterEntity : IFilter
    {
        /// <summary>
        /// PK. 필터 고유 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Zoom 비율
        /// </summary>
        public float Zoom { get; set; }
        /// <summary>
        /// 선명도
        /// </summary>
        public float Sharpness{ get; set; }
        /// <summary>
        /// 밝기
        /// </summary>
        public float Brightness{ get; set; }
        /// <summary>
        /// 명암
        /// </summary>
        public float Contrast { get; set; }
        /// <summary>
        /// 컬러 모드
        /// </summary>
        public ColorRole ColorMode { get; set; }

        /// <summary>
        /// FK. 사용자 ID
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

    /// <summary>
    /// Material(물성) 분석 옵션 Entity
    /// 
    /// - Blur, 밀도, 투명도 등 이미지 분석 파라미터 관리
    /// - 각 사용자별 개별 저장
    /// - DB 테이블: Material
    /// </summary>
    [Table("Material")]
    public class MaterialEntity : IMaterial
    {
        /// <summary>
        /// PK. 물성분석 고유 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Blur
        /// </summary>
        public double Blur { get; set; }
        /// <summary>
        /// 경계 비율
        /// (ex 1:1.05)
        /// </summary>
        public double HighLowRate { get; set; }
        /// <summary>
        /// 밀도(Density)
        /// </summary>
        public double Density { get; set; }
        /// <summary>
        /// 에지(Edge)
        /// </summary>
        public double EdgeBinary { get; set; }
        /// <summary>
        /// 투명도(Transparency)
        /// </summary>
        public double Transparency { get; set; }

        /// <summary>
        /// FK. 사용자 ID
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

        /// <summary>
        /// 물성분석 목록(그래프 등) (1:N)
        /// </summary>
        public ICollection<MaterialControlEntity> MaterialControls { get; set; }
    }

    /// <summary>
    /// 물성 분석 파라미터(그래프) Entity
    /// 
    /// - Material(물성 분석)과 1:N
    /// - 그래프상 좌표, 범위, 색상 등 분석 파라미터 관리
    /// </summary>
    [Table("MaterialControl")]
    public class MaterialControlEntity : IMaterialControl
    {
        /// <summary>
        /// PK. 물성분석 제어 고유 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// X 최소값
        /// </summary>
        public double XMin { get; set; }
        /// <summary>
        /// X 최대값
        /// </summary>
        public double XMax { get; set; }
        /// <summary>
        /// 색상(그래프/구간별 표시)
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// FK. 재질 옵션 ID
        /// </summary>
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public MaterialEntity Material { get; set; }
    }

    /// <summary>
    /// Zeffect 설정 Entity
    /// 
    /// - Zeffect는 특수효과, 임계값, 색상 등 사용자 커스텀 관리
    /// - DB 테이블: ZeffectControl
    /// </summary>
    [Table("ZeffectControl")]
    public class ZeffectControlEntity : IZeffectControl
    {
        /// <summary>
        /// PK. Zeffect 컨트롤 고유 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 커스텀 적용 여부
        /// </summary>
        public bool Check { get; set; }
        /// <summary>
        /// 커스텀 설명
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 커스텀 적용 최소값
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// 커스텀 적용 최대값
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// 색상(효과/구간별 표시)
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 연관 사용자 정보
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

    }
}
