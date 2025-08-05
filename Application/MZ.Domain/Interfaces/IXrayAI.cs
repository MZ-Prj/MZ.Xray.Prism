using System;

namespace MZ.Domain.Interfaces
{
    /// <summary>
    /// AI 분석 모델 옵션 정보 인터페이스
    /// 
    /// - ONNX 등 인공지능 모델의 선택/세부 옵션을 관리
    /// - 모델 유형, 하드웨어 가속(GPU), 임계값 등 분석 환경 전체를 설정
    /// - Entity, DTO, ViewModel 등에서 구현
    /// </summary>
    public interface IAIOption
    {
        /// <summary>
        /// ONNX 모델 파일명 또는 경로  
        /// </summary>
        public string OnnxModel { get; set; }
        /// <summary>
        /// 모델 타입(ModelType)
        /// </summary>
        public int ModelType { get; set; }
        /// <summary>
        /// GPU(CUDA) 사용 여부  
        /// - true: GPU 연산, false: CPU 연산
        /// </summary>
        public bool Cuda { get; set; }
        /// <summary>
        /// Prime GPU(CUDA) 사용 여부(확장)
        /// </summary>
        public bool PrimeGpu { get; set; }
        /// <summary>
        /// 할당 GPU ID
        /// </summary>
        public int GpuId { get; set; }
        /// <summary>
        /// 현재 선택/활성화 여부  
        /// - true: 사용중, false: 미사용
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// AI 객체 검출 신뢰도(Confidence) 임계값  
        /// - 0.0~1.0, 임계값 이상만 검출로 인정
        /// </summary>
        public double Confidence { get; set; }
        /// <summary>
        /// IoU(Intersection over Union) 임계값  
        /// </summary>
        public double IoU { get; set; }
        /// <summary>
        /// 옵션 생성 일시
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// AI 분류(Category) 정보 인터페이스  
    /// 
    /// - AI 모델이 구분하는 객체 클래스(라벨) 정보
    /// - 색상, 활성화 여부 등 포함
    /// </summary>
    public interface ICategory
    {
        /// <summary>
        /// 분류 인덱스
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 분류명
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 색상(HEX)
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 분류 활성화(사용/미사용)
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 신뢰도(임계값, 0.0~1.0)
        /// </summary>
        public double Confidence { get; set; }
    }

    /// <summary>
    /// 객체 검출(Object Detection) 결과 정보 인터페이스  
    /// 
    /// - AI 분석 후, 이미지 내 객체별 검출 결과
    /// - 바운딩박스 좌표, 클래스명, 신뢰도 등 포함
    /// </summary>
    public interface IObjectDetection
    {
        /// <summary>
        /// 검출 결과 인덱스
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 검출 클래스명
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 바운딩박스 컬러(HEX)
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 검출 신뢰도(Confidence)
        /// </summary>
        public double Confidence { get; set; }
        /// <summary>
        /// X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 검출 생성 일시(분석 수행 시각)
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
