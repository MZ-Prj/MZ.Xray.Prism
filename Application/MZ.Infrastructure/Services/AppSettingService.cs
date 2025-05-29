using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Logger;
using MZ.Domain.Entities;
using MZ.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace MZ.Infrastructure.Services
{
    public class AppSettingService : IAppSettingService
    {
        protected readonly IAppSettingRepository appSettingRepository;

        public AppSettingService(IAppSettingRepository appSettingRepository)
        {
            this.appSettingRepository = appSettingRepository;
        }

        public async Task<BaseResponse<AppSettingRole, AppSettingEntity>> Register(AppSettingRegisterRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await appSettingRepository.DeleteAllAsync(cancellationToken);

                var appSetting = new AppSettingEntity()
                {
                    LastestUsername = request.LastestUsername,
                    IsUsernameSave = request.IsUsernameSave,
                };
                await appSettingRepository.AddAsync(appSetting, cancellationToken);
                
                return BaseResponseExtensions.Success(AppSettingRole.Success, appSetting);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<AppSettingRole, AppSettingEntity>(AppSettingRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<AppSettingRole, AppSettingEntity>> GetAppSetting(CancellationToken cancellationToken = default)
        {
            try
            {
                AppSettingEntity appSetting = await appSettingRepository.GetByIdAsync(1, cancellationToken);
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
