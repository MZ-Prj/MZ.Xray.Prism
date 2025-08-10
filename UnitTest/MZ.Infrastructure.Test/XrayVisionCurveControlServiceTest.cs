using Moq;
using MZ.Domain.Entities;
using MZ.DTO.Enums;
using MZ.DTO;
using MZ.Infrastructure.Interfaces;
using MZ.Infrastructure.Services;
using Xunit;
using Assert = Xunit.Assert;
using MZ.Domain.Enums;

namespace MZ.Infrastructure.Test
{
    public class XrayVisionCurveControlServiceTes
    {

        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IUserSession> _session = new();
        private readonly Mock<IXrayVisionCurveControlRepository> _repo = new();

        public UserEntity MakeUser(int id = 1, string username = "sample")
        {
            return new UserEntity() { Id = id, Username = username, Role = UserRole.User };
        }

        private XrayVisionCurveControlService CreateSut()
        {
            return new(_users.Object, _session.Object, _repo.Object);
        }

        [Fact]
        public async Task Load_ByUsername_WhenEmpty_ReturnsValidFailure()
        {
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CurveControlEntity>());

            var sut = CreateSut();
            var resp = await sut.Load(new CurveControlLoadRequest("sample"));

            Assert.False(resp.Success);
            Assert.Equal(BaseRole.Valid, resp.Code);
        }

        [Fact]
        public async Task Save_DeletesAllThenAddsAll_AndReturnsFresh()
        {
            _session.SetupGet(s => s.CurrentUser).Returns("sample");
            var user = MakeUser(1, "sample");
            _users.Setup(r => r.GetByUsername("sample")).Returns(user);

            _repo.Setup(r => r.DeleteByUserIdAsync(1)).Returns(Task.CompletedTask);
            _repo.Setup(r => r.AddAsync(It.IsAny<CurveControlEntity>())).Returns(Task.CompletedTask);
            _repo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(
            [
                new(){ Id=1, UserId=1 }, new(){ Id=2, UserId=1 }
            ]);

            var sut = CreateSut();
            var req = new CurveControlSaveRequest(
            [
                new(){ Id=0, X=0, Y=1},
                new(){ Id=0, X=0, Y=2},
            ]);

            var resp = await sut.Save(req);

            Assert.True(resp.Success);
            Assert.Equal(2, resp.Data!.Count);
            _repo.Verify(r => r.DeleteByUserIdAsync(1), Times.Once);
            _repo.Verify(r => r.AddAsync(It.Is<CurveControlEntity>(c => c.UserId == 1)), Times.Exactly(2));
        }
    }
}
