using MZ.Domain.Entities;
using MZ.Domain.Models;
using MZ.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using YoloDotNet.Models;

namespace MZ.AI.Engine
{
    /// <summary>
    /// AI 기반 객체탐지 서비스 인터페이스 (YOLO 등 AI 옵션 포함)
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// YOLO 프로세서 인스턴스
        /// </summary>
        YoloProcessor Yolo { get; set; }

        /// <summary>
        /// 지정된 모델 파일 경로로 객체탐지 엔진 생성
        /// </summary>
        void Create(string path);
        /// <summary>
        /// 모델 옵션, 카테고리, 탐지옵션으로 엔진 초기화
        /// </summary>
        void Create(YoloOptions yoloOption, ObservableCollection<CategoryModel> categories, ObjectDetectionOptionModel objectDetectionOption);
        /// <summary>
        /// 엔진 및 리소스 해제
        /// </summary>
        void Dispose();
        /// <summary>
        /// DB의 AI 옵션 엔티티로 엔진 세팅(옵션 불러오기)
        /// </summary>
        void Load(AIOptionEntity entity);
        /// <summary>
        /// 입력 이미지(스트림)로 객체탐지 실행
        /// </summary>
        void Predict(MemoryStream stream, Size imageSize);
        /// <summary>
        /// 입력 이미지(스트림)로 객체탐지 실행 (UI 매핑용)
        /// </summary>
        void Predict(MemoryStream stream, Size imageSize, Size canvasSize, int offsetX = 0);
        /// <summary>
        /// 예측 결과 객체의 X 좌표 Shift
        /// </summary>
        void Shift(int width);
        /// <summary>
        /// 예측 결과 객체의 X 좌표 Shift (비동기)
        /// </summary>
        Task ShiftAsync(int width);
        /// <summary>
        /// 객체탐지 결과 추가
        /// </summary>
        void AddObjectDetection();
        /// <summary>
        /// 인덱스 기반 객체탐지 결과 삭제
        /// </summary>
        void RemoveObjectDetection(int index = 0);
        /// <summary>
        /// 현재 객체탐지 결과 개수 반환
        /// </summary>
        int Count();
        /// <summary>
        /// 탐지 결과 이미지 저장
        /// </summary>
        Task Save(string path, string time, int start = 0);
        /// <summary>
        /// 스트림 이미지로 결과 저장
        /// </summary>
        Task Save(string path, string time, MemoryStream stream);
        /// <summary>
        /// ObjectDetectionModel 리스트로 매핑
        /// </summary>
        ICollection<ObjectDetectionModel> Mapper(int start);
        /// <summary>
        /// 현재 카테고리 리스트 반환
        /// </summary>
        ObservableCollection<CategoryModel> GetCategories();
        /// <summary>
        /// 객체탐지 결과 특정 인덱스 값 변경 (UI동기화용)
        /// </summary>
        void ChangeObjectDetections(int index);
        /// <summary>
        /// 카테고리 색상 변경
        /// </summary>
        void ChangeCategoryColor(int index, string color);
        /// <summary>
        /// 지정 루트경로에 저장된 모델이 있는지 확인
        /// </summary>
        bool IsSavedModel(string root);
        /// <summary>
        /// 현재 설정값들을 AIOption 생성용 DTO로 변환
        /// </summary>
        AIOptionCreateRequest YoloToRequest();
    }
}
