using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Services
{
    /// <summary>
    /// AI 모델 옵션(AIOption) 관리 서비스
    /// - ONNX 모델 및 카테고리 등 AI 옵션 등록/변경/조회/삭제 처리
    /// </summary>
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

        /// <summary>
        /// AIOption 신규 등록
        /// - 이미 등록된 레코드가 1개 존재할 경우 등록 불가
        /// - 신규 AIOptionEntity와 하위 Category 엔티티 생성 및 저장
        /// </summary>
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

        /// <summary>
        /// 카테고리 정보 저장(수정)
        /// - 기존 옵션이 있을 때만 카테고리 수정 가능
        /// - CategoryEntity의 속성 값 동기화
        /// </summary>
        public async Task<BaseResponse<BaseRole, AIOptionEntity>> Save(AIOptionSaveRequest request)
        {
            try
            {
                var exist = await xrayAIOptionRepository.GetByIdSingleAsync();
                if (exist == null)
                {
                    return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Valid);
                }

                var option = await xrayAIOptionRepository.UpdateCategoriesAsync(exist.Id, request.Categories);

                return BaseResponseExtensions.Success(BaseRole.Success, option);
            }
            catch (Exception ex)
            {
                return BaseResponseExtensions.Failure<BaseRole, AIOptionEntity>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 단일 AIOption 레코드 존재여부 확인
        /// - 하나만 존재해야 정상(true)
        /// </summary>
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

        /// <summary>
        /// AIOption 단일 옵션 정보 로드
        /// - 옵션이 없으면 Fail 반환
        /// </summary>
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

        /// <summary>
        /// 모든 AIOption(및 하위 카테고리) 데이터 삭제
        /// </summary>
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
