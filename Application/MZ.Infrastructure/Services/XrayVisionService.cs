using MZ.Logger;
using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MZ.Infrastructure.Services
{
    public class XrayVisionImageService : IXrayVisionImageService
    {
        protected readonly IUserRepository userRepository;
        protected readonly IXrayVisionImageRepository xrayVisionImageRepository;

        public XrayVisionImageService(IUserRepository userRepository, 
                                      IXrayVisionImageRepository xrayVisionImageRepository)
        {
            this.userRepository = userRepository;
            this.xrayVisionImageRepository = xrayVisionImageRepository;
        }

        public async Task<BaseResponse<BaseRole, ICollection<ImageEntity>>> Load(ImageLoadRequest request)
        {
            try
            {
                var images = await xrayVisionImageRepository.GetImageByDateTimeBetweenStartEndAndPageSize(request.Start,request.End, request.Page,request.Size);
                return BaseResponseExtensions.Success(BaseRole.Success, images);
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
                string path = $"{request.Path}\\{subPath}";

                ImageEntity image = new()
                {
                    Path = path,
                    Filename = request.Filename,
                    Width = request.Width,
                    Height = request.Height,
                    ObjectDetections = [],
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

    public class XrayVisionFilterService : IXrayVisionFilterService
    {
    }

    public class XrayVisionMaterialService : IXrayVisionMaterialService
    {
    }
}
