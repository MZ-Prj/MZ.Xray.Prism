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
    /// <summary>
    /// 사용자(User) 관련 비즈니스 로직 서비스
    /// - 로그인/로그아웃, 회원가입, 세션/설정 조회 및 변경, 테마/언어 변경 등 계정 전반의 비즈니스 처리 담당
    /// </summary>
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
        /// <summary>
        /// 사용자 로그인
        /// - 사용자명으로 사용자 조회
        /// - 비밀번호 해시 검증
        /// - 마지막 로그인 일시 갱신 및 세션 저장
        /// </summary>
        public async Task<BaseResponse<UserLoginRole, UserEntity>> Login(UserLoginRequest requeset)
        {
            try
            {
                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(requeset.Username);

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
                userSession.LoginTime = DateTime.Now;

                return BaseResponseExtensions.Success(UserLoginRole.Success, user);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<UserLoginRole, UserEntity>(UserLoginRole.Fail, ex);
            }
        }
        /// <summary>
        /// 사용자 로그아웃
        /// - 세션 정보 삭제
        /// </summary>
        public async Task<BaseResponse<BaseRole, string>> Logout()
        {
            try
            {
                if (string.IsNullOrEmpty(userSession.CurrentUser) )
                {
                    return BaseResponseExtensions.Failure<BaseRole, string>(BaseRole.Valid);
                }
                var usingTime = (DateTime.Now - userSession.LoginTime);
                UserEntity user = await userRepository.GetByUsernameAsync(userSession.CurrentUser);
                user.UsingDate += usingTime;
                await userRepository.UpdateAsync(user);

                userSession.ClearAll();
                return BaseResponseExtensions.Success(BaseRole.Success, userSession.CurrentUser);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, string>(BaseRole.Fail, ex);
            }
        }
        /// <summary>
        /// 현재 세션의 사용자명 조회
        /// </summary>
        public BaseResponse<BaseRole, string> CurrentUser()
        {
            try
            {
                return BaseResponseExtensions.Success(BaseRole.Success, userSession.CurrentUser);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, string>(BaseRole.Fail, ex);
            }
        }
        /// <summary>
        /// 사용자 회원가입
        /// - 비밀번호 일치 확인
        /// - 동일 계정 존재여부 확인
        /// - 신규 사용자, 기본설정 생성 후 저장
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<UserRegisterRole, UserEntity>(UserRegisterRole.Fail, ex);
            }
        }

        /// <summary>
        /// 언어 설정 변경 (로그인 유저의 설정을 변경)
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, LanguageRole>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 테마 설정 변경 (로그인 유저의 설정을 변경)
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, ThemeRole>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 로그인 사용자의 UserSetting(환경설정) 정보 반환
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 로그인 사용자의 전체 정보(UserEntity + UserSetting) 반환
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, UserEntity>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 로그인 사용자의 환경설정(UserSetting) 저장
        /// </summary>
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
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, UserSettingEntity>(BaseRole.Fail, ex);
            }
        }
        /// <summary>
        /// 현재 로그인 여부 반환
        /// </summary>
        public bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(CurrentUser().Data);
        }

        /// <summary>
        /// 관리자 여부
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse<BaseRole, bool>> IsAdmin()
        {
            try
            {
                bool check = false;
                if (string.IsNullOrEmpty(userSession.CurrentUser))
                {
                    return BaseResponseExtensions.Failure<BaseRole, bool>(BaseRole.Valid);
                }
                UserEntity user = await userRepository.GetByUsernameAllRelationsAsync(userSession.CurrentUser);

                if (user.Role == UserRole.Admin)
                {
                    check = true;
                }

                return BaseResponseExtensions.Success(BaseRole.Success, check);
            }
            catch (Exception ex)
            {
                MZLogger.Error(ex.Message);
                return BaseResponseExtensions.Failure<BaseRole, bool>(BaseRole.Fail, ex);
            }
        }

        /// <summary>
        /// 현제 접속중인 유저 정보
        /// </summary>
		public async Task<BaseResponse<BaseRole, UserEntity>> GetUser()
		{

			try
			{
				if (string.IsNullOrEmpty(userSession.CurrentUser))
				{
					return BaseResponseExtensions.Failure<BaseRole, UserEntity>(BaseRole.Valid);
				}
				UserEntity user = await userRepository.GetByUsernameAsync(userSession.CurrentUser);

				return BaseResponseExtensions.Success(BaseRole.Success, user);
			}
			catch (Exception ex)
			{
				MZLogger.Error(ex.Message);
				return BaseResponseExtensions.Failure<BaseRole, UserEntity>(BaseRole.Fail, ex);
			}
		}
	}
}