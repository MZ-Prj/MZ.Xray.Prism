using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Services
{
    [Service]
    public class XrayAIOptionService : ServiceBase, IXrayAIOptionService
    {
        #region Repositorise
        protected readonly IXrayAIOptionRepository xrayAIOptionRepository;
        #endregion

        public XrayAIOptionService(IXrayAIOptionRepository xrayAIOptionRepository)
        {
            this.xrayAIOptionRepository = xrayAIOptionRepository;
        }

        public async Task<BaseResponse<BaseRole, AIOptionEntity>> Create(AIOptionCreateRequest request)
        {
            try
            {
                bool isOne = await xrayAIOptionRepository.IsOneAsync();

                if (isOne)
                {
                    return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Valid);
                }

                var option = new AIOptionEntity
                {
                    OnnxModel = request.OnnxModel,
                    ModelType = request.ModelType,
                    Cuda = request.Cuda,
                    PrimeGpu = request.PrimeGpu,
                    GpuId = request.GpuId,
                    IsChecked = request.IsChecked,
                    Confidence = request.Confidence,
                    IoU = request.IoU,
                    CreateDate = DateTime.Now,
                    Categories = [.. request.Categories.Select(e => new CategoryEntity
                    {
                        Index = e.Index,
                        Name = e.Name,
                        Color = e.Color,
                        IsUsing = e.IsUsing,
                        Confidence = e.Confidence
                    })]
                };

                await xrayAIOptionRepository.AddAsync(option);

                return BaseResponseExtensions.Success(BaseRole.Success, option);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Fail, ex);
            }
        }


        public async Task<BaseResponse<BaseRole, AIOptionEntity>> Save(AIOptionSaveRequest request)
        {
            try
            {
                var exist = await xrayAIOptionRepository.GetByIdAsync(request.AIOptionId);
                if (exist == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Valid);
                }

                var option = await xrayAIOptionRepository.UpdateCategoriesAsync(request.AIOptionId, request.Categories);

                return BaseResponseExtensions.Success(BaseRole.Success, option);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, bool>> ExistOneRecord()
        {
            try
            {
                bool isOne = await xrayAIOptionRepository.IsOneAsync();
                if (!isOne)
                {
                    return BaseResponseExtensions.Failure<BaseRole, bool>(BaseRole.Valid);
                }

                return BaseResponseExtensions.Success(BaseRole.Success, isOne);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, bool>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, AIOptionEntity>> Load()
        {
            try
            {
                var load = await xrayAIOptionRepository.GetByIdSingleAsync();
                if (load == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Valid);
                }

                return BaseResponseExtensions.Success(BaseRole.Success, load);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, bool>> Delete()
        {
            try
            {
                await xrayAIOptionRepository.DeleteAllAsync();

                return BaseResponseExtensions.Success(BaseRole.Success, true);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, bool>(BaseRole.Fail, ex);
            }
        }
    }

}
