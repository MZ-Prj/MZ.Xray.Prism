using Moq;
using MZ.Domain.Entities;
using MZ.DTO;
using MZ.DTO.Enums;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using MZ.Domain.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionFilterServiceTest
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserSession> _session = new();
        private readonly Mock<IXrayVisionFilterRepository> _repo = new();

        private XrayVisionFilterService CreateSut()
        {
            return new(_users.Object, _session.Object, _repo.Object);
        }

        public UserEntity MakeUser(int id = 1, string username = "sample")
        {
            return new UserEntity() { Id = id, Username = username, Role = UserRole.User };
        }

        [Fact]
        public async Task Load_ByUsername_WhenMissing_ReturnsValidFailure()
        {
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((FilterEntity)null!);

            var sut = CreateSut();
            var resp = await sut.Load(new FilterLoadRequest("sample"));

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_WhenNoSessionUser_ReturnsFail()
        {
            _session.SetupGet(s => s.CurrentUser).Returns((string)null!);
            _users.Setup(r => r.GetByUsername(null!)).Returns((UserEntity)null!);
            var sut = CreateSut();

            var req = new FilterSaveRequest(1, 2, 3, 4, ColorRole.Color);
            var resp = await sut.Save(req);

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Fail, resp.Code);
        }

        [Fact]
        public async Task Save_New_Adds()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync((FilterEntity)null!);
            _repo.Setup(r => r.AddAsync(It.IsAny<FilterEntity>())).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new FilterSaveRequest(1, 2, 3, 4, ColorRole.Color);
            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            _repo.Verify(r => r.AddAsync(It.Is<FilterEntity>(f =>
                f.UserId == 1 && 
                f.Zoom == 1 && 
                f.ColorMode == ColorRole.Color)), Times.Once);
        }

        [Fact]
        public async Task Save_Update_Updates()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            var exist = new FilterEntity { Id = 10, UserId = 1 };
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(exist);
            _repo.Setup(r => r.UpdateAsync(exist)).Returns(Task.CompletedTask);

            var sut = CreateSut();
            var req = new FilterSaveRequest(5, 6, 7, 8, ColorRole.Color);
            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            _repo.Verify(r => r.UpdateAsync(It.Is<FilterEntity>(f =>
                f.Zoom == 5 && 
                f.Sharpness == 6 && 
                f.Brightness == 7 && 
                f.Contrast == 8 && 
                f.ColorMode == ColorRole.Color
            )), Times.Once);
        }
    }
}
