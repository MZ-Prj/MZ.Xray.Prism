using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Logger;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Services
{
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

        public Task<BaseResponse<AppSettingRole, AppSettingEntity>> SaveAppSetting()
        {
            return null;
        }
    }
}
