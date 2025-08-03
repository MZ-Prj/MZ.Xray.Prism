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
    public record ImageSaveRequest(
        string Path,
        string Filename,
        int Width,
        int Height,
        ICollection<ObjectDetectionEntity> ObjectDetections
    );

    public record ImageLoadRequest(
        DateTime Start,
        DateTime End,
        int Page,
        int Size
    );

    public record ReportImageLoadRequest(
        DateTime Start,
        DateTime End
    );

    public record FilterSaveRequest(
        float Zoom,
        float Sharpness,
        float Brightness,
        float Contrast,
        ColorRole ColorMode
    );

    public record FilterLoadRequest(
        string Username
    );

    public record MaterialSaveRequest(
        double Blur,
        double HighLowRate,
        double Density,
        double EdgeBinary,
        double Transparency,
        ICollection<MaterialControlEntity> MaterialControls
    );

    public record MaterialLoadRequest(
        string Username
    );

    public record CalibrationSaveRequest(
        double RelativeWidthRatio,
        double OffsetRegion,
        double GainRegion,
        double BoundaryArtifact,
        double ActivationThresholdRatio,
        int MaxImageWidth,
        int SensorImageWidth
    );

    public record CalibrationLoadRequest(
        string Username
    );

    public record ZeffectControlSaveRequest(
        ICollection<ZeffectControlEntity> ZeffectControls
    );

    public record ZeffectControlLoadRequest(
        string Username
    );
    #endregion

    #region Response
    public record ImageLoadResponse(
        string PathName,
        string Filename,
        DateTime CreateDate
    );

    #endregion

    #region Mapper : Filter
    public static class XrayVisionFilterMapper
    {
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
    #endregion

    #region Mapper : Calibration
    public static class XrayVisionCalibrationMapper
    {
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
    #endregion

    #region Mapper : Material
    public static class XrayVisionMaterialMapper
    {
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
    #endregion

    #region Mapper : Zeffect Control
    public static class XrayVisionZeffectControlMapper
    {

        public static ICollection<ZeffectControlModel> EntitiesToModels(ICollection<ZeffectControlEntity> entity)
        {
            return [.. entity.Select(EntityToModel).Where(model => model != null)];
        }

        public static ICollection<ZeffectControlEntity> ModelsToEntities(ICollection<ZeffectControlModel> model)
        {

            return [.. model.Select(ModelToEntity).Where(entity => entity != null)];
        }

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

        public static ZeffectControlSaveRequest ModelToRequest(ICollection<ZeffectControlModel> model)
        {
            var entities = model.Select(ModelToEntity).ToList();

            return new (entities);
        }
    }
    #endregion
}
