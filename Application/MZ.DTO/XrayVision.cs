using MZ.Domain.Entities;
using MZ.Domain.Enums;
using System;
using System.Collections.Generic;

#nullable enable
namespace MZ.DTO
{

    #region Request
    public record ImageSaveRequest(
        string Path,
        string Filename,
        int Width,
        int Height
    );

    public record ImageLoadRequest(
        DateTime Start,
        DateTime End,
        int Page,
        int Size
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

    #endregion

    #region Response
    public record ImageLoadResponse(
        string PathName,
        string Filename,
        DateTime CreateDate
    );

    #endregion

}
