using Moq;
using MZ.Domain.Entities;
using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Domain.Enums;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionCalibrationServiceTest
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserSession> _session = new();
        private readonly Mock<IXrayVisionCalibrationRepository> _repo = new();

        public UserEntity MakeUser(int id = 1, string username = "sample")
        {
            return new UserEntity() { Id = id, Username = username, Role = UserRole.User };
        }

        private XrayVisionCalibrationService CreateSut()
        {
            return new XrayVisionCalibrationService( _users.Object, _session.Object, _repo.Object);
        }

        [Fact]
        public async Task Load_ByUsername_WhenNotFound_ReturnsValidFailure()
        {
            _users.Setup(r => r.GetByUsername("sample")).Returns((UserEntity)null!);

            var sut = CreateSut();
            var resp = await sut.Load(new CalibrationLoadRequest("sample"));

        }

        [Fact]
        public async Task Load_ByUsername_WhenCalibrationMissing_ReturnsValidFailure()
        {
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((CalibrationEntity)null!);

            var sut = CreateSut();
            var resp = await sut.Load(new CalibrationLoadRequest("sample"));

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Load_ByUsername_ReturnsCalibration()
        {
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);

            var cal = new CalibrationEntity { Id = 10, UserId = 1 };
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(cal);

            var sut = CreateSut();
            var resp = await sut.Load(new CalibrationLoadRequest("sample"));

            Assert.True(resp.Success);
            Assert.Equal(cal, resp.Data);
        }

        [Fact]
        public async Task Save_WhenNoLoggedInUser_ReturnsFail()
        {
            _session.SetupGet(s => s.CurrentUser).Returns((string)null!);
            _users.Setup(r => r.GetByUsername(null!)).Returns((UserEntity)null!);

            var sut = CreateSut();
            var req = new CalibrationSaveRequest(1, 2, 3, 4, 5, 6, 7);
            var resp = await sut.Save(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Fail, resp.Code);
        }

        [Fact]
        public async Task Save_WhenNoExisting_AddsNew()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            
            var user = MakeUser(1, "sample");

            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((CalibrationEntity)null!);
            _repo.Setup(r => r.AddAsync(It.IsAny<CalibrationEntity>())).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new CalibrationSaveRequest(1, 2, 3, 4, 5, 6, 7);
            var resp = await sut.Save(req);

            Assert.True(resp.Success);

            _repo.Verify(r => r.AddAsync(It.Is<CalibrationEntity>(c =>
                c.UserId == 1 &&
                c.RelativeWidthRatio == 1
            )), Times.Once);
        }

        [Fact]
        public async Task Save_WhenExisting_Updates()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");

            _users.Setup(r => r.GetByUsername("sample")).Returns(user);

            var exist = new CalibrationEntity { Id = 10, UserId = 1 };
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(exist);
            _repo.Setup(r => r.UpdateAsync(exist)).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new CalibrationSaveRequest(11, 22, 33, 44, 55, 66, 77);
            var resp = await sut.Save(req);

            Assert.True(resp.Success);

            _repo.Verify(r => r.UpdateAsync(It.Is<CalibrationEntity>(c =>
                c.RelativeWidthRatio == 11 &&
                c.SensorImageWidth == 77
            )), Times.Once);
        }
    }
}
