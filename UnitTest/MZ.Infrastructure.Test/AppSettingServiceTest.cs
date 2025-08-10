using Moq;
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class AppSettingServiceTest
    {
        private readonly Mock<IAppSettingRepository> _repo = new();

        private static AppSettingRegisterRequest MakeReq(string user = "sample", bool save = true)
        {
            return new AppSettingRegisterRequest(user, save);
        }

        [Fact]
        public async Task Register_DeletesAllThenAdds_ReturnsSuccessWithEntity()
        {
            _repo.Setup(r => r.DeleteAllAsync()).Returns(Task.CompletedTask);
            _repo.Setup(r => r.AddAsync(It.IsAny<AppSettingEntity>())).Returns(Task.CompletedTask);

            var sut = new AppSettingService(_repo.Object);
            var req = MakeReq("sample", false);

            var resp = await sut.Register(req);

            Assert.True(resp.Success);
            Assert.Equal(AppSettingRole.Success, resp.Code);
            Assert.NotNull(resp.Data);
            Assert.Equal("sample", resp.Data!.LastestUsername);
            Assert.False(resp.Data.IsUsernameSave);

            _repo.Verify(r => r.DeleteAllAsync(), Times.Once);
            _repo.Verify(r => r.AddAsync(It.Is<AppSettingEntity>(a =>
                a.LastestUsername == "sample" && 
                a.IsUsernameSave == false
            )), Times.Once);
        }

        [Fact]
        public async Task Register_WhenRepositoryThrows_ReturnsFail()
        {
            _repo.Setup(r => r.DeleteAllAsync()).ThrowsAsync(new InvalidOperationException("boom"));

            var sut = new AppSettingService(_repo.Object);
            var resp = await sut.Register(MakeReq());

            Assert.False(resp.Success);
            Assert.Equal(AppSettingRole.Fail, resp.Code);
        }

        [Fact]
        public async Task GetAppSetting_WhenMissing_ReturnsFail()
        {
            _repo.Setup(r => r.GetByIdSingleAsync()).ReturnsAsync((AppSettingEntity)null!);

            var sut = new AppSettingService(_repo.Object);
            var resp = await sut.GetAppSetting();

            Assert.False(resp.Success);
            Assert.Equal(AppSettingRole.Fail, resp.Code);
        }

        [Fact]
        public async Task GetAppSetting_WhenExists_ReturnsSuccessWithEntity()
        {
            var entity = new AppSettingEntity { Id = 1, LastestUsername = "sample", IsUsernameSave = true };
            _repo.Setup(r => r.GetByIdSingleAsync()).ReturnsAsync(entity);

            var sut = new AppSettingService(_repo.Object);
            var resp = await sut.GetAppSetting();

            Assert.True(resp.Success);
            Assert.Equal(AppSettingRole.Success, resp.Code);
            Assert.Equal(entity, resp.Data);
            Assert.Equal("sample", resp.Data!.LastestUsername);
            Assert.True(resp.Data.IsUsernameSave);
        }

        [Fact]
        public async Task GetAppSetting_WhenRepositoryThrows_ReturnsFail()
        {
            _repo.Setup(r => r.GetByIdSingleAsync()).ThrowsAsync(new Exception("error"));

            var sut = new AppSettingService(_repo.Object);
            var resp = await sut.GetAppSetting();

            Assert.False(resp.Success);
            Assert.Equal(AppSettingRole.Fail, resp.Code);
        }
    }
}
