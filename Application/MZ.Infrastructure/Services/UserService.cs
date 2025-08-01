using MZ.Domain.Auths;
using MZ.Domain.Entities;
using MZ.Domain.Enums;
using MZ.Domain.Interfaces;
using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Infrastructure.Interfaces;
using MZ.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Services
{
    [Service]
    public class UserService : ServiceBase, IUserService
    {
        protected readonly IInformationEncoder _informationEncoder;

        #region Repositorise
        protected readonly IUserRepository userRepository;
        protected readonly IUserSettingRepository userSettingRepository;
        protected readonly IUserSession userSession;
        #endregion

        public UserService(IUserRepository userRepository,
                           IUserSettingRepository userSettingRepository,
                           IUserSession userSession)
        {
            this.userRepository = userRepository;
            this.userSettingRepository = userSettingRepository;
            this.userSession = userSession;

            _informationEncoder = new Sha256InformationEncoder();
        }

        public async Task<BaseResponse<UserLoginRole, UserEntity>> Login(UserLoginRequest requeset)
        {
            try
            {
                UserEntity user = await userRepository.GetByUsernameAsync(requeset.Username);

                if (user == null)
                {
                    return BaseResponseExtensions.Failure<UserLoginRole, UserEntity>(UserLoginRole.NotFound);
                }

                if (!user.VerifyPassword(requeset.Password, _informationEncoder))
                {
                    return BaseResponseExtensions.Failure<UserLoginRole, UserEntity>(UserLoginRole.Valid);
                }

                await userRepository.UpdateLastLoginDateAsync(user.Id);
                userSession.CurrentUser = user.Username;
                return BaseResponseExtensions.Success<UserLoginRole, UserEntity>(UserLoginRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<UserLoginRole, UserEntity>(UserLoginRole.Fail, ex);
            }
        }

        public BaseResponse<BaseRole, string> Logout()
        {
            try
            {
                userSession.ClearAll();
                return BaseResponseExtensions.Success(BaseRole.Success, userSession.CurrentUser);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, string>(BaseRole.Fail, ex);
            }
        }

        public BaseResponse<BaseRole, string> CurrentUser()
        {
            try
            {
                return BaseResponseExtensions.Success(BaseRole.Success, userSession.CurrentUser);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, string>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<UserRegisterRole, UserEntity>> Register(UserRegisterRequest request)
        {
            try
            {
                if (request.Password != request.RePassword)
                {
                    return BaseResponseExtensions.Failure<UserRegisterRole, UserEntity>(UserRegisterRole.NotMatchPassword);
                }

                UserEntity existUser = await userRepository.GetByUsernameAsync(request.Username);

                if (existUser != null)
                {
                    return BaseResponseExtensions.Failure<UserRegisterRole, UserEntity>(UserRegisterRole.AleadyExist);
                }

                UserEntity user = new ()
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    CreateDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    Role = request.UserRole,
                    UserSetting = new()
                    {
                        Theme = ThemeRole.LightSteel,
                        Language = LanguageRole.KoKR,
                        Buttons = [.. UserSettingButtonKeys.GetAllKeys().Select(key => new UserButtonEntity(){
                            Name = key,
                            IsVisibility = true
                        })]
                    }
                };

                user.HashPassword(request.Password, _informationEncoder);
                await userRepository.AddAsync(user);

                return BaseResponseExtensions.Success<UserRegisterRole, UserEntity>(UserRegisterRole.Success);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<UserRegisterRole, UserEntity>(UserRegisterRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, LanguageRole>> ChangeLanguage(LanguageRequest request)
        {
            try
            {
                LanguageRole? language = request.LanguageRole;
                if (language == null)
                {
                    return BaseResponseExtensions.Success(BaseRole.Warning, LanguageRole.EnUS);
                }
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Success(BaseRole.Warning, LanguageRole.EnUS);
                }

                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);

                user.UserSetting.Language = language.Value;
                await userRepository.UpdateAsync(user);

                return BaseResponseExtensions.Success(BaseRole.Success, language.Value);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, LanguageRole>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, ThemeRole>> ChangeTheme(ThemeRequest request)
        {
            try
            {
                ThemeRole? theme = request.ThemeRole;
                if (theme == null)
                {
                    return BaseResponseExtensions.Success(BaseRole.Warning, ThemeRole.LightSteel);
                }
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Success(BaseRole.Warning, ThemeRole.LightSteel);
                }

                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);

                user.UserSetting.Theme = theme.Value;
                await userRepository.UpdateAsync(user);
                return BaseResponseExtensions.Success(BaseRole.Success, theme.Value);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, ThemeRole>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, UserSettingEntity>> GetUserSetting()
        {
            try
            {
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Valid);
                }
                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);

                return BaseResponseExtensions.Success(BaseRole.Success, user.UserSetting);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, UserEntity>> GetUserWithUserSetting()
        {
            try
            {
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Failure<BaseRole, UserEntity>(BaseRole.Valid);
                }
                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);

                return BaseResponseExtensions.Success(BaseRole.Success, user);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, UserEntity>(BaseRole.Fail, ex);
            }
        }

        public async Task<BaseResponse<BaseRole, UserSettingEntity>> SaveUserSetting(UserSettingSaveRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Valid);
                }

                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);
                user.UserSetting.Theme = request.Theme;
                user.UserSetting.Language = request.Language;
                user.UserSetting.Buttons = request.Buttons;

                await userRepository.UpdateAsync(user);
                return BaseResponseExtensions.Success(BaseRole.Success, user.UserSetting);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.ToString());
                return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Fail, ex);
            }
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(CurrentUser().Data);
        }
    }
}