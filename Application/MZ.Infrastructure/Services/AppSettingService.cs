using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Logger;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Services
{
    /// <summary>
    /// AppSetting(환경설정) 관련 서비스 
    /// - 환경설정 등록(저장), 조회, 기타 환경설정 관련 기능 제공
    /// - 예외 상황 및 로깅 관리
    /// </summary>
    [Service]
    public class AppSettingService : ServiceBase, IAppSettingService
    {
        #region Repositorise
        protected readonly IAppSettingRepository appSettingRepository;
        #endregion

        public AppSettingService(IAppSettingRepository appSettingRepository)
        {
            this.appSettingRepository = appSettingRepository;
        }
        /// <summary>
        /// 애플리케이션 환경설정 등록(최신값 저장)
        /// - 기존 모든 환경설정 레코드를 삭제 후, 새로운 레코드로 갱신(1개만 유지)
        /// - 사용자명, '사용자명 저장' 여부를 저장
        /// - 예외 발생 시 실패 반환 및 로그 기록
        /// </summary>
        /// <param name="request">환경설정 등록 요청 데이터</param>
        /// <returns>등록 결과 및 저장된 엔터티를 포함한 응답</returns>
        public async Task<BaseResponse<AppSettingRole, AppSettingEntity>> Register(AppSettingRegisterRequest request)
        {
            try
            {
                await appSettingRepository.DeleteAllAsync();

                var appSetting = new AppSettingEntity()
                {
                    LastestUsername = request.LastestUsername,
                    IsUsernameSave = request.IsUsernameSave,
                };
                await appSettingRepository.AddAsync(appSetting);
                
                return BaseResponseExtensions.Success(AppSettingRole.Success, appSetting);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<AppSettingRole, AppSettingEntity>(AppSettingRole.Fail, ex);
            }
        }

        /// <summary>
        /// 현재 저장된 애플리케이션 환경설정 정보를 조회
        /// - 환경설정이 존재하지 않으면 실패 반환
        /// - 예외 발생 시 로그 기록 및 실패 반환
        /// </summary>
        /// <returns>조회 결과 및 환경설정 엔터티</returns>
        public async Task<BaseResponse<AppSettingRole, AppSettingEntity>> GetAppSetting()
        {
            try
            {
                AppSettingEntity appSetting = await appSettingRepository.GetByIdSingleAsync();
                if (appSetting == null)
                {
                    return BaseResponseExtensions.Failure<AppSettingRole, AppSettingEntity>(AppSettingRole.Fail);
                }
                return BaseResponseExtensions.Success(AppSettingRole.Success, appSetting);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<AppSettingRole, AppSettingEntity>(AppSettingRole.Fail, ex);
            }
        }
    }
}
