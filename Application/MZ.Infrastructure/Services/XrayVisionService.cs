using MZ.Logger;
using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MZ.Infrastructure.Services
{
    [Service]
    public class XrayVisionImageService : ServiceBase, IXrayVisionImageService
    {
        #region Repositorise
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionImageRepository xrayVisionImageRepository;
        #endregion

        public XrayVisionImageService(IUserRepository userRepository, 
                                      IXrayVisionImageRepository xrayVisionImageRepository)
        {
            this.userRepository = userRepository;
            this.xrayVisionImageRepository = xrayVisionImageRepository;
        }

        public async Task<BaseResponse<BaseRole, ICollection<ImageLoadResponse>>> Load(ImageLoadRequest request)
        {
            try
            {
                var loads = await xrayVisionImageRepository.GetByDateTimeBetweenStartEndAndPageSize(request.Start,request.End, request.Page,request.Size);

                ICollection<ImageLoadResponse> images = [.. loads.Select(
                    image => new ImageLoadResponse(
                        Path.Combine(image.Path, image.Filename), image.Filename, image.CreateDate))];

                return BaseResponseExtensions.Success(BaseRole.Success, images);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ImageLoadResponse>>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ReportImageLoadRequest request)
        {
            try
            {
                var loads = await xrayVisionImageRepository.GetByDateTimeBetweenStartEnd(request.Start, request.End);

                if (loads.Count == 0)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<ImageEntity>>(BaseRole.Valid);
                }

                return BaseResponseExtensions.Success(BaseRole.Success, loads);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ImageEntity>>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, ImageEntity>> Save(ImageSaveRequest request)
        {
            try
            {
                string subPath = "Image";
                string path = Path.Combine(request.Path, subPath);

                ImageEntity image = new()
                {
                    Path = path,
                    Filename = request.Filename,
                    Width = request.Width,
                    Height = request.Height,
                    ObjectDetections = [.. request.ObjectDetections.Select(c => new ObjectDetectionEntity() {
                        Index = c.Index,
                        Name = c.Name,
                        Color = c.Color,
                        Confidence = c.Confidence,
                        X  = c.X,
                        Y  = c.Y,
                        Width = c.Width,
                        Height = c.Height,
                    }) ?? []],
                };

                await xrayVisionImageRepository.AddAsync(image);

                return BaseResponseExtensions.Success<BaseRole, ImageEntity>(BaseRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, ImageEntity>(BaseRole.Fail, ex);
            }
        }
    }

    [Service]
    public class XrayVisionCalibrationService : IXrayVisionCalibrationService
    {
        protected readonly IUserSession userSession;
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionCalibrationRepository xrayVisionCalibrationRepository;

        public XrayVisionCalibrationService(IUserRepository userRepository,
                                            IUserSession userSession,
                                            IXrayVisionCalibrationRepository xrayVisionCalibrationRepository)
        {
            this.userRepository = userRepository;
            this.userSession = userSession;
            this.xrayVisionCalibrationRepository = xrayVisionCalibrationRepository;
        }

        public async Task<BaseResponse<BaseRole, CalibrationEntity>> Load(CalibrationLoadRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(request.Username);
                var filter = await xrayVisionCalibrationRepository.GetByUserIdAsync(user.Id);

                if (filter == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Valid);
                }
                return BaseResponseExtensions.Success(BaseRole.Success, filter);

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, CalibrationEntity>> Save(CalibrationSaveRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(userSession.CurrentUser);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Fail);
                }

                var calibration = await xrayVisionCalibrationRepository.GetByUserIdAsync(user.Id);

                if (calibration == null)
                {
                    calibration = new()
                    {
                        RelativeWidthRatio = request.RelativeWidthRatio,
                        OffsetRegion = request.OffsetRegion,
                        GainRegion = request.GainRegion,
                        BoundaryArtifact = request.BoundaryArtifact,
                        ActivationThresholdRatio = request.ActivationThresholdRatio,
                        MaxImageWidth = request.MaxImageWidth,
                        SensorImageWidth = request.SensorImageWidth,
                        UserId = user.Id
                    };

                    await xrayVisionCalibrationRepository.AddAsync(calibration);
                }
                else
                {
                    calibration.RelativeWidthRatio = request.RelativeWidthRatio;
                    calibration.OffsetRegion = request.OffsetRegion;
                    calibration.GainRegion = request.GainRegion;
                    calibration.BoundaryArtifact = request.BoundaryArtifact;
                    calibration.ActivationThresholdRatio = request.ActivationThresholdRatio;
                    calibration.MaxImageWidth = request.MaxImageWidth;
                    calibration.SensorImageWidth = request.SensorImageWidth;

                    await xrayVisionCalibrationRepository.UpdateAsync(calibration);
                }

                return BaseResponseExtensions.Success<BaseRole, CalibrationEntity>(BaseRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Fail, ex);
            }
        }
    }

    [Service]
    public class XrayVisionFilterService : IXrayVisionFilterService
    {
        protected readonly IUserSession userSession;
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionFilterRepository xrayVisionFilterRepository;

        public XrayVisionFilterService(IUserRepository userRepository,
                                       IUserSession userSession,
                                       IXrayVisionFilterRepository xrayVisionFilterRepository)
        {
            this.userSession = userSession;
            this.userRepository = userRepository;
            this.xrayVisionFilterRepository = xrayVisionFilterRepository;
        }

        public async Task<BaseResponse<BaseRole, FilterEntity>> Load(FilterLoadRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(request.Username);
                var filter = await xrayVisionFilterRepository.GetByUserIdAsync(user.Id);

                if (filter == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Valid);
                }
                return BaseResponseExtensions.Success(BaseRole.Success, filter);

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, FilterEntity>> Save(FilterSaveRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(userSession.CurrentUser);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Fail);
                }

                var filter = await xrayVisionFilterRepository.GetByUserIdAsync(user.Id);

                if (filter == null)
                {
                    filter = new()
                    {
                        Zoom = request.Zoom,
                        Sharpness = request.Sharpness,
                        Brightness = request.Brightness,
                        Contrast = request.Contrast,
                        ColorMode = request.ColorMode,
                        UserId = user.Id
                    };

                    await xrayVisionFilterRepository.AddAsync(filter);
                }
                else
                {
                    filter.Zoom = request.Zoom;
                    filter.Sharpness = request.Sharpness;
                    filter.Brightness = request.Brightness;
                    filter.Contrast = request.Contrast;
                    filter.ColorMode = request.ColorMode;

                    await xrayVisionFilterRepository.UpdateAsync(filter);
                }

                return BaseResponseExtensions.Success<BaseRole, FilterEntity>(BaseRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Fail, ex);
            }
        }
    }

    [Service]
    public class XrayVisionMaterialService : IXrayVisionMaterialService
    {
        protected readonly IUserSession userSession;
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionMaterialRepository xrayVisionMaterialRepository;

        public XrayVisionMaterialService(IUserRepository userRepository,
                                         IUserSession userSession,
                                         IXrayVisionMaterialRepository xrayVisionMaterialRepository)
        {
            this.userSession = userSession;
            this.userRepository = userRepository;
            this.xrayVisionMaterialRepository = xrayVisionMaterialRepository;
        }

        public async Task<BaseResponse<BaseRole, MaterialEntity>> Load(MaterialLoadRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(request.Username);
                var material = await xrayVisionMaterialRepository.GetByUserIdAsync(user.Id);

                if (material == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Valid);
                }
                return BaseResponseExtensions.Success(BaseRole.Success, material);

            }
            catch (Exception ex) 
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, MaterialEntity>> Save(MaterialSaveRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(userSession.CurrentUser);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Fail);
                }

                var material = await xrayVisionMaterialRepository.GetByUserIdAsync(user.Id);

                if (material == null)
                {
                    material = new()
                    {
                        Blur = request.Blur,
                        HighLowRate = request.HighLowRate,
                        Density = request.Density,
                        EdgeBinary = request.EdgeBinary,
                        Transparency = request.Transparency,
                        MaterialControls = request.MaterialControls,
                        UserId = user.Id
                    };

                    await xrayVisionMaterialRepository.AddAsync(material);
                }
                else
                {
                    material.Blur = request.Blur;
                    material.HighLowRate = request.HighLowRate;
                    material.Density = request.Density;
                    material.EdgeBinary = request.EdgeBinary;
                    material.Transparency = request.Transparency;

                    var materialControls = material.MaterialControls.ToList();

                    foreach (var materialControl in materialControls)
                    {
                        if (!request.MaterialControls.Any(m => m.Id == materialControl.Id))
                        {
                            material.MaterialControls.Remove(materialControl);
                        }
                    }

                    foreach (var materialControl in request.MaterialControls)
                    {
                        var exist = material.MaterialControls.FirstOrDefault(m => m.Id == materialControl.Id);
                        if (exist == null)
                        {
                            material.MaterialControls.Add(new MaterialControlEntity
                            {
                                Y = materialControl.Y,
                                XMin = materialControl.XMin,
                                XMax = materialControl.XMax,
                                Color = materialControl.Color
                            });
                        }
                        else
                        {
                            exist.Y = materialControl.Y;
                            exist.XMin = materialControl.XMin;
                            exist.XMax = materialControl.XMax;
                            exist.Color = materialControl.Color;
                        }
                    }

                    await xrayVisionMaterialRepository.UpdateAsync(material);
                }
                    
                return BaseResponseExtensions.Success<BaseRole, MaterialEntity>(BaseRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Fail, ex);
            }
        }
    }


    [Service]
    public class XrayVisionZeffectControlService : IXrayVisionZeffectControlService
    {
        protected readonly IUserSession userSession;
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionZeffectControlRepository xrayVisionZeffectControlRepository;

        public XrayVisionZeffectControlService(IUserRepository userRepository,
                                         IUserSession userSession,
                                         IXrayVisionZeffectControlRepository xrayVisionZeffectControlRepository)
        {
            this.userSession = userSession;
            this.userRepository = userRepository;
            this.xrayVisionZeffectControlRepository = xrayVisionZeffectControlRepository;
        }

        public async Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Load(ZeffectControlLoadRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(request.Username);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail);
                }

                var zeffects = await xrayVisionZeffectControlRepository.GetByUserIdAsync(user.Id);

                if (zeffects == null || zeffects.Count == 0)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Valid);
                }

                return BaseResponseExtensions.Success(BaseRole.Success, zeffects);

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, ICollection<ZeffectControlEntity>>> Save(ZeffectControlSaveRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(userSession.CurrentUser);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail);
                }

                var exists = await xrayVisionZeffectControlRepository.GetByUserIdAsync(user.Id);

                foreach (var control in request.ZeffectControls)
                {
                    control.UserId = user.Id;

                    if (control.Id == 0)
                    {
                        await xrayVisionZeffectControlRepository.AddAsync(control);
                    }
                    else
                    {
                        var exist = exists?.FirstOrDefault(e => e.Id == control.Id);
                        if (exist != null)
                        {
                            exist.Check = control.Check;
                            exist.Content = control.Content;
                            exist.Min = control.Min;
                            exist.Max = control.Max;
                            exist.Color = control.Color;

                            await xrayVisionZeffectControlRepository.UpdateAsync(exist);
                        }
                        else
                        {
                            await xrayVisionZeffectControlRepository.AddAsync(control);
                        }
                    }
                }

                var results = await xrayVisionZeffectControlRepository.GetByUserIdAsync(user.Id);
                return BaseResponseExtensions.Success(BaseRole.Success, results);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail, ex);
            }
        }
    }
}
