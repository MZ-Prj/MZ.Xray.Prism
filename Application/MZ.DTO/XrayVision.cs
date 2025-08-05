using MZ.Domain.Entities;
using MZ.Domain.Enums;
using MZ.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

#nullable enable
namespace MZ.DTO
{

    #region Request
    /// <summary>
    /// 이미지 저장 요청 DTO
    /// </summary>
    /// <param name="Path">저장 경로</param>
    /// <param name="Filename">파일명</param>
    /// <param name="Width">이미지 가로 크기</param>
    /// <param name="Height">이미지 세로 크기</param>
    /// <param name="ObjectDetections">객체 탐지 결과 리스트</param>
    public record ImageSaveRequest(
        string Path,
        string Filename,
        int Width,
        int Height,
        ICollection<ObjectDetectionEntity> ObjectDetections
    );

    /// <summary>
    /// 이미지 조회(페이지네이션) 요청 DTO
    /// </summary>
    /// <param name="Start">조회 시작일(포함)</param>
    /// <param name="End">조회 종료일(포함)</param>
    /// <param name="Page">페이지 번호</param>
    /// <param name="Size">페이지당 데이터 개수</param>
    public record ImageLoadRequest(
        DateTime Start,
        DateTime End,
        int Page,
        int Size
    );

    /// <summary>
    /// 리포트용 이미지 조회 요청 DTO (기간 내 전체)
    /// </summary>
    /// <param name="Start">조회 시작일</param>
    /// <param name="End">조회 종료일</param>
    public record ReportImageLoadRequest(
        DateTime Start,
        DateTime End
    );
    /// <summary>
    /// 필터 설정 저장 요청 DTO
    /// </summary>
    /// <param name="Zoom">확대(Zoom) 비율</param>
    /// <param name="Sharpness">선명도(Sharpness) 값</param>
    /// <param name="Brightness">밝기(Brightness) 값</param>
    /// <param name="Contrast">명암(Contrast) 값</param>
    /// <param name="ColorMode">컬러모드</param>
    public record FilterSaveRequest(
        float Zoom,
        float Sharpness,
        float Brightness,
        float Contrast,
        ColorRole ColorMode
    );
    /// <summary>
    /// 필터 설정 불러오기 요청 DTO
    /// </summary>
    /// <param name="Username">조회할 사용자명</param>
    public record FilterLoadRequest(
        string Username
    );
    /// <summary>
    /// 물성 설정 저장 요청 DTO
    /// </summary>
    /// <param name="Blur">블러(Blur) 값</param>
    /// <param name="HighLowRate">High/Low 비율</param>
    /// <param name="Density">밀도(Density) 값</param>
    /// <param name="EdgeBinary">Edge Binary 값</param>
    /// <param name="Transparency">투명도(Transparency) 값</param>
    /// <param name="MaterialControls">물성 컨트롤 리스트</param>
    public record MaterialSaveRequest(
        double Blur,
        double HighLowRate,
        double Density,
        double EdgeBinary,
        double Transparency,
        ICollection<MaterialControlEntity> MaterialControls
    );

    /// <summary>
    /// 물성 설정 불러오기 요청 DTO
    /// </summary>
    /// <param name="Username">조회할 사용자명</param>
    public record MaterialLoadRequest(
        string Username
    );
    /// <summary>
    /// Calibration 값 저장 요청 DTO
    /// </summary>
    /// <param name="RelativeWidthRatio">가로 방향 상대 비율</param>
    /// <param name="OffsetRegion">센서 오프셋값</param>
    /// <param name="GainRegion">센서 게인값</param>
    /// <param name="BoundaryArtifact">경계선 영역</param>
    /// <param name="ActivationThresholdRatio">활성화 임계값 비율</param>
    /// <param name="MaxImageWidth">최대 이미지 가로길이</param>
    /// <param name="SensorImageWidth">센서 이미지 가로길이</param>
    public record CalibrationSaveRequest(
        double RelativeWidthRatio,
        double OffsetRegion,
        double GainRegion,
        double BoundaryArtifact,
        double ActivationThresholdRatio,
        int MaxImageWidth,
        int SensorImageWidth
    );
    /// <summary>
    /// Calibration 값 불러오기 요청 DTO
    /// </summary>
    /// <param name="Username">조회할 사용자명</param>
    public record CalibrationLoadRequest(
        string Username
    );
    /// <summary>
    /// Zeffect 컨트롤 저장 요청 DTO
    /// </summary>
    /// <param name="ZeffectControls">Zeffect 컨트롤 리스트</param>
    public record ZeffectControlSaveRequest(
        ICollection<ZeffectControlEntity> ZeffectControls
    );
    /// <summary>
    /// Z효과 컨트롤 불러오기 요청 DTO
    /// </summary>
    /// <param name="Username">조회할 사용자명</param>
    public record ZeffectControlLoadRequest(
        string Username
    );
    #endregion

    #region Response

    /// <summary>
    /// 이미지 정보 응답 DTO (조회 시)
    /// </summary>
    /// <param name="PathName">이미지 파일 전체 경로</param>
    /// <param name="Filename">파일명</param>
    /// <param name="CreateDate">생성일</param>
    public record ImageLoadResponse(
        string PathName,
        string Filename,
        DateTime CreateDate
    );

    #endregion

    #region Mapper
    /// <summary>
    /// Xray Filter Mapper 유틸리티 클래스
    /// </summary>
    public static class XrayVisionFilterMapper
    {
        /// <summary>
        /// DB 엔티티(FilterEntity) -> 화면/비즈니스 모델(FilterModel) 변환
        /// </summary>
        /// <param name="entity">FilterEntity</param>
        /// <returns>FilterModel</returns>
        public static FilterModel EntityToModel(FilterEntity entity)
        {
            return new FilterModel
            {
                Zoom = entity.Zoom,
                Sharpness = entity.Sharpness,
                Brightness = entity.Brightness,
                Contrast = entity.Contrast,
                ColorMode = entity.ColorMode,
            };
        }

        /// <summary>
        /// 화면/비즈니스 모델(FilterModel) -> DB 엔티티(FilterEntity) 변환
        /// </summary>
        /// <param name="model">FilterModel</param>
        /// <returns>FilterEntity</returns>
        public static FilterEntity ModelToEntity(FilterModel model)
        {
            return new FilterEntity
            {
                Zoom = model.Zoom,
                Sharpness = model.Sharpness,
                Brightness = model.Brightness,
                Contrast = model.Contrast,
                ColorMode = model.ColorMode,
            };
        }
        /// <summary>
        /// FilterModel -> 서비스 저장 요청 DTO(FilterSaveRequest) 변환
        /// </summary>
        /// <param name="model">필터 설정 모델</param>
        /// <returns>저장용 DTO(FilterSaveRequest)</returns>
        public static FilterSaveRequest ModelToRequest(FilterModel model)
        {
            return new FilterSaveRequest(
                Zoom: model.Zoom,
                Sharpness: model.Sharpness,
                Brightness: model.Brightness,
                Contrast: model.Contrast,
                ColorMode: model.ColorMode
            );
        }
    }

    /// <summary>
    /// Xray Calibration Mapper  유틸리티 클래스
    /// </summary>
    public static class XrayVisionCalibrationMapper
    {
        /// <summary>
        /// DB CalibrationEntity -> 화면/비즈니스 로직용 CalibrationModel 변환
        /// </summary>
        /// <param name="entity">CalibrationEntity</param>
        /// <returns>CalibrationModel</returns>
        public static CalibrationModel EntityToModel(CalibrationEntity entity)
        {
            var model = new CalibrationModel
            {
                RelativeWidthRatio = entity.RelativeWidthRatio,
                OffsetRegion = entity.OffsetRegion,
                GainRegion = entity.GainRegion,
                BoundaryArtifact = entity.BoundaryArtifact,
                ActivationThresholdRatio = entity.ActivationThresholdRatio,
                MaxImageWidth = entity.MaxImageWidth,
                SensorImageWidth = entity.SensorImageWidth
            };

            return model;
        }


        /// <summary>
        /// 화면/비즈니스 로직용 CalibrationModel -> DB CalibrationEntity 변환
        /// </summary>
        /// <param name="model">CalibrationModel</param>
        /// <returns>CalibrationEntity</returns>
        public static CalibrationEntity ModelToEntity(CalibrationModel model)
        {
            return new CalibrationEntity
            {
                RelativeWidthRatio = model.RelativeWidthRatio,
                OffsetRegion = model.OffsetRegion,
                GainRegion = model.GainRegion,
                BoundaryArtifact = model.BoundaryArtifact,
                ActivationThresholdRatio = model.ActivationThresholdRatio,
                MaxImageWidth = model.MaxImageWidth,
                SensorImageWidth = model.SensorImageWidth,
            };
        }


        /// <summary>
        /// CalibrationModel → 서비스 저장요청용 CalibrationSaveRequest DTO 변환
        /// </summary>
        /// <param name="model">CalibrationModel</param>
        /// <returns>CalibrationSaveRequest</returns>
        public static CalibrationSaveRequest ModelToRequest(CalibrationModel model)
        {
            return new CalibrationSaveRequest(
                RelativeWidthRatio: model.RelativeWidthRatio,
                OffsetRegion: model.OffsetRegion,
                GainRegion: model.GainRegion,
                BoundaryArtifact: model.BoundaryArtifact,
                ActivationThresholdRatio: model.ActivationThresholdRatio,
                MaxImageWidth: model.MaxImageWidth,
                SensorImageWidth: model.SensorImageWidth
            );
        }
    }

    /// <summary>
    /// Xray Material Mapper 유틸리티 클래스
    /// </summary>
    public static class XrayVisionMaterialMapper
    {
        /// <summary>
        /// DB 엔티티(MaterialEntity)를 -> 화면용 비즈니스 모델(MaterialModel)로 변환
        /// </summary>
        /// <param name="entity">MaterialEntity</param>
        /// <param name="action">Action : MaterialControlModel Action(콜백)</param>
        /// <returns>MaterialModel</returns>
        public static MaterialModel EntityToModel(MaterialEntity entity, Action action)
        {
            var model = new MaterialModel
            {   
                Blur = entity.Blur,
                HighLowRate = entity.HighLowRate,
                Density = entity.Density,
                EdgeBinary = entity.EdgeBinary,
                Transparency = entity.Transparency,
                Controls = [.. entity.MaterialControls?.Select(m => new MaterialControlModel(action)
                    {
                        Id = m.Id,
                        Y = m.Y,
                        XMin = m.XMin,
                        XMax = m.XMax,
                        Color = (Color)ColorConverter.ConvertFromString(m.Color)
                    }) ?? []]
            };

            return model;
        }

        /// <summary>
        /// 화면/비즈니스 모델(MaterialModel) -> DB저장용 MaterialEntity 변환
        /// </summary>
        /// <param name="model">MaterialModel</param>
        /// <returns>MaterialEntity</returns>
        public static MaterialEntity ModelToEntity(MaterialModel model)
        {
            return new MaterialEntity
            {
                Blur = model.Blur,
                HighLowRate = model.HighLowRate,
                Density = model.Density,
                EdgeBinary = model.EdgeBinary,
                Transparency = model.Transparency,
                MaterialControls = [.. model.Controls.Select(m => new MaterialControlEntity
                {
                    Id = m.Id,
                    Y = m.Y,
                    XMin = m.XMin,
                    XMax = m.XMax,
                    Color = $"#{m.Color.A:X2}{m.Color.R:X2}{m.Color.G:X2}{m.Color.B:X2}"
                })]
            };
        }

        /// <summary>
        /// MaterialModel -> API/서비스용 MaterialSaveRequest DTO로 변환
        /// </summary>
        /// <param name="model">MaterialModel</param>
        /// <returns>MaterialSaveRequest</returns>
        public static MaterialSaveRequest ModelToRequest(MaterialModel model)
        {
            var materialControls = model.Controls.Select(m => new MaterialControlEntity
            {
                Id = m.Id,
                Y = m.Y,
                XMin = m.XMin,
                XMax = m.XMax,
                Color = $"#{m.Color.A:X2}{m.Color.R:X2}{m.Color.G:X2}{m.Color.B:X2}"
            }).ToList();

            return new MaterialSaveRequest(
                Blur: model.Blur,
                HighLowRate: model.HighLowRate,
                Density: model.Density,
                EdgeBinary: model.EdgeBinary,
                Transparency: model.Transparency,
                MaterialControls: materialControls
            );
        }
    }

    /// <summary>
    /// Xray ZeffectControl Mapper 유틸리티 클래스
    /// </summary>
    public static class XrayVisionZeffectControlMapper
    {
        /// <summary>
        /// DB 엔티티 컬렉션(ZeffectControlEntity) -> 화면/로직용 ZeffectControlModel 컬렉션 변환
        /// </summary>
        /// <param name="entity">ICollection<ZeffectControlEntity> </param>
        /// <returns>ICollection<ZeffectControlModel></returns>
        public static ICollection<ZeffectControlModel> EntitiesToModels(ICollection<ZeffectControlEntity> entity)
        {
            return [.. entity.Select(EntityToModel).Where(model => model != null)];
        }

        /// <summary>
        /// ZeffectControlModel 컬렉션 -> DB저장용 ZeffectControlEntity 컬렉션 변환
        /// </summary>
        /// <param name="model">ICollection<ZeffectControlModel></param>
        /// <returns>ICollection<ZeffectControlEntity></returns>
        public static ICollection<ZeffectControlEntity> ModelsToEntities(ICollection<ZeffectControlModel> model)
        {

            return [.. model.Select(ModelToEntity).Where(entity => entity != null)];
        }

        /// <summary>
        /// ZeffectControlEntity 단일 객체 -> ZeffectControlModel 단일 객체 변환
        /// </summary>
        /// <param name="entity">ZeffectControlEntity</param>
        /// <returns>ZeffectControlModel</returns>
        public static ZeffectControlModel EntityToModel(ZeffectControlEntity entity)
        {

            var model = new ZeffectControlModel
            {
                Id = entity.Id,
                Check = entity.Check,
                Content = entity.Content,
                Min = entity.Min,
                Max = entity.Max,
                Color = (Color)ColorConverter.ConvertFromString(entity.Color)
            };

            return model;
        }

        /// <summary>
        /// ZeffectControlModel 단일 객체 -> DB저장용 ZeffectControlEntity 단일 객체 변환
        /// </summary>
        /// <param name="model">ZeffectControlModel</param>
        /// <returns>ZeffectControlEntity</returns>
        public static ZeffectControlEntity ModelToEntity(ZeffectControlModel model)
        {
            var entity = new ZeffectControlEntity
            {
                Id = model.Id,
                Check = model.Check,
                Content = model.Content,
                Min = model.Min,
                Max = model.Max,
                Color = $"#{model.Color.A:X2}{model.Color.R:X2}{model.Color.G:X2}{model.Color.B:X2}"
            };

            return entity;
        }

        /// <summary>
        /// ZeffectControlModel 컬렉션 -> 저장용 ZeffectControlSaveRequest DTO 변환
        /// </summary>
        /// <param name="model">ZeffectControlModel</param>
        /// <returns>ZeffectControlSaveRequest</returns>
        public static ZeffectControlSaveRequest ModelToRequest(ICollection<ZeffectControlModel> model)
        {
            var entities = model.Select(ModelToEntity).ToList();

            return new (entities);
        }
    }
    #endregion
}
