using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Domain.Enums;
using MZ.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserLoginRole, UserEntity>> Login(UserLoginRequest request);
        BaseResponse<BaseRole, string> Logout();
        BaseResponse<BaseRole, string> CurrentUser();

        Task<BaseResponse<UserRegisterRole, UserEntity>> Register(UserRegisterRequest request);

        Task<BaseResponse<BaseRole, LanguageRole>> ChangeLanguage(LanguageRequest language);

        Task<BaseResponse<BaseRole, ThemeRole>> ChangeTheme(ThemeRequest theme);
    }
}
