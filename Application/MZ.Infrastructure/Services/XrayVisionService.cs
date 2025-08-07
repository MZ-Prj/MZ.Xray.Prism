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
using MZ.Util;

namespace MZ.Infrastructure.Services
{
    /// <summary>
    /// Xray 영상 및 결과 이미지에 대한 비즈니스 로직을 담당하는 서비스
    /// - 이미지 저장, 조회, 분석 결과 반환 등
    /// </summary>
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
        /// <summary>
        /// 요청 조건(기간, 페이지 등)에 맞는 이미지 목록을 조회
        /// - 결과는 LoadResponse로 가공
        /// </summary>
        public async Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ImageLoadRequest request)
        {
            try
            {
                var loads = await xrayVisionImageRepository.GetByDateTimeBetweenStartEndAndPageSize(request.Start, request.End, request.Page, request.Size);

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

        /// <summary>
        /// 리포트 등에서 기간별 이미지 전체 조회
        /// </summary>
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

        /// <summary>
        /// 이미지 및 객체탐지 결과 저장
        /// - DB에 신규 이미지와 ObjectDetection 리스트를 함께 저장
        /// </summary>
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
                    ObjectDetections = [.. request.ObjectDetections.Select(c => {
                        var copy = new ObjectDetectionEntity();
                        c.CopyTo(copy);
                        return copy;
                    }) ?? []],
                };

                await xrayVisionImageRepository.AddAsync(image);

                return BaseResponseExtensions.Success<BaseRole, ImageEntity>(BaseRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ImageEntity>(BaseRole.Fail, ex);
            }
        }
    }

    /// <summary>
    /// 사용자별 Xray Calibration 관리 서비스
    /// - 사용자 ID로 Calibration 등록/수정/조회
    /// </summary>
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

        /// <summary>
        /// 지정 사용자명에 해당하는 보정값(Calibration) 조회
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 현재 로그인 사용자 보정값(Calibration) 저장(없으면 신규, 있으면 수정)
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, CalibrationEntity>(BaseRole.Fail, ex);
            }
        }
    }

    /// <summary>
    /// 사용자별 Xray Filter(필터값) 관리 서비스
    /// - 사용자별 필터값 조회/저장 처리
    /// </summary>
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
        /// <summary>
        /// 지정 사용자명에 해당하는 필터값(Filter) 조회
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Fail, ex);
            }
        }
        /// <summary>
        /// 현재 로그인 사용자 필터값(Filter) 저장(없으면 신규, 있으면 수정)
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, FilterEntity>(BaseRole.Fail, ex);
            }
        }
    }

    /// <summary>
    /// 사용자별 Xray Material 관리 서비스
    /// - Material 및 MaterialControl 집합 저장/수정/조회
    /// </summary>
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
        /// <summary>
        /// 지정 사용자명에 해당하는 Material 조회
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Fail, ex);
            }
        }
        /// <summary>
        /// 현재 로그인 사용자 Material 저장/수정
        /// - MaterialControl의 추가/수정/삭제까지 반영
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, MaterialEntity>(BaseRole.Fail, ex);
            }
        }
    }

    /// <summary>
    /// 사용자별 Zeffect 컨트롤 집합 관리 서비스
    /// - Zeffect 컨트롤 저장/수정/조회
    /// </summary>
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

        /// <summary>
        /// 지정 사용자명에 해당하는 Zeffect 컨트롤 조회
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 현재 로그인 사용자 Zeffect 컨트롤 저장/수정(존재하면 수정, 없으면 신규)
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ICollection<ZeffectControlEntity>>(BaseRole.Fail, ex);
            }
        }
    }

    /// <summary>
    /// 사용자별 Curve 컨트롤 집합 관리 서비스
    /// - Curve 컨트롤 저장/수정/조회
    /// </summary>
    [Service]
    public class XrayVisionCurveControlService : IXrayVisionCurveControlService
    {
        protected readonly IUserSession userSession;
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionCurveControlRepository xrayVisionCurveControlRepository;

        public XrayVisionCurveControlService(IUserRepository userRepository,
                                         IUserSession userSession,
                                         IXrayVisionCurveControlRepository xrayVisionCurveControlRepository)
        {
            this.userSession = userSession;
            this.userRepository = userRepository;
            this.xrayVisionCurveControlRepository = xrayVisionCurveControlRepository;
        }

        /// <summary>
        /// 지정 사용자명에 해당하는 Curve 컨트롤 조회
        /// </summary>
        public async Task<BaseResponse<BaseRole, ICollection<CurveControlEntity>>> Load(CurveControlLoadRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(request.Username);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<CurveControlEntity>>(BaseRole.Fail);
                }

                var zeffects = await xrayVisionCurveControlRepository.GetByUserIdAsync(user.Id);

                if (zeffects == null || zeffects.Count == 0)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<CurveControlEntity>>(BaseRole.Valid);
                }

                return BaseResponseExtensions.Success(BaseRole.Success, zeffects);

            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ICollection<CurveControlEntity>>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 현재 로그인 사용자 Curve 컨트롤 저장/수정(존재하면 수정, 없으면 신규)
        /// </summary>
        public async Task<BaseResponse<BaseRole, ICollection<CurveControlEntity>>> Save(CurveControlSaveRequest request)
        {
            try
            {
                var user = userRepository.GetByUsername(userSession.CurrentUser);
                if (user == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, ICollection<CurveControlEntity>>(BaseRole.Fail);
                }

                await xrayVisionCurveControlRepository.DeleteByUserIdAsync(user.Id);

                foreach (var control in request.CurveControls)
                {
                    control.UserId = user.Id;
                    await xrayVisionCurveControlRepository.AddAsync(control);
                }

                var results = await xrayVisionCurveControlRepository.GetByUserIdAsync(user.Id);
                return BaseResponseExtensions.Success(BaseRole.Success, results);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ICollection<CurveControlEntity>>(BaseRole.Fail, ex);
            }
        }
    }
}
