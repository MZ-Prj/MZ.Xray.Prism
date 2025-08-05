using MZ.Domain.Enums;
using System;

namespace MZ.Domain.Interfaces
{
    /// <summary>
    /// Xray 캘리브레이션(보정) 정보 인터페이스
    /// 
    /// - 영상 센서/장비/알고리즘 보정값 저장
    /// - 각 값은 이미지 처리, 품질 유지에 필수적
    /// </summary>
    public interface ICalibration
    {
        /// <summary>
        /// 이미지 가로 방향의 상대적 비율(표준값 대비)
        /// </summary>
        public double RelativeWidthRatio { get; set; }
        /// <summary>
        /// 방사선이 꺼진 상태의 센서 최대 신호값
        /// </summary>
        public double OffsetRegion { get; set; }
        /// <summary>
        /// 방사선이 켜진 상태의 센서 최소 신호값
        /// </summary>
        public double GainRegion { get; set; }
        /// <summary>
        /// Detector 사이의 빈 영역에서 발생하는 저신호 경계
        /// </summary>
        public double BoundaryArtifact { get; set; }
        /// <summary>
        /// 센서가 물체를 검출하기 위한 최소 신호 값(비율)
        /// </summary>
        public double ActivationThresholdRatio { get; set; }
        /// <summary>
        /// 이미지 최대 가로 길이(픽셀)
        /// </summary>
        public int MaxImageWidth { get; set; }
        /// <summary>
        /// 센서에서 받아온 이미지 가로 길이(픽셀)
        /// </summary>
        public int SensorImageWidth { get; set; }
    }

    /// <summary>
    /// 이미지 정보 인터페이스
    /// 
    /// - 저장 파일 경로, 해상도, 생성일시 등
    /// - 실제 파일(분석/리포트/검색 등) 기반
    /// </summary>
    public interface IImage
    {
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
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 필터(Filter) 파라미터 정보 인터페이스
    /// 
    /// - UI 조작/영상 처리(줌, 밝기, 색상모드 등)
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Zoom 비율
        /// </summary>
        public float Zoom { get; set; }
        /// <summary>
        /// 선명도
        /// </summary>
        public float Sharpness { get; set; }
        /// <summary>
        /// 밝기
        /// </summary>
        public float Brightness { get; set; }
        /// <summary>
        /// 명암
        /// </summary>
        public float Contrast { get; set; }
        /// <summary>
        /// 컬러 모드
        /// </summary>
        public ColorRole ColorMode { get; set; }
    }

    /// <summary>
    /// 물성(Material) 분석 파라미터 정보 인터페이스
    /// 
    /// - Blur, Density, Edge 등 각종 물질 판별용 파라미터
    /// </summary>
    public interface IMaterial
    {
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

    }

    /// <summary>
    /// 물성 분석용 조작 컨트롤 인터페이스
    /// 
    /// - 그래프상 좌표, 범위, 색상 등 분석 파라미터 관리
    /// </summary>
    public interface IMaterialControl
    {
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

    }

    /// <summary>
    /// Zeffect 설정값 인터페이스
    /// 
    /// - Zeffect는 특수효과, 임계값, 색상 등 사용자 커스텀 효과 관리
    /// </summary>
    public interface IZeffectControl
    {
        /// <summary>
        /// 커스텀 적용 여부
        /// </summary>
        public bool Check { get; set; }
        /// <summary>
        /// 커스텀 설명(명칭)
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
    }

}
