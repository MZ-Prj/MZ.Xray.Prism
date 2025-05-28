using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace MZ.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserLoginRole, UserEntity>> Login(UserLoginRequest request);

        BaseResponse<BaseRole, string> CurrentUser();

        Task<BaseResponse<UserRegisterRole, UserEntity>> Register(
            UserRegisterRequest request,
            CancellationToken cancellationToken = default);
    }
}
