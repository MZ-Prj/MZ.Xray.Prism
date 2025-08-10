using Moq;
using MZ.Domain.Auths;
using MZ.Domain.Entities;
using MZ.Domain.Enums;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IUserSettingRepository> _userSettingRepo = new();
        private readonly Mock<IUserSession> _session = new();

        private UserService CreateSut()
        {
            _session.SetupAllProperties();
            return new UserService(_userRepo.Object, _userSettingRepo.Object, _session.Object);
        }


        private static UserEntity MakeUser(string username, string password, UserRole role = UserRole.User, bool withSettings = true)
        {
            var user = new UserEntity
            {
                Id = 1,
                Username = username,
                Password = password,
                Role = role,
                CreateDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                UserSetting = withSettings ? new UserSettingEntity
                {
                    Theme = ThemeRole.LightSteel,
                    Language = LanguageRole.KoKR,
                    Buttons = []
                } : null
            };

            var encoder = new Sha256InformationEncoder();
            user.HashPassword(password, encoder);
            return user;
        }

        [Fact]
        public async Task Login_Success_SetsSession_UpdatesLastLogin_ReturnsSuccess()
        {
            var sut = CreateSut();
            var user = MakeUser("sample", "password", UserRole.Admin);

            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateLastLoginDateAsync(user.Id)).Returns(Task.CompletedTask);

            var resp = await sut.Login(new UserLoginRequest("sample", "password"));

            Assert.True(resp.Success);
            Assert.Equal(UserLoginRole.Success, resp.Code);
            Assert.Equal("sample", _session.Object.CurrentUser);
            Assert.True(_session.Object.LoginTime <= DateTime.Now);

            _userRepo.Verify(r => r.UpdateLastLoginDateAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task Login_NotFound_ReturnsNotFound()
        {
            var sut = CreateSut();
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync((UserEntity)null!);

            var resp = await sut.Login(new UserLoginRequest("sample", "password"));

            Assert.False(resp.Success);
            Assert.Equal(UserLoginRole.NotFound, resp.Code);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsValid()
        {
            var sut = CreateSut();
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);

            var resp = await sut.Login(new UserLoginRequest("sample", "wrongpassword"));

            Assert.False(resp.Success);
            Assert.Equal(UserLoginRole.Valid, resp.Code);
            _userRepo.Verify(r => r.UpdateLastLoginDateAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Logout_WhenNotLoggedIn_ReturnsValid()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.Logout();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Logout_UpdatesUsingTime_ClearsSession_ReturnsSuccess()
        {
            var sut = CreateSut();
            var user = MakeUser("sample", "password");
            _session.Object.CurrentUser = "sample";
            _session.Object.LoginTime = DateTime.Now.AddMinutes(-5);

            _userRepo.Setup(r => r.GetByUsernameAsync("sample")).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);
            _session.Setup(s => s.ClearAll());

            var resp = await sut.Logout();

            Assert.True(resp.Success);
            Assert.Equal(BaseRole.Success, resp.Code);
            _userRepo.Verify(r => r.UpdateAsync(It.Is<UserEntity>(u => u.UsingDate >= TimeSpan.FromMinutes(5))), Times.Once);
            _session.Verify(s => s.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task Register_WhenPasswordsMismatch_ReturnsNotMatch()
        {
            var sut = CreateSut();
            var req = new UserRegisterRequest("sample", "password", "repassword", UserRole.User);

            var resp = await sut.Register(req);

            Assert.False(resp.Success);
            Assert.Equal(UserRegisterRole.NotMatchPassword, resp.Code);
        }

        [Fact]
        public async Task Register_WhenUserExists_ReturnsAlreadyExist()
        {
            var sut = CreateSut();
            _userRepo.Setup(r => r.GetByUsernameAsync("sample")).ReturnsAsync(MakeUser("sample", "password"));

            var req = new UserRegisterRequest("sample", "password", "password", UserRole.User);
            var resp = await sut.Register(req);

            Assert.False(resp.Success);
            Assert.Equal(UserRegisterRole.AleadyExist, resp.Code);
        }

        [Fact]
        public async Task Register_Success_AddsUserWithDefaults()
        {
            var sut = CreateSut();
            _userRepo.Setup(r => r.GetByUsernameAsync("sample")).ReturnsAsync((UserEntity)null!);
            _userRepo.Setup(r => r.AddAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);

            var req = new UserRegisterRequest("sample", "password", "password", UserRole.Admin);
            var resp = await sut.Register(req);

            Assert.True(resp.Success);
            Assert.Equal(UserRegisterRole.Success, resp.Code);
            _userRepo.Verify(r => r.AddAsync(It.Is<UserEntity>(u =>
                u.Username == "sample" &&
                u.Role == UserRole.Admin &&
                u.UserSetting != null &&
                u.UserSetting.Buttons != null
            )), Times.Once);
        }

        [Fact]
        public async Task ChangeLanguage_WhenNoSession_ReturnsWarning_DefaultEnUS()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.ChangeLanguage(new LanguageRequest(null));

            Assert.True(resp.Success); 
            Assert.Equal(BaseRole.Warning, resp.Code);
            Assert.Equal(LanguageRole.EnUS, resp.Data);
        }

        [Fact]
        public async Task ChangeLanguage_Success_UpdatesRepository()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var resp = await sut.ChangeLanguage(new LanguageRequest(LanguageRole.KoKR));

            Assert.True(resp.Success);
            Assert.Equal(LanguageRole.KoKR, resp.Data);

            _userRepo.Verify(r => r.UpdateAsync(It.Is<UserEntity>(u => u.UserSetting.Language == LanguageRole.KoKR)), Times.Once);
        }

        [Fact]
        public async Task ChangeTheme_WhenNoSession_ReturnsWarning_DefaultLightSteel()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.ChangeTheme(new ThemeRequest(null));

            Assert.True(resp.Success);
            Assert.Equal(BaseRole.Warning, resp.Code);
            Assert.Equal(ThemeRole.LightSteel, resp.Data);
        }

        [Fact]
        public async Task ChangeTheme_Success_UpdatesRepository()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var resp = await sut.ChangeTheme(new ThemeRequest(ThemeRole.DarkSteel));

            Assert.True(resp.Success);
            Assert.Equal(ThemeRole.DarkSteel, resp.Data);
            _userRepo.Verify(r => r.UpdateAsync(It.Is<UserEntity>(u => u.UserSetting.Theme == ThemeRole.DarkSteel)), Times.Once);
        }

        [Fact]
        public async Task GetUserSetting_WhenNoSession_ReturnsValidFailure()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.GetUserSetting();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task GetUserSetting_Success_ReturnsSettings()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);

            var resp = await sut.GetUserSetting();

            Assert.True(resp.Success);
            Assert.NotNull(resp.Data);
            Assert.Equal(user.UserSetting, resp.Data);
        }

        [Fact]
        public async Task GetUserWithUserSetting_NoSession_ReturnsValidFailure()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.GetUserWithUserSetting();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task GetUserWithUserSetting_Success_ReturnsUser()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);

            var resp = await sut.GetUserWithUserSetting();

            Assert.True(resp.Success);
            Assert.Equal(user, resp.Data);
        }

        [Fact]
        public async Task SaveUserSetting_NoSession_ReturnsValidFailure()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var req = new UserSettingSaveRequest(ThemeRole.DarkSteel, LanguageRole.EnUS, []);
            var resp = await sut.SaveUserSetting(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task SaveUserSetting_Success_PersistsToRepository()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var buttons = user.UserSetting.Buttons; 
            var req = new UserSettingSaveRequest(ThemeRole.DarkSteel, LanguageRole.EnUS, buttons);

            var resp = await sut.SaveUserSetting(req);

            Assert.True(resp.Success);
            Assert.Equal(ThemeRole.DarkSteel, resp.Data?.Theme);
            Assert.Equal(LanguageRole.EnUS, resp.Data?.Language);
            _userRepo.Verify(r => r.UpdateAsync(It.Is<UserEntity>(u =>
                u.UserSetting.Theme == ThemeRole.DarkSteel &&
                u.UserSetting.Language == LanguageRole.EnUS)), Times.Once);
        }

        [Fact]
        public void IsLoggedIn_ReturnsTrueWhenSessionHasUser()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            Assert.True(sut.IsLoggedIn());
        }

        [Fact]
        public async Task IsAdmin_WhenNoSession_ReturnsValidFailure()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.IsAdmin();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task IsAdmin_ReturnsTrueForAdmin()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password", UserRole.Admin);
            _userRepo.Setup(r => r.GetByUsernameAllRelationsAsync("sample")).ReturnsAsync(user);

            var resp = await sut.IsAdmin();

            Assert.True(resp.Success);
            Assert.True(resp.Data);
        }

        [Fact]
        public async Task GetUser_NoSession_ReturnsValidFailure()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = null;

            var resp = await sut.GetUser();

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task GetUser_Success_ReturnsEntity()
        {
            var sut = CreateSut();
            _session.Object.CurrentUser = "sample";
            var user = MakeUser("sample", "password");
            _userRepo.Setup(r => r.GetByUsernameAsync("sample")).ReturnsAsync(user);

            var resp = await sut.GetUser();

            Assert.True(resp.Success);
            Assert.Equal(user, resp.Data);
        }
    }
}
